// Backend of the csharp-openhab-rest-client test app: makes every method of the C# library
// callable over HTTP. The browser sends the openHAB address, the credentials, the class and
// method name and the arguments as JSON object.
//
//   GET  /                     health check (wake-up on Render)
//   GET  /api/methods          all methods with parameters
//   POST /api/connect          {url, token | username+password}
//   POST /api/call             {url, token, ..., class, method, args: {...}}
//   GET  /api/events/stream    ?url&token&...&class&method&args=<json>  (Server-Sent Events)
using System.Text.Json;
using System.Text.Json.Nodes;
using OpenHABBackend;
using OpenHABRestClient;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
var app = builder.Build();
app.UseCors();

var json = new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
IResult Json(JsonNode? value, int status = 200) =>
    Results.Text(value?.ToJsonString(json) ?? "null", "application/json", System.Text.Encoding.UTF8, status);
IResult Error(string message, int status) => Json(new JsonObject { ["error"] = message }, status);

app.MapGet("/", () => Json(new JsonObject
{
    ["status"] = "ok", ["library"] = "csharp-openhab-rest-client", ["version"] = OpenHABClient.VERSION,
}));

app.MapGet("/api/methods", () => Json(Library.Describe()));

app.MapPost("/api/connect", async (HttpRequest req) =>
{
    if (await Body(req) is not JsonObject body) return Error("Invalid JSON body", 400);
    using var c = Library.Client(body.Str("url"), body.Str("token"), body.Str("username"), body.Str("password"));
    var j = new JsonObject { ["loggedIn"] = c.IsLoggedIn, ["isCloud"] = c.IsCloud, ["url"] = c.Url };
    if (c.LastError.Length > 0) j["error"] = c.LastError;
    return Json(j);
});

app.MapPost("/api/call", async (HttpRequest req, CancellationToken ct) =>
{
    if (await Body(req) is not JsonObject body) return Error("Invalid JSON body", 400);
    string cls = body.Str("class"), method = body.Str("method");
    if (cls.Length == 0 || method.Length == 0) return Error("class and method are required", 400);
    var m = Library.Find(cls, method);
    if (m == null || m.Stream) return Error($"Unknown method {cls}.{method}", 400);
    using var c = Library.Client(body.Str("url"), body.Str("token"), body.Str("username"), body.Str("password"));
    try
    {
        return Json(await m.CallAsync(c, body["args"] as JsonObject ?? new JsonObject(), ct));
    }
    catch (ArgumentException e) { return Error("Invalid argument: " + e.Message, 400); }
});

// Event stream: the C# library reads openHAB, the backend forwards every event.
app.MapGet("/api/events/stream", async (HttpContext ctx) =>
{
    var q = ctx.Request.Query;
    string cls = q["class"].ToString(), method = q["method"].ToString();
    if (cls.Length == 0) { cls = "Events"; method = "getEvents"; }
    JsonObject args;
    try { args = JsonNode.Parse(q["args"].ToString() is { Length: > 0 } a ? a : "{}") as JsonObject ?? new JsonObject(); }
    catch (JsonException) { args = new JsonObject(); }
    if (q["topics"].ToString() is { Length: > 0 } topics && !args.ContainsKey("topics")) args["topics"] = topics;
    var m = Library.Find(cls, method);
    if (m == null || !m.Stream) { await Error($"Unknown event stream {cls}.{method}", 400).ExecuteAsync(ctx); return; }
    using var c = Library.Client(q["url"].ToString(), q["token"].ToString(), q["username"].ToString(), q["password"].ToString());
    EventStream s;
    try { s = await m.OpenAsync(c, args, ctx.RequestAborted); }
    catch (ArgumentException e) { await Error("Invalid argument: " + e.Message, 400).ExecuteAsync(ctx); return; }
    await using (s)
    {
        if (s.IsError) { await Json(s.ErrorResult, 502).ExecuteAsync(ctx); return; }
        ctx.Response.Headers.CacheControl = "no-cache";
        ctx.Response.ContentType = "text/event-stream";
        var ct = ctx.RequestAborted;
        try
        {
            while (!ct.IsCancellationRequested)
            {
                // wait up to 15 s; ALIVE messages keep the connection to the browser open
                var ev = await s.NextEventAsync(TimeSpan.FromSeconds(15), includeAlive: true, cancellationToken: ct);
                string chunk;
                if (ev != null) chunk = "data: " + ev.ToJsonString(json) + "\n\n";
                else if (s.IsOpen) chunk = ": keep-alive\n\n";
                else
                {
                    if (s.IsError) await ctx.Response.WriteAsync("event: error\ndata: " + s.ErrorResult!.ToJsonString(json) + "\n\n", ct);
                    break;
                }
                await ctx.Response.WriteAsync(chunk, ct);
                await ctx.Response.Body.FlushAsync(ct);
            }
        }
        catch (OperationCanceledException) { }
    }
});

app.Logger.LogInformation("csharp-openhab-rest-client {Version} backend listening on port {Port}", OpenHABClient.VERSION, port);
app.Run();

static async Task<JsonNode?> Body(HttpRequest req)
{
    try { return await JsonNode.ParseAsync(req.Body); }
    catch (JsonException) { return null; }
}

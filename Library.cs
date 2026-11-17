using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using OpenHABRestClient;

namespace OpenHABBackend;

/// <summary>One method of the library, found by reflection.</summary>
public sealed class LibraryMethod(Type cls, MethodInfo sync, MethodInfo async)
{
    public string Class => cls.Name;
    public string Name => sync.Name;
    public bool Stream => sync.ReturnType == typeof(EventStream);
    public ParameterInfo[] Parameters => sync.GetParameters();

    static string TypeName(Type t) =>
        (Nullable.GetUnderlyingType(t) ?? t) switch
        {
            var x when x == typeof(string) => "str",
            var x when x == typeof(bool) => "bool",
            var x when x == typeof(int) => "int",
            var x when x == typeof(double) => "float",
            _ => "json",
        };

    public JsonObject Describe() => new()
    {
        ["class"] = Class,
        ["method"] = Name,
        ["function"] = $"{Class}.{Name}",
        ["stream"] = Stream,
        ["params"] = new JsonArray(Parameters.Select(p => (JsonNode)new JsonObject
        {
            ["name"] = p.Name,
            ["type"] = TypeName(p.ParameterType),
            ["required"] = !p.IsOptional,
            ["default"] = p.IsOptional ? JsonSerializer.SerializeToNode(p.DefaultValue) : null,
        }).ToArray()),
    };

    object?[] Arguments(JsonObject args, CancellationToken ct)
    {
        var ps = async.GetParameters();
        var vals = new object?[ps.Length];
        for (var i = 0; i < ps.Length; i++)
        {
            var p = ps[i];
            if (p.ParameterType == typeof(CancellationToken)) { vals[i] = ct; continue; }
            if (!args.TryGetPropertyValue(p.Name!, out var v))
            {
                if (!p.IsOptional) throw new ArgumentException($"{p.Name} is required");
                vals[i] = p.DefaultValue;
                continue;
            }
            var t = Nullable.GetUnderlyingType(p.ParameterType) ?? p.ParameterType;
            try
            {
                if (v == null) vals[i] = p.IsOptional ? null : throw new ArgumentException($"{p.Name} is required");
                else if (t == typeof(string)) vals[i] = v.GetValueKind() == JsonValueKind.String ? v.GetValue<string>() : v.ToJsonString();
                else if (t == typeof(bool)) vals[i] = v.GetValueKind() == JsonValueKind.String ? bool.Parse(v.GetValue<string>()) : v.GetValue<bool>();
                else if (t == typeof(int)) vals[i] = v.GetValueKind() == JsonValueKind.String ? int.Parse(v.GetValue<string>()) : v.GetValue<int>();
                else if (t == typeof(double)) vals[i] = v.GetValueKind() == JsonValueKind.String ? double.Parse(v.GetValue<string>(), System.Globalization.CultureInfo.InvariantCulture) : v.GetValue<double>();
                else vals[i] = v.DeepClone();
            }
            catch (Exception e) when (e is FormatException or InvalidOperationException)
            {
                throw new ArgumentException($"{p.Name}: {e.Message}");
            }
        }
        return vals;
    }

    object Instance(OpenHABClient c) => Activator.CreateInstance(cls, c)!;

    public async Task<JsonNode?> CallAsync(OpenHABClient c, JsonObject args, CancellationToken ct) =>
        await (Task<JsonNode?>)async.Invoke(Instance(c), Arguments(args, ct))!;

    public async Task<EventStream> OpenAsync(OpenHABClient c, JsonObject args, CancellationToken ct) =>
        await (Task<EventStream>)async.Invoke(Instance(c), Arguments(args, ct))!;
}

public static class Library
{
    static readonly List<LibraryMethod> Methods = typeof(OpenHABClient).Assembly.GetExportedTypes()
        .Where(t => t.GetConstructor(new[] { typeof(OpenHABClient) }) != null)
        .OrderBy(t => t.Name, StringComparer.Ordinal)
        .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => !m.Name.EndsWith("Async", StringComparison.Ordinal))
            .Select(m => (t, m, a: t.GetMethod(m.Name + "Async")))
            .Where(x => x.a != null)
            .Select(x => new LibraryMethod(x.t, x.m, x.a!)))
        .ToList();

    public static JsonArray Describe() => new(Methods.Select(m => (JsonNode)m.Describe()).ToArray());

    /// <summary>Class and method; the method also with a small first letter (Python names).</summary>
    public static LibraryMethod? Find(string cls, string method)
    {
        var name = method.Length > 0 ? char.ToUpperInvariant(method[0]) + method[1..] : method;
        return Methods.FirstOrDefault(m => m.Class == cls && m.Name == name);
    }

    public static OpenHABClient Client(string url, string token, string user, string pass) =>
        new(url.Length == 0 ? "https://myopenhab.org" : url,
            user.Length == 0 ? null : user, pass.Length == 0 ? null : pass, token.Length == 0 ? null : token);
}

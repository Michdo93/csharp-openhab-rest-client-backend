namespace OpenHABBackend.Models;

public class OpenHABRequest
{
    public string? Url      { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Token    { get; set; }
    public string? Body     { get; set; }
    public Dictionary<string, string>? Params { get; set; }
}

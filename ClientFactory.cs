using OpenHABRestClient;
using OpenHABBackend.Models;

namespace OpenHABBackend;

public class ClientFactory
{
    public OpenHABClient Build(OpenHABRequest req)
    {
        var url = req.Url?.TrimEnd('/') ?? "https://myopenhab.org";
        if (!string.IsNullOrWhiteSpace(req.Token))
            return new OpenHABClient(url, token: req.Token);
        return new OpenHABClient(url, req.Username, req.Password);
    }
}

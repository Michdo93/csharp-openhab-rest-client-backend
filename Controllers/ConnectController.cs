using Microsoft.AspNetCore.Mvc;
using OpenHABBackend.Models;

namespace OpenHABBackend.Controllers;

[ApiController]
[Route("api/connect")]
public class ConnectController(ClientFactory factory) : ControllerBase
{
    [HttpPost]
    public IActionResult Connect([FromBody] OpenHABRequest req)
    {
        try
        {
            var client = factory.Build(req);
            return Ok(new { loggedIn = client.IsLoggedIn, isCloud = client.IsCloud, url = client.BaseUrl });
        }
        catch (Exception e) { return StatusCode(502, new { error = e.Message }); }
    }
}

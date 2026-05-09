using Microsoft.AspNetCore.Mvc;

namespace AgentWorking.API.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet(Name = "GetHealth")]
    public IActionResult Get()
    {
        var response = new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow.ToString("o")
        };

        return Ok(response);
    }
}
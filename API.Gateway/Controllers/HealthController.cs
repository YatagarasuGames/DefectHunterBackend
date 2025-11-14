using Microsoft.AspNetCore.Mvc;

namespace API.Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { Status = "API Gateway is running", Timestamp = DateTime.UtcNow });
        }
    }
}

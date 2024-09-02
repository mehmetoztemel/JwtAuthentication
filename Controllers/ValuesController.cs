using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MO.Result;

namespace JwtAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(Result<string>.Success("API is working"));
        }
    }
}

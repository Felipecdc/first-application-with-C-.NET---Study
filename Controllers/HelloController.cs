using Microsoft.AspNetCore.Mvc;

namespace first_dotnet_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HelloController : ControllerBase
    {
        [HttpGet]
        public string Get() => "Hello World";

        [HttpGet("{name}")]
        public string Get(string name)
        {
            return $"Hello, {name}!";
        }
    }
}
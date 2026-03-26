using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("{id}")]
        public string Get(string id)
        {
            return "Hola mundo: " + id;
        }

        [HttpPost]
        public ActionResult<string> Post([FromBody] Dummy dummy)
        {
            return $"PostDummy: i '{dummy.i}' s '{dummy.s}' d '{dummy.d}'";
        }

        [HttpPut("{id}")]
        public string Put(string id)
        {
            return "Put: " + id;
        }

        [HttpDelete("{id}")]
        public string Delete(string id)
        {
            return "Delete: " + id;
        }
    }
}

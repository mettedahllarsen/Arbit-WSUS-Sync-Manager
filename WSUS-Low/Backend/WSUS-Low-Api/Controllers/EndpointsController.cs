using Microsoft.AspNetCore.Mvc;

namespace WSUS_Low_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EndpointsController : ControllerBase
    {
        // GET: api/<ValuesController>
        [HttpGet]
        public string Get()
        {
            return "Welcome to WSUS Low. These are the following endpoints:" +
                "\n- fetch/categories:" +
                "\n- ";
        }

        // GET: api/<EndpointsController>/fetch/categories
        [HttpGet("fetch/categories")]
        public string FetchCategories()
        {
            return "Successfully fetched categories";
        }

        // GET: api/<EndpointsController>/fetch/updates
        [HttpGet("fetch/updates")]
        public string FetchUpdates()
        {
            return "Successfully fetched updates";
        }

        // GET: api/<EndpointsController>/fetch/content
        [HttpGet("fetch/content")]
        public string FetchContent()
        {
            return "Successfully fetched content";
        }

        /*// POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }*/
    }
}

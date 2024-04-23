using Microsoft.AspNetCore.Mvc;

namespace WSUSLowAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WSUSDbController : ControllerBase
    {
        // GET: api/<WSUSDbController>/fetch/metadata
        [HttpGet]
        public string Get()
        {
            return "Welcome to WSUSLow, here are the endpoints:" +
                "\n";
        }

        // GET: api/<WSUSDbController>/fetch/metadata
        [HttpGet("fetch/metadata")]
        public string FetchMetadata()
        {
            return "Successfully fetched metadata";
        }

        // GET: api/<WSUSDbController>/fetch/updatecontent
        [HttpGet("fetch/updatecontent")]
        public string FetchUpdateContent()
        {
            return "Successfully fetched updatecontent";
        }

        // POST api/<WSUSDbController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<WSUSDbController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<WSUSDbController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

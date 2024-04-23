using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using WSUSLowAPI.Models;
using WSUSLowAPI.Repositories;

namespace WSUSLowAPI.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateMetadatasController(IUpdateMetadataRepository repository) : ControllerBase
    {
        // GET: api/<UpdateMetadatasController>
        [HttpGet]
        public string Welcome()
        {
            return "Welcome to WSUSLow, here are the endpoints:" +
                "\n fetch: Fetch all metadata" +
                "\n get:";
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        // GET: api/<UpdateMetadatasController>/fetch
        [HttpPost("fetch/{filter}")]
        public ActionResult<IEnumerable<string>> FetchMetadata(string filter)
        {
            string resultmessage = repository.FetchToDb(filter);

            return Ok(resultmessage);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        // GET: api/<UpdateMetadatasController>/get
        [HttpGet("get")]
        public ActionResult<IEnumerable<UpdateMetadata>> GetAll()
        {
            List<UpdateMetadata> result = repository.GetAll();
            if (result == null || result.Count < 1)
            {
                return NoContent();
            }
            return Ok(result);
        }
    }
}

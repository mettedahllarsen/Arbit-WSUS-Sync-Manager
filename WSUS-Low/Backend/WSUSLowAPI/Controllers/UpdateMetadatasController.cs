using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using WSUSLowAPI.Models;
using WSUSLowAPI.Repositories;

namespace WSUSLowAPI.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateMetadatasController : ControllerBase
    {
        private readonly IUpdateMetadataRepository _repository;
        public UpdateMetadatasController(IUpdateMetadataRepository repository)
        {
            _repository = repository;
        }

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
        [HttpPost("fetch")]
        public IActionResult FetchMetadata([FromBody] FetchFilter newFilter)
        {
            string resultmessage = _repository.FetchToDb(newFilter.Title);

            return Ok(resultmessage);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        // GET: api/<UpdateMetadatasController>/get
        [HttpGet("get")]
        public IActionResult GetAll()
        {
            var result = _repository.GetAll();
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
    }
}

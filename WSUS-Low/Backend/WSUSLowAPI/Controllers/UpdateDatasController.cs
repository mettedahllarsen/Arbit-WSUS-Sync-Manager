using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using WSUSLowAPI.Models;
using WSUSLowAPI.Repositories;

namespace WSUSLowAPI.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateDatasController : ControllerBase
    {
        private readonly IUpdateDataRepository _repository;
        public UpdateDatasController(IUpdateDataRepository repository)
        {
            _repository = repository;
        }

        // GET: api/<UpdateDatasController>
        [HttpGet]
        public string Welcome()
        {
            return "Welcome to WSUSLow, here are the endpoints:" +
                "\n fetch: Fetch all metadata" +
                "\n get:";
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        // GET: api/<UpdateDatasController>/fetch
        [HttpPost("fetch")]
        public IActionResult FetchMetadata([FromBody] FetchFilter newFilter)
        {
            string resultmessage = _repository.FetchToDb(newFilter.Title);

            return Ok(resultmessage);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        // GET: api/<UpdateDatasController>/get
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

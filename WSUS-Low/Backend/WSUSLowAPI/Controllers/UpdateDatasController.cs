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
        private IUpdateDataRepository _repository;

        public UpdateDatasController(IUpdateDataRepository repository)
        {
            _repository = repository;
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        // GET: api/<UpdateDatasController>
        [HttpGet]
        public string Welcome()
        {
            return "Welcome to WSUSLow, here are the endpoints:" +
                "\n fetch: Fetch all metadata" +
                "\n get:";
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        // GET: api/<UpdateDatasController>/fetch/metadata
        [HttpGet("fetch")]
        public string FetchMetadata()
        {
            return "Successfully fetched metadata";
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        // GET: api/<UpdateDatasController>/metadata
        [HttpGet("get")]
        public ActionResult<IEnumerable<UpdateData>> GetAll()
        {
            List<UpdateData> result = _repository.GetAll();
            if (result.Count < 1)
            {
                return NoContent();
            }
            return Ok(result);
        }
    }
}

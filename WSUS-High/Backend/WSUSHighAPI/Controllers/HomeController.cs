using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WSUSHighAPI.Models;
using WSUSHighAPI.Repositories;

namespace WSUSHighAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComputersController : ControllerBase
    {
        private readonly ComputersRepository _computersRepository;

        public ComputersController(ComputersRepository computersRepository)
        {
            _computersRepository = computersRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var computers = _computersRepository.GetAllComputers();
            return Ok(computers);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var computer = _computersRepository.GetComputerById(id);
            if (computer == null)
            {
                return NotFound();
            }
            return Ok(computer);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Computer computer)
        {
            _computersRepository.AddComputer(computer);
            return CreatedAtAction(nameof(Get), new { id = computer.ComputerID }, computer);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Computer computer)
        {
            if (computer == null)
            {
                return BadRequest("Computer object is null.");
            }
            else if (id != computer.ComputerID)
            {
                return BadRequest("Computer ID in the URL does not match the ID in the request body:" + $"URL: {id}." + $"Body: {computer.ComputerID}.");
            }
            else
            {
                _computersRepository.UpdateComputer(computer);
                return Ok("Computer updated successfully");
            }
        }
    }
}
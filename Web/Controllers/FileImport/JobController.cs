using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Application.Commands;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobController : BaseController
    {
        private readonly ILogger<JobController> _logger;

        public JobController(ILogger<JobController> logger)
        {
            _logger = logger;
        }


        [HttpGet("ImportFile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ImportFile()
        {
            try
            {
                var result = await Mediator.Send(new ImportFileCommand());

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to process file");

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }
}

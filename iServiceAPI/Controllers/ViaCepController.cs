using iServiceServices.Services;
using iServiceServices.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace iServiceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ViaCepController : ControllerBase
    {
        [HttpGet("Search/{cep}")]
        public async Task<ActionResult<Result<ViaCep>>> Search(string cep)
        {
            var result = await new ViaCepService().GetByCep(cep);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }
    }
}

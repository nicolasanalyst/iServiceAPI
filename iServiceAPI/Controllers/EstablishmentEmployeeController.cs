using iServiceRepositories.Repositories.Models;
using iServiceServices.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iServiceAPI.Controllers
{
    public class EmployeeAvailabilityRequest
    {
        public int ServiceId { get; set; }
        public DateTime Start { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Establishment,Client")]
    public class EstablishmentEmployeeController : ControllerBase
    {
        private readonly ITokenInfoService _tokenInfoService;
        private readonly EstablishmentEmployeeService _establishmentEmployeeService;

        public EstablishmentEmployeeController(IConfiguration configuration, ITokenInfoService tokenInfoService)
        {
            _tokenInfoService = tokenInfoService;
            _establishmentEmployeeService = new EstablishmentEmployeeService(configuration);
        }

        [HttpGet]
        [Authorize(Roles = "Establishment")]
        public async Task<ActionResult<List<EstablishmentEmployee>>> Get()
        {
            var tokenInfo = TokenService.GetTokenInfo(TokenService.GetJwtToken(HttpContext));

            var tokenInfoo = _tokenInfoService.TokenInfo;

            var result = await _establishmentEmployeeService.GetAllEstablishmentEmployees(tokenInfo);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPost("GetEmployeeAvailability")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<List<EstablishmentEmployee>>> GetEmployeeAvailability([FromBody] EmployeeAvailabilityRequest request)
        {
            var tokenInfo = _tokenInfoService.TokenInfo;

            var result = await _establishmentEmployeeService.GetEmployeeAvailability(tokenInfo, request.ServiceId, request.Start);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpGet("GetEmployeeByService/{serviceId}")]
        [Authorize(Roles = "Establishment")]
        public async Task<ActionResult<List<EstablishmentEmployee>>> GetEmployeeByService(int serviceId)
        {
            var tokenInfo = _tokenInfoService.TokenInfo;

            var result = await _establishmentEmployeeService.GetEmployeeByService(tokenInfo, serviceId);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpGet("{EstablishmentEmployeeId}")]
        [Authorize(Roles = "Establishment")]
        public async Task<ActionResult<EstablishmentEmployee>> GetById(int EstablishmentEmployeeId)
        {
            var tokenInfo = TokenService.GetTokenInfo(TokenService.GetJwtToken(HttpContext));

            var result = await _establishmentEmployeeService.GetEstablishmentEmployeeById(tokenInfo, EstablishmentEmployeeId);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return NotFound(new { message = result.ErrorMessage });
        }

        [HttpPost]
        [Authorize(Roles = "Establishment")]
        public async Task<ActionResult<EstablishmentEmployee>> Post([FromForm] EstablishmentEmployee EstablishmentEmployeeModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tokenInfo = TokenService.GetTokenInfo(TokenService.GetJwtToken(HttpContext));

            var result = await _establishmentEmployeeService.AddEstablishmentEmployee(tokenInfo, EstablishmentEmployeeModel);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetById), new { EstablishmentEmployeeId = result.Value.EstablishmentEmployeeId }, result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPut("{EstablishmentEmployeeId}")]
        [Authorize(Roles = "Establishment")]
        public async Task<ActionResult<EstablishmentEmployee>> Put(int EstablishmentEmployeeId, [FromForm] EstablishmentEmployee EstablishmentEmployee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tokenInfo = TokenService.GetTokenInfo(TokenService.GetJwtToken(HttpContext));

            var result = await _establishmentEmployeeService.UpdateEstablishmentEmployee(tokenInfo, EstablishmentEmployee);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPut("{EstablishmentEmployeeId}/SetActive")]
        [Authorize(Roles = "Establishment")]
        public async Task<ActionResult<bool>> SetActive(int EstablishmentEmployeeId, [FromBody] bool isActive)
        {
            var tokenInfo = TokenService.GetTokenInfo(TokenService.GetJwtToken(HttpContext));

            var result = await _establishmentEmployeeService.SetActiveStatus(tokenInfo, EstablishmentEmployeeId, isActive);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpDelete("{EstablishmentEmployeeId}/SetDeleted")]
        [Authorize(Roles = "Establishment")]
        public async Task<ActionResult<bool>> SetDeleted(int EstablishmentEmployeeId, [FromBody] bool isDeleted)
        {
            var tokenInfo = TokenService.GetTokenInfo(TokenService.GetJwtToken(HttpContext));

            var result = await _establishmentEmployeeService.SetDeletedStatus(tokenInfo, EstablishmentEmployeeId, isDeleted);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }
    }
}

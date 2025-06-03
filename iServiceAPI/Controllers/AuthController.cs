using iServiceRepositories.Repositories.Models;
using iServiceServices.Services;
using iServiceServices.Services.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iServiceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost("Login")]
        public async Task<ActionResult<UserInfo>> LoginAsync([FromBody] Login model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await new AuthService(_configuration).Login(model);

                if (result.IsSuccess)
                {
                    result.Value.Token = TokenService.GenerateToken((result.Value.User, result.Value.UserRole, result.Value.UserProfile));
                    return Ok(result.Value);
                }
                else
                {
                    return BadRequest(new { message = result.ErrorMessage });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro inesperado.");
            }
        }
        [HttpPost("PreRegister")]
        public async Task<ActionResult<UserInfo>> PreRegisterAsync([FromBody] PreRegister model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await new AuthService(_configuration).PreRegister(model);

                if (result.IsSuccess)
                {
                    return Ok(result.Value);
                }
                else
                {
                    return BadRequest(new { message = result.ErrorMessage });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro inesperado.");
            }
        }
        [HttpPost("RegisterUserProfile")]
        public async Task<ActionResult<UserInfo>> RegisterUserProfileAsync([FromBody] UserInfo model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await new AuthService(_configuration).RegisterUserProfile(model);

                if (result.IsSuccess)
                {
                    return Ok(result.Value);
                }
                else
                {
                    return BadRequest(new { message = result.ErrorMessage });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro inesperado.");
            }
        }

        [HttpPost("RegisterAddress")]
        public async Task<ActionResult<UserInfo>> RegisterAddressAsync([FromBody] UserInfo model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await new AuthService(_configuration).RegisterAddress(model);

                if (result.IsSuccess)
                {
                    return Ok(result.Value);
                }
                else
                {
                    return BadRequest(new { message = result.ErrorMessage });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro inesperado.");
            }
        }

        [HttpGet("admin")]
        [Authorize(Roles = "admin")]
        public string Employee() => "admin";
    }

}

using iServiceRepositories.Repositories.Models;
using iServiceServices.Services;
using Microsoft.AspNetCore.Mvc;

namespace iServiceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserRoleController : ControllerBase
    {
        private readonly UserRoleService _userRoleService;

        public UserRoleController(IConfiguration configuration)
        {
            _userRoleService = new UserRoleService(configuration);
        }

        [HttpGet]
        public async Task<ActionResult<List<UserRole>>> Get()
        {
            var result = await _userRoleService.GetAllUserRoles();

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<UserRole>> GetById(int id)
        {
            var result = await _userRoleService.GetUserRoleById(id);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return NotFound(new { message = result.ErrorMessage });
        }

        [HttpPost]
        public async Task<ActionResult<UserRole>> Post([FromBody] UserRole userRoleModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userRoleService.AddUserRole(userRoleModel);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetById), new { id = result.Value.UserRoleId }, result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserRole>> Put(int id, [FromBody] UserRole userRole)
        {
            if (id != userRole.UserRoleId)
            {
                return BadRequest();
            }

            var result = await _userRoleService.UpdateUserRole(userRole);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPut("{id}/SetActive")]
        public async Task<ActionResult<bool>> SetActive(int id, [FromBody] bool isActive)
        {
            var result = await _userRoleService.SetActiveStatus(id, isActive);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpDelete("{id}/SetDeleted")]
        public async Task<ActionResult<bool>> SetDeleted(int id, [FromBody] bool isDeleted)
        {
            var result = await _userRoleService.SetDeletedStatus(id, isDeleted);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }
    }
}

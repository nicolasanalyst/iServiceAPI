using iServiceRepositories.Repositories.Models;
using iServiceServices.Services;
using Microsoft.AspNetCore.Mvc;

namespace iServiceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserInfoController : ControllerBase
    {
        private readonly UserInfoService _userInfoService;

        public UserInfoController(IConfiguration configuration)
        {
            _userInfoService = new UserInfoService(configuration);
        }

        [HttpGet("GetUserInfoByUserId/{userId}")]
        public async Task<ActionResult<UserInfo>> GetUserInfoByUserId(int userId)
        {
            var result = await _userInfoService.GetUserInfoByUserId(userId);

            if (result.IsSuccess)
            {
                result.Value.User.Password = "";
                return Ok(result.Value);
            }

            return NotFound(new { message = result.ErrorMessage });
        }

        [HttpGet("GetUserInfoByUserRoleId/{userRoleId}")]
        public async Task<ActionResult<List<UserInfo>>> GetUserInfoByUserRoleId(int userRoleId)
        {
            var result = await _userInfoService.GetUserInfoByUserRoleId(userRoleId);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return NotFound(new { message = result.ErrorMessage });
        }

        [HttpGet("GetUserInfoByEstablishmentCategoryId/{establishmentCategoryId}")]
        public async Task<ActionResult<List<UserInfo>>> GetUserInfoByUserRoleIdAndEstablishmentCategoryId(int establishmentCategoryId)
        {
            var result = await _userInfoService.GetUserInfoByEstablishmentCategoryId(establishmentCategoryId);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return NotFound(new { message = result.ErrorMessage });
        }
    }
}

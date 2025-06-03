using iServiceRepositories.Repositories.Models;
using iServiceServices.Services;
using iServiceServices.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace iServiceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserProfileController : ControllerBase
    {
        private readonly UserProfileService _userProfileService;

        public UserProfileController(IConfiguration configuration)
        {
            _userProfileService = new UserProfileService(configuration);
        }

        [HttpGet]
        public async Task<ActionResult<List<UserProfile>>> Get()
        {
            var result = await _userProfileService.GetAllUserProfiles();

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpGet("GetByEstablishmentCategoryId/{establishmentCategoryId}")]
        public async Task<ActionResult<List<UserProfile>>> GetByEstablishmentCategoryId(int establishmentCategoryId)
        {
            var result = await _userProfileService.GetByEstablishmentCategoryId(establishmentCategoryId);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpGet("{userProfileId}")]
        public async Task<ActionResult<UserProfile>> GetById(int userProfileId)
        {
            var result = await _userProfileService.GetUserProfileById(userProfileId);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return NotFound(new { message = result.ErrorMessage });
        }

        [HttpGet("GetUserInfoByUserId/{userId}")]
        public async Task<ActionResult<UserProfile>> GetUserInfoByUserId(int userId)
        {
            var result = await _userProfileService.GetUserInfoByUserId(userId);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return NotFound(new { message = result.ErrorMessage });
        }

        [HttpPost]
        public async Task<ActionResult<UserProfile>> Post([FromBody] UserProfile profileModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userProfileService.AddUserProfile(profileModel);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetById), new { userProfileId = result.Value.UserProfileId }, result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPost("Save")]
        public async Task<ActionResult<UserInfo>> Save([FromBody] UserInfo request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userProfileService.SaveUserProfile(request);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetById), new { userProfileId = result.Value.UserProfile.UserProfileId }, result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPost("UpdateProfileImage")]
        public async Task<ActionResult<string>> UpdateProfileImage([FromForm] ImageModel profileModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userProfileService.UpdateProfileImage(profileModel);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPut("{userProfileId}")]
        public async Task<ActionResult<UserProfile>> Put(int userProfileId, [FromBody] UserProfile profile)
        {
            if (userProfileId != profile.UserProfileId)
            {
                return BadRequest();
            }

            var result = await _userProfileService.UpdateUserProfile(profile);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpDelete("{userProfileId}")]
        public async Task<IActionResult> Delete(int userProfileId)
        {
            var result = await _userProfileService.DeleteUserProfile(userProfileId);

            if (result.IsSuccess)
            {
                return NoContent();
            }

            return NotFound(new { message = result.ErrorMessage });
        }
    }
}

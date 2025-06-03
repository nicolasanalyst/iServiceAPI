using iServiceServices.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iServiceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly HomeServices _homeService;

        public HomeController(IConfiguration configuration)
        {
            _homeService = new HomeServices(configuration);
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(int userId)
        {
            var result = await _homeService.GetAsync(userId);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }
    }
}

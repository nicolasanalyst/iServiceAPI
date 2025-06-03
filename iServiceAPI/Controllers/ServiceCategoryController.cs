using iServiceRepositories.Repositories.Models;
using iServiceServices.Services;
using Microsoft.AspNetCore.Mvc;

namespace iServiceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServiceCategoryController : ControllerBase
    {
        private readonly ServiceCategoryService _serviceCategoryService;

        public ServiceCategoryController(IConfiguration configuration)
        {
            _serviceCategoryService = new ServiceCategoryService(configuration);
        }

        [HttpGet]
        public async Task<ActionResult<List<ServiceCategory>>> Get()
        {
            var result = await _serviceCategoryService.GetAllServiceCategories();

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpGet("{categoryId}")]
        public async Task<ActionResult<ServiceCategory>> GetById(int categoryId)
        {
            var result = await _serviceCategoryService.GetServiceCategoryById(categoryId);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return NotFound(new { message = result.ErrorMessage });
        }

        [HttpGet("GetByUserProfileId/{userProfileId}")]
        public async Task<ActionResult<List<ServiceCategory>>> GetByUserProfileId(int userProfileId)
        {
            var result = await _serviceCategoryService.GetByUserProfileId(userProfileId);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPost]
        public async Task<ActionResult<ServiceCategory>> Post([FromBody] ServiceCategory categoryModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _serviceCategoryService.AddServiceCategory(categoryModel);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetById), new { categoryId = result.Value.ServiceCategoryId }, result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPut("{categoryId}")]
        public async Task<ActionResult<ServiceCategory>> Put(int categoryId, [FromBody] ServiceCategory category)
        {
            if (categoryId != category.ServiceCategoryId)
            {
                return BadRequest();
            }

            var result = await _serviceCategoryService.UpdateServiceCategory(category);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPut("{categoryId}/SetActive")]
        public async Task<ActionResult<bool>> SetActive(int categoryId, [FromBody] bool isActive)
        {
            var result = await _serviceCategoryService.SetActiveStatus(categoryId, isActive);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpDelete("{categoryId}/SetDeleted")]
        public async Task<ActionResult<bool>> SetDeleted(int categoryId, [FromBody] bool isDeleted)
        {
            var result = await _serviceCategoryService.SetDeletedStatus(categoryId, isDeleted);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }
    }
}

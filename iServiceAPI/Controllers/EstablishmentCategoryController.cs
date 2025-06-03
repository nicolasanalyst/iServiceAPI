using iServiceRepositories.Repositories.Models;
using iServiceServices.Services;
using Microsoft.AspNetCore.Mvc;

namespace iServiceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EstablishmentCategoryController : ControllerBase
    {
        private readonly EstablishmentCategoryService _establishmentCategoryService;

        public EstablishmentCategoryController(IConfiguration configuration)
        {
            _establishmentCategoryService = new EstablishmentCategoryService(configuration);
        }

        [HttpGet]
        public async Task<ActionResult<List<EstablishmentCategory>>> Get()
        {
            var result = await _establishmentCategoryService.GetAllEstablishmentCategories();

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpGet("{categoryId}")]
        public async Task<ActionResult<EstablishmentCategory>> GetById(int categoryId)
        {
            var result = await _establishmentCategoryService.GetEstablishmentCategoryById(categoryId);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return NotFound(new { message = result.ErrorMessage });
        }

        [HttpPost]
        public async Task<ActionResult<EstablishmentCategory>> Post([FromForm] EstablishmentCategory categoryModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _establishmentCategoryService.AddEstablishmentCategory(categoryModel);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetById), new { categoryId = result.Value.EstablishmentCategoryId }, result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPut("{categoryId}")]
        public async Task<ActionResult<EstablishmentCategory>> Put(int categoryId, [FromForm] EstablishmentCategory category)
        {
            if (categoryId != category.EstablishmentCategoryId)
            {
                return BadRequest();
            }

            var result = await _establishmentCategoryService.UpdateEstablishmentCategory(category);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPut("{categoryId}/SetActive")]
        public async Task<ActionResult<bool>> SetActive(int categoryId, [FromBody] bool isActive)
        {
            var result = await _establishmentCategoryService.SetActiveStatus(categoryId, isActive);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpDelete("{categoryId}/SetDeleted")]
        public async Task<ActionResult<bool>> SetDeleted(int categoryId, [FromBody] bool isDeleted)
        {
            var result = await _establishmentCategoryService.SetDeletedStatus(categoryId, isDeleted);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }
    }

}

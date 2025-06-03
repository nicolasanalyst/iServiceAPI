using iServiceRepositories.Repositories.Models;
using iServiceServices.Services;
using Microsoft.AspNetCore.Mvc;

namespace iServiceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SpecialScheduleController : ControllerBase
    {
        private readonly SpecialScheduleService _specialScheduleService;

        public SpecialScheduleController(IConfiguration configuration)
        {
            _specialScheduleService = new SpecialScheduleService(configuration);
        }

        [HttpGet]
        public async Task<ActionResult<List<SpecialSchedule>>> Get()
        {
            var result = await _specialScheduleService.GetAllSpecialSchedules();

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpGet("{specialScheduleId}")]
        public async Task<ActionResult<SpecialSchedule>> GetById(int specialScheduleId)
        {
            var result = await _specialScheduleService.GetSpecialScheduleById(specialScheduleId);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return NotFound(new { message = result.ErrorMessage });
        }

        [HttpGet("GetByUserProfileId/{userProfileId}")]
        public async Task<ActionResult<List<SpecialSchedule>>> GetByUserProfileId(int userProfileId)
        {
            var result = await _specialScheduleService.GetByUserProfileId(userProfileId);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return NotFound(new { message = result.ErrorMessage });
        }

        [HttpPost]
        public async Task<ActionResult<SpecialSchedule>> Post([FromBody] SpecialSchedule scheduleModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _specialScheduleService.AddSpecialSchedule(scheduleModel);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetById), new { specialScheduleId = result.Value.SpecialScheduleId }, result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPost("Save")]
        public async Task<ActionResult<SpecialSchedule>> Save([FromBody] SpecialSchedule scheduleModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _specialScheduleService.Save(scheduleModel);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetById), new { specialScheduleId = result.Value.SpecialScheduleId }, result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPut("{specialScheduleId}")]
        public async Task<ActionResult<SpecialSchedule>> Put(int specialScheduleId, [FromBody] SpecialSchedule schedule)
        {
            if (specialScheduleId != schedule.SpecialScheduleId)
            {
                return BadRequest();
            }

            var result = await _specialScheduleService.UpdateSpecialSchedule(schedule);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPut("{specialScheduleId}/SetActive")]
        public async Task<ActionResult<bool>> SetActive(int specialScheduleId, [FromBody] bool isActive)
        {
            var result = await _specialScheduleService.SetActiveStatus(specialScheduleId, isActive);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpDelete("{specialScheduleId}/SetDeleted")]
        public async Task<ActionResult<bool>> SetDeleted(int specialScheduleId, [FromBody] bool isDeleted)
        {
            var result = await _specialScheduleService.SetDeletedStatus(specialScheduleId, isDeleted);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }
    }
}
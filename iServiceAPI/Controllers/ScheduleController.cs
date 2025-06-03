using iServiceRepositories.Repositories.Models;
using iServiceServices.Services;
using Microsoft.AspNetCore.Mvc;

namespace iServiceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly ScheduleService _scheduleService;

        public ScheduleController(IConfiguration configuration)
        {
            _scheduleService = new ScheduleService(configuration);
        }

        [HttpGet]
        public async Task<ActionResult<List<Schedule>>> Get()
        {
            var result = await _scheduleService.GetAllSchedules();

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpGet("{scheduleId}")]
        public async Task<ActionResult<Schedule>> GetById(int scheduleId)
        {
            var result = await _scheduleService.GetScheduleById(scheduleId);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return NotFound(new { message = result.ErrorMessage });
        }

        [HttpGet("GetByUserProfileId/{userProfileId}")]
        public async Task<ActionResult<Schedule>> GetByUserProfileId(int userProfileId)
        {
            var result = await _scheduleService.GetByUserProfileId(userProfileId);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return NotFound(new { message = result.ErrorMessage });
        }

        [HttpPost]
        public async Task<ActionResult<Schedule>> Post([FromBody] Schedule scheduleModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _scheduleService.AddSchedule(scheduleModel);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetById), new { scheduleId = result.Value.ScheduleId }, result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPost("Save")]
        public async Task<ActionResult<Schedule>> Save([FromBody] Schedule scheduleModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _scheduleService.Save(scheduleModel);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetById), new { scheduleId = result.Value.ScheduleId }, result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPut("{scheduleId}")]
        public async Task<ActionResult<Schedule>> Put(int scheduleId, [FromBody] Schedule schedule)
        {
            if (scheduleId != schedule.ScheduleId)
            {
                return BadRequest();
            }

            var result = await _scheduleService.UpdateSchedule(schedule);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPut("{scheduleId}/SetActive")]
        public async Task<ActionResult<bool>> SetActive(int scheduleId, [FromBody] bool isActive)
        {
            var result = await _scheduleService.SetActiveStatus(scheduleId, isActive);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpDelete("{scheduleId}/SetDeleted")]
        public async Task<ActionResult<bool>> SetDeleted(int scheduleId, [FromBody] bool isDeleted)
        {
            var result = await _scheduleService.SetDeletedStatus(scheduleId, isDeleted);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }
    }
}

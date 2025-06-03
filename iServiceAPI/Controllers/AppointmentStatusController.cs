using iServiceRepositories.Repositories.Models;
using iServiceServices.Services;
using Microsoft.AspNetCore.Mvc;

namespace iServiceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppointmentStatusController : ControllerBase
    {
        private readonly AppointmentStatusService _appointmentStatusService;

        public AppointmentStatusController(IConfiguration configuration)
        {
            _appointmentStatusService = new AppointmentStatusService(configuration);
        }

        [HttpGet]
        public async Task<ActionResult<List<AppointmentStatus>>> Get()
        {
            var result = await _appointmentStatusService.GetAllAppointmentStatuses();

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpGet("{appointmentStatusId}")]
        public async Task<ActionResult<AppointmentStatus>> GetById(int appointmentStatusId)
        {
            var result = await _appointmentStatusService.GetAppointmentStatusById(appointmentStatusId);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return NotFound(new { message = result.ErrorMessage });
        }

        [HttpPost]
        public async Task<ActionResult<AppointmentStatus>> Post([FromBody] AppointmentStatus appointmentStatusModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _appointmentStatusService.AddAppointmentStatus(appointmentStatusModel);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetById), new { appointmentStatusId = result.Value.AppointmentStatusId }, result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPut("{appointmentStatusId}")]
        public async Task<ActionResult<AppointmentStatus>> Put(int appointmentStatusId, [FromBody] AppointmentStatus appointmentStatus)
        {
            if (appointmentStatusId != appointmentStatus.AppointmentStatusId)
            {
                return BadRequest();
            }

            var result = await _appointmentStatusService.UpdateAppointmentStatus(appointmentStatus);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPut("{appointmentStatusId}/SetActive")]
        public async Task<ActionResult<bool>> SetActive(int appointmentStatusId, [FromBody] bool isActive)
        {
            var result = await _appointmentStatusService.SetActiveStatus(appointmentStatusId, isActive);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpDelete("{appointmentStatusId}/SetDeleted")]
        public async Task<ActionResult<bool>> SetDeleted(int appointmentStatusId, [FromBody] bool isDeleted)
        {
            var result = await _appointmentStatusService.SetDeletedStatus(appointmentStatusId, isDeleted);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }
    }
}

using iServiceRepositories.Repositories.Models;
using iServiceServices.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iServiceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly AppointmentService _appointmentService;

        public AppointmentController(IConfiguration configuration)
        {
            _appointmentService = new AppointmentService(configuration);
        }

        [HttpGet]
        public async Task<ActionResult<List<Appointment>>> Get()
        {
            var result = await _appointmentService.GetAllAppointments();

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpGet("{appointmentId}")]
        public async Task<ActionResult<Appointment>> GetById(int appointmentId)
        {
            var result = await _appointmentService.GetAppointmentById(appointmentId);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return NotFound(new { message = result.ErrorMessage });
        }

        [HttpGet("GetAllAppointments/{userRoleId}/{userProfileId}")]
        public async Task<ActionResult<List<Appointment>>> GetAllAppointments(int userRoleId, int userProfileId)
        {
            var result = await _appointmentService.GetAllAppointments(userRoleId, userProfileId);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPost]
        public async Task<ActionResult<Appointment>> Post([FromBody] Appointment appointmentModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _appointmentService.AddAppointment(appointmentModel);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetById), new { appointmentId = result.Value.AppointmentId }, result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPut("{appointmentId}")]
        public async Task<ActionResult<Appointment>> Put(int appointmentId, [FromBody] Appointment appointment)
        {
            if (appointmentId != appointment.AppointmentId)
            {
                return BadRequest();
            }

            var result = await _appointmentService.UpdateAppointment(appointment);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPut("{appointmentId}/{status}")]
        [Authorize(Roles = "Establishment,Client")]
        public async Task<ActionResult<Appointment>> Put(int appointmentId, AppointmentStatusEnum status)
        {
            if (appointmentId > 0 == false)
            {
                return BadRequest();
            }

            var jwtToken = TokenService.GetJwtToken(HttpContext);

            var tokenInfo = TokenService.GetTokenInfo(jwtToken);

            var result = await _appointmentService.UpdateStatusAppointment(tokenInfo, appointmentId, status);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPut("{appointmentId}/SetActive")]
        public async Task<ActionResult<bool>> SetActive(int appointmentId, [FromBody] bool isActive)
        {
            var result = await _appointmentService.SetActiveStatus(appointmentId, isActive);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpDelete("{appointmentId}/SetDeleted")]
        public async Task<ActionResult<bool>> SetDeleted(int appointmentId, [FromBody] bool isDeleted)
        {
            var result = await _appointmentService.SetDeletedStatus(appointmentId, isDeleted);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }
    }
}

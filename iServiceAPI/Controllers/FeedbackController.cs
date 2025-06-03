using iServiceRepositories.Repositories.Models;
using iServiceServices.Services;
using Microsoft.AspNetCore.Mvc;

namespace iServiceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly FeedbackService _feedbackService;

        public FeedbackController(IConfiguration configuration)
        {
            _feedbackService = new FeedbackService(configuration);
        }

        [HttpGet]
        public async Task<ActionResult<List<Feedback>>> Get()
        {
            var result = await _feedbackService.GetAllFeedbacks();

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpGet("{feedbackId}")]
        public async Task<ActionResult<Feedback>> GetById(int feedbackId)
        {
            var result = await _feedbackService.GetFeedbackById(feedbackId);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return NotFound(new { message = result.ErrorMessage });
        }

        [HttpPost]
        public async Task<ActionResult<Feedback>> Post([FromBody] Feedback feedbackModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _feedbackService.AddFeedback(feedbackModel);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetById), new { feedbackId = result.Value.FeedbackId }, result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPut("{feedbackId}")]
        public async Task<ActionResult<Feedback>> Put(int feedbackId, [FromBody] Feedback feedback)
        {
            if (feedbackId != feedback.FeedbackId)
            {
                return BadRequest();
            }

            var result = await _feedbackService.UpdateFeedback(feedback);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPut("{feedbackId}/SetActive")]
        public async Task<ActionResult<bool>> SetActive(int feedbackId, [FromBody] bool isActive)
        {
            var result = await _feedbackService.SetActiveStatus(feedbackId, isActive);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpDelete("{feedbackId}/SetDeleted")]
        public async Task<ActionResult<bool>> SetDeleted(int feedbackId, [FromBody] bool isDeleted)
        {
            var result = await _feedbackService.SetDeletedStatus(feedbackId, isDeleted);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }
    }
}

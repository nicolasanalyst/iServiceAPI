using iServiceRepositories.Repositories;
using iServiceRepositories.Repositories.Models;
using iServiceServices.Services.Models;
using Microsoft.Extensions.Configuration;

namespace iServiceServices.Services
{
    public class FeedbackService
    {
        private readonly FeedbackRepository _feedbackRepository;

        public FeedbackService(IConfiguration configuration)
        {
            _feedbackRepository = new FeedbackRepository(configuration);
        }
        
        public async Task<Result<List<Feedback>>> GetAllFeedbacks()
        {
            try
            {
                var feedbacks = await _feedbackRepository.GetAsync();
                return Result<List<Feedback>>.Success(feedbacks);
            }
            catch (Exception ex)
            {
                return Result<List<Feedback>>.Failure($"Falha ao obter os feedbacks: {ex.Message}");
            }
        }

        public async Task<Result<Feedback>> GetFeedbackById(int feedbackId)
        {
            try
            {
                var feedback = await _feedbackRepository.GetByIdAsync(feedbackId);

                if (feedback == null)
                {
                    return Result<Feedback>.Failure("Feedback não encontrado.");
                }

                return Result<Feedback>.Success(feedback);
            }
            catch (Exception ex)
            {
                return Result<Feedback>.Failure($"Falha ao obter o feedback: {ex.Message}");
            }
        }

        public async Task<Result<Feedback>> GetByAppointmentId(int appointmentId)
        {
            try
            {
                var feedback = await _feedbackRepository.GetByAppointmentIdAsync(appointmentId);

                if (feedback == null)
                {
                    return Result<Feedback>.Failure("Feedback não encontrado.");
                }

                return Result<Feedback>.Success(feedback);
            }
            catch (Exception ex)
            {
                return Result<Feedback>.Failure($"Falha ao obter o feedback: {ex.Message}");
            }
        }

        public async Task<Result<Feedback>> AddFeedback(Feedback feedbackModel)
        {
            try
            {
                var newFeedback = await _feedbackRepository.InsertAsync(feedbackModel);
                return Result<Feedback>.Success(newFeedback);
            }
            catch (Exception ex)
            {
                return Result<Feedback>.Failure($"Falha ao inserir o feedback: {ex.Message}");
            }
        }

        public async Task<Result<Feedback>> UpdateFeedback(Feedback feedback)
        {
            try
            {
                var updatedFeedback = await _feedbackRepository.UpdateAsync(feedback);
                return Result<Feedback>.Success(updatedFeedback);
            }
            catch (Exception ex)
            {
                return Result<Feedback>.Failure($"Falha ao atualizar o feedback: {ex.Message}");
            }
        }

        public async Task<Result<bool>> SetActiveStatus(int feedbackId, bool isActive)
        {
            try
            {
                await _feedbackRepository.SetActiveStatusAsync(feedbackId, isActive);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Falha ao definir o status ativo do feedback: {ex.Message}");
            }
        }

        public async Task<Result<bool>> SetDeletedStatus(int feedbackId, bool isDeleted)
        {
            try
            {
                await _feedbackRepository.SetDeletedStatusAsync(feedbackId, isDeleted);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Falha ao definir o status excluído do feedback: {ex.Message}");
            }
        }
    }
}

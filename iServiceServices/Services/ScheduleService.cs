using iServiceRepositories.Repositories;
using iServiceRepositories.Repositories.Models;
using iServiceServices.Services.Models;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace iServiceServices.Services
{
    public class ScheduleService
    {
        private readonly ScheduleRepository _scheduleRepository;

        public ScheduleService(IConfiguration configuration)
        {
            _scheduleRepository = new ScheduleRepository(configuration);
        }

        public async Task<Result<List<Schedule>>> GetAllSchedules()
        {
            try
            {
                var schedules = await _scheduleRepository.GetAsync();
                return Result<List<Schedule>>.Success(schedules);
            }
            catch (Exception ex)
            {
                return Result<List<Schedule>>.Failure($"Falha ao obter os horários: {ex.Message}");
            }
        }

        public async Task<Result<Schedule>> GetScheduleById(int scheduleId)
        {
            try
            {
                var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);

                if (schedule == null)
                {
                    return Result<Schedule>.Failure("Horário não encontrado.");
                }

                return Result<Schedule>.Success(schedule);
            }
            catch (Exception ex)
            {
                return Result<Schedule>.Failure($"Falha ao obter o horário: {ex.Message}");
            }
        }

        public async Task<Result<Schedule>> GetByUserProfileId(int userProfileId)
        {
            try
            {
                var schedule = await _scheduleRepository.GetByEstablishmentUserProfileIdAsync(userProfileId);

                if (schedule == null)
                {
                    return Result<Schedule>.Failure("Horário não encontrado.");
                }

                return Result<Schedule>.Success(schedule);
            }
            catch (Exception ex)
            {
                return Result<Schedule>.Failure($"Falha ao obter o horário: {ex.Message}");
            }
        }

        public async Task<Result<Schedule>> AddSchedule(Schedule scheduleModel)
        {
            try
            {
                var newSchedule = await _scheduleRepository.InsertAsync(scheduleModel);
                return Result<Schedule>.Success(newSchedule);
            }
            catch (Exception ex)
            {
                return Result<Schedule>.Failure($"Falha ao inserir o horário: {ex.Message}");
            }
        }

        public async Task<Result<Schedule>> Save(Schedule schedule)
        {
            try
            {
                if (schedule.ScheduleId > 0)
                {
                    schedule = await _scheduleRepository.UpdateAsync(schedule);
                }
                else
                {
                    schedule = await _scheduleRepository.InsertAsync(schedule);
                }
                return Result<Schedule>.Success(schedule);
            }
            catch (Exception ex)
            {
                return Result<Schedule>.Failure($"Falha ao inserir o horário: {ex.Message}");
            }
        }

        public async Task<Result<Schedule>> UpdateSchedule(Schedule schedule)
        {
            try
            {
                var updatedSchedule = await _scheduleRepository.UpdateAsync(schedule);
                return Result<Schedule>.Success(updatedSchedule);
            }
            catch (Exception ex)
            {
                return Result<Schedule>.Failure($"Falha ao atualizar o horário: {ex.Message}");
            }
        }

        public async Task<Result<bool>> SetActiveStatus(int scheduleId, bool isActive)
        {
            try
            {
                await _scheduleRepository.SetActiveStatusAsync(scheduleId, isActive);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Falha ao definir o status ativo do horário: {ex.Message}");
            }
        }

        public async Task<Result<bool>> SetDeletedStatus(int scheduleId, bool isDeleted)
        {
            try
            {
                await _scheduleRepository.SetDeletedStatusAsync(scheduleId, isDeleted);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Falha ao definir o status excluído do horário: {ex.Message}");
            }
        }
    }
}

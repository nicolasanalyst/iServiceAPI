using iServiceRepositories.Repositories;
using iServiceRepositories.Repositories.Models;
using iServiceServices.Services.Models;
using Microsoft.Extensions.Configuration;

namespace iServiceServices.Services
{
    public class SpecialScheduleService
    {
        private readonly SpecialScheduleRepository _specialScheduleRepository;

        public SpecialScheduleService(IConfiguration configuration)
        {
            _specialScheduleRepository = new SpecialScheduleRepository(configuration);
        }

        public async Task<Result<List<SpecialSchedule>>> GetAllSpecialSchedules()
        {
            try
            {
                var specialSchedules = await _specialScheduleRepository.GetAsync();
                return Result<List<SpecialSchedule>>.Success(specialSchedules);
            }
            catch (Exception ex)
            {
                return Result<List<SpecialSchedule>>.Failure($"Falha ao obter os horários especiais: {ex.Message}");
            }
        }

        public async Task<Result<SpecialSchedule>> GetSpecialScheduleById(int scheduleId)
        {
            try
            {
                var specialSchedule = await _specialScheduleRepository.GetByIdAsync(scheduleId);

                if (specialSchedule == null)
                {
                    return Result<SpecialSchedule>.Failure("Horário especial não encontrado.");
                }

                return Result<SpecialSchedule>.Success(specialSchedule);
            }
            catch (Exception ex)
            {
                return Result<SpecialSchedule>.Failure($"Falha ao obter o horário especial: {ex.Message}");
            }
        }

        public async Task<Result<List<SpecialSchedule>>> GetByUserProfileId(int userProfileId)
        {
            try
            {
                var specialSchedule = await _specialScheduleRepository.GetByUserProfileIdAsync(userProfileId);

                if (specialSchedule == null)
                {
                    return Result<List<SpecialSchedule>>.Failure("Horário especial não encontrado.");
                }

                return Result<List<SpecialSchedule>>.Success(specialSchedule);
            }
            catch (Exception ex)
            {
                return Result<List<SpecialSchedule>>.Failure($"Falha ao obter o horário especial: {ex.Message}");
            }
        }

        public async Task<Result<SpecialSchedule>> AddSpecialSchedule(SpecialSchedule scheduleModel)
        {
            try
            {
                var newSpecialSchedule = await _specialScheduleRepository.InsertAsync(scheduleModel);
                return Result<SpecialSchedule>.Success(newSpecialSchedule);
            }
            catch (Exception ex)
            {
                return Result<SpecialSchedule>.Failure($"Falha ao inserir o horário especial: {ex.Message}");
            }
        }

        public async Task<Result<SpecialSchedule>> Save(SpecialSchedule schedule)
        {
            try
            {
                if (schedule.SpecialScheduleId > 0)
                {
                    schedule = await _specialScheduleRepository.UpdateAsync(schedule);
                }
                else
                {
                    schedule = await _specialScheduleRepository.InsertAsync(schedule);
                }
                return Result<SpecialSchedule>.Success(schedule);
            }
            catch (Exception ex)
            {
                return Result<SpecialSchedule>.Failure($"Falha ao inserir o horário especial: {ex.Message}");
            }
        }

        public async Task<Result<SpecialSchedule>> UpdateSpecialSchedule(SpecialSchedule schedule)
        {
            try
            {
                var updatedSpecialSchedule = await _specialScheduleRepository.UpdateAsync(schedule);
                return Result<SpecialSchedule>.Success(updatedSpecialSchedule);
            }
            catch (Exception ex)
            {
                return Result<SpecialSchedule>.Failure($"Falha ao atualizar o horário especial: {ex.Message}");
            }
        }

        public async Task<Result<bool>> SetActiveStatus(int scheduleId, bool isActive)
        {
            try
            {
                await _specialScheduleRepository.SetActiveStatusAsync(scheduleId, isActive);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Falha ao definir o status ativo do horário especial: {ex.Message}");
            }
        }

        public async Task<Result<bool>> SetDeletedStatus(int scheduleId, bool isDeleted)
        {
            try
            {
                await _specialScheduleRepository.SetDeletedStatusAsync(scheduleId, isDeleted);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Falha ao definir o status excluído do horário especial: {ex.Message}");
            }
        }
    }

}

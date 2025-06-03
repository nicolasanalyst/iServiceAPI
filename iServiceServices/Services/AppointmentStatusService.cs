using iServiceRepositories.Repositories;
using iServiceRepositories.Repositories.Models;
using iServiceServices.Services.Models;
using Microsoft.Extensions.Configuration;

namespace iServiceServices.Services
{
    public class AppointmentStatusService
    {
        private readonly AppointmentStatusRepository _appointmentStatusRepository;

        public AppointmentStatusService(IConfiguration configuration)
        {
            _appointmentStatusRepository = new AppointmentStatusRepository(configuration);
        }

        public async Task<Result<List<AppointmentStatus>>> GetAllAppointmentStatuses()
        {
            try
            {
                var appointmentStatuses = await _appointmentStatusRepository.GetAsync();
                return Result<List<AppointmentStatus>>.Success(appointmentStatuses);
            }
            catch (Exception ex)
            {
                return Result<List<AppointmentStatus>>.Failure($"Falha ao obter os status dos agendamentos: {ex.Message}");
            }
        }

        public async Task<Result<AppointmentStatus>> GetAppointmentStatusById(int appointmentStatusId)
        {
            try
            {
                var appointmentStatus = await _appointmentStatusRepository.GetByIdAsync(appointmentStatusId);

                if (appointmentStatus == null)
                {
                    return Result<AppointmentStatus>.Failure("Status do agendamento não encontrado.");
                }

                return Result<AppointmentStatus>.Success(appointmentStatus);
            }
            catch (Exception ex)
            {
                return Result<AppointmentStatus>.Failure($"Falha ao obter o status do agendamento: {ex.Message}");
            }
        }

        public async Task<Result<AppointmentStatus>> AddAppointmentStatus(AppointmentStatus appointmentStatusModel)
        {
            try
            {
                var newAppointmentStatus = await _appointmentStatusRepository.InsertAsync(appointmentStatusModel);
                return Result<AppointmentStatus>.Success(newAppointmentStatus);
            }
            catch (Exception ex)
            {
                return Result<AppointmentStatus>.Failure($"Falha ao inserir o status do agendamento: {ex.Message}");
            }
        }

        public async Task<Result<AppointmentStatus>> UpdateAppointmentStatus(AppointmentStatus appointmentStatus)
        {
            try
            {
                var updatedAppointmentStatus = await _appointmentStatusRepository.UpdateAsync(appointmentStatus);
                return Result<AppointmentStatus>.Success(updatedAppointmentStatus);
            }
            catch (Exception ex)
            {
                return Result<AppointmentStatus>.Failure($"Falha ao atualizar o status do agendamento: {ex.Message}");
            }
        }

        public async Task<Result<bool>> SetActiveStatus(int appointmentStatusId, bool isActive)
        {
            try
            {
                await _appointmentStatusRepository.SetActiveStatusAsync(appointmentStatusId, isActive);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Falha ao definir o status ativo do status do agendamento: {ex.Message}");
            }
        }

        public async Task<Result<bool>> SetDeletedStatus(int appointmentStatusId, bool isDeleted)
        {
            try
            {
                await _appointmentStatusRepository.SetDeletedStatusAsync(appointmentStatusId, isDeleted);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Falha ao definir o status excluído do status do agendamento: {ex.Message}");
            }
        }
    }
}

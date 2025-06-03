using iServiceRepositories.Repositories;
using iServiceRepositories.Repositories.Models;
using iServiceServices.Services.Models;
using Microsoft.Extensions.Configuration;

namespace iServiceServices.Services
{
    public class AppointmentService
    {
        private readonly AppointmentRepository _appointmentRepository;
        private readonly UserInfoService _userInfoService;
        private readonly ServiceService _serviceService;
        private readonly FeedbackService _feedbackService;
        private readonly EstablishmentEmployeeRepository _establishmentEmployeeRepository;
        public AppointmentService(IConfiguration configuration)
        {
            _appointmentRepository = new AppointmentRepository(configuration);
            _userInfoService = new UserInfoService(configuration);
            _serviceService = new ServiceService(configuration);
            _feedbackService = new FeedbackService(configuration);
            _establishmentEmployeeRepository = new EstablishmentEmployeeRepository(configuration);
        }

        public async Task<Result<List<Appointment>>> GetAllAppointments()
        {
            try
            {
                var appointments = await _appointmentRepository.GetAsync();
                return Result<List<Appointment>>.Success(appointments);
            }
            catch (Exception ex)
            {
                return Result<List<Appointment>>.Failure($"Falha ao obter os agendamentos: {ex.Message}");
            }
        }

        public async Task<Result<List<Appointment>>> GetAllAppointments(int userRoleId, int userProfileId)
        {
            try
            {
                var appointments = new List<Appointment>();

                if (userRoleId == 2)
                {
                    appointments = await _appointmentRepository.GetEstablishmentAppointmentsAsync(userProfileId);
                }
                else
                {
                    appointments = await _appointmentRepository.GetClientAppointmentsAsync(userProfileId);
                }

                if (appointments?.Count > 0)
                {
                    foreach (var appointment in appointments)
                    {
                        if (userRoleId == 2)
                        {
                            var clientUserInfo = await _userInfoService.GetUserInfoByUserProfileId(appointment.ClientUserProfileId);
                            appointment.ClientUserInfo = clientUserInfo.Value;
                        }
                        else
                        {
                            var establishmentUserInfo = await _userInfoService.GetUserInfoByUserProfileId(appointment.EstablishmentUserProfileId);
                            appointment.EstablishmentUserInfo = establishmentUserInfo.Value;
                        }

                        var service = await _serviceService.GetServiceById(appointment.ServiceId);
                        appointment.Service = service.Value;

                        if (appointment?.EstablishmentEmployeeId > 0) 
                        { 
                            appointment.EstablishmentEmployee = await _establishmentEmployeeRepository.GetByIdAsync(appointment.EstablishmentEmployeeId);
                        }

                        var feedback = await _feedbackService.GetByAppointmentId(appointment.AppointmentId);
                        if (feedback.IsSuccess)
                        {
                            appointment.Feedback = feedback.Value;
                        }
                    }

                    _ = appointments.OrderBy(d => d.Start).ToList();
                }

                return Result<List<Appointment>>.Success(appointments);
            }
            catch (Exception ex)
            {
                return Result<List<Appointment>>.Failure($"Falha ao obter os agendamentos: {ex.Message}");
            }
        }

        public async Task<Result<bool>> UpdateStatusAppointment(TokenInfo tokenInfo, int appointmentId, AppointmentStatusEnum status)
        {
            var appointment = new Appointment();
            switch (tokenInfo.Role)
            {
                case "Establishment":
                    appointment = await _appointmentRepository.GetByFilterAsync(appointmentId, null, tokenInfo.UserProfileId);
                    break;
                case "Client":
                    appointment = await _appointmentRepository.GetByFilterAsync(appointmentId, tokenInfo.UserProfileId, null);
                    break;
                default:
                    return Result<bool>.Failure("Não foi possível realizar o cancelamento do seu agendamento, favor contatar o suporte!");
            }

            switch (status)
            {
                case AppointmentStatusEnum.Novo:
                    break;
                case AppointmentStatusEnum.Confirmado:
                    break;
                case AppointmentStatusEnum.Iniciado:
                    if (appointment.AppointmentStatusId == AppointmentStatusEnum.Novo || appointment.AppointmentStatusId == AppointmentStatusEnum.Confirmado)
                    {
                        appointment.AppointmentStatusId = AppointmentStatusEnum.Iniciado;
                        appointment.StartTime = DateTime.Now;
                        await _appointmentRepository.UpdateAsync(appointment);
                        return Result<bool>.Success(true);
                    }
                    break;
                case AppointmentStatusEnum.Finalizado:
                    if (appointment.AppointmentStatusId == AppointmentStatusEnum.Iniciado)
                    {
                        appointment.AppointmentStatusId = AppointmentStatusEnum.Finalizado;
                        appointment.EndTime = DateTime.Now;
                        await _appointmentRepository.UpdateAsync(appointment);
                        return Result<bool>.Success(true);
                    }
                    break;
                case AppointmentStatusEnum.Cancelado:
                    if (appointment.AppointmentStatusId == AppointmentStatusEnum.Novo || appointment.AppointmentStatusId == AppointmentStatusEnum.Confirmado)
                    {
                        var now = DateTime.Now;
                        if (appointment.Start - TimeSpan.FromMinutes(30) <= now && now < appointment.Start)
                        {
                            throw new Exception("Não é possivel realizar o cancelamento, faltam menos de 30 minutos para o horário.");
                        }

                        appointment.AppointmentStatusId = AppointmentStatusEnum.Cancelado;
                        var updatedAppointment = await _appointmentRepository.UpdateAsync(appointment);

                        if (updatedAppointment.AppointmentStatusId == AppointmentStatusEnum.Cancelado)
                        {
                            return Result<bool>.Success(true);
                        }
                    }
                    break;
            }

            return Result<bool>.Failure("Não foi possível realizar o cancelamento do seu agendamento, favor contatar o suporte!");
        }

        public async Task<Result<Appointment>> GetAppointmentById(int appointmentId)
        {
            try
            {
                var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);

                if (appointment == null)
                {
                    return Result<Appointment>.Failure("Agendamento não encontrado.");
                }

                return Result<Appointment>.Success(appointment);
            }
            catch (Exception ex)
            {
                return Result<Appointment>.Failure($"Falha ao obter o agendamento: {ex.Message}");
            }
        }

        public async Task<Result<Appointment>> AddAppointment(Appointment appointmentModel)
        {
            try
            {
                if (appointmentModel?.EstablishmentEmployeeId > 0 == false)
                {
                    var establishmentEmployees = await _establishmentEmployeeRepository
                        .GetEmployeeAvailability(appointmentModel.ServiceId, appointmentModel.Start);

                    appointmentModel.EstablishmentEmployeeId = establishmentEmployees
                        .Where(ee => ee.IsAvailable == true)
                        .OrderBy(ee => ee.AppointmentCount.GetValueOrDefault())
                        .FirstOrDefault()?.EstablishmentEmployeeId ?? 0;

                    if (appointmentModel.EstablishmentEmployeeId == 0)
                    {
                        return Result<Appointment>.Failure("Falha ao inserir o agendamento: Nenhum funcionário disponível no momento.");
                    }
                }
                var newAppointment = await _appointmentRepository.InsertAsync(appointmentModel);
                return Result<Appointment>.Success(newAppointment);
            }
            catch (Exception ex)
            {
                return Result<Appointment>.Failure($"Falha ao inserir o agendamento: {ex.Message}");
            }
        }

        public async Task<Result<Appointment>> UpdateAppointment(Appointment appointment)
        {
            try
            {
                var updatedAppointment = await _appointmentRepository.UpdateAsync(appointment);
                return Result<Appointment>.Success(updatedAppointment);
            }
            catch (Exception ex)
            {
                return Result<Appointment>.Failure($"Falha ao atualizar o agendamento: {ex.Message}");
            }
        }

        public async Task<Result<bool>> SetActiveStatus(int appointmentId, bool isActive)
        {
            try
            {
                await _appointmentRepository.SetActiveStatusAsync(appointmentId, isActive);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Falha ao definir o status ativo do agendamento: {ex.Message}");
            }
        }

        public async Task<Result<bool>> SetDeletedStatus(int appointmentId, bool isDeleted)
        {
            try
            {
                await _appointmentRepository.SetDeletedStatusAsync(appointmentId, isDeleted);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Falha ao definir o status excluído do agendamento: {ex.Message}");
            }
        }
    }
}

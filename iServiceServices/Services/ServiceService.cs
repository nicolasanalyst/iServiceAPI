using iServiceRepositories.Repositories;
using iServiceRepositories.Repositories.Models;
using iServiceServices.Services.Models;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace iServiceServices.Services
{
    public class ServiceService
    {
        private readonly ServiceRepository _serviceRepository;
        private readonly ScheduleRepository _scheduleRepository;
        private readonly SpecialScheduleRepository _specialScheduleRepository;
        private readonly AppointmentRepository _appointmentRepository;
        private readonly ServiceCategoryRepository _serviceCategoryRepository;
        private readonly EstablishmentEmployeeRepository _establishmentEmployeeRepository;
        private readonly ServiceEmployeeRepository _serviceEmployeeRepository;
        public ServiceService(IConfiguration configuration)
        {
            _serviceRepository = new ServiceRepository(configuration);
            _scheduleRepository = new ScheduleRepository(configuration);
            _specialScheduleRepository = new SpecialScheduleRepository(configuration);
            _appointmentRepository = new AppointmentRepository(configuration);
            _serviceCategoryRepository = new ServiceCategoryRepository(configuration);
            _establishmentEmployeeRepository = new EstablishmentEmployeeRepository(configuration);
            _serviceEmployeeRepository = new ServiceEmployeeRepository(configuration);
        }

        public async Task<Result<List<Service>>> GetAllServices()
        {
            try
            {
                var services = await _serviceRepository.GetAsync();
                return Result<List<Service>>.Success(services);
            }
            catch (Exception ex)
            {
                return Result<List<Service>>.Failure($"Falha ao obter os serviços: {ex.Message}");
            }
        }

        public async Task<Result<List<Service>>> Search(string service, int pageSize, int currentPage)
        {
            try
            {
                var services = await _serviceRepository.Search(service, pageSize, currentPage);
                return Result<List<Service>>.Success(services);
            }
            catch (Exception ex)
            {
                return Result<List<Service>>.Failure($"Falha ao obter os serviços: {ex.Message}");
            }
        }

        public async Task<Result<Service>> GetServiceById(int serviceId)
        {
            try
            {
                var service = await _serviceRepository.GetByIdAsync(serviceId);

                if (service?.ServiceId > 0 == false)
                {
                    return Result<Service>.Failure("Serviço não encontrado.");
                }

                var serviceCategory = await _serviceCategoryRepository.GetByIdAsync(service.ServiceCategoryId);

                if (serviceCategory?.ServiceCategoryId > 0 == false)
                {
                    return Result<Service>.Failure("Serviço não encontrado.");
                }

                service.ServiceCategory = serviceCategory;

                return Result<Service>.Success(service);
            }
            catch (Exception ex)
            {
                return Result<Service>.Failure($"Falha ao obter o serviço: {ex.Message}");
            }
        }

        public async Task<Result<List<Service>>> GetServiceByUserProfileId(int userProfileId)
        {
            try
            {
                var services = await _serviceRepository.GetServiceByEstablishmentUserProfileIdAsync(userProfileId);

                if (services?.Count > 0 == false)
                {
                    return Result<List<Service>>.Success(new List<Service>());
                }

                foreach (var service in services)
                {
                    var serviceCategory = await _serviceCategoryRepository.GetByIdAsync(service.ServiceCategoryId);

                    if (serviceCategory?.ServiceCategoryId > 0 == false)
                    {
                        return Result<List<Service>>.Failure("Serviço não encontrado.");
                    }

                    service.ServiceCategory = serviceCategory;
                }

                return Result<List<Service>>.Success(services);
            }
            catch (Exception ex)
            {
                return Result<List<Service>>.Failure($"Falha ao obter o serviço: {ex.Message}");
            }
        }

        public async Task<Result<List<string>>> GetAvailableTimes(int serviceId, DateTime date)
        {
            try
            {
                var schedules = new List<string>();
                var service = await _serviceRepository.GetByIdAsync(serviceId);
                var schedule = await _scheduleRepository.GetByEstablishmentUserProfileIdAsync(service.EstablishmentUserProfileId);
                var appointments = await _appointmentRepository.GetByEstablishmentAndDate(service.EstablishmentUserProfileId, date);
                var specialSchedule = await _specialScheduleRepository.GetByEstablishmentAndDate(service.EstablishmentUserProfileId, date);
                var establishmentEmployees = await _establishmentEmployeeRepository.GetByEstablishmentUserProfileIdAsync(service.EstablishmentUserProfileId, service.ServiceId);
                var appointmentFinder = new AppointmentFinderService();
                var availableSlots = appointmentFinder.FindAvailableSlots(schedule, specialSchedule, service, date, appointments, establishmentEmployees);
                if (availableSlots.Any())
                {
                    foreach (var slot in availableSlots)
                    {
                        schedules.Add(slot.ToString("hh\\:mm"));
                    }
                }
                else
                {
                    return Result<List<string>>.Failure($"Nenhum horário disponível.");
                }

                return Result<List<string>>.Success(schedules);
            }
            catch (Exception ex)
            {
                return Result<List<string>>.Failure($"Falha ao obter os serviços: {ex.Message}");
            }
        }

        public async Task<Result<Service>> AddService(Service request)
        {
            try
            {
                var serviceCategory = await _serviceCategoryRepository.GetByFilterAsync(request.EstablishmentUserProfileId, request.ServiceCategoryId);

                if (serviceCategory?.ServiceCategoryId > 0 == false)
                {
                    return Result<Service>.Failure($"Falha ao buscar a categoria.");
                }

                _ = decimal.TryParse(request.PriceNet.Replace('.', ','), out decimal value);

                request.Price = value > 0 ? value : 0;

                var newService = await _serviceRepository.InsertAsync(request);

                if (newService?.ServiceId > 0 == false)
                {
                    return Result<Service>.Failure("Falha ao inserir o serviço.");
                }

                if (request.File != null)
                {
                    var image = await UpdateServiceImage(new ImageModel
                    {
                        Id = newService.ServiceId,
                        File = request.File,
                    });
                    newService.ServiceImage = image.Value;
                }

                if (string.IsNullOrEmpty(request.EstablishmentEmployeeIds) == false)
                {
                    var requestedEmployees = request.EstablishmentEmployeeIds
                        .Split(',')
                        .Select(int.Parse)
                        .ToList();

                    var employees = await _establishmentEmployeeRepository.GetAsync(newService.EstablishmentUserProfileId);

                    if (requestedEmployees.All(id => employees.Any(emp => emp.EstablishmentEmployeeId == id)))
                    {
                        foreach (var employee in requestedEmployees)
                        {
                            _ = _serviceEmployeeRepository.InsertAsync(new ServiceEmployee
                            {
                                ServiceEmployeeId = 0,
                                EstablishmentEmployeeId = employee,
                                ServiceId = newService.ServiceId,
                                Active = true,
                                Deleted = false,
                                CreationDate = DateTime.Now,
                                LastUpdateDate = DateTime.Now
                            });
                        }
                    }
                    else
                    {
                        return Result<Service>.Failure("Erro inserir os funcionários neste serviço.");
                    }
                }

                return Result<Service>.Success(newService);
            }
            catch (Exception ex)
            {
                return Result<Service>.Failure($"Falha ao inserir o serviço: {ex.Message}");
            }
        }

        public async Task<Result<Service>> UpdateService(Service request)
        {
            try
            {
                var service = await _serviceRepository.GetByIdAsync(request.ServiceId);

                if (service?.ServiceId > 0 == false)
                {
                    return Result<Service>.Failure("Serviço não encontrado.");
                }

                var serviceCategory = await _serviceCategoryRepository.GetByIdAsync(request.ServiceCategoryId);

                if (serviceCategory?.ServiceCategoryId > 0 == false)
                {
                    return Result<Service>.Failure("Categoria não encontrado.");
                }

                if (request.File != null)
                {
                    var image = await UpdateServiceImage(new ImageModel
                    {
                        Id = request.ServiceId,
                        File = request.File
                    });
                    request.ServiceImage = image.Value;
                }

                _ = decimal.TryParse(request.PriceNet.Replace('.', ','), out decimal value);

                request.Price = value > 0 ? value : 0;

                var updatedService = await _serviceRepository.UpdateAsync(request);

                if (string.IsNullOrEmpty(request.EstablishmentEmployeeIds) == false)
                {
                    var requestedEmployees = request.EstablishmentEmployeeIds
                        .Split(',')
                        .Select(int.Parse)
                        .ToList();

                    var employees = await _establishmentEmployeeRepository.GetAsync(updatedService.EstablishmentUserProfileId);
                    var serviceEmployees = await _serviceEmployeeRepository.GetByServiceIdAsync(updatedService.ServiceId);

                    var serviceEmployeeIds = serviceEmployees.Select(se => se.EstablishmentEmployeeId).ToList();

                    var employeesToDelete = serviceEmployeeIds.Where(id => !requestedEmployees.Contains(id)).ToList();
                    foreach (var id in employeesToDelete)
                    {
                        await _serviceEmployeeRepository.SetDeletedByEstablishmentEmployeeIdAsync(id, true);
                    }

                    var newEmployeeIds = requestedEmployees.Where(id => !serviceEmployeeIds.Contains(id)).ToList();

                    foreach (var id in newEmployeeIds)
                    {
                        if (employees.Any(emp => emp.EstablishmentEmployeeId == id))
                        {
                            var newServiceEmployee = new ServiceEmployee
                            {
                                ServiceEmployeeId = 0,
                                ServiceId = updatedService.ServiceId,
                                EstablishmentEmployeeId = id,
                                Active = true,
                                Deleted = false,
                                CreationDate = DateTime.Now,
                                LastUpdateDate = DateTime.Now
                            };
                            await _serviceEmployeeRepository.InsertAsync(newServiceEmployee);
                        }
                        else
                        {
                            return Result<Service>.Failure("Erro inserir os funcionários neste serviço.");
                        }
                    }
                }

                return Result<Service>.Success(updatedService);
            }
            catch (Exception ex)
            {
                return Result<Service>.Failure($"Falha ao atualizar o serviço: {ex.Message}");
            }
        }

        public async Task<Result<bool>> SetActiveStatus(int serviceId, bool isActive)
        {
            try
            {
                await _serviceRepository.SetActiveStatusAsync(serviceId, isActive);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Falha ao definir o status ativo do serviço: {ex.Message}");
            }
        }

        public async Task<Result<bool>> SetDeletedStatus(int serviceId, bool isDeleted)
        {
            try
            {
                await _serviceRepository.SetDeletedStatusAsync(serviceId, isDeleted);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Falha ao definir o status excluído do serviço: {ex.Message}");
            }
        }

        private async Task<Result<string>> UpdateServiceImage(ImageModel model)
        {
            try
            {
                if (model.File == null)
                {
                    return Result<string>.Failure("Falha ao ler o arquivo.");
                }

                var path = await new FtpServices().UploadFileAsync(model.File, "service", $"service{model.Id}.png");

                if (string.IsNullOrEmpty(path))
                {
                    return Result<string>.Failure($"Falha ao subir o arquivo de imagem.");
                }

                if (await _serviceRepository.UpdateServiceImageAsync(model.Id, path))
                {
                    return Result<string>.Success(path);
                }

                return Result<string>.Failure("Falha ao atualizar a foto de perfil do usuário.");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Falha ao inserir o perfil de cliente: {ex.Message}");
            }
        }
    }
}

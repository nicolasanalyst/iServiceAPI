using iServiceRepositories.Repositories;
using iServiceRepositories.Repositories.Models;
using iServiceServices.Services.Models;
using Microsoft.Extensions.Configuration;

namespace iServiceServices.Services
{
    public class EstablishmentEmployeeService
    {
        private readonly EstablishmentEmployeeRepository _establishmentEmployeeRepository;

        public EstablishmentEmployeeService(IConfiguration configuration)
        {
            _establishmentEmployeeRepository = new EstablishmentEmployeeRepository(configuration);
        }

        public async Task<Result<List<EstablishmentEmployee>>> GetAllEstablishmentEmployees(TokenInfo tokenInfo)
        {
            try
            {
                var EstablishmentEmployees = await _establishmentEmployeeRepository.GetAsync(tokenInfo.UserProfileId);
                return Result<List<EstablishmentEmployee>>.Success(EstablishmentEmployees);
            }
            catch (Exception ex)
            {
                return Result<List<EstablishmentEmployee>>.Failure($"Falha ao obter os EstablishmentEmployees: {ex.Message}");
            }
        }

        public async Task<Result<List<EstablishmentEmployee>>> GetEmployeeAvailability(TokenInfo tokenInfo, int serviceId, DateTime start)
        {
            try
            {
                var EstablishmentEmployees = await _establishmentEmployeeRepository.GetEmployeeAvailability(serviceId, start);
                return Result<List<EstablishmentEmployee>>.Success(EstablishmentEmployees);
            }
            catch (Exception ex)
            {
                return Result<List<EstablishmentEmployee>>.Failure($"Falha ao obter os EstablishmentEmployees: {ex.Message}");
            }
        }

        public async Task<Result<List<EstablishmentEmployee>>> GetEmployeeByService(TokenInfo tokenInfo, int serviceId)
        {
            try
            {
                var EstablishmentEmployees = await _establishmentEmployeeRepository.GetAllByEstablishmentUserProfileIdAsync(tokenInfo.UserProfileId, serviceId);
                return Result<List<EstablishmentEmployee>>.Success(EstablishmentEmployees);
            }
            catch (Exception ex)
            {
                return Result<List<EstablishmentEmployee>>.Failure($"Falha ao obter os EstablishmentEmployees: {ex.Message}");
            }
        }

        public async Task<Result<EstablishmentEmployee>> GetEstablishmentEmployeeById(TokenInfo tokenInfo, int EstablishmentEmployeeId)
        {
            try
            {
                var EstablishmentEmployee = await _establishmentEmployeeRepository.GetByIdAsync(EstablishmentEmployeeId, tokenInfo.UserProfileId);

                if (EstablishmentEmployee == null)
                {
                    return Result<EstablishmentEmployee>.Failure("EstablishmentEmployee não encontrado.");
                }

                return Result<EstablishmentEmployee>.Success(EstablishmentEmployee);
            }
            catch (Exception ex)
            {
                return Result<EstablishmentEmployee>.Failure($"Falha ao obter o EstablishmentEmployee: {ex.Message}");
            }
        }

        public async Task<Result<EstablishmentEmployee>> AddEstablishmentEmployee(TokenInfo tokenInfo, EstablishmentEmployee EstablishmentEmployeeModel)
        {
            try
            {
                EstablishmentEmployeeModel.EstablishmentUserProfileId = tokenInfo.UserProfileId;
                var newEstablishmentEmployee = await _establishmentEmployeeRepository.InsertAsync(EstablishmentEmployeeModel);

                if (newEstablishmentEmployee != null)
                {
                    if (EstablishmentEmployeeModel.File != null)
                    {
                        var path = await new FtpServices().UploadFileAsync(EstablishmentEmployeeModel.File, "employee", $"employee_{newEstablishmentEmployee.EstablishmentEmployeeId}.png");

                        newEstablishmentEmployee = await _establishmentEmployeeRepository.UpdateImageAsync(newEstablishmentEmployee.EstablishmentEmployeeId, path);
                    }
                }

                return Result<EstablishmentEmployee>.Success(newEstablishmentEmployee);
            }
            catch (Exception ex)
            {
                return Result<EstablishmentEmployee>.Failure($"Falha ao inserir o EstablishmentEmployee: {ex.Message}");
            }
        }

        public async Task<Result<EstablishmentEmployee>> UpdateEstablishmentEmployee(TokenInfo tokenInfo, EstablishmentEmployee EstablishmentEmployee)
        {
            try
            {
                var establishmentEmployee = await _establishmentEmployeeRepository.GetByIdAsync(EstablishmentEmployee.EstablishmentEmployeeId);

                if (establishmentEmployee != null)
                {
                    if (establishmentEmployee.EstablishmentUserProfileId == tokenInfo.UserProfileId)
                    {
                        EstablishmentEmployee.EstablishmentUserProfileId = tokenInfo.UserProfileId;
                        var updatedEstablishmentEmployee = await _establishmentEmployeeRepository.UpdateAsync(EstablishmentEmployee);

                        if (updatedEstablishmentEmployee != null)
                        {
                            if (EstablishmentEmployee.File != null)
                            {
                                var path = await new FtpServices().UploadFileAsync(EstablishmentEmployee.File, "employee", $"employee_{EstablishmentEmployee.EstablishmentEmployeeId}.png");

                                updatedEstablishmentEmployee = await _establishmentEmployeeRepository.UpdateImageAsync(EstablishmentEmployee.EstablishmentEmployeeId, path);
                            }
                        }

                        return Result<EstablishmentEmployee>.Success(updatedEstablishmentEmployee);
                    }

                    return Result<EstablishmentEmployee>.Failure($"Você não tem permissão para atualizar os dados deste funcionário.");
                }

                return Result<EstablishmentEmployee>.Failure($"Falha ao localizar funcionário.");
            }
            catch (Exception ex)
            {
                return Result<EstablishmentEmployee>.Failure($"Falha ao atualizar o EstablishmentEmployee: {ex.Message}");
            }
        }

        public async Task<Result<bool>> SetActiveStatus(TokenInfo tokenInfo, int EstablishmentEmployeeId, bool isActive)
        {
            try
            {
                await _establishmentEmployeeRepository.SetActiveStatusAsync(EstablishmentEmployeeId, tokenInfo.UserProfileId, isActive);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Falha ao definir o status ativo do EstablishmentEmployee: {ex.Message}");
            }
        }

        public async Task<Result<bool>> SetDeletedStatus(TokenInfo tokenInfo, int EstablishmentEmployeeId, bool isDeleted)
        {
            try
            {
                await _establishmentEmployeeRepository.SetDeletedStatusAsync(EstablishmentEmployeeId, tokenInfo.UserProfileId, isDeleted);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Falha ao definir o status excluído do EstablishmentEmployee: {ex.Message}");
            }
        }
    }
}

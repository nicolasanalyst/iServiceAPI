using iServiceRepositories.Repositories;
using iServiceRepositories.Repositories.Models;
using iServiceServices.Services.Models;
using Microsoft.Extensions.Configuration;

namespace iServiceServices.Services
{
    public class UserRoleService
    {
        private readonly UserRoleRepository _userRoleRepository;

        public UserRoleService(IConfiguration configuration)
        {
            _userRoleRepository = new UserRoleRepository(configuration);
        }

        public async Task<Result<List<UserRole>>> GetAllUserRoles()
        {
            try
            {
                var roles = await _userRoleRepository.GetAsync();
                return Result<List<UserRole>>.Success(roles);
            }
            catch (Exception ex)
            {
                return Result<List<UserRole>>.Failure($"Erro ao buscar os perfis de usuário: {ex.Message}");
            }
        }

        public async Task<Result<UserRole>> GetUserRoleById(int id)
        {
            try
            {
                var role = await _userRoleRepository.GetByIdAsync(id);
                if (role == null) return Result<UserRole>.Failure("Perfil de usuário não encontrado.");
                return Result<UserRole>.Success(role);
            }
            catch (Exception ex)
            {
                return Result<UserRole>.Failure($"Erro ao buscar o perfil de usuário: {ex.Message}");
            }
        }

        public async Task<Result<UserRole>> AddUserRole(UserRole userRoleModel)
        {
            try
            {
                var newUserRole = await _userRoleRepository.InsertAsync(userRoleModel);
                return Result<UserRole>.Success(newUserRole);
            }
            catch (Exception ex)
            {
                return Result<UserRole>.Failure($"Erro ao adicionar o perfil de usuário: {ex.Message}");
            }
        }

        public async Task<Result<UserRole>> UpdateUserRole(UserRole userRole)
        {
            try
            {
                var updatedUserRole = await _userRoleRepository.UpdateAsync(userRole);
                return Result<UserRole>.Success(updatedUserRole);
            }
            catch (Exception ex)
            {
                return Result<UserRole>.Failure($"Erro ao atualizar o perfil de usuário: {ex.Message}");
            }
        }

        public async Task<Result<bool>> SetActiveStatus(int id, bool isActive)
        {
            try
            {
                await _userRoleRepository.SetActiveStatusAsync(id, isActive);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Falha ao definir o status ativo do serviço: {ex.Message}");
            }
        }

        public async Task<Result<bool>> SetDeletedStatus(int id, bool isDeleted)
        {
            try
            {
                await _userRoleRepository.SetDeletedStatusAsync(id, isDeleted);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Falha ao definir o status excluído do serviço: {ex.Message}");
            }
        }
    }
}

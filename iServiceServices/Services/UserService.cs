using iServiceRepositories.Repositories;
using iServiceRepositories.Repositories.Models;
using iServiceServices.Services.Models;
using Microsoft.Extensions.Configuration;
using MySqlX.XDevAPI.Common;
using System.Configuration;

namespace iServiceServices.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly UserRoleRepository _userRoleRepository;
        private readonly UserProfileRepository _userProfileRepository;
        private readonly AddressRepository _addressRepository;
        private readonly EstablishmentCategoryRepository _establishmentCategoryRepository;

        public UserService(IConfiguration configuration)
        {
            _userRepository = new UserRepository(configuration);
            _userRoleRepository = new UserRoleRepository(configuration);
            _userProfileRepository = new UserProfileRepository(configuration);
            _addressRepository = new AddressRepository(configuration);
            _establishmentCategoryRepository = new EstablishmentCategoryRepository(configuration);
        }

        public async Task<Result<List<User>>> GetAllUsers()
        {
            try
            {
                var users = await _userRepository.GetAsync();
                return Result<List<User>>.Success(users);
            }
            catch (Exception ex)
            {
                return Result<List<User>>.Failure($"Falha ao obter os usuários: {ex.Message}");
            }
        }

        public async Task<Result<User>> GetUserById(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);

                if (user == null)
                {
                    return Result<User>.Failure("Usuário não encontrado.");
                }

                return Result<User>.Success(user);
            }
            catch (Exception ex)
            {
                return Result<User>.Failure($"Falha ao obter o usuário: {ex.Message}");
            }
        }

        public async Task<Result<User>> AddUser(User userModel)
        {
            try
            {
                var newUser = await _userRepository.InsertAsync(userModel);
                return Result<User>.Success(newUser);
            }
            catch (Exception ex)
            {
                return Result<User>.Failure($"Falha ao inserir o usuário: {ex.Message}");
            }
        }

        public async Task<Result<User>> UpdateUser(User user)
        {
            try
            {
                var updatedUser = await _userRepository.UpdateAsync(user);
                return Result<User>.Success(updatedUser);
            }
            catch (Exception ex)
            {
                return Result<User>.Failure($"Falha ao atualizar o usuário: {ex.Message}");
            }
        }

        public async Task<Result<bool>> DeleteUser(int userId)
        {
            try
            {
                bool success = await _userRepository.DeleteAsync(userId);

                if (!success)
                {
                    return Result<bool>.Failure("Falha ao excluir o usuário ou usuário não encontrado.");
                }

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Falha ao excluir o usuário: {ex.Message}");
            }
        }
    }
}

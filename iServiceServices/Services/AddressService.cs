using iServiceRepositories.Repositories;
using iServiceRepositories.Repositories.Models;
using iServiceServices.Services.Models;
using Microsoft.Extensions.Configuration;

namespace iServiceServices.Services
{
    public class AddressService
    {
        private readonly AddressRepository _addressRepository;
        private readonly UserProfileRepository _userProfileRepository;

        public AddressService(IConfiguration configuration)
        {
            _addressRepository = new AddressRepository(configuration);
            _userProfileRepository = new UserProfileRepository(configuration);
        }

        public async Task<Result<List<Address>>> GetAllAddresses()
        {
            try
            {
                var addresses = await _addressRepository.GetAsync();
                return Result<List<Address>>.Success(addresses);
            }
            catch (Exception ex)
            {
                return Result<List<Address>>.Failure($"Falha ao obter os endereços: {ex.Message}");
            }
        }

        public async Task<Result<Address>> GetAddressById(int addressId)
        {
            try
            {
                var address = await _addressRepository.GetByIdAsync(addressId);

                if (address == null)
                {
                    return Result<Address>.Failure("Endereço não encontrado.");
                }

                return Result<Address>.Success(address);
            }
            catch (Exception ex)
            {
                return Result<Address>.Failure($"Falha ao obter o endereço: {ex.Message}");
            }
        }

        public async Task<Result<Address>> AddAddress(Address addressModel)
        {
            try
            {
                var newAddress = await _addressRepository.InsertAsync(addressModel);
                return Result<Address>.Success(newAddress);
            }
            catch (Exception ex)
            {
                return Result<Address>.Failure($"Falha ao inserir o endereço: {ex.Message}");
            }
        }

        public async Task<Result<UserInfo>> SaveAddress(UserInfo request)
        {
            try
            {
                if (request?.Address?.AddressId > 0)
                {
                    request.Address = await _addressRepository.UpdateAsync(new Address
                    {
                        AddressId = request.Address.AddressId,
                        Street = request.Address.Street,
                        Number = request.Address.Number,
                        Neighborhood = request.Address.Neighborhood,
                        AdditionalInfo = request.Address.AdditionalInfo,
                        City = request.Address.City,
                        State = request.Address.State,
                        Country = request.Address.Country,
                        PostalCode = request.Address.PostalCode,
                        Active = request.Address.Active,
                        Deleted = request.Address.Deleted,
                        CreationDate = request.Address.CreationDate,
                        LastUpdateDate = request.Address.LastUpdateDate
                    });
                }
                else
                {
                    request.Address = await _addressRepository.InsertAsync(new Address
                    {
                        AddressId = 0,
                        Street = request.Address.Street,
                        Number = request.Address.Number,
                        Neighborhood = request.Address.Neighborhood,
                        AdditionalInfo = request.Address.AdditionalInfo,
                        City = request.Address.City,
                        State = request.Address.State,
                        Country = request.Address.Country,
                        PostalCode = request.Address.PostalCode,
                        Active = true,
                        Deleted = false,
                        CreationDate = DateTime.Now,
                        LastUpdateDate = DateTime.Now
                    });

                    if (!await _userProfileRepository.UpdateAddressAsync(request.UserProfile.UserProfileId, request.Address.AddressId))
                    {
                        return Result<UserInfo>.Failure("Falha ao atualizar o endereço do usuário.");
                    }
                }
                return Result<UserInfo>.Success(request);
            }
            catch (Exception ex)
            {
                return Result<UserInfo>.Failure($"Falha ao inserir o endereço: {ex.Message}");
            }
        }

        public async Task<Result<Address>> UpdateAddress(Address address)
        {
            try
            {
                var updatedAddress = await _addressRepository.UpdateAsync(address);
                return Result<Address>.Success(updatedAddress);
            }
            catch (Exception ex)
            {
                return Result<Address>.Failure($"Falha ao atualizar o endereço: {ex.Message}");
            }
        }

        public async Task<Result<bool>> SetActiveStatus(int addressId, bool isActive)
        {
            try
            {
                await _addressRepository.SetActiveStatusAsync(addressId, isActive);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Falha ao definir o status ativo do endereço: {ex.Message}");
            }
        }

        public async Task<Result<bool>> SetDeletedStatus(int addressId, bool isDeleted)
        {
            try
            {
                await _addressRepository.SetDeletedStatusAsync(addressId, isDeleted);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Falha ao definir o status excluído do endereço: {ex.Message}");
            }
        }
    }
}

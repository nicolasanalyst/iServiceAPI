using iServiceRepositories.Repositories;
using iServiceRepositories.Repositories.Models;
using iServiceServices.Services.Models;
using iServiceServices.Services.Models.Auth;
using Microsoft.Extensions.Configuration;

namespace iServiceServices.Services
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<Result<UserInfo>> PreRegister(PreRegister model)
        {
            try
            {
                if (await new UserRepository(_configuration).CheckUserAsync(model.Email))
                {
                    return Result<UserInfo>.Failure("Usuário já cadastrado.");
                }

                var userRole = await new UserRoleRepository(_configuration).GetByIdAsync(model.UserRoleId);

                if (userRole?.UserRoleId > 0 == false)
                {
                    return Result<UserInfo>.Failure("Falha ao recuperar a Role do usuário.");
                }

                var user = await new UserRepository(_configuration).InsertAsync(new User
                {
                    UserRoleId = model.UserRoleId,
                    Email = model.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Name = model.Name,
                });

                if (user.UserId > 0 == false)
                {
                    return Result<UserInfo>.Failure("Falha no registro do usuário.");
                }

                return Result<UserInfo>.Success(new UserInfo
                {
                    User = user,
                    UserRole = userRole,
                });
            }
            catch (Exception)
            {
                return Result<UserInfo>.Failure("Falha no registro do usuário.");
            }
        }
        public async Task<Result<UserInfo>> RegisterUserProfile(UserInfo model)
        {
            try
            {
                var typeDocument = new UtilService().ValidadorDeDocumento(model.UserProfile.Document);

                if (typeDocument)
                {
                    if (model.UserProfile.EstablishmentCategoryId > 0 == false)
                    {
                        return Result<UserInfo>.Failure("Por favor informe a categoria do estabelecimento.");
                    }

                    var establishmentCategory = await new EstablishmentCategoryRepository(_configuration).GetByIdAsync(model.UserProfile.EstablishmentCategoryId.GetValueOrDefault());

                    if (establishmentCategory?.EstablishmentCategoryId > 0 == false)
                    {
                        return Result<UserInfo>.Failure("Categoria não encontrada.");
                    }
                }
            }
            catch (Exception e)
            {
                return Result<UserInfo>.Failure(e.Message);
            }

            var user = await new UserRepository(_configuration).GetByIdAsync(model.UserProfile.UserId);

            if (user?.UserId > 0 == false)
            {
                return Result<UserInfo>.Failure("Usuário sem pré-cadastro.");
            }

            var userRole = await new UserRoleRepository(_configuration).GetByIdAsync(user.UserRoleId);

            if (userRole?.UserRoleId > 0 == false)
            {
                return Result<UserInfo>.Failure("Falha ao buscar os dados do usuário.");
            }

            try
            {
                var userProfile = await new UserProfileRepository(_configuration).InsertAsync(new UserProfile
                {
                    UserId = model.UserProfile.UserId,
                    EstablishmentCategoryId = model.UserProfile.EstablishmentCategoryId,
                    AddressId = null,
                    Document = model.UserProfile.Document,
                    DateOfBirth = model.UserProfile.DateOfBirth,
                    Phone = model.UserProfile.Phone,
                    CommercialName = model.UserProfile.CommercialName,
                    CommercialPhone = model.UserProfile.CommercialPhone,
                    CommercialEmail = model.UserProfile.CommercialEmail,
                    Description = model.UserProfile.Description,
                    ProfileImage = model.UserProfile.ProfileImage
                });

                return Result<UserInfo>.Success(new UserInfo
                {
                    User = user,
                    UserRole = userRole,
                    UserProfile = userProfile,
                    Address = null
                });
            }
            catch (Exception)
            {
                return Result<UserInfo>.Failure("Falha no registro do usuário.");
            }
        }
        public async Task<Result<UserInfo>> RegisterAddress(UserInfo model)
        {
            try
            {
                if (model.Address == null)
                {
                    return Result<UserInfo>.Failure("Falha ao cadastrar endereço.");
                }

                var user = await new UserRepository(_configuration).GetByIdAsync(model.UserProfile.UserId);

                if (user?.UserId > 0 == false)
                {
                    return Result<UserInfo>.Failure("Usuário sem pré-cadastro.");
                }

                var userRole = await new UserRoleRepository(_configuration).GetByIdAsync(user.UserRoleId);

                if (userRole?.UserRoleId > 0 == false)
                {
                    return Result<UserInfo>.Failure("Falha ao buscar os dados do usuário.");
                }

                var userProfile = await new UserProfileRepository(_configuration).GetByIdAsync(model.UserProfile.UserProfileId);

                Address address = new();

                if (userProfile?.UserProfileId > 0)
                {
                    address = await new AddressRepository(_configuration).InsertAsync(new Address
                    {
                        Street = model.Address.Street,
                        Number = model.Address.Number,
                        Neighborhood = model.Address.Neighborhood,
                        AdditionalInfo = model.Address.AdditionalInfo,
                        City = model.Address.City,
                        State = model.Address.State,
                        Country = model.Address.Country,
                        PostalCode = model.Address.PostalCode
                    });

                    if (address?.AddressId > 0 == false)
                    {
                        return Result<UserInfo>.Failure("Falha ao cadastrar o endereço do usuário.");
                    }

                    if (!await new UserProfileRepository(_configuration).UpdateAddressAsync(userProfile.UserProfileId, address.AddressId))
                    {
                        return Result<UserInfo>.Failure("Falha ao atualizar o endereço do usuário.");
                    }

                    userProfile.AddressId = address.AddressId;
                }

                return Result<UserInfo>.Success(new UserInfo
                {
                    User = user,
                    UserRole = userRole,
                    UserProfile = userProfile,
                    Address = address
                });
            }
            catch (Exception)
            {
                return Result<UserInfo>.Failure("Falha no registro do usuário.");
            }
        }
        public async Task<Result<UserInfo>> Login(Login model)
        {
            try
            {
                var user = await new UserRepository(_configuration).GetByEmailAsync(model.Email);

                if (user?.UserId > 0 == false)
                {
                    return Result<UserInfo>.Failure("Usuário não cadastrado.");
                }

                if (!BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    return Result<UserInfo>.Failure("Email ou senha incorretos.");
                }

                var userRole = await new UserRoleRepository(_configuration).GetByIdAsync(user.UserRoleId);

                if (userRole == null)
                {
                    return Result<UserInfo>.Failure("Falha ao recuperar os dados do usuário. (UserRole)");
                }

                var output = new UserInfo
                {
                    User = user,
                    UserRole = userRole,
                };

                var userProfile = await new UserProfileRepository(_configuration).GetByUserIdAsync(user.UserId);

                if (userProfile?.UserProfileId > 0)
                {
                    output.UserProfile = userProfile;

                    if (userProfile?.AddressId > 0)
                    {
                        output.Address = await new AddressRepository(_configuration).GetByIdAsync(userProfile.AddressId.GetValueOrDefault());
                    }
                }
                output.User.Password = "";
                return Result<UserInfo>.Success(output);
            }
            catch (Exception ex)
            {
                return Result<UserInfo>.Failure($"Falha ao realizar o login do usuário: {ex.Message}");
            }
        }

        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }
        public static bool VerifyPassword(string password, string correctHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, correctHash);
        }
    }
}

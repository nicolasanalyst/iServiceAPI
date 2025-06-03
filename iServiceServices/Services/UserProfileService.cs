using iServiceRepositories.Repositories;
using iServiceRepositories.Repositories.Models;
using iServiceServices.Services.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace iServiceServices.Services
{
    public class UserProfileService
    {
        private readonly UserProfileRepository _userProfileRepository;
        private readonly UserInfoService _userInfoService;
        private readonly UserRepository _userRepository;
        private readonly FeedbackRepository _feedbackRepository;

        public UserProfileService(IConfiguration configuration)
        {
            _userProfileRepository = new UserProfileRepository(configuration);
            _userInfoService = new UserInfoService(configuration);
            _userRepository = new UserRepository(configuration);
            _feedbackRepository = new FeedbackRepository(configuration);
        }

        public async Task<Result<List<UserProfile>>> GetAllUserProfiles()
        {
            try
            {
                var userProfiles = await _userProfileRepository.GetAsync();
                return Result<List<UserProfile>>.Success(userProfiles);
            }
            catch (Exception ex)
            {
                return Result<List<UserProfile>>.Failure($"Falha ao obter os perfis de usuário: {ex.Message}");
            }
        }

        public async Task<Result<List<UserProfile>>> GetByEstablishmentCategoryId(int establishmentCategoryId)
        {
            try
            {
                var userProfiles = await _userProfileRepository.GetByEstablishmentCategoryIdAsync(establishmentCategoryId);

                foreach (var profile in userProfiles)
                {
                    var feedbacks = await _feedbackRepository.GetFeedbackByUserProfileIdAsync(profile.UserProfileId);
                    if (feedbacks?.Count > 0)
                    {
                        profile.Rating = new Rating
                        {
                            Total = feedbacks.Count,
                            Value = feedbacks.Sum(f => f.Rating) / feedbacks.Count,
                            Feedback = feedbacks
                        };
                    }
                }

                return Result<List<UserProfile>>.Success(userProfiles);
            }
            catch (Exception ex)
            {
                return Result<List<UserProfile>>.Failure($"Falha ao obter os perfis de usuário: {ex.Message}");
            }
        }

        public async Task<Result<UserProfile>> GetUserProfileById(int userProfileId)
        {
            try
            {
                var userProfile = await _userProfileRepository.GetByIdAsync(userProfileId);

                if (userProfile == null)
                {
                    return Result<UserProfile>.Failure("Perfil de usuário não encontrado.");
                }

                var feedbacks = await _feedbackRepository.GetFeedbackByUserProfileIdAsync(userProfileId);

                if (feedbacks?.Count > 0)
                {
                    userProfile.Rating = new Rating
                    {
                        Value = feedbacks.Sum(f => f.Rating) / feedbacks.Count,
                        Total = feedbacks.Count,
                        Feedback = feedbacks
                    };
                }

                return Result<UserProfile>.Success(userProfile);
            }
            catch (Exception ex)
            {
                return Result<UserProfile>.Failure($"Falha ao obter o perfil de usuário: {ex.Message}");
            }
        }

        public async Task<Result<UserInfo>> GetUserInfoByUserId(int userId)
        {
            try
            {
                var userInfo = await _userInfoService.GetUserInfoByUserId(userId);

                if (userInfo.Value == null)
                {
                    return Result<UserInfo>.Failure("Perfil de usuário não encontrado.");
                }

                var feedbacks = await _feedbackRepository.GetFeedbackByUserProfileIdAsync(userInfo.Value.UserProfile.UserProfileId);

                if (feedbacks?.Count > 0)
                {
                    userInfo.Value.UserProfile.Rating = new Rating
                    {
                        Value = feedbacks.Sum(f => f.Rating) / feedbacks.Count,
                        Total = feedbacks.Count,
                        Feedback = feedbacks
                    };
                }

                return Result<UserInfo>.Success(userInfo.Value);
            }
            catch (Exception ex)
            {
                return Result<UserInfo>.Failure($"Falha ao obter o perfil de usuário: {ex.Message}");
            }
        }

        public async Task<Result<UserProfile>> AddUserProfile(UserProfile profileModel)
        {
            try
            {
                var newUserProfile = await _userProfileRepository.InsertAsync(profileModel);
                return Result<UserProfile>.Success(newUserProfile);
            }
            catch (Exception ex)
            {
                return Result<UserProfile>.Failure($"Falha ao inserir o perfil de usuário: {ex.Message}");
            }
        }

        public async Task<Result<UserInfo>> SaveUserProfile(UserInfo request)
        {
            try
            {
                if (request?.UserProfile?.UserProfileId > 0 == false)
                {
                    return Result<UserInfo>.Failure($"Falha ao recuperar o perfil do usuário.");
                }
                request.User = await _userRepository.UpdateNameAsync(request.User.UserId, request.User.Name);
                request.UserProfile = await _userProfileRepository.UpdateAsync(new UserProfile
                {
                    UserProfileId = request.UserProfile.UserProfileId,
                    UserId = request.UserProfile.UserId,
                    EstablishmentCategoryId = request.UserProfile.EstablishmentCategoryId,
                    AddressId = request.UserProfile.AddressId,
                    Document = request.UserProfile.Document,
                    DateOfBirth = request.UserProfile.DateOfBirth,
                    Phone = request.UserProfile.Phone,
                    CommercialName = request.UserProfile.CommercialName,
                    CommercialPhone = request.UserProfile.CommercialPhone,
                    CommercialEmail = request.UserProfile.CommercialEmail,
                    Description = request.UserProfile.Description,
                    ProfileImage = request.UserProfile.ProfileImage,
                    CreationDate = request.UserProfile.CreationDate,
                    LastUpdateDate = request.UserProfile.LastUpdateDate
                });
                return Result<UserInfo>.Success(request);
            }
            catch (Exception ex)
            {
                return Result<UserInfo>.Failure($"Falha ao inserir o perfil de usuário: {ex.Message}");
            }
        }

        public async Task<Result<UserProfile>> UpdateUserProfile(UserProfile profile)
        {
            try
            {
                var updatedUserProfile = await _userProfileRepository.UpdateAsync(profile);
                return Result<UserProfile>.Success(updatedUserProfile);
            }
            catch (Exception ex)
            {
                return Result<UserProfile>.Failure($"Falha ao atualizar o perfil de usuário: {ex.Message}");
            }
        }

        public async Task<Result<string>> UpdateProfileImage(ImageModel model)
        {
            try
            {
                var userProfile = await _userProfileRepository.GetByIdAsync(model.Id);

                if (userProfile?.UserProfileId > 0 == false)
                {
                    return Result<string>.Failure("Usuário sem pré-cadastro.");
                }

                if (model.File == null)
                {
                    return Result<string>.Failure("Falha ao ler o arquivo.");
                }

                var path = await new FtpServices().UploadFileAsync(model.File, "profile", $"profile{model.Id}.png");

                if (string.IsNullOrEmpty(path))
                {
                    return Result<string>.Failure($"Falha ao subir o arquivo de imagem.");
                }

                if (await _userProfileRepository.UpdateProfileImageAsync(model.Id, path))
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

        public async Task<Result<bool>> DeleteUserProfile(int userProfileId)
        {
            try
            {
                bool success = await _userProfileRepository.DeleteAsync(userProfileId);

                if (!success)
                {
                    return Result<bool>.Failure("Falha ao excluir o perfil de usuário ou perfil não encontrado.");
                }

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Falha ao excluir o perfil de usuário: {ex.Message}");
            }
        }
    }

}

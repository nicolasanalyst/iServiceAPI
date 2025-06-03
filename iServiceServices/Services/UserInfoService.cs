using iServiceRepositories.Repositories;
using iServiceRepositories.Repositories.Models;
using iServiceServices.Services.Models;
using Microsoft.Extensions.Configuration;

namespace iServiceServices.Services
{
    public class UserInfoService
    {
        private readonly UserRepository _userRepository;
        private readonly UserRoleRepository _userRoleRepository;
        private readonly UserProfileRepository _userProfileRepository;
        private readonly AddressRepository _addressRepository;
        private readonly EstablishmentCategoryRepository _establishmentCategoryRepository;
        private readonly FeedbackRepository _feedbackRepository;
        private readonly ScheduleRepository _scheduleRepository;

        public UserInfoService(IConfiguration configuration)
        {
            _userRepository = new UserRepository(configuration);
            _userRoleRepository = new UserRoleRepository(configuration);
            _userProfileRepository = new UserProfileRepository(configuration);
            _addressRepository = new AddressRepository(configuration);
            _establishmentCategoryRepository = new EstablishmentCategoryRepository(configuration);
            _feedbackRepository = new FeedbackRepository(configuration);
            _scheduleRepository = new ScheduleRepository(configuration);
        }

        public async Task<Result<UserInfo>> GetUserInfoByUserId(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);

                if (user?.UserId > 0 == false)
                {
                    return Result<UserInfo>.Failure("Falha ao recuperar os dados do usuário. (UserRole)");
                }

                return await GetUserInfo(user);
            }
            catch (Exception ex)
            {
                return Result<UserInfo>.Failure($"Falha ao obter os usuários: {ex.Message}");
            }
        }

        public async Task<Result<UserInfo>> GetUserInfoByUserProfileId(int userProfileId)
        {
            try
            {
                var userProfile = await _userProfileRepository.GetByIdAsync(userProfileId);

                if (userProfile?.UserProfileId > 0 == false)
                {
                    return Result<UserInfo>.Failure("Falha ao recuperar os dados do usuário. (UserRole)");
                }

                var user = await _userRepository.GetByIdAsync(userProfile.UserId);

                if (user?.UserId > 0 == false)
                {
                    return Result<UserInfo>.Failure("Falha ao recuperar os dados do usuário. (UserRole)");
                }

                return await GetUserInfo(user);
            }
            catch (Exception ex)
            {
                return Result<UserInfo>.Failure($"Falha ao obter os usuários: {ex.Message}");
            }
        }

        public async Task<Result<List<UserInfo>>> GetUserInfoByUserRoleId(int userRoleId)
        {
            try
            {
                var userRole = await _userRoleRepository.GetByIdAsync(userRoleId);

                if (userRole?.UserRoleId > 0 == false)
                {
                    return Result<List<UserInfo>>.Failure("Falha ao recuperar os dados do usuário. (UserRole)");
                }

                var users = await _userRepository.GetUserByUserRoleIdAsync(userRoleId);

                if (users?.Count > 0)
                {
                    return await GetUserInfo(users);
                }

                return Result<List<UserInfo>>.Success([]);
            }
            catch (Exception ex)
            {
                return Result<List<UserInfo>>.Failure($"Falha ao obter os usuários: {ex.Message}");
            }
        }

        public async Task<Result<List<UserInfo>>> GetUserInfoByEstablishmentCategoryId(int establishmentCategoryId)
        {
            try
            {
                var establishmentCategory = await _establishmentCategoryRepository.GetByIdAsync(establishmentCategoryId);

                if (establishmentCategory?.EstablishmentCategoryId > 0 == false)
                {
                    return Result<List<UserInfo>>.Failure("Falha ao recuperar a categoria.");
                }

                var users = await _userRepository.GetUserByEstablishmentCategoryIdAsync(establishmentCategoryId);

                if (users?.Count > 0)
                {
                    return await GetUserInfo(users);
                }

                return Result<List<UserInfo>>.Success([]);
            }
            catch (Exception ex)
            {
                return Result<List<UserInfo>>.Failure($"Falha ao obter os usuários: {ex.Message}");
            }
        }

        private async Task<Result<UserInfo>> GetUserInfo(User user)
        {
            var userRole = await _userRoleRepository.GetByIdAsync(user.UserRoleId);

            if (userRole?.UserRoleId > 0 == false)
            {
                return Result<UserInfo>.Failure("Falha ao recuperar os dados do usuário. (UserRole)");
            }

            var userProfile = await _userProfileRepository.GetByUserIdAsync(user.UserId);

            if (userProfile?.UserProfileId > 0 == false)
            {
                return Result<UserInfo>.Failure("Falha ao recuperar os dados do perfil do usuário. (UserProfile)");
            }

            var address = await _addressRepository.GetByIdAsync(userProfile.AddressId.GetValueOrDefault());

            if (userRole.UserRoleId == 2)
            {
                var feedbacks = await _feedbackRepository.GetFeedbackByUserProfileIdAsync(userProfile.UserProfileId);

                if (feedbacks?.Count > 0)
                {
                    userProfile.Rating = new Rating
                    {
                        Value = feedbacks.Sum(f => f.Rating) / feedbacks.Count,
                        Total = feedbacks.Count,
                        Feedback = feedbacks,
                    };
                }

                var schedule = await _scheduleRepository.GetByEstablishmentUserProfileIdAsync(userProfile.UserProfileId);

                userProfile.Schedule = schedule;
            }

            return Result<UserInfo>.Success(new UserInfo
            {
                User = user,
                UserRole = userRole,
                UserProfile = userProfile,
                Address = address
            });
        }
        private async Task<Result<List<UserInfo>>> GetUserInfo(List<User> users)
        {
            var result = new List<UserInfo>();

            foreach (var user in users)
            {
                if (user?.UserId > 0 == false) continue;

                var userRole = await _userRoleRepository.GetByIdAsync(user.UserRoleId);

                if (userRole?.UserRoleId > 0 == false)
                {
                    return Result<List<UserInfo>>.Failure("Falha ao recuperar os dados do usuário. (UserRole)");
                }

                var userProfile = await _userProfileRepository.GetByUserIdAsync(user.UserId);

                if (userProfile?.UserProfileId > 0 == false)
                {
                    return Result<List<UserInfo>>.Failure("Falha ao recuperar os dados do perfil do usuário. (UserProfile)");
                }

                if (userRole.UserRoleId == 1)
                {
                    var feedbacks = await _feedbackRepository.GetFeedbackByUserProfileIdAsync(userProfile.UserProfileId);

                    if (feedbacks?.Count > 0)
                    {
                        userProfile.Rating = new Rating
                        {
                            Value = feedbacks.Sum(f => f.Rating) / feedbacks.Count,
                            Total = feedbacks.Count,
                            Feedback = feedbacks,
                        };
                    }
                }

                var address = await _addressRepository.GetByIdAsync(userProfile.AddressId.GetValueOrDefault());

                result.Add(new UserInfo
                {
                    User = user,
                    UserRole = userRole,
                    UserProfile = userProfile,
                    Address = address
                });
            }

            return Result<List<UserInfo>>.Success(result);
        }
    }
}

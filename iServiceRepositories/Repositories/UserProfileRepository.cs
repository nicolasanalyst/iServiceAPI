using Dapper;
using iServiceRepositories.Repositories.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace iServiceRepositories.Repositories
{
    public class UserProfileRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public UserProfileRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        private async Task<MySqlConnection> OpenConnectionAsync()
        {
            var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        public async Task<List<UserProfile>> GetAsync()
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<UserProfile>("SELECT * FROM UserProfile");
                return queryResult.ToList();
            }
        }

        public async Task<List<UserProfile>> GetByEstablishmentCategoryIdAsync(int establishmentCategoryId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<UserProfile>("SELECT * FROM ActiveUserProfiles WHERE EstablishmentCategoryId = @EstablishmentCategoryId", new { EstablishmentCategoryId = establishmentCategoryId });
                return queryResult.ToList();
            }
        }

        public async Task<UserProfile> GetByIdAsync(int userProfileId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                return await connection.QueryFirstOrDefaultAsync<UserProfile>(
                    "SELECT * FROM UserProfile WHERE UserProfileId = @UserProfileId", new { UserProfileId = userProfileId });
            }
        }

        public async Task<UserProfile> GetByUserIdAsync(int userId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                return await connection.QueryFirstOrDefaultAsync<UserProfile>(
                    "SELECT * FROM UserProfile WHERE UserId = @UserId", new { UserId = userId });
            }
        }

        public async Task<UserProfile> InsertAsync(UserProfile userProfileModel)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var id = await connection.QuerySingleAsync<int>(
                    "INSERT INTO UserProfile (UserId, EstablishmentCategoryId, AddressId, Document, DateOfBirth, Phone, CommercialName, CommercialPhone, CommercialEmail, Description, ProfileImage) VALUES (@UserId, @EstablishmentCategoryId, @AddressId, @Document, @DateOfBirth, @Phone, @CommercialName, @CommercialPhone, @CommercialEmail, @Description, @ProfileImage); SELECT LAST_INSERT_ID();", userProfileModel);
                return await GetByIdAsync(id);
            }
        }

        public async Task<UserProfile> UpdateAsync(UserProfile userProfile)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE UserProfile SET EstablishmentCategoryId = @EstablishmentCategoryId, AddressId = @AddressId, Document = @Document, DateOfBirth = @DateOfBirth, Phone = @Phone, CommercialName = @CommercialName, CommercialPhone = @CommercialPhone, CommercialEmail = @CommercialEmail, Description = @Description, ProfileImage = @ProfileImage, LastUpdateDate = NOW() WHERE UserProfileId = @UserProfileId", userProfile);
                return await GetByIdAsync(userProfile.UserProfileId);
            }
        }

        public async Task<bool> UpdateAddressAsync(int userProfileId, int addressId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                int affectedRows = await connection.ExecuteAsync("UPDATE UserProfile SET AddressId = @AddressId WHERE UserProfileId = @UserProfileId", new { UserProfileId = userProfileId, AddressId = addressId });
                return affectedRows > 0;
            }
        }

        public async Task<bool> UpdateProfileImageAsync(int id, string path)
        {
            using (var connection = await OpenConnectionAsync())
            {
                int affectedRows = await connection.ExecuteAsync("UPDATE UserProfile SET ProfileImage = @ProfileImage, LastUpdateDate = NOW() WHERE UserProfileId = @UserProfileId", new { UserProfileId = id, ProfileImage = path });
                return affectedRows > 0;
            }
        }

        public async Task<bool> DeleteAsync(int userProfileId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                int affectedRows = await connection.ExecuteAsync("DELETE FROM UserProfile WHERE UserProfileId = @UserProfileId", new { UserProfileId = userProfileId });
                return affectedRows > 0;
            }
        }
    }
}

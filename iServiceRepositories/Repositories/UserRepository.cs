using Dapper;
using iServiceRepositories.Repositories.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace iServiceRepositories.Repositories
{
    public class UserRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
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

        public async Task<List<User>> GetAsync()
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<User>("SELECT * FROM User");
                return queryResult.ToList();
            }
        }

        public async Task<List<User>> GetUserByUserRoleIdAsync(int userRoleId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<User>(
                    "SELECT * FROM User WHERE UserRoleId = @UserRoleId", new { UserRoleId = userRoleId });
                return queryResult.ToList();
            }
        }

        public async Task<List<User>> GetUserByEstablishmentCategoryIdAsync(int establishmentCategoryId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<User>(
                    "SELECT U.UserId, U.UserRoleId, U.Email, U.Password, U.Name, U.CreationDate, U.LastLogin, U.LastUpdateDate FROM User U RIGHT JOIN UserProfile UP ON UP.UserId = U.UserId WHERE U.UserRoleId = 2 AND UP.EstablishmentCategoryId = @EstablishmentCategoryId",
                    new { EstablishmentCategoryId = establishmentCategoryId });
                return queryResult.ToList();
            }
        }

        public async Task<User> GetByIdAsync(int userId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                return await connection.QueryFirstOrDefaultAsync<User>(
                    "SELECT * FROM User WHERE UserId = @UserId", new { UserId = userId });
            }
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            using (var connection = await OpenConnectionAsync())
            {
                return await connection.QueryFirstOrDefaultAsync<User>(
                    "SELECT * FROM User WHERE Email = @Email", new { Email = email });
            }
        }

        public async Task<bool> CheckUserAsync(string email)
        {
            using (var connection = await OpenConnectionAsync())
            {
                int count = await connection.QueryFirstOrDefaultAsync<int>(
                    "SELECT COUNT(*) FROM User WHERE Email = @Email", new { Email = email });
                return count > 0;
            }
        }

        public async Task<User> InsertAsync(User userModel)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var id = await connection.QuerySingleAsync<int>(
                    "INSERT INTO User (UserRoleId, Email, Password, Name) VALUES (@UserRoleId, @Email, @Password, @Name); SELECT LAST_INSERT_ID();", userModel);
                return await GetByIdAsync(id);
            }
        }

        public async Task<User> UpdateAsync(User userUpdateModel)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE User SET UserRoleId = @UserRoleId, Email = @Email, Password = @Password, Name = @Name, LastLogin = @LastLogin, LastUpdateDate = NOW() WHERE UserId = @UserId", userUpdateModel);
                return await GetByIdAsync(userUpdateModel.UserId);
            }
        }

        public async Task<User> UpdateNameAsync(int userId, string name)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE User SET Name = @Name, LastUpdateDate = NOW() WHERE UserId = @UserId", new { UserId = userId, Name = name });
                return await GetByIdAsync(userId);
            }
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                int affectedRows = await connection.ExecuteAsync(
                    "DELETE FROM User WHERE UserId = @UserId", new { UserId = userId });
                return affectedRows > 0;
            }
        }
    }
}

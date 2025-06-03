using Dapper;
using iServiceRepositories.Repositories.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace iServiceRepositories.Repositories
{
    public class UserRoleRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public UserRoleRepository(IConfiguration configuration)
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

        public async Task<List<UserRole>> GetAsync()
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<UserRole>("SELECT * FROM UserRole");
                return queryResult.ToList();
            }
        }

        public async Task<UserRole> GetByIdAsync(int userRoleId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                return await connection.QueryFirstOrDefaultAsync<UserRole>(
                    "SELECT * FROM UserRole WHERE UserRoleId = @UserRoleId", new { UserRoleId = userRoleId });
            }
        }

        public async Task<UserRole> InsertAsync(UserRole userRoleModel)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var id = await connection.QuerySingleAsync<int>(
                    "INSERT INTO UserRole (Name) VALUES (@Name); SELECT LAST_INSERT_ID();", userRoleModel);
                return await GetByIdAsync(id);
            }
        }

        public async Task<UserRole> UpdateAsync(UserRole userRoleUpdateModel)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE UserRole SET Name = @Name, LastUpdateDate = NOW() WHERE UserRoleId = @UserRoleId", userRoleUpdateModel);
                return await GetByIdAsync(userRoleUpdateModel.UserRoleId);
            }
        }

        public async Task SetActiveStatusAsync(int userRoleId, bool isActive)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE UserRole SET Active = @IsActive WHERE UserRoleId = @UserRoleId", new { IsActive = isActive, UserRoleId = userRoleId });
            }
        }

        public async Task SetDeletedStatusAsync(int userRoleId, bool isDeleted)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE UserRole SET Deleted = @IsDeleted WHERE UserRoleId = @UserRoleId", new { IsDeleted = isDeleted, UserRoleId = userRoleId });
            }
        }
    }
}

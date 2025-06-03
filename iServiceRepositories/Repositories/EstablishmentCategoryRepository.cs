using Dapper;
using iServiceRepositories.Repositories.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace iServiceRepositories.Repositories
{
    public class EstablishmentCategoryRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public EstablishmentCategoryRepository(IConfiguration configuration)
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

        public async Task<List<EstablishmentCategory>> GetAsync()
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<EstablishmentCategory>("SELECT * FROM EstablishmentCategory WHERE Active = 1 AND Deleted = 0");
                return queryResult.ToList();
            }
        }

        public async Task<EstablishmentCategory> GetByIdAsync(int establishmentCategoryId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                return await connection.QueryFirstOrDefaultAsync<EstablishmentCategory>(
                    "SELECT * FROM EstablishmentCategory WHERE EstablishmentCategoryId = @EstablishmentCategoryId AND Active = 1 AND Deleted = 0", new { EstablishmentCategoryId = establishmentCategoryId });
            }
        }

        public async Task<EstablishmentCategory> InsertAsync(EstablishmentCategory establishmentCategoryModel)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var id = await connection.QuerySingleAsync<int>(
                    "INSERT INTO EstablishmentCategory (Name) VALUES (@Name); SELECT LAST_INSERT_ID();", establishmentCategoryModel);
                return await GetByIdAsync(id);
            }
        }

        public async Task<EstablishmentCategory> UpdateAsync(EstablishmentCategory establishmentCategoryUpdateModel)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE EstablishmentCategory SET Name = @Name, LastUpdateDate = NOW() WHERE EstablishmentCategoryId = @EstablishmentCategoryId", establishmentCategoryUpdateModel);
                return await GetByIdAsync(establishmentCategoryUpdateModel.EstablishmentCategoryId);
            }
        }

        public async Task<EstablishmentCategory> UpdateIconAsync(int establishmentCategoryId, string path)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE EstablishmentCategory SET Icon = @Icon, LastUpdateDate = NOW() WHERE EstablishmentCategoryId = @EstablishmentCategoryId", new { EstablishmentCategoryId = establishmentCategoryId, Icon = path });
                return await GetByIdAsync(establishmentCategoryId);
            }
        }

        public async Task SetActiveStatusAsync(int establishmentCategoryId, bool isActive)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE EstablishmentCategory SET Active = @IsActive WHERE EstablishmentCategoryId = @EstablishmentCategoryId", new { IsActive = isActive, EstablishmentCategoryId = establishmentCategoryId });
            }
        }

        public async Task SetDeletedStatusAsync(int establishmentCategoryId, bool isDeleted)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE EstablishmentCategory SET Deleted = @IsDeleted WHERE EstablishmentCategoryId = @EstablishmentCategoryId", new { IsDeleted = isDeleted, EstablishmentCategoryId = establishmentCategoryId });
            }
        }
    }
}

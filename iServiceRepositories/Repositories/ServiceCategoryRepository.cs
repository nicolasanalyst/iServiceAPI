using Dapper;
using iServiceRepositories.Repositories.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace iServiceRepositories.Repositories
{
    public class ServiceCategoryRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ServiceCategoryRepository(IConfiguration configuration)
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

        public async Task<List<ServiceCategory>> GetAsync()
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<ServiceCategory>("SELECT * FROM ServiceCategory WHERE Deleted = 0");
                return queryResult.ToList();
            }
        }

        public async Task<ServiceCategory> GetByIdAsync(int serviceCategoryId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                return await connection.QueryFirstOrDefaultAsync<ServiceCategory>(
                    "SELECT * FROM ServiceCategory WHERE ServiceCategoryId = @ServiceCategoryId AND Deleted = 0", new { ServiceCategoryId = serviceCategoryId });
            }
        }

        public async Task<List<ServiceCategory>> GetByEstablishmentUserProfileIdAsync(int establishmentUserProfileId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<ServiceCategory>(
                    "SELECT * FROM ServiceCategory WHERE EstablishmentUserProfileId = @EstablishmentUserProfileId AND Deleted = 0", new { EstablishmentUserProfileId = establishmentUserProfileId });
                return queryResult.ToList();
            }
        }

        public async Task<ServiceCategory> GetByFilterAsync(int establishmentUserProfileId, int serviceCategoryId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                return await connection.QueryFirstOrDefaultAsync<ServiceCategory>(
                    "SELECT * FROM ServiceCategory WHERE ServiceCategoryId = @ServiceCategoryId AND EstablishmentUserProfileId = @EstablishmentUserProfileId", new { ServiceCategoryId = serviceCategoryId, EstablishmentUserProfileId = establishmentUserProfileId });
            }
        }

        public async Task<ServiceCategory> InsertAsync(ServiceCategory serviceCategoryModel)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var id = await connection.QuerySingleAsync<int>(
                    "INSERT INTO ServiceCategory (EstablishmentUserProfileId, Name) VALUES (@EstablishmentUserProfileId, @Name); SELECT LAST_INSERT_ID();", serviceCategoryModel);
                return await GetByIdAsync(id);
            }
        }

        public async Task<ServiceCategory> UpdateAsync(ServiceCategory serviceCategoryUpdateModel)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE ServiceCategory SET Name = @Name, LastUpdateDate = NOW() WHERE ServiceCategoryId = @ServiceCategoryId", serviceCategoryUpdateModel);
                return await GetByIdAsync(serviceCategoryUpdateModel.ServiceCategoryId);
            }
        }

        public async Task SetActiveStatusAsync(int serviceCategoryId, bool isActive)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE ServiceCategory SET Active = @IsActive WHERE ServiceCategoryId = @ServiceCategoryId", new { IsActive = isActive, ServiceCategoryId = serviceCategoryId });
            }
        }

        public async Task SetDeletedStatusAsync(int serviceCategoryId, bool isDeleted)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE ServiceCategory SET Deleted = @IsDeleted WHERE ServiceCategoryId = @ServiceCategoryId", new { IsDeleted = isDeleted, ServiceCategoryId = serviceCategoryId });
            }
        }
    }
}

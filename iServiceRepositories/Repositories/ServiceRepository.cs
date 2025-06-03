using Dapper;
using iServiceRepositories.Repositories.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace iServiceRepositories.Repositories
{
    public class ServiceRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ServiceRepository(IConfiguration configuration)
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

        public async Task<List<Service>> GetAsync()
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<Service>("SELECT * FROM Service");
                return queryResult.ToList();
            }
        }

        public async Task<int> GetCountActivesAsync(int establishmentUserProfileId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                return await connection.QueryFirstOrDefaultAsync<int>("SELECT COUNT(*) FROM Service WHERE EstablishmentUserProfileId = @EstablishmentUserProfileId AND Active = 1 AND Deleted = 0", new { EstablishmentUserProfileId = establishmentUserProfileId });
            }
        }

        public async Task<List<Service>> Search(string service, int pageSize, int currentPage)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<Service>("CALL GetPagedServices(@Service, @PageSize, @CurrentPage)", new { Service = service, PageSize = pageSize, CurrentPage = currentPage });
                return queryResult.ToList();
            }
        }

        public async Task<Service> GetByIdAsync(int serviceId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                return await connection.QueryFirstOrDefaultAsync<Service>(
                    "SELECT * FROM Service WHERE ServiceId = @ServiceId", new { ServiceId = serviceId });
            }
        }

        public async Task<List<Service>> GetServiceByEstablishmentUserProfileIdAsync(int establishmentUserProfileId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<Service>(
                    "SELECT * FROM Service WHERE EstablishmentUserProfileId = @EstablishmentUserProfileId AND Deleted = 0", new { EstablishmentUserProfileId = establishmentUserProfileId });
                return queryResult.ToList();
            }
        }

        public async Task<Service> InsertAsync(Service serviceModel)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var id = await connection.QuerySingleAsync<int>(
                    "INSERT INTO Service (EstablishmentUserProfileId, ServiceCategoryId, Name, Description, Price, EstimatedDuration, ServiceImage) VALUES (@EstablishmentUserProfileId, @ServiceCategoryId, @Name, @Description, @Price, @EstimatedDuration, @ServiceImage); SELECT LAST_INSERT_ID();", serviceModel);
                return await GetByIdAsync(id);
            }
        }

        public async Task<Service> UpdateAsync(Service serviceUpdateModel)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE Service SET ServiceCategoryId = @ServiceCategoryId, Name = @Name, Description = @Description, Price = @Price, EstimatedDuration = @EstimatedDuration, ServiceImage = @ServiceImage, LastUpdateDate = NOW() WHERE ServiceId = @ServiceId", serviceUpdateModel);
                return await GetByIdAsync(serviceUpdateModel.ServiceId);
            }
        }

        public async Task<bool> UpdateServiceImageAsync(int id, string path)
        {
            using (var connection = await OpenConnectionAsync())
            {
                int affectedRows = await connection.ExecuteAsync("UPDATE Service SET ServiceImage = @ServiceImage, LastUpdateDate = NOW() WHERE ServiceId = @ServiceId", new { ServiceId = id, ServiceImage = path });
                return affectedRows > 0;
            }
        }

        public async Task SetActiveStatusAsync(int serviceId, bool isActive)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE Service SET Active = @IsActive WHERE ServiceId = @ServiceId", new { IsActive = isActive, ServiceId = serviceId });
            }
        }

        public async Task SetDeletedStatusAsync(int serviceId, bool isDeleted)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE Service SET Deleted = @IsDeleted WHERE ServiceId = @ServiceId", new { IsDeleted = isDeleted, ServiceId = serviceId });
            }
        }
    }
}

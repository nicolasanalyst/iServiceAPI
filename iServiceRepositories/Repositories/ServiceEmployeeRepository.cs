using Dapper;
using iServiceRepositories.Repositories.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace iServiceRepositories.Repositories
{
    public class ServiceEmployeeRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ServiceEmployeeRepository(IConfiguration configuration)
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

        public async Task<List<ServiceEmployee>> GetAsync()
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<ServiceEmployee>("SELECT * FROM ServiceEmployee");
                return queryResult.ToList();
            }
        }

        public async Task<ServiceEmployee> GetByIdAsync(int serviceEmployeeId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                return await connection.QueryFirstOrDefaultAsync<ServiceEmployee>(
                    "SELECT * FROM ServiceEmployee WHERE ServiceEmployeeId = @ServiceEmployeeId", new { ServiceEmployeeId = serviceEmployeeId });
            }
        }

        public async Task<List<ServiceEmployee>> GetByServiceIdAsync(int serviceId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<ServiceEmployee>(
                    "SELECT * FROM ServiceEmployee WHERE ServiceId = @ServiceId AND Active = 1 AND Deleted = 0;", new { ServiceId = serviceId });
                return queryResult.ToList();
            }
        }

        public async Task<ServiceEmployee> InsertAsync(ServiceEmployee serviceEmployeeModel)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var id = await connection.QuerySingleAsync<int>(
                    "INSERT INTO ServiceEmployee (ServiceId, EstablishmentEmployeeId) VALUES (@ServiceId, @EstablishmentEmployeeId); SELECT LAST_INSERT_ID();", serviceEmployeeModel);
                return await GetByIdAsync(id);
            }
        }

        public async Task<ServiceEmployee> UpdateAsync(ServiceEmployee serviceEmployeeUpdateModel)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE ServiceEmployee SET EstablishmentEmployeeId = @EstablishmentEmployeeId, LastUpdateDate = NOW() WHERE ServiceEmployeeId = @ServiceEmployeeId", serviceEmployeeUpdateModel);
                return await GetByIdAsync(serviceEmployeeUpdateModel.ServiceEmployeeId);
            }
        }

        public async Task SetActiveStatusAsync(int serviceEmployeeId, bool isActive)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE ServiceEmployee SET Active = @IsActive WHERE ServiceEmployeeId = @ServiceEmployeeId", new { IsActive = isActive, ServiceEmployeeId = serviceEmployeeId });
            }
        }

        public async Task SetDeletedStatusAsync(int serviceEmployeeId, bool isDeleted)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE ServiceEmployee SET Deleted = @IsDeleted WHERE ServiceEmployeeId = @ServiceEmployeeId", new { IsDeleted = isDeleted, ServiceEmployeeId = serviceEmployeeId });
            }
        }

        public async Task SetDeletedByEstablishmentEmployeeIdAsync(int establishmentEmployeeId, bool isDeleted)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE ServiceEmployee SET Deleted = @IsDeleted WHERE EstablishmentEmployeeId = @EstablishmentEmployeeId", new { IsDeleted = isDeleted, EstablishmentEmployeeId = establishmentEmployeeId });
            }
        }
    }
}

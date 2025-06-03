using Dapper;
using iServiceRepositories.Repositories.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iServiceRepositories.Repositories
{
    public class EstablishmentEmployeeRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public EstablishmentEmployeeRepository(IConfiguration configuration)
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

        public async Task<List<EstablishmentEmployee>> GetAsync(int establishmentUserProfileId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<EstablishmentEmployee>("SELECT * FROM EstablishmentEmployee WHERE EstablishmentUserProfileId = @EstablishmentUserProfileId AND Deleted = 0", new { EstablishmentUserProfileId = establishmentUserProfileId });
                return queryResult.ToList();
            }
        }

        public async Task<List<EstablishmentEmployee>> GetEmployeeAvailability(int serviceId, DateTime start)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<EstablishmentEmployee>("CALL GetEmployeeAvailability(@ServiceId, @p_Data)", new { ServiceId = serviceId, p_Data = start });
                return queryResult.ToList();
            }
        }

        public async Task<EstablishmentEmployee> GetByIdAsync(int establishmentEmployeeId, int establishmentUserProfileId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                return await connection.QueryFirstOrDefaultAsync<EstablishmentEmployee>(
                    "SELECT * FROM EstablishmentEmployee WHERE EstablishmentEmployeeId = @EstablishmentEmployeeId AND EstablishmentUserProfileId = @EstablishmentUserProfileId AND Deleted = 0", new { EstablishmentEmployeeId = establishmentEmployeeId, EstablishmentUserProfileId = establishmentUserProfileId });
            }
        }

        public async Task<EstablishmentEmployee> GetByIdAsync(int establishmentEmployeeId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                return await connection.QueryFirstOrDefaultAsync<EstablishmentEmployee>(
                    "SELECT * FROM EstablishmentEmployee WHERE EstablishmentEmployeeId = @EstablishmentEmployeeId AND Deleted = 0", new { EstablishmentEmployeeId = establishmentEmployeeId });
            }
        }

        public async Task<List<EstablishmentEmployee>> GetAllByEstablishmentUserProfileIdAsync(int establishmentUserProfileId, int serviceId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<EstablishmentEmployee>(
                    "SELECT EE.EstablishmentEmployeeId, EE.EstablishmentUserProfileId, EE.Name, EE.Document, EE.DateOfBirth, EE.EmployeeImage, EE.Active, EE.Deleted, EE.CreationDate, EE.LastUpdateDate, CASE WHEN SE.EstablishmentEmployeeId IS NOT NULL THEN TRUE ELSE FALSE END AS isAvailable " +
                    "FROM iServiceTest.EstablishmentEmployee EE " +
                    "LEFT JOIN ServiceEmployee SE ON SE.ServiceId = @ServiceId AND SE.EstablishmentEmployeeId = EE.EstablishmentEmployeeId AND SE.Active = 1 AND SE.Deleted = 0 " +
                    "WHERE EE.EstablishmentUserProfileId = @EstablishmentUserProfileId AND EE.Active = 1 AND EE.Deleted = 0", new { ServiceId = serviceId, EstablishmentUserProfileId = establishmentUserProfileId });
                return queryResult.ToList();
            }
        }

        public async Task<List<EstablishmentEmployee>> GetByEstablishmentUserProfileIdAsync(int establishmentUserProfileId, int serviceId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<EstablishmentEmployee>(
                    "SELECT EE.EstablishmentEmployeeId, EE.EstablishmentUserProfileId, EE.Name, EE.Document, EE.DateOfBirth, EE.EmployeeImage, " +
                    "EE.Active, EE.Deleted, EE.CreationDate, EE.LastUpdateDate FROM iServiceTest.EstablishmentEmployee EE INNER JOIN ServiceEmployee SE ON SE.ServiceId = @ServiceId " +
                    "AND SE.EstablishmentEmployeeId = EE.EstablishmentEmployeeId WHERE EE.EstablishmentUserProfileId = @EstablishmentUserProfileId AND EE.Active = 1 AND EE.Deleted = 0;", new { ServiceId = serviceId, EstablishmentUserProfileId = establishmentUserProfileId });
                return queryResult.ToList();
            }
        }

        public async Task<EstablishmentEmployee> InsertAsync(EstablishmentEmployee establishmentEmployeeModel)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var id = await connection.QuerySingleAsync<int>(
                    "INSERT INTO EstablishmentEmployee (EstablishmentUserProfileId, Name, Document, DateOfBirth, EmployeeImage) VALUES (@EstablishmentUserProfileId, @Name, @Document, @DateOfBirth, @EmployeeImage); SELECT LAST_INSERT_ID();", establishmentEmployeeModel);
                return await GetByIdAsync(id);
            }
        }

        public async Task<EstablishmentEmployee> UpdateAsync(EstablishmentEmployee establishmentEmployeeUpdateModel)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE EstablishmentEmployee SET EstablishmentUserProfileId = @EstablishmentUserProfileId, Name = @Name, Document = @Document, DateOfBirth = @DateOfBirth, EmployeeImage = @EmployeeImage, LastUpdateDate = NOW() WHERE EstablishmentEmployeeId = @EstablishmentEmployeeId", establishmentEmployeeUpdateModel);
                return await GetByIdAsync(establishmentEmployeeUpdateModel.EstablishmentEmployeeId);
            }
        }

        public async Task<EstablishmentEmployee> UpdateImageAsync(int establishmentEmployeeId, string path)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE EstablishmentEmployee SET EmployeeImage = @EmployeeImage, LastUpdateDate = NOW() WHERE EstablishmentEmployeeId = @EstablishmentEmployeeId", new { EstablishmentEmployeeId = establishmentEmployeeId, EmployeeImage = path });
                return await GetByIdAsync(establishmentEmployeeId);
            }
        }

        public async Task SetActiveStatusAsync(int establishmentEmployeeId, int establishmentUserProfileId, bool isActive)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE EstablishmentEmployee SET Active = @IsActive WHERE EstablishmentEmployeeId = @EstablishmentEmployeeId AND EstablishmentUserProfileId = @EstablishmentUserProfileId", new { IsActive = isActive, EstablishmentEmployeeId = establishmentEmployeeId, EstablishmentUserProfileId = establishmentUserProfileId });
            }
        }

        public async Task SetDeletedStatusAsync(int establishmentEmployeeId, int establishmentUserProfileId, bool isDeleted)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE EstablishmentEmployee SET Deleted = @IsDeleted WHERE EstablishmentEmployeeId = @EstablishmentEmployeeId AND EstablishmentUserProfileId = @EstablishmentUserProfileId", new { IsDeleted = isDeleted, EstablishmentEmployeeId = establishmentEmployeeId, EstablishmentUserProfileId = establishmentUserProfileId });
            }
        }
    }
}


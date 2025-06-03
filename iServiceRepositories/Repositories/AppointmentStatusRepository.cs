using Dapper;
using iServiceRepositories.Repositories.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace iServiceRepositories.Repositories
{
    public class AppointmentStatusRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public AppointmentStatusRepository(IConfiguration configuration)
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

        public async Task<List<AppointmentStatus>> GetAsync()
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<AppointmentStatus>("SELECT * FROM AppointmentStatus");
                return queryResult.ToList();
            }
        }

        public async Task<AppointmentStatus> GetByIdAsync(int appointmentStatusId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                return await connection.QueryFirstOrDefaultAsync<AppointmentStatus>(
                    "SELECT * FROM AppointmentStatus WHERE AppointmentStatusId = @AppointmentStatusId", new { AppointmentStatusId = appointmentStatusId });
            }
        }

        public async Task<AppointmentStatus> InsertAsync(AppointmentStatus appointmentStatusModel)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var id = await connection.QuerySingleAsync<int>(
                    "INSERT INTO AppointmentStatus (Name) VALUES (@Name); SELECT LAST_INSERT_ID();", appointmentStatusModel);
                return await GetByIdAsync(id);
            }
        }

        public async Task<AppointmentStatus> UpdateAsync(AppointmentStatus appointmentStatus)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE AppointmentStatus SET Name = @Name, LastUpdateDate = NOW() WHERE AppointmentStatusId = @AppointmentStatusId", appointmentStatus);
                return await GetByIdAsync(appointmentStatus.AppointmentStatusId);
            }
        }

        public async Task SetActiveStatusAsync(int appointmentStatusId, bool isActive)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE AppointmentStatus SET Active = @IsActive WHERE AppointmentStatusId = @AppointmentStatusId", new { IsActive = isActive, AppointmentStatusId = appointmentStatusId });
            }
        }

        public async Task SetDeletedStatusAsync(int appointmentStatusId, bool isDeleted)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE AppointmentStatus SET Deleted = @IsDeleted WHERE AppointmentStatusId = @AppointmentStatusId", new { IsDeleted = isDeleted, AppointmentStatusId = appointmentStatusId });
            }
        }
    }
}

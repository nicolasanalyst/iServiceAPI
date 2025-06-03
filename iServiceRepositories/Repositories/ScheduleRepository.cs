using Dapper;
using iServiceRepositories.Repositories.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace iServiceRepositories.Repositories
{
    public class ScheduleRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ScheduleRepository(IConfiguration configuration)
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

        public async Task<List<Schedule>> GetAsync()
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<Schedule>("SELECT * FROM Schedule");
                return queryResult.ToList();
            }
        }

        public async Task<Schedule> GetByIdAsync(int scheduleId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                return await connection.QueryFirstOrDefaultAsync<Schedule>(
                    "SELECT * FROM Schedule WHERE ScheduleId = @ScheduleId", new { ScheduleId = scheduleId });
            }
        }

        public async Task<Schedule> GetByEstablishmentUserProfileIdAsync(int establishmentUserProfileId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                return await connection.QueryFirstOrDefaultAsync<Schedule>(
                    "SELECT * FROM Schedule WHERE EstablishmentUserProfileId = @EstablishmentUserProfileId AND Deleted = 0", new { EstablishmentUserProfileId = establishmentUserProfileId });
            }
        }

        public async Task<Schedule> InsertAsync(Schedule scheduleModel)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var id = await connection.QuerySingleAsync<int>(
                    "INSERT INTO Schedule (EstablishmentUserProfileId, Days, Start, End, BreakStart, BreakEnd) VALUES (@EstablishmentUserProfileId, @Days, @Start, @End, @BreakStart, @BreakEnd); SELECT LAST_INSERT_ID();", scheduleModel);
                return await GetByIdAsync(id);
            }
        }

        public async Task<Schedule> UpdateAsync(Schedule scheduleUpdateModel)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE Schedule SET Days = @Days, Start = @Start, End = @End, BreakStart = @BreakStart, BreakEnd = @BreakEnd, LastUpdateDate = NOW() WHERE ScheduleId = @ScheduleId", scheduleUpdateModel);
                return await GetByIdAsync(scheduleUpdateModel.ScheduleId);
            }
        }

        public async Task SetActiveStatusAsync(int scheduleId, bool isActive)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE Schedule SET Active = @IsActive WHERE ScheduleId = @ScheduleId", new { IsActive = isActive, ScheduleId = scheduleId });
            }
        }

        public async Task SetDeletedStatusAsync(int scheduleId, bool isDeleted)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE Schedule SET Deleted = @IsDeleted WHERE ScheduleId = @ScheduleId", new { IsDeleted = isDeleted, ScheduleId = scheduleId });
            }
        }
    }
}

using Dapper;
using iServiceRepositories.Repositories.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace iServiceRepositories.Repositories
{
    public class SpecialScheduleRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public SpecialScheduleRepository(IConfiguration configuration)
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

        public async Task<List<SpecialSchedule>> GetAsync()
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<SpecialSchedule>("SELECT * FROM SpecialSchedule");
                return queryResult.ToList();
            }
        }

        public async Task<List<SpecialSchedule>> GetByEstablishmentAndDate(int establishmentUserProfileId, DateTime date)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<SpecialSchedule>("SELECT * FROM SpecialSchedule WHERE EstablishmentUserProfileId = @EstablishmentUserProfileId AND CAST(Date AS DATE) = CAST(@Date AS DATE)", new { EstablishmentUserProfileId = establishmentUserProfileId, Date = date });
                return queryResult.ToList();
            }
        }

        public async Task<SpecialSchedule> GetByIdAsync(int specialScheduleId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                return await connection.QueryFirstOrDefaultAsync<SpecialSchedule>(
                    "SELECT * FROM SpecialSchedule WHERE SpecialScheduleId = @SpecialScheduleId", new { SpecialScheduleId = specialScheduleId });
            }
        }

        public async Task<List<SpecialSchedule>> GetByUserProfileIdAsync(int userProfileId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<SpecialSchedule>(
                    "SELECT * FROM SpecialSchedule WHERE EstablishmentUserProfileId = @EstablishmentUserProfileId AND Deleted = 0", new { EstablishmentUserProfileId = userProfileId });
                return queryResult.ToList();
            }
        }

        public async Task<SpecialSchedule> InsertAsync(SpecialSchedule specialScheduleModel)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var id = await connection.QuerySingleAsync<int>(
                    "INSERT INTO SpecialSchedule (EstablishmentUserProfileId, Date, Start, End, BreakStart, BreakEnd) VALUES (@EstablishmentUserProfileId, @Date, @Start, @End, @BreakStart, @BreakEnd); SELECT LAST_INSERT_ID();", specialScheduleModel);
                return await GetByIdAsync(id);
            }
        }

        public async Task<SpecialSchedule> UpdateAsync(SpecialSchedule specialScheduleUpdateModel)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE SpecialSchedule SET Date = @Date, Start = @Start, End = @End, BreakStart = @BreakStart, BreakEnd = @BreakEnd, LastUpdateDate = NOW() WHERE SpecialScheduleId = @SpecialScheduleId", specialScheduleUpdateModel);
                return await GetByIdAsync(specialScheduleUpdateModel.SpecialScheduleId);
            }
        }

        public async Task SetActiveStatusAsync(int specialScheduleId, bool isActive)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE SpecialSchedule SET Active = @IsActive WHERE SpecialScheduleId = @SpecialScheduleId", new { IsActive = isActive, SpecialScheduleId = specialScheduleId });
            }
        }

        public async Task SetDeletedStatusAsync(int specialScheduleId, bool isDeleted)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE SpecialSchedule SET Deleted = @IsDeleted WHERE SpecialScheduleId = @SpecialScheduleId", new { IsDeleted = isDeleted, SpecialScheduleId = specialScheduleId });
            }
        }
    }
}

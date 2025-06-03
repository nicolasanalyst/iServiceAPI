using Dapper;
using iServiceRepositories.Repositories.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace iServiceRepositories.Repositories
{
    public class FeedbackRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public FeedbackRepository(IConfiguration configuration)
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

        public async Task<List<Feedback>> GetAsync()
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<Feedback>("SELECT * FROM Feedback");
                return queryResult.ToList();
            }
        }

        public async Task<Feedback> GetByIdAsync(int feedbackId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                return await connection.QueryFirstOrDefaultAsync<Feedback>(
                    "SELECT * FROM Feedback WHERE FeedbackId = @FeedbackId", new { FeedbackId = feedbackId });
            }
        }

        public async Task<Feedback> GetByAppointmentIdAsync(int appointmentId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                return await connection.QueryFirstOrDefaultAsync<Feedback>(
                    "SELECT F.FeedbackId, F.AppointmentId, F.Description, F.Rating, F.Active, F.Deleted, F.CreationDate, F.LastUpdateDate FROM Feedback F INNER JOIN Appointment A ON A.AppointmentId = F.AppointmentId WHERE F.AppointmentId = @AppointmentId AND F.Active = 1 AND F.Deleted = 0;", new { AppointmentId = appointmentId });
            }
        }

        public async Task<List<Feedback>> GetFeedbackByUserProfileIdAsync(int userProfileId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<Feedback>(
                    "SELECT F.FeedbackId, F.AppointmentId, F.Description, F.Rating, F.Active, F.Deleted, F.CreationDate, F.LastUpdateDate, U.Name FROM Feedback F INNER JOIN Appointment A ON A.AppointmentId = F.AppointmentId LEFT JOIN UserProfile UP ON UP.UserProfileId = A.ClientUserProfileId LEFT JOIN User U ON U.UserId = UP.UserId WHERE A.EstablishmentUserProfileId = @EstablishmentUserProfileId;",
                    new { EstablishmentUserProfileId = userProfileId });
                return queryResult.ToList();
            }
        }

        public async Task<Feedback> InsertAsync(Feedback feedbackModel)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var id = await connection.QuerySingleAsync<int>(
                    "INSERT INTO Feedback (AppointmentId, Description, Rating) VALUES (@AppointmentId, @Description, @Rating); SELECT LAST_INSERT_ID();", feedbackModel);
                return await GetByIdAsync(id);
            }
        }

        public async Task<Feedback> UpdateAsync(Feedback feedbackUpdateModel)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE Feedback SET Description = @Description, Rating = @Rating, LastUpdateDate = NOW() WHERE FeedbackId = @FeedbackId", feedbackUpdateModel);
                return await GetByIdAsync(feedbackUpdateModel.FeedbackId);
            }
        }

        public async Task SetActiveStatusAsync(int feedbackId, bool isActive)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE Feedback SET Active = @IsActive WHERE FeedbackId = @FeedbackId", new { IsActive = isActive, FeedbackId = feedbackId });
            }
        }

        public async Task SetDeletedStatusAsync(int feedbackId, bool isDeleted)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE Feedback SET Deleted = @IsDeleted WHERE FeedbackId = @FeedbackId", new { IsDeleted = isDeleted, FeedbackId = feedbackId });
            }
        }
    }
}

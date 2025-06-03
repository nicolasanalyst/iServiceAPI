using Dapper;
using iServiceRepositories.Repositories.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace iServiceRepositories.Repositories
{
    public class AppointmentRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public AppointmentRepository(IConfiguration configuration)
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

        public async Task<List<Appointment>> GetAsync()
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<Appointment>("SELECT * FROM Appointment");
                return queryResult.ToList();
            }
        }

        public async Task<List<MonthlyReport>> GetMonthlyReportsAsync(int establishmentUserProfileId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<MonthlyReport>("SELECT * FROM MonthlyReport WHERE EstablishmentUserProfileId = @EstablishmentUserProfileId;", new { EstablishmentUserProfileId = establishmentUserProfileId });
                return queryResult.ToList();
            }
        }

        public async Task<List<Appointment>> GetAllByDateAsync(int userProfileId, DateTime date)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<Appointment>("SELECT * FROM Appointment WHERE EstablishmentUserProfileId = @EstablishmentUserProfileId AND Start > NOW() AND DATE(Start) = CURDATE() AND Active = 1 AND Deleted = 0", new { EstablishmentUserProfileId = userProfileId });
                return queryResult.ToList();
            }
        }

        public async Task<int> GetCountByDateAsync(int userProfileId, DateTime date)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<int>("SELECT COUNT(*) FROM Appointment WHERE EstablishmentUserProfileId = @EstablishmentUserProfileId AND Start > NOW() AND DATE(Start) = CURDATE() AND Active = 1 AND Deleted = 0", new { EstablishmentUserProfileId = userProfileId });
                return queryResult.FirstOrDefault();
            }
        }

        public async Task<Appointment> GetNextAppointmentEstablishmentAsync(int userProfileId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<Appointment>("SELECT * FROM Appointment WHERE EstablishmentUserProfileId = @EstablishmentUserProfileId AND Start > NOW() AND DATE(Start) = CURDATE() AND AppointmentStatusId IN (1,2,3) AND Active = 1 AND Deleted = 0 ORDER BY Start ASC LIMIT 1;", new { EstablishmentUserProfileId = userProfileId});
                return queryResult.FirstOrDefault();
            }
        }

        public async Task<Appointment> GetNextAppointmentClientAsync(int userProfileId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<Appointment>("SELECT * FROM Appointment WHERE ClientUserProfileId = @ClientUserProfileId AND Start > NOW() AND AppointmentStatusId IN (1,2,3) AND Active = 1 AND Deleted = 0 ORDER BY Start ASC LIMIT 1", new { ClientUserProfileId = userProfileId});
                return queryResult.FirstOrDefault();
            }
        }

        public async Task<List<Appointment>> GetClientAppointmentsAsync(int userProfileId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<Appointment>("SELECT * FROM Appointment WHERE Active = 1 AND Deleted = 0 AND ClientUserProfileId = @ClientUserProfileId", new { ClientUserProfileId = userProfileId });
                return queryResult.ToList();
            }
        }

        public async Task<List<Appointment>> GetEstablishmentAppointmentsAsync(int userProfileId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<Appointment>("SELECT * FROM Appointment WHERE Active = 1 AND Deleted = 0 AND EstablishmentUserProfileId = @EstablishmentUserProfileId", new { EstablishmentUserProfileId = userProfileId });
                return queryResult.ToList();
            }
        }

        public async Task<List<Appointment>> GetByEstablishmentAndDate(int establishmentUserProfileId, DateTime date)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<Appointment>("SELECT * FROM Appointment WHERE EstablishmentUserProfileId = @EstablishmentUserProfileId AND CAST(Start AS DATE) = CAST(@Start AS DATE)", new { EstablishmentUserProfileId = establishmentUserProfileId, Start = date });
                return queryResult.ToList();
            }
        }

        public async Task<Appointment> GetByIdAsync(int appointmentId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                return await connection.QueryFirstOrDefaultAsync<Appointment>(
                    "SELECT * FROM Appointment WHERE AppointmentId = @AppointmentId", new { AppointmentId = appointmentId });
            }
        }

        public async Task<Appointment> GetByFilterAsync(int appointmentId, int? clientUserProfileId, int? establishmentUserProfileId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var query = "SELECT * FROM Appointment WHERE AppointmentId = @AppointmentId";

                if (clientUserProfileId > 0)
                {
                    query += " AND ClientUserProfileId = @ClientUserProfileId";
                }
                else if (establishmentUserProfileId > 0)
                {
                    query += " AND EstablishmentUserProfileId = @EstablishmentUserProfileId";
                }

                return await connection.QueryFirstOrDefaultAsync<Appointment>(
                    query,
                    new { AppointmentId = appointmentId, ClientUserProfileId = clientUserProfileId, EstablishmentUserProfileId = establishmentUserProfileId });
            }
        }


        public async Task<Appointment> InsertAsync(Appointment appointmentModel)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var id = await connection.QuerySingleAsync<int>(
                    "INSERT INTO Appointment (ServiceId, ClientUserProfileId, EstablishmentUserProfileId, AppointmentStatusId, EstablishmentEmployeeId, Start, End) VALUES (@ServiceId, @ClientUserProfileId, @EstablishmentUserProfileId, @AppointmentStatusId, @EstablishmentEmployeeId, @Start, @End); SELECT LAST_INSERT_ID();", appointmentModel);
                return await GetByIdAsync(id);
            }
        }

        public async Task<Appointment> UpdateAsync(Appointment appointmentUpdateModel)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE Appointment SET AppointmentStatusId = @AppointmentStatusId, EstablishmentEmployeeId = @EstablishmentEmployeeId, Start = @Start, End = @End, StartTime = @StartTime, EndTime = @EndTime, LastUpdateDate = NOW() WHERE AppointmentId = @AppointmentId", appointmentUpdateModel);
                return await GetByIdAsync(appointmentUpdateModel.AppointmentId);
            }
        }

        public async Task SetActiveStatusAsync(int appointmentId, bool isActive)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE Appointment SET Active = @IsActive WHERE AppointmentId = @AppointmentId", new { IsActive = isActive, AppointmentId = appointmentId });
            }
        }

        public async Task SetDeletedStatusAsync(int appointmentId, bool isDeleted)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE Appointment SET Deleted = @IsDeleted WHERE AppointmentId = @AppointmentId", new { IsDeleted = isDeleted, AppointmentId = appointmentId });
            }
        }
    }
}

using Dapper;
using iServiceRepositories.Repositories.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace iServiceRepositories.Repositories
{
    public class AddressRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public AddressRepository(IConfiguration configuration)
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

        public async Task<List<Address>> GetAsync()
        {
            using (var connection = await OpenConnectionAsync())
            {
                var queryResult = await connection.QueryAsync<Address>("SELECT * FROM Address");
                return queryResult.ToList();
            }
        }

        public async Task<Address> GetByIdAsync(int addressId)
        {
            using (var connection = await OpenConnectionAsync())
            {
                return await connection.QueryFirstOrDefaultAsync<Address>(
                    "SELECT * FROM Address WHERE AddressId = @AddressId", new { AddressId = addressId });
            }
        }

        public async Task<Address> InsertAsync(Address addressModel)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var id = await connection.QuerySingleAsync<int>(
                    "INSERT INTO Address (Street, Number, Neighborhood, AdditionalInfo, City, State, Country, PostalCode) VALUES (@Street, @Number, @Neighborhood, @AdditionalInfo, @City, @State, @Country, @PostalCode); SELECT LAST_INSERT_ID();", addressModel);
                return await GetByIdAsync(id);
            }
        }

        public async Task<Address> UpdateAsync(Address addressUpdateModel)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE Address SET Street = @Street, Number = @Number, Neighborhood = @Neighborhood, AdditionalInfo = @AdditionalInfo, City = @City, State = @State, Country = @Country, PostalCode = @PostalCode, LastUpdateDate = NOW() WHERE AddressId = @AddressId", addressUpdateModel);
                return await GetByIdAsync(addressUpdateModel.AddressId);
            }
        }

        public async Task SetActiveStatusAsync(int addressId, bool isActive)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE Address SET Active = @IsActive WHERE AddressId = @AddressId", new { IsActive = isActive, AddressId = addressId });
            }
        }

        public async Task SetDeletedStatusAsync(int addressId, bool isDeleted)
        {
            using (var connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(
                    "UPDATE Address SET Deleted = @IsDeleted WHERE AddressId = @AddressId", new { IsDeleted = isDeleted, AddressId = addressId });
            }
        }
    }
}

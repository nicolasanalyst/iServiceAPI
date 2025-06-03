using MySql.Data.MySqlClient;
using System.Data;

namespace iServiceRepositories
{
    public class DatabaseConnectionManager
    {
        private readonly string _connectionString;

        public DatabaseConnectionManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<MySqlConnection> GetOpenConnectionAsync()
        {
            var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.data
{
    public class DbManager
    {
        private string _connectionString;

        public DbManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int UploadImage(string fileName, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Picture (FileName, Password, Views) " +
                "VALUES (@fileName, @password, @views) SELECT SCOPE_IDENTITY()";
            command.Parameters.AddWithValue("@fileName", fileName);
            command.Parameters.AddWithValue("@password", password);
            command.Parameters.AddWithValue("@views", 0);

            connection.Open();

            return (int)(decimal)command.ExecuteScalar();


        }

        public Picture GetPicture(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Picture WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new Picture
            {
                Id = (int)reader["Id"],
                FileName = (string)reader["FileName"],
                Password = (string)reader["Password"],
                Views = (int)reader["views"]
            };
        }

        public void IncrementViews(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE Picture SET Views = VIEWS + 1
                            WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}

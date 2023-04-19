using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace April_17_homework.Data
{
    public class ImageManager
    {
        private string _connectionString;
        public ImageManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int AddImage(string fileName, string password)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO ImageTable (FileName, Password, Views)
                                    VALUES(@fileName, @password, @views); SELECT SCOPE_IDENTITY()";
            command.Parameters.AddWithValue("@fileName", fileName);
            command.Parameters.AddWithValue("@password", password);
            command.Parameters.AddWithValue("@views", 0);
            connection.Open();
            return (int)(decimal)command.ExecuteScalar();
            //command.ExecuteNonQuery();
        }

        public Image GetImage(int id)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM ImageTable WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }
            return new()
            {
                Id = id,
                FileName = (string)reader["FileName"],
                Password = (string)reader["Password"],
                Views = (int)reader["Views"]
            };
        }

        public void UpdateView(int id, int views)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE ImageTable SET Views = @views WHERE Id = @id";

            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@views", views);
            connection.Open();
            command.ExecuteNonQuery();
            //return (int)(decimal)command.ExecuteScalar();

        }
    }
}

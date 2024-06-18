using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WSUSHighAPI.Models;

namespace WSUSHighAPI.Repositories
{

    public class ComputersRepository
    {
        private readonly string _connectionString = "server=localhost;database=WSUSHighDB;" + "user id=IDHERE;password=PASSHERE;TrustServerCertificate=True";

        public IEnumerable<Computer> GetAllComputers()
        {
            List<Computer> computers = new List<Computer>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM Computers", connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Computer computer = new Computer
                    {
                        ComputerID = Convert.ToInt32(reader["ComputerID"]),
                        ComputerName = reader["ComputerName"] != DBNull.Value ? reader["ComputerName"].ToString() : "Unknown", // Default to "Unknown" if null
                        IPAddress = reader["IPAddress"] != DBNull.Value ? reader["IPAddress"].ToString() : "No IP", // Default to "No IP" if null
                        OSVersion = reader["OSVersion"] != DBNull.Value ? reader["OSVersion"].ToString() : "No OS", // Default to "No OS" if null
                        LastConnection = reader["LastConnection"] != DBNull.Value ? Convert.ToDateTime(reader["LastConnection"]) : null
                    };
                    computers.Add(computer);
                }
            }

            return computers;
        }

        public Computer GetComputerById(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM Computers WHERE ComputerID = @ComputerID", connection);
                command.Parameters.AddWithValue("@ComputerID", id);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new Computer
                    {
                        ComputerID = Convert.ToInt32(reader["ComputerID"]),
                        ComputerName = reader["ComputerName"].ToString(),
                        IPAddress = reader["IPAddress"].ToString(),
                        OSVersion = reader["OSVersion"].ToString(),
                        LastConnection = Convert.ToDateTime(reader["LastConnection"])
                    };
                }
                else
                {
                    return null; // Return null if no computer with the given ID is found
                }
            }
        }

        public Computer AddComputer(Computer computer)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("INSERT INTO Computers (ComputerName, IPAddress, OSVersion, LastConnection) VALUES (@ComputerName, @IPAddress, @OSVersion, @LastConnection); SELECT SCOPE_IDENTITY();", connection);
                command.Parameters.AddWithValue("@ComputerName", computer.ComputerName);
                command.Parameters.AddWithValue("@IPAddress", computer.IPAddress);
                command.Parameters.AddWithValue("@OSVersion", computer.OSVersion);
                command.Parameters.AddWithValue("@LastConnection", computer.LastConnection);

                // Execute the command and get the newly generated ID
                int newComputerId = Convert.ToInt32(command.ExecuteScalar());

                // Set the ID of the computer object to the newly generated ID
                computer.ComputerID = newComputerId;

                // Return the computer object
                return computer;
            }
        }



        public void UpdateComputer(Computer computer)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("UPDATE Computers SET ComputerName = @ComputerName, IPAddress = @IPAddress, OSVersion = @OSVersion, LastConnection = @LastConnection WHERE ComputerID = @ComputerID", connection))
                {
                    command.Parameters.Add(new SqlParameter("@ComputerID", SqlDbType.Int) { Value = computer.ComputerID });
                    command.Parameters.Add(new SqlParameter("@ComputerName", SqlDbType.NVarChar, 100) { Value = computer.ComputerName });
                    command.Parameters.Add(new SqlParameter("@IPAddress", SqlDbType.NVarChar, 50) { Value = computer.IPAddress });
                    command.Parameters.Add(new SqlParameter("@OSVersion", SqlDbType.NVarChar, 100) { Value = computer.OSVersion });

                    if (computer.LastConnection.HasValue)
                    {
                        command.Parameters.Add(new SqlParameter("@LastConnection", SqlDbType.DateTime) { Value = computer.LastConnection });
                    }
                    else
                    {
                        command.Parameters.Add(new SqlParameter("@LastConnection", SqlDbType.DateTime) { Value = DBNull.Value });
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteComputer(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("DELETE FROM Computers WHERE ComputerID = @ComputerID", connection);
                command.Parameters.AddWithValue("@ComputerID", id);
                command.ExecuteNonQuery();
            }
        }
    }
}
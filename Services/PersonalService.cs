using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Module07DataAccess.Model;
using MySql.Data.MySqlClient;

namespace Module07DataAccess.Services
{
    public class PersonalService
    {
        private readonly string _connectionString;

        public PersonalService()
        {
            var dbService = new DatabaseConnectionService();
            _connectionString = dbService.GetConnectionString();
        }

        public async Task<List<Personal>> GetAllEmployeeAsync()
        {
            var personalService = new List<Personal>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Retrieve data
                var cmd = new MySqlCommand("SELECT * FROM tblemployee", conn);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        personalService.Add(new Personal
                        {
                            EmployeeID = reader.GetInt32("EmployeeID"),
                            Name = reader.GetString("Name"),
                            email = reader.GetString("email"),
                            ContactNo = reader.GetString("Contactno"),
                            Address = reader.GetString("Address")  // New field added here
                        });
                    }
                }
            }
            return personalService;
        }

        public async Task<bool> InsertEmployeeAsync(Personal newPerson)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var cmd = new MySqlCommand("INSERT INTO tblemployee (Name, email, ContactNo, Address) VALUES (@Name, @Email, @ContactNo, @Address)", conn);
                    cmd.Parameters.AddWithValue("@Name", newPerson.Name);
                    cmd.Parameters.AddWithValue("@Email", newPerson.email);
                    cmd.Parameters.AddWithValue("@ContactNo", newPerson.ContactNo);
                    cmd.Parameters.AddWithValue("@Address", newPerson.Address);  // New parameter for Address

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding personal record: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new MySqlCommand("DELETE FROM tblemployee WHERE EmployeeID = @EmployeeID", conn);
                    cmd.Parameters.AddWithValue("@EmployeeID", id);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting personal record: {ex.Message}");
                return false;
            }
        }

        // New: Update Employee
        public async Task<bool> UpdateEmployeeAsync(Personal updatedPerson)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var cmd = new MySqlCommand("UPDATE tblemployee SET Name = @Name, email = @Email, ContactNo = @ContactNo, Address = @Address WHERE EmployeeID = @EmployeeID", conn);
                    cmd.Parameters.AddWithValue("@Name", updatedPerson.Name);
                    cmd.Parameters.AddWithValue("@Email", updatedPerson.email);
                    cmd.Parameters.AddWithValue("@ContactNo", updatedPerson.ContactNo);
                    cmd.Parameters.AddWithValue("@Address", updatedPerson.Address);
                    cmd.Parameters.AddWithValue("@EmployeeID", updatedPerson.EmployeeID);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating personal record: {ex.Message}");
                return false;
            }
        }

        // New: Search Employee
        public async Task<List<Personal>> SearchEmployeeAsync(string keyword)
        {
            var resultList = new List<Personal>();

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var cmd = new MySqlCommand("SELECT * FROM tblemployee WHERE Name LIKE @Keyword OR email LIKE @Keyword OR ContactNo LIKE @Keyword OR Address LIKE @Keyword", conn);
                    cmd.Parameters.AddWithValue("@Keyword", $"%{keyword}%");

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            resultList.Add(new Personal
                            {
                                EmployeeID = reader.GetInt32("EmployeeID"),
                                Name = reader.GetString("Name"),
                                email = reader.GetString("email"),
                                ContactNo = reader.GetString("Contactno"),
                                Address = reader.GetString("Address")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching for personal records: {ex.Message}");
            }

            return resultList;
        }
    }
}

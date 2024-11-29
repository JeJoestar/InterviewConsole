using EmployeeService.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace EmployeeService.Code
{
    public class EmployeeDataService : IEmployeeDataService
    {
        private readonly string _connectionString;

        public EmployeeDataService()
        {
            _connectionString = ConnectionStringService.GetConnectionString();
        }

        public async Task<EmployeeDto> GetEmployeeByIDAsync(int employeeId)
        {
            var employees = new List<Employee>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    WITH EmployeeTree AS (
                        SELECT ID, Name, ManagerID
                        FROM Employee
                        WHERE ID = @EmployeeID AND Enable = 1
                        UNION ALL
                        SELECT e.ID, e.Name, e.ManagerID
                        FROM Employee e
                        INNER JOIN EmployeeTree et ON e.ManagerID = et.ID AND e.Enable = 1
                    )
                    SELECT ID, Name, ManagerID FROM EmployeeTree;
                ";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeID", employeeId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            employees.Add(new Employee
                            {
                                ID = reader.GetInt32(reader.GetOrdinal("ID")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                ManagerID = reader.IsDBNull(reader.GetOrdinal("ManagerID"))
                                    ? (int?)null
                                    : reader.GetInt32(reader.GetOrdinal("ManagerID"))
                            });
                        }
                    }
                }
            }

            return BuildWorkStructure(employeeId, employees);
        }

        public async Task EnableEmployeeAsync(int employeeId, bool newEnableValue)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE Employee SET Enable = @EnableValue WHERE ID = @EmployeeID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeID", employeeId);
                    command.Parameters.AddWithValue("@EnableValue", newEnableValue);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private EmployeeDto BuildWorkStructure(int employeeId, List<Employee> allEmployees)
        {
            var employee = allEmployees.FirstOrDefault(e => e.ID == employeeId);

            if (employee == null) {
                return null;
            } 

            var employeeDto = employee.ToDto();

            employeeDto.Subordinates = allEmployees
                .Where(e => e.ManagerID == employeeId)
                .Select(e => BuildWorkStructure(e.ID, allEmployees))
                .ToList();

            return employeeDto;
        }
    }
}
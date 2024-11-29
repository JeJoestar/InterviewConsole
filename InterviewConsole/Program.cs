using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InterviewConsole
{
    class Program
    {
        const string ConnectionString = "Data Source=DESKTOP-EI1DIVE\\JEJOESTAR;Initial Catalog=Test;User ID=testUser;Password=pass@word1;";
        const string BaseUrl = "http://localhost:64014/EmployeeService.svc";
        public class Employee
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public bool Enable { get; set; }
        }

        static async Task Main(string[] args)
        {
            await PutRequest();
            await GetRequest();
            //PopulateData();
            //SetupEmployeeTable();
            //DataTable dtEmployees = GetQueryResult("SELECT * FROM Employee");
        }

        private static async Task PutRequest()
        {
            int id = 1;
            int enable = 1;

            using (HttpClient client = new HttpClient { BaseAddress = new Uri(BaseUrl) })
            {
                try
                {
                    string url = $"EnableEmployee?id={id}";

                    var payload = new { enable };
                    var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync(url, content);

                    response.EnsureSuccessStatusCode();

                    Console.WriteLine("Employee enabled successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private static async Task GetRequest()
        {
            using (HttpClient client = new HttpClient { BaseAddress = new Uri(BaseUrl) })
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync("GetEmployeeById?id=1");
                    response.EnsureSuccessStatusCode();

                    var employeeJson = await response.Content.ReadAsStringAsync();


                    Console.WriteLine(employeeJson);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private static void SetupEmployeeTable()
        {
            string createTableScript = @"
                CREATE TABLE Employee (
                    ID INT PRIMARY KEY IDENTITY(1,1),
                    Name NVARCHAR(100) NOT NULL,
                    ManagerID INT NULL,
                    Enable BIT NOT NULL DEFAULT 1
                );

                ALTER TABLE Employee
                ADD CONSTRAINT FK_Employee_Manager
                FOREIGN KEY (ManagerID) REFERENCES Employee(ID)
                ON DELETE NO ACTION
                ON UPDATE NO ACTION;
            ";

                string createTriggerScript = @"
                CREATE TRIGGER trg_SetNullOnDelete
                ON Employee
                AFTER DELETE
                AS
                BEGIN
                    UPDATE Employee
                    SET ManagerID = NULL
                    WHERE ManagerID IN (SELECT ID FROM DELETED);
                END;
            ";

            string insertSampleDataScript = @"
                INSERT INTO Employee (Name, ManagerID, Enable)
                VALUES 
                ('Andrey', NULL, 1),
                ('Alexey', 1, 1),
                ('Roman', 2, 1),
                ('Maria', 1, 0),
                ('Anna', NULL, 1);
            ";

            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(createTableScript, connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Table and foreign key created successfully.");
                    }

                    using (SqlCommand command = new SqlCommand(createTriggerScript, connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Trigger created successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static void PopulateData()
        {
            string insertSampleDataScript = @"
                INSERT INTO Employee (Name, ManagerID, Enable)
                VALUES 
                ('Andrey', NULL, 1),
                ('Alexey', 1, 1),
                ('Roman', 2, 1),
                ('Maria', 1, 0),
                ('Anna', NULL, 1);
            ";

            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(insertSampleDataScript, connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Data populated.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }


        private static DataTable GetQueryResult(string query)
        {
            var dt = new DataTable();

			using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
					command.CommandText = query;

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

			return dt;
        }
    }
}

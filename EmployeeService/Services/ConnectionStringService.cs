using System.Configuration;
using System.Data.Entity.Core.EntityClient;

namespace EmployeeService.Code
{
    public static class ConnectionStringService
    {
        public static string GetConnectionString()
        {
            string entityConnectionString = ConfigurationManager.ConnectionStrings["Employee"].ConnectionString;

            // Parse the Entity Framework connection string
            var entityConnection = new EntityConnectionStringBuilder(entityConnectionString);

            // Extract the provider connection string (actual SQL Server connection string)
            string sqlConnectionString = entityConnection.ProviderConnectionString;

            return sqlConnectionString;
        }
    }
}
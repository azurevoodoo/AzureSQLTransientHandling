using Dapper;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace AzureSQLTransientHandling
{
    class Program
    {
        private static readonly string SampleDBConnectionString = Environment.GetEnvironmentVariable("SQLAZURECONNSTR_WCOMSAMPLE");

        static void Main(string[] args)
        {
            Demo().Wait();
        }

        private static async Task Demo()
        {
            using (var connection = new SqlConnection(SampleDBConnectionString))
            {
                connection.OpenWithRetry();
            }

            using (var connection = new SqlConnection(SampleDBConnectionString))
            {
                await connection.OpenWithRetryAsync();
            }

            using (var connection = new SqlConnection(SampleDBConnectionString))
            {
                var customer = connection
                                .OpenWithRetry()
                                .Query<Customer>(SelectCustomerStatement, new { CustomerId = 1})
                                .FirstOrDefault();
            }

            using (var connection = new SqlConnection(SampleDBConnectionString))
            {
                var customer = connection
                                .OpenWithRetry(
                                    conn=>conn.Query<Customer>(
                                        SelectCustomerStatement, new { CustomerId = 1 })
                                ).FirstOrDefault();
            }

            using (var connection = new SqlConnection(SampleDBConnectionString))
            {
                await connection.OpenWithRetryAsync();
                var customer = (await connection.QueryAsync<Customer>(
                                                        SelectCustomerStatement,
                                                        new { CustomerId = 1 }
                                                    )
                                ).FirstOrDefault();
            }
        }

        private const string SelectCustomerStatement = @"
            SELECT  CustomerID,
                    FirstName,
                    LastName,
                    CompanyName,
                    EmailAddress,
                    SalesPerson
                FROM [SalesLT].[Customer]
                WHERE CustomerID = @CustomerID";
    }
}
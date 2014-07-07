using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Atlassed.Repositories
{
    public class SqlConnectionFactory
    {
        private const string __defaultConnection = "DefaultConnection";

        private readonly string _connectionString;

        public SqlConnectionFactory()
        {
            _connectionString = ConfigurationManager.ConnectionStrings[__defaultConnection].ConnectionString;
        }

        public SqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlConnection GetConnection()
        {
            var c = new SqlConnection(_connectionString);
            c.Open();
            return c;
        }
    }
}
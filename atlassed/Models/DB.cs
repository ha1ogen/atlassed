using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Atlassed.Models
{

    public static class DB
    {
        private const string ReturnParamName = "__RETURN";

        public static string ConnectionString = null;

        private static SqlConnection _transactionConnection = null;

        public class FakeTransaction : DbTransaction
        {
            public override void Commit()
            {
            }

            public override void Rollback()
            {
            }

            protected override DbConnection DbConnection
            {
                get { return _transactionConnection; }
            }

            public override IsolationLevel IsolationLevel
            {
                get { return IsolationLevel.Unspecified; }
            }
        }

        public class RealTransation : DbTransaction
        {
            private readonly SqlTransaction _innerTransaction;

            public RealTransation(SqlTransaction innerTransaction)
            {
                _innerTransaction = innerTransaction;
            }

            public override void Commit()
            {
                _innerTransaction.Commit();
                _transactionConnection.Close();
                _transactionConnection = null;
            }

            public override void Rollback()
            {
                _innerTransaction.Rollback();
                _transactionConnection.Close();
                _transactionConnection = null;
            }

            protected override DbConnection DbConnection
            {
                get { return _transactionConnection; }
            }

            public override IsolationLevel IsolationLevel
            {
                get { return _innerTransaction.IsolationLevel; }
            }
        }

        public static SqlConnection GetConnection()
        {
            if (_transactionConnection != null)
            {
                return _transactionConnection;
            }

            if (ConnectionString == null)
                ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            var c = new SqlConnection(ConnectionString);
            c.Open();
            return c;
        }

        public static DbTransaction BeginTransaction()
        {
            if (_transactionConnection == null)
            {
                _transactionConnection = GetConnection();
                return new RealTransation(_transactionConnection.BeginTransaction());
            }

            return new FakeTransaction();
        }

        public static SqlCommand NewText(string query)
        {
            return new SqlCommand(query);
        }

        public static SqlCommand NewSP(string spName)
        {
            return new SqlCommand(spName)
            {
                CommandType = CommandType.StoredProcedure
            };
        }

        public static SqlCommand AddParam(this SqlCommand query, string name, SqlDbType type, object value,
            ParameterDirection direction = ParameterDirection.Input)
        {
            query.Parameters.Add(new SqlParameter
            {
                ParameterName = name,
                SqlDbType = type,
                Value = value ?? DBNull.Value,
                Direction = direction
            });

            return query;
        }

        public static SqlCommand AddParam(this SqlCommand query, string name, int value,
            ParameterDirection direction = ParameterDirection.Input)
        {
            return query.AddParam(name, SqlDbType.Int, value);
        }

        public static SqlCommand AddParam(this SqlCommand query, string name, string value,
            ParameterDirection direction = ParameterDirection.Input)
        {
            return query.AddParam(name, SqlDbType.VarChar, value);
        }

        public static SqlCommand AddParam(this SqlCommand query, string name, int? value,
            ParameterDirection direction = ParameterDirection.Input)
        {
            return query.AddParam(name, SqlDbType.Int, value);
        }

        public static SqlCommand AddParam(this SqlCommand query, string name, bool value,
            ParameterDirection direction = ParameterDirection.Input)
        {
            return query.AddParam(name, SqlDbType.Bit, value);
        }

        public static SqlCommand AddTVParam(this SqlCommand query, string name, IEnumerable<SqlDataRecord> value)
        {
            if (value.Count<SqlDataRecord>() == 0)
                value = null;

            var param = query.Parameters.AddWithValue(name, value);
            param.SqlDbType = SqlDbType.Structured;
            return query;
        }

        public static SqlCommand AddReturn(this SqlCommand query, SqlDbType type)
        {
            query.Parameters.Add(new SqlParameter
            {
                ParameterName = ReturnParamName,
                SqlDbType = type,
                Direction = ParameterDirection.ReturnValue
            });

            return query;
        }
        
        // EXECUTORS
        public static T ExecExpectScalarValue<T>(this SqlCommand query)
        {
            return query.ExecExpectOne<T>(x => (T)x.GetValue(0));
        }

        public static int ExecNonQuery(this SqlCommand query)
        {
            using (var c = GetConnection())
            {
                query.Connection = c;
                return query.ExecuteNonQuery();
            }
        }

        public static bool ExecNonQueryExpectSuccess(this SqlCommand query)
        {
            try
            {
                using (var c = GetConnection())
                {
                    query.Connection = c;
                    query.ExecuteNonQuery();
                    return true;
                }
            }
            catch (SqlException e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        public static bool ExecNonQueryExpectAffected(this SqlCommand query, int expectedAffectedRows)
        {
            using (var c = GetConnection())
            {
                query.Connection = c;
                var affected = query.ExecuteNonQuery();
                Debug.WriteLine(affected + " rows affected.");
                return affected == expectedAffectedRows;
            }
        }

        public static bool ExecNonQueryExpectZeroOrOneAffected(this SqlCommand query)
        {
            using (var c = GetConnection())
            {
                query.Connection = c;
                var affected = query.ExecuteNonQuery();

                return affected == 1 || affected == 0;
            }
        }

        public static T ExecExpectOne<T>(this SqlCommand query, Func<IDataRecord, T> convert)
        {
            return query.ExecExpectMultiple(convert).FirstOrDefault();
        }

        public static Collection<T> ExecExpectMultiple<T>(this SqlCommand query, Func<IDataRecord, T> convert)
        {
            using (var c = GetConnection())
            {
                query.Connection = c;
                var result = query.ExecuteReader();

                var collection = new Collection<T>();
                while (result.Read())
                {
                    collection.Add(convert(result));
                }
                return collection;
            }
        }

        public static int? ToNullable(this SqlInt32 value)
        {
            return value.IsNull ? (int?)null : value.Value;
        }

        // DATA RECORD HELPERS
        public static string GetString(this IDataRecord record, string name)
        {
            return record.GetString(record.GetOrdinal(name));
        }
        public static int GetInt32(this IDataRecord record, string name)
        {
            return record.GetInt32(record.GetOrdinal(name));
        }
        public static bool GetBoolean(this IDataRecord record, string name)
        {
            return record.GetBoolean(record.GetOrdinal(name));
        }
    }
}
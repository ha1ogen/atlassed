using Atlassed.Repositories;
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

namespace Atlassed.Repositories
{

    public static class DB
    {
        private const string __returnParamName = "__RETURN";

        public static SqlCommand NewText(string query, SqlConnectionFactory f)
        {
            return new SqlCommand(query, f.GetConnection());
        }

        public static SqlCommand NewSP(string spName, SqlConnectionFactory f)
        {
            return new SqlCommand(spName, f.GetConnection())
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
                ParameterName = __returnParamName,
                SqlDbType = type,
                Direction = ParameterDirection.ReturnValue
            });

            return query;
        }

        // EXECUTORS
        public static T ExecExpectReturnValue<T>(this SqlCommand query)
        {
            query.Parameters.Add(new SqlParameter
            {
                ParameterName = __returnParamName,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.ReturnValue
            });

            query.ExecNonQuery();
            return query.GetReturnValue<T>();
        }

        public static T GetReturnValue<T>(this SqlCommand query)
        {
            return (T)query.Parameters[__returnParamName].Value;
        }

        public static T ExecExpectScalarValue<T>(this SqlCommand query)
        {
            return query.ExecExpectOne<T>(x => (T)x.GetValue(0));
        }

        public static int ExecNonQuery(this SqlCommand query)
        {
            using (query.Connection)
            {
                return query.ExecuteNonQuery();
            }
        }

        public static bool ExecNonQueryExpectSuccess(this SqlCommand query)
        {
            try
            {
                query.ExecNonQuery();
                return true;
            }
            catch (SqlException e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        public static bool ExecNonQueryExpectAffected(this SqlCommand query, int expectedAffectedRows)
        {
            var affected = query.ExecNonQuery();
            Debug.WriteLine(affected + " rows affected.");
            return affected == expectedAffectedRows;
        }

        public static bool ExecNonQueryExpectZeroOrOneAffected(this SqlCommand query)
        {
            var affected = query.ExecNonQuery();

            return affected == 1 || affected == 0;
        }

        public static T ExecExpectOne<T>(this SqlCommand query, Func<IDataRecord, T> convert)
        {
            return query.ExecExpectMultiple(convert).FirstOrDefault();
        }
        public static SqlCommand ExecExpectOne<T>(this SqlCommand query, Func<IDataRecord, T> convert, out T record)
        {
            record = query.ExecExpectMultiple(convert).FirstOrDefault();
            return query;
        }

        public static Collection<T> ExecExpectMultiple<T>(this SqlCommand query, Func<IDataRecord, T> convert)
        {
            using (query.Connection)
            {
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
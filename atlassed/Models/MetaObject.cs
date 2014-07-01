using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Atlassed.Models
{
    public abstract class MetaObject
    {
        public const string _objectId = "objectId";
        protected const string _metaProperties = "metaProperties";

        public JObject MetaProperties { get; set; }

        protected MetaObject()
        {

        }

        protected MetaObject(string metaProperties)
        {
            MetaProperties = JObject.Parse(metaProperties);
        }

        protected MetaObject(IDataRecord data)
        {
            MetaProperties = JObject.Parse(data.GetString(_metaProperties));
        }

        protected IEnumerable<SqlDataRecord> GenerateMetaFieldTable()
        {
            var metaFields = new List<SqlDataRecord>();

            try
            {
                SqlMetaData[] metaData = new SqlMetaData[2];
                metaData[0] = new SqlMetaData("FieldName", SqlDbType.VarChar, 30);
                metaData[1] = new SqlMetaData("FieldValue", SqlDbType.VarChar, -1);

                foreach (KeyValuePair<string, JToken> prop in MetaProperties)
                {
                    SqlDataRecord record = new SqlDataRecord(metaData);
                    record.SetString(0, prop.Key);
                    record.SetString(1, prop.Value.SelectToken("Value").ToString());
                    metaFields.Add(record);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return metaFields;
        }
    }
}
using Atlassed.Models.MapData;
using Atlassed.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json.Linq;

namespace Atlassed.Repositories.MapData
{
    abstract public class MetaObjectRepository
    {
        public const string _objectId = "objectId";
        public const string _metaProperties = "metaProperties";

        protected string GetMetaProperties(IDataRecord data)
        {
            return data.GetString(_metaProperties);
        }

        protected static IEnumerable<SqlDataRecord> GenerateMetaPropertyTable(MetaObject o)
        {
            var metaFields = new List<SqlDataRecord>();

            try
            {
                SqlMetaData[] metaData = new SqlMetaData[2];
                metaData[0] = new SqlMetaData("FieldName", SqlDbType.VarChar, 30);
                metaData[1] = new SqlMetaData("FieldValue", SqlDbType.VarChar, -1);

                foreach (KeyValuePair<string, JToken> prop in o.MetaPropertiesObject)
                {
                    SqlDataRecord record = new SqlDataRecord(metaData);
                    record.SetString(0, prop.Key);
                    // coming from the DB the value will be an object representing the field, with a "Value" key
                    // coming from the client the value will be a single value
                    var value = prop.Value.SelectToken("Value") ?? prop.Value;
                    if (value.Type == JTokenType.Null)
                    {
                        record.SetDBNull(1);
                    }
                    else
                    {
                        record.SetString(1, value.ToString());
                    }
                    metaFields.Add(record);
                }
            }
            catch (Exception e)
            {
            }

            return metaFields;
        }
    }
}
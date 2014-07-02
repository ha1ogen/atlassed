using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Atlassed.Models
{
    public abstract class MetaObject
    {
        public const string _objectId = "objectId";
        protected const string _metaProperties = "metaProperties";

        [JsonIgnore]
        public string MetaProperties { get; set; }

        [JsonProperty("MetaProperties")]
        public JObject MetaPropertiesObject { get { return JObject.Parse(MetaProperties); } }
        protected MetaObject()
        {

        }

        protected MetaObject(string metaProperties)
        {
            MetaProperties = metaProperties;
        }

        protected MetaObject(IDataRecord data)
        {
            MetaProperties = data.GetString(_metaProperties);
        }

        protected IEnumerable<SqlDataRecord> GenerateMetaFieldTable()
        {
            var metaFields = new List<SqlDataRecord>();

            try
            {
                SqlMetaData[] metaData = new SqlMetaData[2];
                metaData[0] = new SqlMetaData("FieldName", SqlDbType.VarChar, 30);
                metaData[1] = new SqlMetaData("FieldValue", SqlDbType.VarChar, -1);

                var metaPropertiesObject = MetaPropertiesObject;

                foreach (KeyValuePair<string, JToken> prop in metaPropertiesObject)
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
                Debug.WriteLine(e.Message);
            }

            return metaFields;
        }
    }
}
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

namespace Atlassed.Models.MapData
{
    public abstract class MetaObject
    {
        [JsonIgnore]
        public string MetaProperties { get; set; }

        [JsonProperty("MetaProperties")]
        public JObject MetaPropertiesObject { get { return JObject.Parse(MetaProperties ?? "{}"); } }
    }
}
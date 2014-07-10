using Atlassed.Repositories;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Atlassed.Models.MapData
{
    public class MetaField
    {
        public int FieldId { get; set; }
        public string ClassName { get; set; }
        public int FieldOrdinal { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public string FieldLabel { get; set; }
        public string FieldDescription { get; set; }
        public bool FieldIsRequired { get; set; }
        public bool FieldIsUnique { get; set; }
        [JsonIgnore]
        public string MetaConstraints { get; set; }
        [JsonProperty("MetaConstraints")]
        public JObject MetaConstraintsObject { get { return JObject.Parse(MetaConstraints ?? "{}"); } }
    }
    public class NewMetaField : MetaField
    {
        public string DefaultValue { get; set; }
    }

    public class MetaFieldValidator : IValidator<MetaField>
    {
        public bool Validate(MetaField record, out ICollection<ValidationError> errors)
        {
            errors = new List<ValidationError>();
            return true;
        }
    }
}
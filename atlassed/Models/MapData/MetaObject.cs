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
using System.ComponentModel.DataAnnotations;
using Atlassed.Repositories.MapData;

namespace Atlassed.Models.MapData
{
    public abstract class MetaObject
    {
        [JsonIgnore]
        public abstract int ObjectId { get; }
        [JsonIgnore]
        public abstract string _className { get; }
        [JsonIgnore]
        public string MetaProperties { get; set; }

        [JsonProperty("MetaProperties")]
        public JObject MetaPropertiesObject { get { return JObject.Parse(MetaProperties ?? "{}"); } }
    }

    public class MetaObjectValidator : IValidator<MetaObject>
    {
        private readonly MetaFieldRepository _metaFieldRepository;

        public MetaObjectValidator(MetaFieldRepository metaFieldRepository)
        {
            _metaFieldRepository = metaFieldRepository;
        }

        public bool Validate(MetaObject record, out IValidationResult result)
        {
            result = new ValidationResult();

            if (string.IsNullOrEmpty(record.MetaProperties))
            {
                result.AddError("MetaProperties", "MetaProperties are required");
                return result.IsValid();
            }

            if (record.ObjectId == 0 && string.IsNullOrEmpty(record._className))
                return result.IsValid();

            var metaProperties = record.MetaPropertiesObject;
            var metaFields = (record.ObjectId != 0) ? _metaFieldRepository.GetMany(record.ObjectId) : _metaFieldRepository.GetMany(record._className);

            foreach (var field in metaFields)
            {
                var property = metaProperties[field.FieldName];
                if (property == null)
                {
                    result.AddError(field.FieldName, field.FieldLabel + " is required");
                    continue;
                }
                var value = property.Value<string>();

                CheckType(field, value, ref result);
            }

            return result.IsValid();
        }

        private void CheckType(MetaField field, string value, ref IValidationResult result)
        {
            switch (field.FieldType)
            {
                case "BOOL":
                    bool tempBool;
                    if (!Boolean.TryParse(value, out tempBool))
                        result.AddError(field.FieldName, field.FieldLabel + " must be boolean");
                    break;
                case "BRULE":
                    break;
                case "DATE":
                    DateTime tempDateTime;
                    if (!DateTime.TryParse(value, out tempDateTime))
                        result.AddError(field.FieldName, field.FieldLabel + " must be a valid datetime string");
                    break;
                case "DECIM":
                    double tempDouble;
                    if (!Double.TryParse(value, out tempDouble))
                        result.AddError(field.FieldName, field.FieldLabel + " must be a decimal number");
                    break;
                case "INT":
                    int tempInt;
                    if (!Int32.TryParse(value, out tempInt))
                        result.AddError(field.FieldName, field.FieldLabel + " must be an integer");
                    break;
                case "URI":
                    if (!Uri.IsWellFormedUriString(value, UriKind.Absolute))
                        result.AddError(field.FieldName, field.FieldLabel + " must be a valid URI");
                    break;
                case "STR":
                    break;
            }
        }
    }
}
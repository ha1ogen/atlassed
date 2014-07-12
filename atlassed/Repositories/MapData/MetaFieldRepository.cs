using Atlassed.Models;
using Atlassed.Models.MapData;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Atlassed.Repositories.MapData
{
    public class MetaFieldRepository : IRepository<MetaField, NewMetaField, int, string>
    {
        public const string _fieldId = "fieldId";
        private const string _fieldOrdinal = "fieldOrdinal";
        public const string _fieldName = "fieldName";
        private const string _fieldType = "fieldType";
        private const string _fieldLabel = "fieldLabel";
        private const string _fieldDescription = "fieldDescription";
        private const string _fieldIsRequired = "fieldIsRequired";
        private const string _defaultValue = "defaultValue";
        private const string _fieldIsUnique = "fieldIsUnique";
        private const string _metaConstraints = "metaConstraints";

        private const string _spAddMetaField = "AddMetaField";
        private const string _spEditMetaField = "EditMetaField";
        private const string _spDeleteMetaField = "DeleteMetaField";
        private const string _spGetMetaFields = "GetMetaFields";
        private const string _fncCheckMetaFieldExists = "CheckMetaFieldExists";

        private readonly SqlConnectionFactory _connectionFactory;
        private readonly IValidator<MetaField> _validator;

        public MetaFieldRepository(SqlConnectionFactory f, IValidator<MetaField> v)
        {
            _connectionFactory = f;
            _validator = v;
        }

        public MetaField Create(NewMetaField record, out ICollection<ValidationError> errors)
        {
            if (!_validator.Validate(record, out errors))
                return null;

            return DB.NewSP(_spAddMetaField, _connectionFactory)
                .AddParam(MetaClassRepository._className, record.ClassName)
                .AddParam(_fieldName, record.FieldName)
                .AddParam(_fieldType, record.FieldType)
                .AddParam(_fieldLabel, record.FieldLabel)
                .AddParam(_fieldDescription, record.FieldDescription)
                .AddParam(_fieldIsRequired, record.FieldIsRequired)
                .AddParam(_defaultValue, record.DefaultValue)
                .AddParam(_fieldIsUnique, record.FieldIsUnique)
                .AddTVParam(_metaConstraints, GenerateMetaConstraintTable(record.MetaConstraintsObject))
                .AddParam(_fieldId, record.FieldId, ParameterDirection.Output)
                .AddParam(_fieldOrdinal, record.FieldOrdinal, ParameterDirection.Output)
                .ExecExpectOne(x => Create(x));
        }

        private MetaField Create(IDataRecord data)
        {
            return new MetaField()
            {
                FieldId = data.GetInt32(_fieldId),
                ClassName = data.GetString(MetaClassRepository._className),
                FieldOrdinal = data.GetInt32(_fieldOrdinal),
                FieldName = data.GetString(_fieldName),
                FieldType = data.GetString(_fieldType),
                FieldLabel = data.GetString(_fieldLabel),
                FieldDescription = data.GetString(_fieldDescription),
                FieldIsRequired = data.GetBoolean(_fieldIsRequired),
                FieldIsUnique = data.GetBoolean(_fieldIsUnique),
                MetaConstraints = data.GetString(_metaConstraints),
            };
        }

        public bool Update(ref MetaField record, out ICollection<ValidationError> errors)
        {
            if (!_validator.Validate(record, out errors))
                return false;

            return DB.NewSP(_spEditMetaField, _connectionFactory)
                    .AddParam(_fieldId, record.FieldId)
                    .AddParam(MetaClassRepository._className, record.ClassName)
                    .AddParam(_fieldName, record.FieldName)
                    .AddParam(_fieldLabel, record.FieldLabel)
                    .AddParam(_fieldDescription, record.FieldDescription)
                    .AddParam(_fieldIsRequired, record.FieldIsRequired)
                    .AddParam(_fieldIsUnique, record.FieldIsUnique)
                    .AddTVParam(_metaConstraints, GenerateMetaConstraintTable(record.MetaConstraintsObject))
                    .ExecExpectOne(x => Create(x), out record)
                    .GetReturnValue<bool>();
        }

        public bool Delete(int recordId)
        {
            return DB.NewSP(_spDeleteMetaField, _connectionFactory)
                .AddParam(_fieldId, recordId)
                .ExecExpectReturnValue<bool>();
        }

        public MetaField GetOne(int recordId)
        {
            return DB.NewSP(_spGetMetaFields, _connectionFactory)
                .AddParam(_fieldId, recordId)
                .ExecExpectOne(mf => Create(mf));
        }

        public IEnumerable<MetaField> GetMany(string className)
        {
            return DB.NewSP(_spGetMetaFields, _connectionFactory)
                .AddParam(MetaClassRepository._className, className)
                .ExecExpectMultiple(mf => Create(mf));
        }

        public bool RecordExists(int recordId)
        {
            return DB.NewSP(_fncCheckMetaFieldExists, _connectionFactory)
                .AddParam(_fieldId, recordId)
                .ExecExpectReturnValue<bool>();
        }

        private static IEnumerable<SqlDataRecord> GenerateMetaConstraintTable(JObject metaConstraintsObject)
        {
            var metaConstraintsTable = new List<SqlDataRecord>();

            try
            {
                SqlMetaData[] metaData = new SqlMetaData[2];
                metaData[0] = new SqlMetaData("ConstraintType", SqlDbType.VarChar, 10);
                metaData[1] = new SqlMetaData("ConstraintValue", SqlDbType.VarChar, 50);

                foreach (KeyValuePair<string, JToken> prop in metaConstraintsObject)
                {
                    SqlDataRecord record = new SqlDataRecord(metaData);
                    record.SetString(0, prop.Key);
                    record.SetString(1, prop.Value.ToString());
                    metaConstraintsTable.Add(record);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return metaConstraintsTable;
        }
    }
}
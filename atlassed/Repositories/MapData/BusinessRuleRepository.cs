using Atlassed.Models;
using Atlassed.Models.MapData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Atlassed.Repositories.MapData
{
    public class BusinessRuleRepository : MetaObjectRepository, IRepository<BusinessRule, BusinessRule, int, int>
    {
        private const string _businessRuleId = "businessRuleId";
        private const string _businessRuleClass = "businessRuleClass";
        private const string _code = "code";
        private const string _description = "description";
        private const string _isSystemRule = "isSystemRule";

        private const string _spAddBusinessRule = "AddBusinessRule";
        private const string _spEditBusinessRule = "EditBusinessRule";
        private const string _spDeleteBusinessRule = "DeleteBusinessRule";
        private const string _spGetBusinessRules = "GetBusinessRules";
        private const string _fncCheckBusinessRuleExists = "CheckBusinessRuleExists";

        private readonly SqlConnectionFactory _connectionFactory;
        private readonly IValidatorWNew<BusinessRule, BusinessRule> _validator;

        public BusinessRuleRepository(SqlConnectionFactory f, IValidatorWNew<BusinessRule, BusinessRule> v)
        {
            _connectionFactory = f;
            _validator = v;
        }

        public BusinessRule Create(BusinessRule record, out IValidationResult validationResult)
        {
            if (!_validator.ValidateNew(record, out validationResult))
                return null;

            return SqlValidator.TryExecCatchValidation(
                () => DB.NewSP(_spAddBusinessRule, _connectionFactory)
                    .AddParam(MetaClassRepository._className, record.ClassName)
                    .AddParam(_code, record.Code)
                    .AddParam(_description, record.Description)
                    .AddTVParam(_metaProperties, GenerateMetaPropertyTable(record))
                    .ExecExpectOne(x => Create(x))
                , ref validationResult);
        }

        private BusinessRule Create(IDataRecord data)
        {
            return new BusinessRule()
            {
                BusinessRuleId = data.GetInt32(_businessRuleId),
                ClassName = data.GetString(_businessRuleClass),
                Code = data.GetString(_code),
                Description = data.GetString(_description),
                IsSystemRule = data.GetBoolean(_isSystemRule),
                MetaProperties = GetMetaProperties(data)
            };
        }

        public bool Update(ref BusinessRule record, out IValidationResult validationResult)
        {
            if (!_validator.Validate(record, out validationResult))
                return false;

            return SqlValidator.TryExecCatchValidation(
                (rec) => DB.NewSP(_spEditBusinessRule, _connectionFactory)
                    .AddParam(_businessRuleId, rec.BusinessRuleId)
                    .AddParam(_code, rec.Code)
                    .AddParam(_description, rec.Description)
                    .AddTVParam(_metaProperties, GenerateMetaPropertyTable(rec))
                    .ExecExpectOne(x => Create(x), out rec)
                    .GetReturnValue<bool>()
                , ref validationResult, ref record);
        }

        public bool Delete(int recordId)
        {
            return DB.NewSP(_spDeleteBusinessRule, _connectionFactory)
                    .AddParam(_businessRuleId, recordId)
                    .ExecExpectReturnValue<bool>();
        }

        public bool RecordExists(int recordId)
        {
            return DB.NewSP(_fncCheckBusinessRuleExists, _connectionFactory)
                .AddParam(_businessRuleId, recordId)
                .ExecExpectReturnValue<bool>();
        }

        public BusinessRule GetOne(int recordId)
        {
            return DB.NewSP(_spGetBusinessRules, _connectionFactory)
                .AddParam(_businessRuleId, recordId)
                .ExecExpectOne(x => Create(x));
        }

        public IEnumerable<BusinessRule> GetMany(int parentId)
        {
            return DB.NewSP(_spGetBusinessRules, _connectionFactory)
                .AddParam(MetaClassRepository._classId, parentId)
                .ExecExpectMultiple(x => Create(x));
        }
    }
}
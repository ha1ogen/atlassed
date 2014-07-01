using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Atlassed.Models
{
    public class BusinessRule : MetaObject, IDbRow<BusinessRule>
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

        public int BusinessRuleId { get; set; }
        public string BusinessRuleClass { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsSystemRule { get; set; }

        private readonly bool _isCommitted = false;

        public BusinessRule()
        {
        }

        public BusinessRule(IDataRecord data)
        {
            BusinessRuleId = data.GetInt32(_businessRuleId);
            BusinessRuleClass = data.GetString(_businessRuleClass);
            Code = data.GetString(_code);
            Description = data.GetString(_description);
            IsSystemRule = data.GetBoolean(_isSystemRule);

            _isCommitted = true;
        }

        public BusinessRule(string className, string code, string description, string metaProperties)
            : base(metaProperties)
        {
            BusinessRuleClass = className;
            Code = code;
            Description = description;

            BusinessRuleId = DB.NewSP(_spAddBusinessRule)
                .AddParam(MetaClass._className, BusinessRuleClass)
                .AddParam(_code, Code)
                .AddParam(_description, Description)
                .AddTVParam(_metaProperties, GenerateMetaFieldTable())
                .AddReturn(SqlDbType.Int)
                .ExecExpectReturnValue<int>();

            _isCommitted = true;
        }

        public BusinessRule CommitUpdate()
        {
            if (!_isCommitted)
            {
                throw new Exception("record must be committed before updating");
            }

            return DB.NewSP(_spEditBusinessRule)
                    .AddParam(_businessRuleId, BusinessRuleId)
                    .AddParam(_code, Code)
                    .AddParam(_description, Description)
                    .AddTVParam(_metaProperties, GenerateMetaFieldTable())
                    .ExecNonQueryExpectSuccess()
                ? GetBusinessRule(BusinessRuleId)
                : null;

        }

        public bool Delete()
        {
            if (!_isCommitted)
            {
                throw new Exception("record must be committed before deleting");
            }

            return DB.NewSP(_spDeleteBusinessRule)
                    .AddParam(_businessRuleId, BusinessRuleId)
                    .ExecNonQueryExpectSuccess();
        }

        public static BusinessRule GetBusinessRule(int businessRuleId)
        {
            return DB.NewSP(_spGetBusinessRules)
                .AddParam(_businessRuleId, businessRuleId)
                .ExecExpectOne(x => new BusinessRule(x));
        }

        public static List<BusinessRule> GetAllBusinessRules(string className)
        {
            return DB.NewSP(_spGetBusinessRules)
                .AddParam(MetaClass._className, className)
                .ExecExpectMultiple(x => new BusinessRule(x)).ToList();
        }
    }
}
using Atlassed.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Atlassed.Models.MapData
{
    public class BusinessRule : MetaObject
    {
        public override int ObjectId { get { return BusinessRuleId; } }
        public override string _className { get { return ClassName; } }
        public int BusinessRuleId { get; set; }
        public string ClassName { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsSystemRule { get; set; }
    }

    public class BusinessRuleValidator : IValidatorWNew<BusinessRule, BusinessRule>
    {
        private IValidator<MetaObject> _metaObjectValidator;

        public BusinessRuleValidator(IValidator<MetaObject> metaObjectValidator)
        {
            _metaObjectValidator = metaObjectValidator;
        }

        public bool Validate(BusinessRule record, out IValidationResult result)
        {
            ValidateCommon(record, out result);

            if (record.BusinessRuleId == 0)
                result.AddError("BusinessRuleId", "Business Rule ID is required");

            return result.IsValid();
        }

        public bool ValidateNew(BusinessRule record, out IValidationResult result)
        {
            return ValidateCommon(record, out result);
        }

        private bool ValidateCommon(BusinessRule record, out IValidationResult result)
        {
            _metaObjectValidator.Validate(record, out result);

            if (string.IsNullOrEmpty(record.ClassName))
                result.AddError("ClassName", "Class Name is required");

            if (string.IsNullOrEmpty(record.Code))
                result.AddError("Code", "Code is required");
            else if (record.Code.Length > 5)
                result.AddError("Code", "Code must not exceed 5 characters");

            if (string.IsNullOrEmpty(record.Description))
                result.AddError("Description", "Description is requires");
            else if (record.Description.Length > 50)
                result.AddError("Description", "Description must not exceed 50 characters");

            return result.IsValid();
        }

    }
}
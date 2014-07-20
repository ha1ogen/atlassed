using Atlassed.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Atlassed.Models.MapData
{
    public class BusinessRuleClass : MetaClass
    {
    }

    public class BusinessRuleClassValidator : IValidatorWNew<BusinessRuleClass, BusinessRuleClass>
    {
        private readonly IValidatorWNew<MetaClass, MetaClass> _metaClassValidator;

        public BusinessRuleClassValidator(IValidatorWNew<MetaClass, MetaClass> metaClassValidator)
        {
            _metaClassValidator = metaClassValidator;
        }

        public bool Validate(BusinessRuleClass record, out IValidationResult result)
        {
            return _metaClassValidator.Validate(record, out result);
        }

        public bool ValidateNew(BusinessRuleClass record, out IValidationResult result)
        {
            return _metaClassValidator.ValidateNew(record, out result);
        }
    }
}
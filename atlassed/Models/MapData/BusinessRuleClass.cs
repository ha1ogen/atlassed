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

    public class BusinessRuleClassValidator : IValidator<BusinessRuleClass>
    {
        public bool Validate(BusinessRuleClass record, out ICollection<ValidationError> errors)
        {
            errors = new List<ValidationError>();
            return true;
        }
    }
}
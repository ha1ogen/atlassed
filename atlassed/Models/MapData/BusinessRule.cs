using Atlassed.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Atlassed.Models.MapData
{
    public class BusinessRule : MetaObject
    {

        public int BusinessRuleId { get; set; }
        public string ClassName { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsSystemRule { get; set; }
    }

    public class BusinessRuleValidator : IValidator<BusinessRule>
    {
        public bool Validate(BusinessRule record, out ICollection<ValidationError> errors)
        {
            errors = new List<ValidationError>();

            if (record.Code.Length > 5)
                errors.Add(new ValidationError("Code must not exceed 5 characters"));

            return !errors.Any();
        }
    }
}
using Atlassed.Repositories.MapData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Atlassed.Models.MapData
{
    public class MetaClass
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string ClassType { get; set; }
        public string ClassTypeDescription { get; set; }
        public string ClassLabel { get; set; }
        public IEnumerable<MetaField> MetaFields { get; set; }
    }

    public class MetaClassValidator : IValidatorWNew<MetaClass, MetaClass>
    {
        public bool Validate(MetaClass record, out IValidationResult result)
        {
            ValidateCommon(record, out result);

            if (record.ClassId == 0)
                result.AddError("ClassId", "Class ID is required");

            return result.IsValid();
        }


        public bool ValidateNew(MetaClass record, out IValidationResult result)
        {
            ValidateCommon(record, out result);

            if (string.IsNullOrEmpty(record.ClassName))
                result.AddError("ClassName", "Class Name is required");
            else if (record.ClassName.Length > 20)
                result.AddError("ClassName", "Class Name must not exceed 20 characters");

            return result.IsValid();
        }

        private bool ValidateCommon(MetaClass record, out IValidationResult result)
        {
            result = new ValidationResult();

            if (string.IsNullOrEmpty(record.ClassLabel))
                result.AddError("ClassLabel", "Class Label is required");
            else if (record.ClassLabel.Length > 50)
                result.AddError("ClassLabel", "Class Label must not exceed 50 characters");

            return result.IsValid();
        }
    }
}
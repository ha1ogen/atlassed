using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Atlassed.Models.MapData
{
    public class BusinessRuleClass : MetaClass, IDbRow<BusinessRuleClass>
    {
        private const string _spAddBusinessRuleClass = "AddBusinessRuleClass";
        private const string _spEditBusinessRuleClass = "EditBusinessRuleClass";
        private const string _spDeleteBusinessRuleClass = "DeleteBusinessRuleClass";

        private readonly bool _isCommitted = false;

        public BusinessRuleClass()
        {
            _isCommitted = false;
        }
        public BusinessRuleClass(IDataRecord data)
            : base(data)
        {
            _isCommitted = true;
        }

        public BusinessRuleClass(string className, string classLabel)
            : base(className, MapData.ClassType.BRULE, classLabel)
        {
            DB.NewSP(_spAddBusinessRuleClass)
                .AddParam(_className, ClassName)
                .AddParam(_classLabel, classLabel)
                .ExecNonQuery();

            _isCommitted = true;
        }

        public static BusinessRuleClass Create(BusinessRuleClass businessRuleClass)
        {
            new BusinessRuleClass(businessRuleClass.ClassName, businessRuleClass.ClassLabel);
            // populate class type description
            return GetBusinessRuleClass(businessRuleClass.ClassName);
        }

        public static BusinessRuleClass Update(BusinessRuleClass businessRuleClass)
        {
            var brc = GetBusinessRuleClass(businessRuleClass.ClassName);
            if (brc == null) return null;

            brc.ClassLabel = businessRuleClass.ClassLabel;

            return brc;
        }

        public BusinessRuleClass CommitUpdate()
        {
            if (!_isCommitted)
            {
                throw new Exception("record must be committed before updating");
            }

            return DB.NewSP(_spEditBusinessRuleClass)
                    .AddParam(_className, ClassName)
                    .AddParam(_classLabel, ClassLabel)
                    .ExecNonQueryExpectSuccess()
                ? GetBusinessRuleClass(ClassName)
                : null;

        }

        public bool Delete()
        {
            if (!_isCommitted)
            {
                throw new Exception("record must be committed before deleting");
            }

            return DB.NewSP(_spDeleteBusinessRuleClass)
                    .AddParam(_className, ClassName)
                    .ExecNonQueryExpectSuccess();
        }

        public static BusinessRuleClass GetBusinessRuleClass(string className)
        {
            return DB.NewSP(_spGetMetaClasses)
                .AddParam(_className, className)
                .AddParam(_classType, MapData.ClassType.BRULE.ToString())
                .ExecExpectOne(x => new BusinessRuleClass(x));
        }

        public static List<BusinessRuleClass> GetAllBusinessRuleClasses()
        {
            return DB.NewSP(_spGetMetaClasses)
                .AddParam(_classType, MapData.ClassType.BRULE.ToString())
                .ExecExpectMultiple(x => new BusinessRuleClass(x)).ToList();
        }
    }
}
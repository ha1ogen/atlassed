using Atlassed.Models;
using Atlassed.Models.MapData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Atlassed.Repositories.MapData
{
    public class BusinessRuleClassRepository : MetaClassRepository, IRepository<BusinessRuleClass, BusinessRuleClass, int, int?>
    {
        private const string _spAddBusinessRuleClass = "AddBusinessRuleClass";
        private const string _spEditBusinessRuleClass = "EditBusinessRuleClass";
        private const string _spDeleteBusinessRuleClass = "DeleteBusinessRuleClass";
        private const string _spGetBusinessRuleClasses = "GetBusinessRuleClasses";

        private readonly SqlConnectionFactory _connectionFactory;
        private readonly IValidator<BusinessRuleClass> _validator;

        public BusinessRuleClassRepository(SqlConnectionFactory f, IValidator<BusinessRuleClass> v)
        {
            _connectionFactory = f;
            _validator = v;
        }

        public BusinessRuleClass Create(BusinessRuleClass record, out IEnumerable<ValidationError> errors)
        {
            if (!_validator.Validate(record, out errors))
                return null;

            return DB.NewSP(_spAddBusinessRuleClass, _connectionFactory)
                .AddParam(_className, record.ClassName)
                .AddParam(_classLabel, record.ClassLabel)
                .ExecExpectOne(x => Create(x));
        }

        private BusinessRuleClass Create(IDataRecord data)
        {
            return new BusinessRuleClass()
            {
                ClassId = data.GetInt32(_classId),
                ClassName = data.GetString(_className),
                ClassType = data.GetString(_classType),
                ClassTypeDescription = data.GetString(_classTypeDescription),
                ClassLabel = data.GetString(_classLabel)
            };
        }

        public bool Update(ref BusinessRuleClass record, out IEnumerable<ValidationError> errors)
        {
            if (!_validator.Validate(record, out errors))
                return false;

            return DB.NewSP(_spEditBusinessRuleClass, _connectionFactory)
                    .AddParam(_classId, record.ClassId)
                    .AddParam(_classLabel, record.ClassLabel)
                    .ExecExpectOne(x => Create(x), out record)
                    .GetReturnValue<bool>();
        }

        public bool Delete(int recordId)
        {
            return DB.NewSP(_spDeleteBusinessRuleClass, _connectionFactory)
                    .AddParam(_classId, recordId)
                    .ExecExpectReturnValue<bool>();
        }

        public BusinessRuleClass GetOne(int recordId)
        {
            return DB.NewSP(_spGetBusinessRuleClasses, _connectionFactory)
                .AddParam(_classId, recordId)
                .ExecExpectOne(x => Create(x));
        }

        public IEnumerable<BusinessRuleClass> GetMany(int? parentId = null)
        {
            return DB.NewSP(_spGetBusinessRuleClasses, _connectionFactory)
                .ExecExpectMultiple(x => Create(x));
        }
    }
}
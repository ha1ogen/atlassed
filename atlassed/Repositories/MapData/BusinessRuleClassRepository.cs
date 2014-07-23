using Atlassed.Models;
using Atlassed.Models.MapData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        private readonly IValidatorWNew<BusinessRuleClass, BusinessRuleClass> _validator;
        private readonly IRepository<MetaField, NewMetaField, int, string> _metaFieldRepository;

        public BusinessRuleClassRepository(SqlConnectionFactory f, IValidatorWNew<BusinessRuleClass, BusinessRuleClass> v)
            : base(f)
        {
            _connectionFactory = f;
            _validator = v;
            _metaFieldRepository = new MetaFieldRepository(f, new MetaFieldValidator());
        }

        public BusinessRuleClass Create(BusinessRuleClass record, out IValidationResult validationResult)
        {
            if (!_validator.ValidateNew(record, out validationResult))
                return null;

            return SqlValidator.TryExecCatchValidation(
                () => DB.NewSP(_spAddBusinessRuleClass, _connectionFactory)
                    .AddParam(_className, record.ClassName)
                    .AddParam(_classLabel, record.ClassLabel)
                    .ExecExpectOne(x => Create(x))
                , ref validationResult);
        }

        private BusinessRuleClass Create(IDataRecord data)
        {
            return new BusinessRuleClass()
            {
                ClassId = data.GetInt32(_classId),
                ClassName = data.GetString(_className),
                ClassType = data.GetString(_classType),
                ClassTypeDescription = data.GetString(_classTypeDescription),
                ClassLabel = data.GetString(_classLabel),
                MetaFields = _metaFieldRepository.GetMany(data.GetString(_className))
            };
        }

        public bool Update(ref BusinessRuleClass record, out IValidationResult validationResult)
        {
            if (!_validator.Validate(record, out validationResult))
                return false;

            try
            {
                return DB.NewSP(_spEditBusinessRuleClass, _connectionFactory)
                    .AddParam(_classId, record.ClassId)
                    .AddParam(_classLabel, record.ClassLabel)
                    .ExecExpectOne(x => Create(x), out record)
                    .GetReturnValue<bool>();
            }
            catch (SqlException e)
            {
                e.ParseValidationMessages(ref validationResult);
                return false;
            }
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
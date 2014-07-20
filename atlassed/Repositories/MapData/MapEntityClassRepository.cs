using Atlassed.Models;
using Atlassed.Models.MapData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Atlassed.Repositories.MapData
{
    public class MapEntityClassRepository : MetaClassRepository, IRepository<MapEntityClass, MapEntityClass, int, int?>
    {
        public const string _mapLabelFieldName = "mapLabelFieldName";

        private const string _spAddMapEntityClass = "AddMapEntityClass";
        private const string _spEditMapEntityClass = "EditMapEntityClass";
        private const string _spDeleteMapEntityClass = "DeleteMapEntityClass";
        private const string _spGetMapEntityClasses = "GetMapEntityClasses";

        private readonly SqlConnectionFactory _connectionFactory;
        private readonly IValidatorWNew<MapEntityClass, MapEntityClass> _validator;
        private readonly IRepository<MetaField, NewMetaField, int, string> _metaFieldRepository;

        public MapEntityClassRepository(SqlConnectionFactory f, IValidatorWNew<MapEntityClass, MapEntityClass> v)
            : base(f)
        {
            _connectionFactory = f;
            _validator = v;
            _metaFieldRepository = new MetaFieldRepository(f, new MetaFieldValidator());
        }

        public MapEntityClass Create(MapEntityClass record, out IValidationResult validationResult)
        {
            if (!_validator.ValidateNew(record, out validationResult))
                return null;

            return SqlValidator.TryExecCatchValidation(
                () => DB.NewSP(_spAddMapEntityClass, _connectionFactory)
                    .AddParam(_className, record.ClassName)
                    .AddParam(_classLabel, record.ClassLabel)
                    .AddParam(_mapLabelFieldName, record.MapLabelFieldName)
                    .ExecExpectOne(x => Create(x))
                , ref validationResult);
        }

        private MapEntityClass Create(IDataRecord data)
        {
            return new MapEntityClass()
            {
                ClassId = data.GetInt32(_classId),
                ClassName = data.GetString(_className),
                ClassType = data.GetString(_classType),
                ClassTypeDescription = data.GetString(_classTypeDescription),
                ClassLabel = data.GetString(_classLabel),
                MapLabelFieldName = data.GetString(_mapLabelFieldName),
                MetaFields = _metaFieldRepository.GetMany(data.GetString(_className))
            };
        }

        public bool Update(ref MapEntityClass record, out IValidationResult validationResult)
        {
            if (!_validator.Validate(record, out validationResult))
                return false;

            return SqlValidator.TryExecCatchValidation(
                (rec) => DB.NewSP(_spEditMapEntityClass, _connectionFactory)
                    .AddParam(_classId, rec.ClassId)
                    .AddParam(_classLabel, rec.ClassLabel)
                    .AddParam(_mapLabelFieldName, rec.MapLabelFieldName)
                    .ExecExpectOne(x => Create(x), out rec)
                    .GetReturnValue<bool>()
                , ref validationResult, ref record);
        }

        public bool Delete(int recordId)
        {
            return DB.NewSP(_spDeleteMapEntityClass, _connectionFactory)
                    .AddParam(_classId, recordId)
                    .ExecExpectReturnValue<bool>();
        }

        public MapEntityClass GetOne(int recordId)
        {
            return DB.NewSP(_spGetMapEntityClasses, _connectionFactory)
                .AddParam(_classId, recordId)
                .ExecExpectOne(x => Create(x));
        }

        public IEnumerable<MapEntityClass> GetMany(int? parentId = null)
        {
            return DB.NewSP(_spGetMapEntityClasses, _connectionFactory)
                .ExecExpectMultiple(x => Create(x));
        }
    }
}
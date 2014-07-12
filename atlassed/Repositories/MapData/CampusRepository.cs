using Atlassed.Models;
using Atlassed.Models.MapData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Atlassed.Repositories.MapData
{
    public class CampusRepository : MetaObjectRepository, IRepository<CampusMap, CampusMap, int, int?>
    {
        public const string _campusMapId = "campusMapId";
        private const string _campusName = "campusName";
        private const string _mapCoordinates = "mapCoordinates";

        private const string _spAddCampusMap = "AddCampusMap";
        private const string _spEditCampusMap = "EditCampusMap";
        private const string _spDeleteCampusMap = "DeleteCampusMap";
        private const string _spGetCampusMaps = "GetCampuses";
        private const string _fncCheckCampusExists = "CheckCampusExists";

        private readonly SqlConnectionFactory _connectionFactory;
        private readonly IValidator<CampusMap> _validator;

        public CampusRepository(SqlConnectionFactory f, IValidator<CampusMap> v)
        {
            _connectionFactory = f;
            _validator = v;
        }

        public CampusMap Create(CampusMap record, out ICollection<ValidationError> errors)
        {
            if (!_validator.Validate(record, out errors))
                return null;

            return DB.NewSP(_spAddCampusMap, _connectionFactory)
                    .AddParam(_campusName, record.CampusName)
                    .AddParam(_mapCoordinates, record.MapCoordinates.ToString())
                    .AddTVParam(_metaProperties, GenerateMetaPropertyTable(record))
                    .ExecExpectOne(x => Create(x));
        }

        private CampusMap Create(IDataRecord data)
        {
            return new CampusMap()
            {
                MapId = data.GetInt32(data.GetOrdinal(_campusMapId)),
                CampusName = data.GetString(data.GetOrdinal(_campusName)),
                MapCoordinates = Coordinate.Parse(data.GetString(_mapCoordinates)),
                MetaProperties = GetMetaProperties(data)
            };
        }

        public bool Update(ref CampusMap record, out ICollection<ValidationError> errors)
        {
            if (!_validator.Validate(record, out errors))
                return false;

            return DB.NewSP(_spEditCampusMap, _connectionFactory)
                    .AddParam(_campusMapId, record.MapId)
                    .AddParam(_campusName, record.CampusName)
                    .AddParam(_mapCoordinates, record.MapCoordinates.ToString())
                    .AddTVParam(_metaProperties, GenerateMetaPropertyTable(record))
                    .ExecExpectOne(x => Create(x), out record)
                    .GetReturnValue<bool>();
        }

        public bool Delete(int recordId)
        {
            return DB.NewSP(_spDeleteCampusMap, _connectionFactory)
                .AddParam(_campusMapId, recordId)
                .ExecExpectReturnValue<bool>();
        }

        public bool RecordExists(int recordId)
        {
            return DB.NewSP(_fncCheckCampusExists, _connectionFactory)
                .AddParam(_campusMapId, recordId)
                .ExecExpectReturnValue<bool>();
        }

        public CampusMap GetOne(int recordId)
        {
            return DB.NewSP(_spGetCampusMaps, _connectionFactory)
                .AddParam(_campusMapId, recordId)
                .ExecExpectOne(c => Create(c));
        }

        public IEnumerable<CampusMap> GetMany(int? parentId = null)
        {
            return DB.NewSP(_spGetCampusMaps, _connectionFactory)
                .ExecExpectMultiple(x => Create(x));
        }
    }
}
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
        private readonly IValidatorWNew<CampusMap, CampusMap> _validator;

        public CampusRepository(SqlConnectionFactory f, IValidatorWNew<CampusMap, CampusMap> v)
        {
            _connectionFactory = f;
            _validator = v;
        }

        public CampusMap Create(CampusMap record, out IValidationResult validationResult)
        {
            if (!_validator.ValidateNew(record, out validationResult))
                return null;

            return SqlValidator.TryExecCatchValidation(
                () => DB.NewSP(_spAddCampusMap, _connectionFactory)
                    .AddParam(_campusName, record.CampusName)
                    .AddParam(_mapCoordinates, record.MapCoordinates.ToString())
                    .AddTVParam(_metaProperties, GenerateMetaPropertyTable(record))
                    .ExecExpectOne(x => Create(x))
                , ref validationResult);
        }

        private CampusMap Create(IDataRecord data)
        {
            var coordinates = Coordinate.ParseMultiCoordinateString(data.GetString(_mapCoordinates));
            if (coordinates.Count() != 2) return null;

            return new CampusMap()
            {
                MapId = data.GetInt32(data.GetOrdinal(_campusMapId)),
                CampusName = data.GetString(data.GetOrdinal(_campusName)),
                MapCoordinates = new Tuple<Coordinate, Coordinate>(coordinates.First(), coordinates.Last()),
                MetaProperties = GetMetaProperties(data)
            };
        }

        public bool Update(ref CampusMap record, out IValidationResult validationResult)
        {
            if (!_validator.Validate(record, out validationResult))
                return false;

            return SqlValidator.TryExecCatchValidation(
                (rec) => DB.NewSP(_spEditCampusMap, _connectionFactory)
                    .AddParam(_campusMapId, rec.MapId)
                    .AddParam(_campusName, rec.CampusName)
                    .AddParam(_mapCoordinates, rec.MapCoordinates.ToString())
                    .AddTVParam(_metaProperties, GenerateMetaPropertyTable(rec))
                    .ExecExpectOne(x => Create(x), out rec)
                    .GetReturnValue<bool>()
                , ref validationResult, ref record);
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
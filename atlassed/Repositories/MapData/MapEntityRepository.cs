using Atlassed.Models;
using Atlassed.Models.MapData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Atlassed.Repositories.MapData
{
    public class MapEntityRepository : MetaObjectRepository, ISearchableRepository<MapEntity, MapEntity, int, int, SearchResult>
    {
        public const string _entityId = "entityId";
        public const string _entityCoordinates = "entityPoints";

        private const string _spAddMapEntity = "AddMapEntity";
        private const string _spEditMapEntity = "EditMapEntity";
        private const string _spDeleteMapEntity = "DeleteMapEntity";
        private const string _spGetMapEntities = "GetMapEntities";
        private const string _fncCheckMapEntityExists = "CheckMapEntityExists";
        private const string _spSearchMapEntities = "SearchMapEntities";

        private readonly SqlConnectionFactory _connectionFactory;
        private readonly IValidatorWNew<MapEntity, MapEntity> _validator;

        public MapEntityRepository(SqlConnectionFactory f, IValidatorWNew<MapEntity, MapEntity> v)
        {
            _connectionFactory = f;
            _validator = v;
        }

        public MapEntity Create(MapEntity record, out IValidationResult validationResult)
        {
            if (!_validator.ValidateNew(record, out validationResult))
                return null;

            return SqlValidator.TryExecCatchValidation(
                () => DB.NewSP(_spAddMapEntity, _connectionFactory)
                    .AddParam(MapRepository._mapId, record.MapId)
                    .AddParam(MetaClassRepository._className, record.ClassName)
                    .AddParam(_entityCoordinates, Coordinate.MultiToString(record.EntityCoordinates))
                    .AddTVParam(_metaProperties, GenerateMetaPropertyTable(record))
                    .ExecExpectOne(x => Create(x))
                , ref validationResult);
        }

        private MapEntity Create(IDataRecord data)
        {
            return new MapEntity()
            {
                EntityId = data.GetInt32(_entityId),
                ClassName = data.GetString(MetaClassRepository._className),
                MapId = data.GetInt32(MapRepository._mapId),
                EntityCoordinates = Coordinate.ParseMultiCoordinateString(data.GetString(_entityCoordinates)),
                MapLabel = data.GetString("mapLabel"),
                MetaProperties = GetMetaProperties(data)
            };
        }

        public bool Update(ref MapEntity record, out IValidationResult validationResult)
        {
            if (!_validator.Validate(record, out validationResult))
                return false;

            return SqlValidator.TryExecCatchValidation(
                (rec) => DB.NewSP(_spEditMapEntity, _connectionFactory)
                    .AddParam(_entityId, rec.EntityId)
                    .AddTVParam(_metaProperties, GenerateMetaPropertyTable(rec))
                    .ExecExpectOne(x => Create(x), out rec)
                    .GetReturnValue<bool>()
                , ref validationResult, ref record);
        }

        public bool Delete(int recordId)
        {
            return DB.NewSP(_spDeleteMapEntity, _connectionFactory)
                    .AddParam(_entityId, recordId)
                    .ExecExpectReturnValue<bool>();
        }

        public bool RecordExists(int recordId)
        {
            return DB.NewSP(_fncCheckMapEntityExists, _connectionFactory)
                .AddParam(_entityId, recordId)
                .ExecExpectReturnValue<bool>();
        }

        public MapEntity GetOne(int recordId)
        {
            return DB.NewSP(_spGetMapEntities, _connectionFactory)
                .AddParam(_entityId, recordId)
                .ExecExpectOne(mf => Create(mf));
        }

        public IEnumerable<MapEntity> GetMany(int mapId)
        {
            return DB.NewSP(_spGetMapEntities, _connectionFactory)
                .AddParam(MapRepository._mapId, mapId)
                .ExecExpectMultiple(x => Create(x));
        }

        public IEnumerable<SearchResult> Search(string query, int skip, int? take)
        {
            return DB.NewSP(_spSearchMapEntities, _connectionFactory)
                        .AddParam(SearchResult._query, query)
                        .AddParam("skip", skip)
                        .AddParam("take", take)
                        .ExecExpectMultiple<SearchResult>(x => new SearchResult(x))
                        .ToList();
        }
    }
}
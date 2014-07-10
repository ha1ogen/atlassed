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
        private const string _spSearchMapEntities = "SearchMapEntities";

        private readonly SqlConnectionFactory _connectionFactory;
        private readonly IValidator<MapEntity> _validator;

        public MapEntityRepository(SqlConnectionFactory f, IValidator<MapEntity> v)
        {
            _connectionFactory = f;
            _validator = v;
        }

        public MapEntity Create(MapEntity record, out ICollection<ValidationError> errors)
        {
            if (!_validator.Validate(record, out errors))
                return null;

            return DB.NewSP(_spAddMapEntity, _connectionFactory)
                    .AddParam(Map._mapId, record.MapId)
                    .AddParam(MetaClassRepository._className, record.ClassName)
                    .AddParam(_entityCoordinates, Coordinate.MultiToString(record.EntityCoordinates))
                    .AddTVParam(_metaProperties, GenerateMetaPropertyTable(record))
                    .ExecExpectOne(x => Create(x));
        }

        private MapEntity Create(IDataRecord data)
        {
            return new MapEntity()
            {
                EntityId = data.GetInt32(_entityId),
                ClassName = data.GetString(MetaClassRepository._className),
                MapId = data.GetInt32(Map._mapId),
                EntityCoordinates = Coordinate.ParseMultiCoordinateString(data.GetString(_entityCoordinates)),
                MapLabel = data.GetString("mapLabel"),
                MetaProperties = GetMetaProperties(data)
            };
        }

        public bool Update(ref MapEntity record, out ICollection<ValidationError> errors)
        {
            if (!_validator.Validate(record, out errors))
                return false;

            return DB.NewSP(_spEditMapEntity, _connectionFactory)
                    .AddParam(_entityId, record.EntityId)
                    .AddTVParam(_metaProperties, GenerateMetaPropertyTable(record))
                    .ExecExpectOne(x => Create(x), out record)
                    .GetReturnValue<bool>();
        }

        public bool Delete(int recordId)
        {
            return DB.NewSP(_spDeleteMapEntity, _connectionFactory)
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
                .AddParam(Map._mapId, mapId)
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
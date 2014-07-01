using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Atlassed.Models
{
    public class MapEntity : MetaObject, IDbRow<MapEntity>
    {
        public const string _entityId = "entityId";
        public const string _entityPoints = "entityPoints";

        private const string _spAddMapEntity = "AddMapEntity";
        private const string _spEditMapEntity = "EditMapEntity";
        private const string _spDeleteMapEntity = "DeleteMapEntity";
        private const string _spGetMapEntities = "GetMapEntities";
        private const string _spSearchMapEntities = "SearchMapEntities";

        public int EntityId { get; set; }
        public string ClassName { get; set; }
        public int MapId { get; set; }
        public List<Point> EntityPoints { get; set; }
        public string MapLabel { get; private set; }

        private readonly bool _isCommitted = false;

        public MapEntity()
        {
        }

        public MapEntity(IDataRecord data)
            : base(data)
        {
            EntityId = data.GetInt32(_entityId);
            ClassName = data.GetString(MetaClass._className);
            MapId = data.GetInt32(Map._mapId);
            EntityPoints = Point.ParseMultiPointString(data.GetString(_entityPoints));

            var label = MetaPropertiesObject().GetValue(data.GetString(MapEntityClass._mapLabelFieldName));
            MapLabel = label == null ? "[null]" : label.Value<string>("Value");

            _isCommitted = true;
        }

        public MapEntity(int mapId, string className, List<Point> entityPoints, string metaProperties)
            : base(metaProperties)
        {
            MapId = mapId;
            ClassName = className;
            EntityPoints = entityPoints;

            EntityId = DB.NewSP(_spAddMapEntity)
                    .AddParam(Map._mapId, MapId)
                    .AddParam(MetaClass._className, ClassName)
                    .AddParam(_entityPoints, Point.MultiToString(EntityPoints))
                    .AddTVParam(_metaProperties, GenerateMetaFieldTable())
                    .AddReturn(SqlDbType.Int)
                    .ExecExpectReturnValue<int>();

            _isCommitted = true;
        }

        public static MapEntity Create(MapEntity entity)
        {
            return new MapEntity(entity.MapId, entity.ClassName, entity.EntityPoints, entity.MetaProperties.ToString());
        }

        public static MapEntity Update(MapEntity entity)
        {
            var e = GetMapEntity(entity.EntityId);
            if (e == null) return null;

            e.MetaProperties = entity.MetaProperties;

            return e;
        }

        public static List<SearchResult> Search(string query)
        {
            return DB.NewSP(_spSearchMapEntities)
                        .AddParam(SearchResult._query, query)
                        .ExecExpectMultiple<SearchResult>(x => new SearchResult(x))
                        .ToList();
        }

        public MapEntity CommitUpdate()
        {
            if (!_isCommitted)
            {
                throw new Exception("record must be committed before updating");
            }

            return DB.NewSP(_spEditMapEntity)
                    .AddParam(_entityId, EntityId)
                    .AddTVParam(_metaProperties, GenerateMetaFieldTable())
                    .ExecNonQueryExpectSuccess()
                ? GetMapEntity(EntityId)
                : null;

        }

        public bool Delete()
        {
            if (!_isCommitted)
            {
                throw new Exception("record must be committed before deleting");
            }

            return DB.NewSP(_spDeleteMapEntity)
                    .AddParam(_entityId, EntityId)
                    .ExecNonQueryExpectSuccess();
        }

        public static MapEntity GetMapEntity(int entityId)
        {
            return DB.NewSP(_spGetMapEntities)
                .AddParam(_entityId, entityId)
                .ExecExpectOne(x => new MapEntity(x));
        }

        public static List<MapEntity> GetAllMapEntities(int mapId, string entityClasses)
        {
            return DB.NewSP(_spGetMapEntities)
                .AddParam(Map._mapId, mapId)
                .AddParam("entityClasses", entityClasses)
                .ExecExpectMultiple(x => new MapEntity(x)).ToList();
        }
    }
}
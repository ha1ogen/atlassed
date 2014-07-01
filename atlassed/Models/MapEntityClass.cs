using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Atlassed.Models
{
    public class MapEntityClass : MetaClass, IDbRow<MapEntityClass>
    {
        public const string _mapLabelFieldName = "mapLabelFieldName";

        private const string _spAddMapEntityClass = "AddMapEntityClass";
        private const string _spEditMapEntityClass = "EditMapEntityClass";
        private const string _spDeleteMapEntityClass = "DeleteMapEntityClass";
        private const string _spGetMapEntityClasses = "GetMapEntityClasses";

        public string MapLabelFieldName { get; set; }

        private readonly bool _isCommitted = false;

        public MapEntityClass(IDataRecord data)
            : base(data)
        {
            MapLabelFieldName = data.GetString(data.GetOrdinal(_mapLabelFieldName));

            _isCommitted = true;
        }
        public MapEntityClass(string className, string classLabel, string mapLabelFieldName, string mapLabelFieldLabel, string mapLabelFieldDescription, string metaConstraints)
            : base(className, ClassType.ENTITY, classLabel)
        {
            MapLabelFieldName = mapLabelFieldName;

            DB.NewSP(_spAddMapEntityClass)
                .AddParam(_className, ClassName)
                .AddParam(_classLabel, classLabel)
                .AddParam(_mapLabelFieldName, MapLabelFieldName)
                .AddParam("mapLabelFieldLabel", mapLabelFieldLabel)
                .AddParam("mapLabelFieldDescription", mapLabelFieldDescription)
                .AddTVParam("mapLabelFieldMetaConstraints", MetaField.GenerateMetaConstraintTable(metaConstraints))
                .ExecuteNonQuery();

            _isCommitted = true;
        }

        public MapEntityClass CommitUpdate()
        {
            if (!_isCommitted)
            {
                throw new Exception("record must be committed before updating");
            }

            return DB.NewSP(_spEditMapEntityClass)
                    .AddParam(_className, ClassName)
                    .AddParam(_classLabel, ClassLabel)
                    .AddParam(_mapLabelFieldName, MapLabelFieldName)
                    .ExecNonQueryExpectSuccess()
                ? GetMapEntityClass(ClassName)
                : null;

        }

        public bool Delete()
        {
            if (!_isCommitted)
            {
                throw new Exception("record must be committed before deleting");
            }

            return DB.NewSP(_spDeleteMapEntityClass)
                    .AddParam(_className, ClassName)
                    .ExecNonQueryExpectSuccess();
        }

        public static MapEntityClass GetMapEntityClass(string className)
        {
            return DB.NewSP(_spGetMapEntityClasses)
                .AddParam(_className, className)
                .AddParam(_classType, ClassType.ENTITY.ToString())
                .ExecExpectOne(x => new MapEntityClass(x));
        }

        public static List<MapEntityClass> GetAllMapEntityClasses()
        {
            return DB.NewSP(_spGetMapEntityClasses)
                .AddParam(_classType, ClassType.ENTITY.ToString())
                .ExecExpectMultiple(x => new MapEntityClass(x)).ToList();
        }
    }
}
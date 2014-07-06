using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Atlassed.Models.MapData
{
    public class MapEntityClass : MetaClass, IDbRow<MapEntityClass>
    {
        private const string _iconFilenameExtension = ".png";

        public const string _mapLabelFieldName = "mapLabelFieldName";

        private const string _spAddMapEntityClass = "AddMapEntityClass";
        private const string _spEditMapEntityClass = "EditMapEntityClass";
        private const string _spDeleteMapEntityClass = "DeleteMapEntityClass";
        private const string _spGetMapEntityClasses = "GetMapEntityClasses";

        public string MapLabelFieldName { get; set; }
        public string IconFilename { get { return "icon-" + ClassId + _iconFilenameExtension; } }

        private readonly bool _isCommitted = false;

        public MapEntityClass()
        {
            _isCommitted = false;
        }
        public MapEntityClass(IDataRecord data)
            : base(data)
        {
            MapLabelFieldName = data.GetString(_mapLabelFieldName);

            _isCommitted = true;
        }
        public MapEntityClass(string className, string classLabel, string mapLabelFieldName)
            : base(className, MapData.ClassType.ENTITY, classLabel)
        {
            MapLabelFieldName = mapLabelFieldName;

            DB.NewSP(_spAddMapEntityClass)
                .AddParam(_className, ClassName)
                .AddParam(_classLabel, classLabel)
                .AddParam(_mapLabelFieldName, mapLabelFieldName)
                .ExecNonQuery();

            _isCommitted = true;
        }

        public static MapEntityClass Create(MapEntityClass mapEntityClass)
        {
            return new MapEntityClass(mapEntityClass.ClassName, mapEntityClass.ClassLabel, mapEntityClass.MapLabelFieldName);
        }

        public static MapEntityClass Update(MapEntityClass mapEntityClass)
        {
            var mec = GetMapEntityClass(mapEntityClass.ClassName);
            if (mec == null) return null;

            mec.ClassLabel = mapEntityClass.ClassLabel;
            mec.MapLabelFieldName = mapEntityClass.MapLabelFieldName;

            return mec;
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
                .ExecExpectOne(x => new MapEntityClass(x));
        }

        public static MapEntityClass GetMapEntityClass(int classId)
        {
            return DB.NewSP(_spGetMapEntityClasses)
                .AddParam(_classId, classId)
                .ExecExpectOne(x => new MapEntityClass(x));
        }

        public static List<MapEntityClass> GetAllMapEntityClasses()
        {
            return DB.NewSP(_spGetMapEntityClasses)
                .ExecExpectMultiple(x => new MapEntityClass(x)).ToList();
        }
    }
}
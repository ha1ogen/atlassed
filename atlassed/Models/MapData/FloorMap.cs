using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Atlassed.Models.MapData
{
    public class FloorMap : MetaObject, IDbRow<FloorMap>
    {
        public const string _floorMapId = "floorMapId";
        private const string _floorOrdinal = "floorOrdinal";
        private const string _floorCode = "floorCode";
        private const string _floorLabel = "floorLabel";
        private const string _mapFilename = "mapFilename";

        private const string _spAddFloor = "AddFloor";
        private const string _spEditFloor = "EditFloor";
        private const string _spDeleteFloor = "DeleteFloor";
        private const string _spGetFloors = "GetFloors";

        public int FloorMapId { get; set; }
        public int BuildingId { get; set; }
        public int FloorOrdinal { get; set; }
        public string FloorCode { get; set; }
        public string FloorLabel { get; set; }

        private readonly bool _isCommitted = false;
        public FloorMap()
        {
            FloorMapId = 0;
            FloorOrdinal = 0;
            FloorCode = string.Empty;
            FloorLabel = string.Empty;

            _isCommitted = false;
        }

        public FloorMap(IDataRecord data)
            : base(data)
        {
            FloorMapId = data.GetInt32(_floorMapId);
            BuildingId = data.GetInt32(Building._buildingId);
            FloorOrdinal = data.GetInt32(_floorOrdinal);
            FloorCode = data.GetString(_floorCode);
            FloorLabel = data.GetString(_floorLabel);

            _isCommitted = true;
        }

        public FloorMap(int buildingId, int floorOrdinal, string floorCode, string floorLabel, string metaProperties)
            : base(metaProperties)
        {
            BuildingId = buildingId;
            FloorOrdinal = floorOrdinal;
            FloorCode = floorCode;
            FloorLabel = floorLabel;

            FloorMapId = DB.NewSP(_spAddFloor)
                .AddParam(Building._buildingId, BuildingId)
                .AddParam(_floorOrdinal, FloorOrdinal)
                .AddParam(_floorCode, FloorCode)
                .AddParam(_floorLabel, FloorLabel)
                .AddTVParam(_metaProperties, GenerateMetaFieldTable())
                .AddReturn(SqlDbType.Int)
                .ExecExpectReturnValue<int>();

            _isCommitted = true;
        }

        internal static FloorMap Create(FloorMap floor)
        {
            return new FloorMap(floor.BuildingId, floor.FloorOrdinal, floor.FloorCode, floor.FloorLabel, floor.MetaProperties.ToString());
        }
        public static FloorMap Update(FloorMap floor)
        {
            var f = FloorMap.GetFloor(floor.FloorMapId);
            if (f == null) return null;

            f.FloorOrdinal = floor.FloorOrdinal;
            f.FloorCode = floor.FloorCode;
            f.FloorLabel = floor.FloorLabel;
            f.MetaProperties = floor.MetaProperties;

            return f;
        }

        public FloorMap CommitUpdate()
        {
            if (!_isCommitted)
            {
                throw new Exception("record must be committed before updating");
            }

            return DB.NewSP(_spEditFloor)
                    .AddParam(_floorMapId, FloorMapId)
                    .AddParam(_floorOrdinal, FloorOrdinal)
                    .AddParam(_floorCode, FloorCode)
                    .AddParam(_floorLabel, FloorLabel)
                    .AddTVParam(_metaProperties, GenerateMetaFieldTable())
                    .ExecNonQueryExpectSuccess()
                ? GetFloor(FloorMapId)
                : null;

        }

        public bool Delete()
        {
            if (!_isCommitted)
            {
                throw new Exception("record must be committed before deleting");
            }

            return DB.NewSP(_spDeleteFloor)
                    .AddParam(_floorMapId, FloorMapId)
                    .ExecNonQueryExpectSuccess();
        }

        public static FloorMap GetFloor(int floorMapId)
        {
            return DB.NewSP(_spGetFloors)
                .AddParam(_floorMapId, floorMapId)
                .ExecExpectOne(x => new FloorMap(x));
        }

        public static List<FloorMap> GetAllFloors(int buildingId)
        {
            return DB.NewSP(_spGetFloors)
                .AddParam(Building._buildingId, buildingId)
                .ExecExpectMultiple(x => new FloorMap(x)).ToList();
        }
    }
}
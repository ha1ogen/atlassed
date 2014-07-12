using Atlassed.Models;
using Atlassed.Models.MapData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Atlassed.Repositories.MapData
{
    public class FloorRepository : MetaObjectRepository, IRepository<FloorMap, FloorMap, int, int>
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
        private const string _fncCheckFloorExists = "CheckFloorExists";

        private readonly SqlConnectionFactory _connectionFactory;
        private readonly IValidator<FloorMap> _validator;

        public FloorRepository(SqlConnectionFactory f, IValidator<FloorMap> v)
        {
            _connectionFactory = f;
            _validator = v;
        }

        public FloorMap Create(FloorMap record, out ICollection<ValidationError> errors)
        {
            if (!_validator.Validate(record, out errors))
                return null;

            return DB.NewSP(_spAddFloor, _connectionFactory)
                .AddParam(BuildingRepository._buildingId, record.BuildingId)
                .AddParam(_floorOrdinal, record.FloorOrdinal)
                .AddParam(_floorCode, record.FloorCode)
                .AddParam(_floorLabel, record.FloorLabel)
                .AddTVParam(_metaProperties, GenerateMetaPropertyTable(record))
                .ExecExpectOne(x => Create(x));
        }

        private FloorMap Create(IDataRecord data)
        {
            return new FloorMap()
            {
                MapId = data.GetInt32(_floorMapId),
                BuildingId = data.GetInt32(BuildingRepository._buildingId),
                FloorOrdinal = data.GetInt32(_floorOrdinal),
                FloorCode = data.GetString(_floorCode),
                FloorLabel = data.GetString(_floorLabel),
                MetaProperties = GetMetaProperties(data)
            };
        }

        public bool Update(ref FloorMap record, out ICollection<ValidationError> errors)
        {
            if (!_validator.Validate(record, out errors))
                return false;

            return DB.NewSP(_spEditFloor, _connectionFactory)
                    .AddParam(_floorMapId, record.MapId)
                    .AddParam(_floorOrdinal, record.FloorOrdinal)
                    .AddParam(_floorCode, record.FloorCode)
                    .AddParam(_floorLabel, record.FloorLabel)
                    .AddTVParam(_metaProperties, GenerateMetaPropertyTable(record))
                    .ExecExpectOne(x => Create(x), out record)
                    .GetReturnValue<bool>();
        }

        public bool Delete(int recordId)
        {
            return DB.NewSP(_spDeleteFloor, _connectionFactory)
                    .AddParam(_floorMapId, recordId)
                    .ExecExpectReturnValue<bool>();
        }

        public bool RecordExists(int recordId)
        {
            return DB.NewSP(_fncCheckFloorExists, _connectionFactory)
                .AddParam(_floorMapId, recordId)
                .ExecExpectReturnValue<bool>();
        }

        public FloorMap GetOne(int recordId)
        {
            return DB.NewSP(_spGetFloors, _connectionFactory)
                .AddParam(_floorMapId, recordId)
                .ExecExpectOne(x => Create(x));
        }

        public IEnumerable<FloorMap> GetMany(int buildingId)
        {
            return DB.NewSP(_spGetFloors, _connectionFactory)
                .AddParam(BuildingRepository._buildingId, buildingId)
                .ExecExpectMultiple(x => Create(x));
        }
    }
}
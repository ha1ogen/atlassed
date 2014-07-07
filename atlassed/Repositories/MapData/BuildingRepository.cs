using Atlassed.Models;
using Atlassed.Models.MapData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Atlassed.Repositories.MapData
{
    public class BuildingRepository : MetaObjectRepository, IRepository<Building, NewBuilding, int, int?>
    {
        public const string _buildingId = "buildingId";

        private const string _spAddBuilding = "AddBuilding";
        private const string _spEditBuilding = "EditBuilding";
        private const string _spDeleteBuilding = "DeleteBuilding";
        private const string _spGetBuildings = "GetBuildings";

        private readonly SqlConnectionFactory _connectionFactory;
        private readonly IValidator<Building> _validator;

        public BuildingRepository(SqlConnectionFactory f, IValidator<Building> v)
        {
            _connectionFactory = f;
            _validator = v;
        }

        public Building Create(NewBuilding record, out IEnumerable<ValidationError> errors)
        {
            if (!_validator.Validate(record, out errors))
                return null;

            return DB.NewSP(_spAddBuilding, _connectionFactory)
                .AddParam(CampusRepository._campusMapId, record.CampusMapId)
                .AddParam(MapEntity._entityPoints, Coordinate.MultiToString(record.EntityCoordinates))
                .AddTVParam(_metaProperties, GenerateMetaPropertyTable(record))
                .ExecExpectOne(x => Create(x));
        }

        private Building Create(IDataRecord data)
        {
            return new Building()
            {
                BuildingId = data.GetInt32(_buildingId),
                CampusMapId = data.GetInt32(CampusRepository._campusMapId),
                MetaProperties = GetMetaProperties(data)
            };
        }

        public bool Update(ref Building record, out IEnumerable<ValidationError> errors)
        {
            if (!_validator.Validate(record, out errors))
                return false;

            return DB.NewSP(_spEditBuilding, _connectionFactory)
                    .AddTVParam(_metaProperties, GenerateMetaPropertyTable(record))
                    .ExecExpectOne(x => Create(x), out record)
                    .GetReturnValue<bool>();
        }

        public bool Delete(int recordId)
        {
            return DB.NewSP(_spDeleteBuilding, _connectionFactory)
                .AddParam(_buildingId, recordId)
                .ExecExpectReturnValue<bool>();
        }

        public Building GetOne(int recordId)
        {
            return DB.NewSP(_spGetBuildings, _connectionFactory)
                .AddParam(_buildingId, recordId)
                .ExecExpectOne(b => Create(b));
        }

        public IEnumerable<Building> GetMany(int? mapId = null)
        {
            return DB.NewSP(_spGetBuildings, _connectionFactory)
                .AddParam(Map._mapId, mapId)
                .ExecExpectMultiple(b => Create(b));
        }
    }
}
using Atlassed.Models;
using Atlassed.Models.MapData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Atlassed.Repositories.MapData
{
    public class BuildingRepository : MetaObjectRepository, IRepository<Building, NewBuilding, int, int?>
    {
        public const string _buildingId = "buildingId";
        private const string _buildingAddress = "buildingAddress";

        private const string _spAddBuilding = "AddBuilding";
        private const string _spEditBuilding = "EditBuilding";
        private const string _spDeleteBuilding = "DeleteBuilding";
        private const string _spGetBuildings = "GetBuildings";
        private const string _fncCheckBuildingExists = "CheckBuildingExists";

        private readonly SqlConnectionFactory _connectionFactory;
        private readonly IValidatorWNew<Building, NewBuilding> _validator;

        public BuildingRepository(SqlConnectionFactory f, IValidatorWNew<Building, NewBuilding> v)
        {
            _connectionFactory = f;
            _validator = v;
        }

        public Building Create(NewBuilding record, out IValidationResult validationResult)
        {
            if (!_validator.ValidateNew(record, out validationResult))
                return null;

            return SqlValidator.TryExecCatchValidation(
                () => DB.NewSP(_spAddBuilding, _connectionFactory)
                    .AddParam(CampusRepository._campusMapId, record.CampusMapId)
                    .AddParam(_buildingAddress, record.BuildingAddress)
                    .AddParam(MapEntityRepository._entityCoordinates, Coordinate.MultiToString(record.EntityCoordinates))
                    .AddTVParam(_metaProperties, GenerateMetaPropertyTable(record))
                    .ExecExpectOne(x => Create(x))
                , ref validationResult);
        }

        private Building Create(IDataRecord data)
        {
            return new Building()
            {
                BuildingId = data.GetInt32(_buildingId),
                CampusMapId = data.GetInt32(CampusRepository._campusMapId),
                BuildingAddress = data.GetString(_buildingAddress),
                MetaProperties = GetMetaProperties(data)
            };
        }

        public bool Update(ref Building record, out IValidationResult validationResult)
        {
            if (!_validator.Validate(record, out validationResult))
                return false;

            try
            {
                return DB.NewSP(_spEditBuilding, _connectionFactory)
                    .AddParam(_buildingId, record.BuildingId)
                    .AddParam(_buildingAddress, record.BuildingAddress)
                    .AddTVParam(_metaProperties, GenerateMetaPropertyTable(record))
                    .ExecExpectOne(x => Create(x), out record)
                    .GetReturnValue<bool>();
            }
            catch (SqlException e)
            {
                e.ParseValidationMessages(ref validationResult);
                return false;
            }
        }

        public bool Delete(int recordId)
        {
            return DB.NewSP(_spDeleteBuilding, _connectionFactory)
                .AddParam(_buildingId, recordId)
                .ExecExpectReturnValue<bool>();
        }

        public bool RecordExists(int recordId)
        {
            return DB.NewSP(_fncCheckBuildingExists, _connectionFactory)
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
                .AddParam(MapRepository._mapId, mapId)
                .ExecExpectMultiple(b => Create(b));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Atlassed.Models.MapData
{
    public class Building : MetaObject, IDbRow<Building>, ISearchable
    {
        public const string _buildingId = "buildingId";
        private const string _buildingName = "buildingName";

        private const string _spAddBuilding = "AddBuilding";
        private const string _spEditBuilding = "EditBuilding";
        private const string _spDeleteBuilding = "DeleteBuilding";
        private const string _spGetBuildings = "GetBuildings";

        public int BuildingId { get; set; }
        public int CampusMapId { get; set; }
        public string BuildingName { get; set; }

        private readonly bool _isCommitted = false;

        public Building()
        {
        }

        private Building(IDataRecord data)
            : base(data)
        {
            BuildingId = data.GetInt32(_buildingId);
            BuildingName = data.GetString(_buildingName);
            CampusMapId = data.GetInt32(CampusMap._campusMapId);

            _isCommitted = true;
        }

        private Building(int campusMapId, string buildingName, string entityPoints, string metaProperties)
            : base(metaProperties)
        {
            CampusMapId = campusMapId;
            BuildingName = buildingName;

            BuildingId = DB.NewSP(_spAddBuilding)
                .AddParam(CampusMap._campusMapId, CampusMapId)
                .AddParam(_buildingName, BuildingName)
                .AddParam(MapEntity._entityPoints, entityPoints)
                .AddTVParam(_metaProperties, GenerateMetaFieldTable())
                .AddReturn(SqlDbType.Int)
                .ExecExpectReturnValue<int>();

            _isCommitted = true;
        }

        public static Building Create(Building building, string entityPoints)
        {
            return new Building(building.CampusMapId, building.BuildingName, entityPoints, building.MetaProperties.ToString());
        }

        public static Building Update(Building building)
        {
            var b = Building.GetBuilding(building.BuildingId);
            if (b == null) return null;

            b.BuildingName = building.BuildingName;
            b.MetaProperties = building.MetaProperties;

            return b;
        }

        public static List<SearchResult> Search(string query)
        {
            return GetAllBuildings().Where(x => x.BuildingName.ToLowerInvariant().Contains(query.ToLowerInvariant())).ToSearchResults();
        }

        public SearchResult ToSearchResult()
        {
            return new SearchResult(BuildingId, this.GetType().Name, BuildingName, CampusMap.GetCampus(CampusMapId).CampusName);
        }

        public Building CommitUpdate()
        {
            if (!_isCommitted)
            {
                throw new Exception("record must be committed before updating");
            }

            return DB.NewSP(_spEditBuilding)
                    .AddParam(_buildingId, BuildingId)
                    .AddParam(_buildingName, BuildingName)
                    .AddTVParam(_metaProperties, GenerateMetaFieldTable())
                    .ExecNonQueryExpectSuccess()
                ? GetBuilding(BuildingId)
                : null;
        }

        public bool Delete()
        {
            if (!_isCommitted)
            {
                throw new Exception("record must be committed before deleting");
            }

            return DB.NewSP(_spDeleteBuilding)
                .AddParam(_buildingId, BuildingId)
                .ExecNonQueryExpectSuccess();
        }

        public static Building GetBuilding(int buildingId)
        {

            return DB.NewSP(_spGetBuildings)
                .AddParam(_buildingId, buildingId)
                .ExecExpectOne(x => new Building(x));
        }

        public static List<Building> GetAllBuildings()
        {
            return DB.NewSP(_spGetBuildings)
                .ExecExpectMultiple(x => new Building(x)).ToList();
        }
        public static List<Building> GetAllBuildings(int mapId)
        {
            return DB.NewSP(_spGetBuildings)
                .AddParam(Map._mapId, mapId)
                .ExecExpectMultiple(x => new Building(x)).ToList();
        }
    }
}
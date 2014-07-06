using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Atlassed.Models.MapData
{
    public class CampusMap : Map, IDbRow<CampusMap>, ISearchable
    {
        public const string _campusMapId = "campusMapId";
        private const string _campusName = "campusName";
        private const string _mapCoordinates = "mapCoordinates";

        private const string _spAddCampusMap = "AddCampusMap";
        private const string _spEditCampusMap = "EditCampusMap";
        private const string _spDeleteCampusMap = "DeleteCampusMap";
        private const string _spGetCampusMaps = "GetCampuses";

        public string CampusName { get; set; }
        public Coordinate MapCoordinates { get; set; }

        private readonly bool _isCommitted = false;

        public CampusMap()
        {
            MapId = 0;
            CampusName = string.Empty;
            MapCoordinates = new Coordinate();

            _isCommitted = false;
        }

        public CampusMap(IDataRecord data)
            : base(data)
        {
            MapId = data.GetInt32(data.GetOrdinal(_campusMapId));
            CampusName = data.GetString(data.GetOrdinal(_campusName));
            MapCoordinates = Coordinate.Parse(data.GetString(_mapCoordinates));

            _isCommitted = true;
        }

        public CampusMap(string campusName, Coordinate mapCoordinates, string metaProperties)
            : base(metaProperties)
        {
            CampusName = campusName;
            MapCoordinates = mapCoordinates;

            MapId = DB.NewSP(_spAddCampusMap)
                    .AddParam(_campusName, CampusName)
                    .AddParam(_mapCoordinates, MapCoordinates.ToString())
                    .AddTVParam(_metaProperties, GenerateMetaFieldTable())
                    .ExecExpectScalarValue<int>();

            _isCommitted = true;
        }

        public static CampusMap Create(CampusMap campus)
        {
            return new CampusMap(campus.CampusName, campus.MapCoordinates, campus.MetaProperties);
        }

        public static CampusMap Update(CampusMap campus)
        {
            var c = GetCampus(campus.MapId);
            if (c == null) return null;

            c.CampusName = campus.CampusName;
            c.MapCoordinates = campus.MapCoordinates;
            c.MetaProperties = campus.MetaProperties;

            return c;
        }

        public static List<SearchResult> Search(string query)
        {
            return GetAllCampuses().Where(x => x.CampusName.ToLowerInvariant().Contains(query.ToLowerInvariant())).ToSearchResults();
        }

        public SearchResult ToSearchResult()
        {
            return new SearchResult(MapId, this.GetType().Name, CampusName, "");
        }

        public CampusMap CommitUpdate()
        {
            if (!_isCommitted)
            {
                throw new Exception("record must be committed before updating");
            }

            return DB.NewSP(_spEditCampusMap)
                    .AddParam(_campusMapId, MapId)
                    .AddParam(_campusName, CampusName)
                    .AddParam(_mapCoordinates, MapCoordinates.ToString())
                    .AddTVParam(_metaProperties, GenerateMetaFieldTable())
                    .ExecNonQueryExpectSuccess()
                ? GetCampus(MapId)
                : null;

        }

        public bool Delete()
        {
            if (!_isCommitted)
            {
                throw new Exception("record must be committed before deleting");
            }

            return DB.NewSP(_spDeleteCampusMap)
                    .AddParam(_campusMapId, MapId)
                    .ExecNonQueryExpectSuccess();
        }

        public static CampusMap GetCampus(int campusMapId)
        {
            return DB.NewSP(_spGetCampusMaps)
                .AddParam(_campusMapId, campusMapId)
                .ExecExpectOne(x => new CampusMap(x));
        }

        public static List<CampusMap> GetAllCampuses()
        {
            return DB.NewSP(_spGetCampusMaps)
                .ExecExpectMultiple(x => new CampusMap(x)).ToList();
        }
    }
}
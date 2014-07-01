using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Atlassed.Models
{
    public class CampusMap : MetaObject, IDbRow<CampusMap>, ISearchable
    {
        public const string _campusMapId = "campusMapId";
        private const string _campusName = "campusName";

        private const string _spAddCampusMap = "AddCampusMap";
        private const string _spEditCampusMap = "EditCampusMap";
        private const string _spDeleteCampusMap = "DeleteCampusMap";
        private const string _spGetCampusMaps = "GetCampuses";

        public int CampusMapId { get; set; }
        public string CampusName { get; set; }

        private readonly bool _isCommitted = false;

        public CampusMap()
        {
            CampusMapId = 0;
            CampusName = string.Empty;

            _isCommitted = false;
        }

        public CampusMap(IDataRecord data)
            : base(data)
        {
            CampusMapId = data.GetInt32(data.GetOrdinal(_campusMapId));
            CampusName = data.GetString(data.GetOrdinal(_campusName));

            _isCommitted = true;
        }

        public CampusMap(string campusName, string metaProperties)
            : base(metaProperties)
        {
            CampusName = campusName;

            CampusMapId = DB.NewSP(_spAddCampusMap)
                    .AddParam(_campusName, CampusName)
                    .AddTVParam(_metaProperties, GenerateMetaFieldTable())
                    .AddReturn(SqlDbType.Int)
                    .ExecExpectReturnValue<int>();

            _isCommitted = true;
        }

        public static List<SearchResult> Search(string query)
        {
            return GetAllCampuses().Where(x => x.CampusName.ToLowerInvariant().Contains(query.ToLowerInvariant())).ToSearchResults();
        }

        public SearchResult ToSearchResult()
        {
            return new SearchResult(CampusMapId, this.GetType().Name, CampusName, "");
        }

        public CampusMap CommitUpdate()
        {
            if (!_isCommitted)
            {
                throw new Exception("record must be committed before updating");
            }

            return DB.NewSP(_spEditCampusMap)
                    .AddParam(_campusMapId, CampusMapId)
                    .AddParam(_campusName, CampusName)
                    .AddTVParam(_metaProperties, GenerateMetaFieldTable())
                    .ExecNonQueryExpectSuccess()
                ? GetCampus(CampusMapId)
                : null;

        }

        public bool Delete()
        {
            if (!_isCommitted)
            {
                throw new Exception("record must be committed before deleting");
            }

            return DB.NewSP(_spDeleteCampusMap)
                    .AddParam(_campusMapId, CampusMapId)
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
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Atlassed.Models.MapData
{
    public class SearchResult
    {
        public const string _query = "query";

        public int PrimaryId { get; set; }
        public string ClassName { get; set; }
        public int SecondaryId { get; set; }
        public string PrimaryText { get; set; }
        public string SecondaryText { get; set; }

        // people, workstations, spaces
        public SearchResult(int primaryId, string className, string primaryText, string secondaryText, int mapId)
        {
            PrimaryId = primaryId;
            ClassName = className;
            SecondaryId = mapId;
            PrimaryText = primaryText;
            SecondaryText = secondaryText;
        }

        // buildings
        public SearchResult(int buildingId, string className, string primaryText, string secondaryText)
            : this(buildingId, className, primaryText, secondaryText, 0)
        {

        }

        public SearchResult(IDataRecord data)
        {
            PrimaryId = data.GetInt32(0);
            SecondaryId = data.GetInt32(1);
            ClassName = data.GetString(2);
            PrimaryText = data.GetString(3);
            SecondaryText = data.GetString(4);
        }
    }
}
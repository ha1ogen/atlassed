using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Atlassed.Models.MapData
{
    public class Map : MetaObject
    {
        public const string _mapId = "mapId";

        private const string _mapFilenameExtension = ".png";
        public int MapId { get; set; }
        public string MapFilename { get { return "map-" + MapId + _mapFilenameExtension; } }

        public Map()
        {
            MapId = 0;
        }
        public Map(IDataRecord data)
            : base(data)
        {

        }
        public Map(string metaProperties)
            : base(metaProperties)
        {

        }
    }
}
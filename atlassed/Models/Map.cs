using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlassed.Models
{
    public class Map
    {
        public const string _mapId = "mapId";

        private const string _mapFilenameExtension = ".png";
        public int MapId { get; set; }
        public string MapTitle { get; set; }
        public string MapFilename { get { return "map-" + MapId + _mapFilenameExtension; } }
    }
}
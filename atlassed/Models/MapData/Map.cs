using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;

namespace Atlassed.Models.MapData
{
    public abstract class Map : MetaObject
    {
        public const string _mapFilenameExtension = ".png";

        public static string MapImagePath { get { return ConfigurationManager.AppSettings["MapImagePath"] + _mapFilenameExtension; } }
        public static int MaxMapImageFileSize { get { return int.Parse(ConfigurationManager.AppSettings["MaxMapImageFileSize"]); } }
        public static MediaTypeHeaderValue AllowedMapImageFileTypes { get { return MediaTypeHeaderValue.Parse(ConfigurationManager.AppSettings["AllowedMapImageFileTypes"]); } }

        public override int ObjectId { get { return MapId; } }
        public int MapId { get; set; }
        public string MapFilename { get { return "map-" + MapId + _mapFilenameExtension; } }
    }
}
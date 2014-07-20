using Atlassed.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;

namespace Atlassed.Models.MapData
{
    public class MapEntityClass : MetaClass
    {
        public const string _iconFilenameExtension = ".png";

        public static string EntityIconPath { get { return ConfigurationManager.AppSettings["EntityIconPath"] + _iconFilenameExtension; } }
        public static int MaxEntityIconFileSize { get { return int.Parse(ConfigurationManager.AppSettings["MaxEntityIconFileSize"]); } }
        public static MediaTypeHeaderValue AllowedEntityIconFileTypes { get { return MediaTypeHeaderValue.Parse(ConfigurationManager.AppSettings["AllowedEntityIconFileTypes"]); } }

        public string MapLabelFieldName { get; set; }
        public string IconFilename { get { return "icon-" + ClassId + _iconFilenameExtension; } }
    }

    public class MapEntityClassValidator : IValidatorWNew<MapEntityClass, MapEntityClass>
    {
        private readonly IValidatorWNew<MetaClass, MetaClass> _metaClassValidator;

        public MapEntityClassValidator(IValidatorWNew<MetaClass, MetaClass> metaClassValidator)
        {
            _metaClassValidator = metaClassValidator;
        }

        public bool Validate(MapEntityClass record, out IValidationResult result)
        {
            _metaClassValidator.Validate(record, out result);

            if (string.IsNullOrEmpty(record.MapLabelFieldName))
                result.AddError("MapLabelFieldName", "Map Label Field Name is required");

            return result.IsValid();
        }


        public bool ValidateNew(MapEntityClass record, out IValidationResult result)
        {
            _metaClassValidator.ValidateNew(record, out result);

            if (string.IsNullOrEmpty(record.MapLabelFieldName))
                result.AddError("MapLabelFieldName", "Map Label Field Name is required");

            return result.IsValid();
        }
    }
}
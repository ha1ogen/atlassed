using Atlassed.Models.MapData;
using Atlassed.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Atlassed.Repositories.MapData
{
    public abstract class MetaClassRepository
    {
        public const string _classId = "classId";
        public const string _className = "className";
        protected const string _classType = "classType";
        protected const string _classTypeDescription = "classTypeDescription";
        protected const string _classLabel = "classLabel";

        protected const string _spGetMetaClasses = "GetMetaClasses";

        protected MetaClass CreateMetaClass(IDataRecord data)
        {
            return new MetaClass()
            {
                ClassId = data.GetInt32(_classId),
                ClassName = data.GetString(_className),
                ClassType = data.GetString(_classType),
                ClassTypeDescription = data.GetString(_classTypeDescription),
                ClassLabel = data.GetString(_classLabel)
            };
        }
    }
}
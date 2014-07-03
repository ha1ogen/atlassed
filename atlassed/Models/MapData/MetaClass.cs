using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Atlassed.Models.MapData
{
    public enum ClassType { BRULE, ENTITY, SYSTEM }

    public class MetaClass
    {

        public const string _className = "className";
        protected const string _classType = "classType";
        protected const string _classTypeDescription = "classTypeDescription";
        protected const string _classLabel = "classLabel";

        protected const string _spGetMetaClasses = "GetMetaClasses";

        public string ClassName { get; private set; }
        public ClassType ClassType { get; set; }
        public string ClassTypeDescription { get; private set; }
        public string ClassLabel { get; set; }
        public List<MetaField> Fields { get { throw new NotImplementedException(); } }

        protected MetaClass(IDataRecord data)
        {
            ClassName = data.GetString(data.GetOrdinal(_className));
            ClassType = (ClassType)Enum.Parse(typeof(ClassType), data.GetString(data.GetOrdinal(_classType)));
            ClassTypeDescription = data.GetString(data.GetOrdinal(_classTypeDescription));
            ClassLabel = data.GetString(data.GetOrdinal(_classLabel));
        }
        protected MetaClass(string className, ClassType classType, string classLabel)
        {
            ClassName = className;
            ClassType = classType;
            ClassLabel = classLabel;
        }
    }
}
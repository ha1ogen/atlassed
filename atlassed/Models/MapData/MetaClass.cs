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
        public const string _classId = "classId";
        public const string _className = "className";
        protected const string _classType = "classType";
        protected const string _classTypeDescription = "classTypeDescription";
        protected const string _classLabel = "classLabel";

        protected const string _spGetMetaClasses = "GetMetaClasses";

        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string ClassType { get; set; }
        public string ClassTypeDescription { get; private set; }
        public string ClassLabel { get; set; }
        public List<MetaField> MetaFields { get { return MetaField.GetAllMetaFields(ClassName); } }

        public MetaClass()
        {

        }
        protected MetaClass(IDataRecord data)
        {
            ClassId = data.GetInt32(_classId);
            ClassName = data.GetString(_className);
            ClassType = data.GetString(_classType);
            ClassTypeDescription = data.GetString(_classTypeDescription);
            ClassLabel = data.GetString(_classLabel);
        }
        protected MetaClass(string className, ClassType classType, string classLabel)
        {
            ClassName = className;
            ClassType = classType.ToString();
            ClassLabel = classLabel;
        }
    }
}
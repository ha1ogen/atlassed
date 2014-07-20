using Atlassed.Models.MapData;
using Atlassed.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Atlassed.Repositories.MapData
{
    public abstract class MetaClassRepository : IExistenceRepository<int>
    {
        public const string _classId = "classId";
        public const string _className = "className";
        protected const string _classType = "classType";
        protected const string _classTypeDescription = "classTypeDescription";
        protected const string _classLabel = "classLabel";

        protected const string _spGetMetaClasses = "GetMetaClasses";
        private const string _fncCheckMetaClassExists = "CheckMetaClassExists";

        private SqlConnectionFactory _connectionFactory;

        public MetaClassRepository(SqlConnectionFactory f)
        {
            _connectionFactory = f;
        }

        public bool RecordExists(int recordId)
        {
            return DB.NewSP(_fncCheckMetaClassExists, _connectionFactory)
                .AddParam(_classId, recordId)
                .ExecExpectReturnValue<bool>();
        }
    }
}
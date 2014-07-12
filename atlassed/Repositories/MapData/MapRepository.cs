using Atlassed.Models.MapData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlassed.Repositories.MapData
{
    public class MapRepository : IExistenceRepository<int>
    {
        public const string _mapId = "mapId";

        private const string _fncCheckMapExists = "CheckMapExists";

        private readonly SqlConnectionFactory _connectionFactory;

        public MapRepository(SqlConnectionFactory f)
        {
            _connectionFactory = f;
        }

        public bool RecordExists(int recordId)
        {
            return DB.NewSP(_fncCheckMapExists, _connectionFactory)
                .AddParam(_mapId, recordId)
                .ExecExpectReturnValue<bool>();
        }
    }
}
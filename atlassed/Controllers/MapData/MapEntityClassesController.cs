using Atlassed.Models;
using Atlassed.Models.MapData;
using Atlassed.Repositories;
using Atlassed.Repositories.MapData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Atlassed.Controllers.MapData
{
    public class MapEntityClassesController : SinglePageAppApiController
    {
        private IRepository<MapEntityClass, MapEntityClass, int, int?> _repository;

        public MapEntityClassesController(SqlConnectionFactory f)
        {
            _repository = new MapEntityClassRepository(f, new MapEntityClassValidator());
        }

        public IEnumerable<MapEntityClass> Get()
        {
            return _repository.GetMany();
        }

        public MapEntityClass Get(int id)
        {
            var mec = _repository.GetOne(id);
            if (mec == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            return mec;
        }

        public HttpResponseMessage Post([FromBody]MapEntityClass mapEntityClass)
        {
            IEnumerable<ValidationError> errors;
            var mec = _repository.Create(mapEntityClass, out errors);
            return Request.CreateResponse(HttpStatusCode.Created, mec);
        }

        public MapEntityClass Put([FromBody]MapEntityClass mapEntityClass)
        {
            IEnumerable<ValidationError> errors;
            if (!_repository.Update(ref mapEntityClass, out errors))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return mapEntityClass;
        }

        public void Delete(int id)
        {
            if (!_repository.Delete(id))
                throw new HttpResponseException(HttpStatusCode.NotFound);
        }
    }
}
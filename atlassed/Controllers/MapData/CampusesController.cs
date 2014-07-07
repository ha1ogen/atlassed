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
    public class CampusesController : SinglePageAppApiController
    {
        private IRepository<CampusMap, CampusMap, int, int?> _repository;

        public CampusesController(SqlConnectionFactory f)
        {
            _repository = new CampusRepository(f, new CampusMapValidator());
        }

        public IEnumerable<CampusMap> Get()
        {
            return _repository.GetMany();
        }

        public CampusMap Get(int id)
        {
            var c = _repository.GetOne(id);
            if (c == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            return c;
        }

        public HttpResponseMessage Post([FromBody]CampusMap campus)
        {
            IEnumerable<ValidationError> errors;
            var c = _repository.Create(campus, out errors);
            if (c == null) throw new HttpResponseException(HttpStatusCode.BadRequest);

            return Request.CreateResponse(HttpStatusCode.Created, c);
        }

        public CampusMap Put([FromBody]CampusMap campus)
        {
            IEnumerable<ValidationError> errors;
            if (!_repository.Update(ref campus, out errors))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return campus;
        }

        public void Delete(int id)
        {
            if (!_repository.Delete(id))
                throw new HttpResponseException(HttpStatusCode.NotFound);
        }
    }
}
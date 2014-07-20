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
            _repository = new CampusRepository(f, new CampusMapValidator(new MetaObjectValidator(new MetaFieldRepository(f))));
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
            if (campus == null) throw new HttpResponseException(HttpStatusCode.BadRequest);

            IValidationResult validationResult;
            var c = _repository.Create(campus, out validationResult);
            if (!validationResult.IsValid())
                return Request.CreateResponse(HttpStatusCode.BadRequest, validationResult);

            return Request.CreateResponse(HttpStatusCode.Created, c);
        }

        public HttpResponseMessage Put([FromBody]CampusMap campus)
        {
            if (campus == null) throw new HttpResponseException(HttpStatusCode.BadRequest);

            IValidationResult validationResult;
            if (!_repository.Update(ref campus, out validationResult))
            {
                if (!validationResult.IsValid())
                    return Request.CreateResponse(HttpStatusCode.BadRequest, validationResult);

                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, campus);
        }

        public bool Delete(int id)
        {
            if (!_repository.Delete(id))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return true;
        }
    }
}
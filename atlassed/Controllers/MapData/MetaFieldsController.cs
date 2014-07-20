using Atlassed.Models;
using Atlassed.Models.MapData;
using Atlassed.Repositories;
using Atlassed.Repositories.MapData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Atlassed.Controllers.MapData
{
    public class MetaFieldsController : SinglePageAppApiController
    {
        private IRepository<MetaField, NewMetaField, int, string> _repository;

        public MetaFieldsController(SqlConnectionFactory f)
        {
            _repository = new MetaFieldRepository(f, new MetaFieldValidator());
        }

        [Route("api/metaclasses/{className}/fields")]
        public IEnumerable<MetaField> GetAll(string className)
        {
            return _repository.GetMany(className);
        }

        public MetaField Get(int id)
        {
            var mf = _repository.GetOne(id);
            if (mf == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            return mf;
        }

        public HttpResponseMessage Post([FromBody]NewMetaField metaField)
        {
            if (metaField == null) throw new HttpResponseException(HttpStatusCode.BadRequest);

            IValidationResult validationResult;
            var mf = _repository.Create(metaField, out validationResult);
            if (!validationResult.IsValid())
                return Request.CreateResponse(HttpStatusCode.BadRequest, validationResult);

            return Request.CreateResponse(HttpStatusCode.Created, mf);
        }

        public HttpResponseMessage Put([FromBody]MetaField metaField)
        {
            if (metaField == null) throw new HttpResponseException(HttpStatusCode.BadRequest);

            IValidationResult validationResult;
            if (!_repository.Update(ref metaField, out validationResult))
            {
                if (!validationResult.IsValid())
                    return Request.CreateResponse(HttpStatusCode.BadRequest, validationResult);

                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, metaField);
        }

        public bool Delete(int id)
        {
            if (!_repository.Delete(id))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return true;
        }
    }
}
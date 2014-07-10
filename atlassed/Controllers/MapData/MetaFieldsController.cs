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
            ICollection<ValidationError> errors;
            var mf = _repository.Create(metaField, out errors);
            if (mf == null) throw new HttpResponseException(HttpStatusCode.BadRequest);

            return Request.CreateResponse(HttpStatusCode.Created, mf);
        }

        public MetaField Put([FromBody]MetaField metaField)
        {
            ICollection<ValidationError> errors;
            if (!_repository.Update(ref metaField, out errors))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return metaField;
        }

        public bool Delete(int id)
        {
            if (!_repository.Delete(id))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return true;
        }
    }
}
using Atlassed.Models.MapData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Atlassed.Controllers.MapData
{
    public class MetaFieldsController : SinglePageAppApiController
    {
        public MetaField Get(int id)
        {
            var mf = MetaField.GetMetaField(id);
            if (mf == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            return mf;
        }

        public HttpResponseMessage Post([FromBody]NewMetaField metaField)
        {
            var mf = MetaField.Create(metaField, metaField.DefaultValue);
            return Request.CreateResponse(HttpStatusCode.Created, mf);
        }

        public MetaField Put([FromBody]MetaField metaField)
        {
            var mf = MetaField.Update(metaField);
            if (mf == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            return mf.CommitUpdate();
        }

        public bool Delete(int id)
        {
            var mf = MetaField.GetMetaField(id);
            if (mf == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            return mf.Delete();
        }
    }

    public class NewMetaField : MetaField
    {
        public string DefaultValue { get; set; }
    }
}
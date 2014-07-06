using Atlassed.Models.MapData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Atlassed.Controllers.MapData
{
    public class BusinessRulesController : SinglePageAppApiController
    {
        [Route("api/BusinessRules/{className:regex(^[A-z_].*)}")]
        public IEnumerable<BusinessRule> Get(string className)
        {
            return BusinessRule.GetAllBusinessRules(className);
        }

        public BusinessRule Get(int id)
        {
            var br = BusinessRule.GetBusinessRule(id);
            if (br == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            return br;
        }

        public HttpResponseMessage Post([FromBody]BusinessRule businessRule)
        {
            var br = BusinessRule.Create(businessRule);
            return Request.CreateResponse(HttpStatusCode.Created, br);
        }

        public BusinessRule Put([FromBody]BusinessRule businessRule)
        {
            var br = BusinessRule.Update(businessRule);
            if (br == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            return br.CommitUpdate();
        }

        public bool Delete(int id)
        {
            var br = BusinessRule.GetBusinessRule(id);
            if (br == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            return br.Delete();
        }
    }
}
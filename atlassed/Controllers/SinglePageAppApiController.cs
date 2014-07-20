using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace Atlassed.Controllers
{
    public abstract class SinglePageAppApiController : ApiController
    {
        public const string _sessionIdHeader = "Session-Id";

        private Guid _sessionId;
        public Guid SessionId
        {
            get
            {
                if (_sessionId == default(Guid))
                {
                    var value = Request.Headers.GetValues(_sessionIdHeader).FirstOrDefault();
                    if (value == null) throw new HttpResponseException(HttpStatusCode.BadRequest);
                    _sessionId = Guid.Parse(value);
                }

                return _sessionId;
            }
        }


    }
}
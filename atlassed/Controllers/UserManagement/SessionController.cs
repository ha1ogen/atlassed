using Atlassed.Models.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Atlassed.Controllers.UserManagement
{
    public class SessionController : SinglePageAppApiController
    {
        private const string _cookieName = "sessionId";

        public Session Get()
        {
            var session = Session.GetSession(SessionId);
            if (session == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return session;
        }

        public HttpResponseMessage Post([FromBody]Credentials credentials)
        {
            var session = new Session(credentials);
            var response = Request.CreateResponse(HttpStatusCode.Created, session);
            response.Headers.AddCookies(new CookieHeaderValue[] {
                new CookieHeaderValue(_cookieName, session.SessionId.ToString()) {
                    HttpOnly = false,
                    Domain = Request.RequestUri.Host,
                    Path = "/"
                }
            });

            return response;
        }

        public bool Delete()
        {
            var session = Session.GetSession(SessionId);
            if (session == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return session.Destroy();
        }
    }
}
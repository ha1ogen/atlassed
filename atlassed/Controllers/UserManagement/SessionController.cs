using Atlassed.Models;
using Atlassed.Models.UserManagement;
using Atlassed.Repositories;
using Atlassed.Repositories.UserManagement;
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

        private IRepository<Session, Credentials, Guid, int?> _repository;

        public SessionController(SqlConnectionFactory f)
        {
            _repository = new SessionRepository(f, new CredentialValidator(f));
        }

        public Session Get()
        {
            var session = _repository.GetOne(SessionId);
            if (session == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            return session;
        }

        public HttpResponseMessage Post(Credentials credentials)
        {
            IValidationResult validationResult;
            var session = _repository.Create(credentials, out validationResult);

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

        public void Delete()
        {
            if (!_repository.Delete(SessionId))
                throw new HttpResponseException(HttpStatusCode.NotFound);
        }
    }
}
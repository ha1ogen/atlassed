using Atlassed.Models;
using Atlassed.Models.UserManagement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Atlassed.Repositories.UserManagement
{
    public class SessionRepository : IRepository<Session, Credentials, Guid, int?>
    {
        private const string _sessionId = "sessionId";
        private const string _created = "created";

        private readonly SqlConnectionFactory _connectionFactory;
        private readonly IValidatorWNew<Credentials, Credentials> _validator;

        public SessionRepository(SqlConnectionFactory f, IValidatorWNew<Credentials, Credentials> v)
        {
            _connectionFactory = f;
            _validator = v;
        }

        public Session Create(Credentials credentials, out IValidationResult validationResult)
        {
            if (!_validator.ValidateNew(credentials, out validationResult))
                return null;

            //TODO: authenticate

            return new Session()
            {
                SessionId = new Guid()
            };

            //TODO: DB sessions
            //TODO: get roles
        }

        private Session Create(IDataRecord data)
        {
            return new Session()
            {
                SessionId = Guid.Parse(data.GetString(_sessionId)),
                Created = DateTime.Parse(data.GetString(_created))
            };
        }

        public bool Delete(Guid recordId)
        {
            return false;
        }

        public bool RecordExists(Guid recordId)
        {
            throw new NotImplementedException();
        }

        public Session GetOne(Guid recordId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Session> GetMany(int? parentId = null)
        {
            throw new NotSupportedException();
        }

        public bool Update(ref Session record, out IValidationResult validationResult)
        {
            throw new NotSupportedException();
        }
    }
}
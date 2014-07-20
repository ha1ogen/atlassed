using Atlassed.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlassed.Models.UserManagement
{
    public class Credentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class CredentialValidator : IValidatorWNew<Credentials, Credentials>
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public CredentialValidator(SqlConnectionFactory f)
        {
            _connectionFactory = f;
        }

        public bool Validate(Credentials record, out IValidationResult result)
        {
            throw new NotSupportedException();
        }

        public bool ValidateNew(Credentials record, out IValidationResult result)
        {
            result = new ValidationResult();
            return result.IsValid();
        }
    }
}
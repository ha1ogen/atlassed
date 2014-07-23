using Atlassed.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlassed.Models
{
    public interface IValidator<T>
    {
        bool Validate(T record, out IValidationResult result);
    }

    public interface IValidatorWNew<T, NT> : IValidator<T>
    {
        bool ValidateNew(NT record, out IValidationResult result);
    }

    public class ValidationError
    {
        public string FieldName { get; set; }
        public string Message { get; set; }

        public ValidationError(string fieldName, string message = "")
        {
            FieldName = fieldName;
            Message = message;
        }
    }

    public interface IValidationResult
    {
        IEnumerable<ValidationError> Errors { get; }
        void AddError(string fieldName, string errorMessage);
        bool IsValid();
    }

    public class ValidationResult : IValidationResult
    {
        private ICollection<ValidationError> _errors;

        public IEnumerable<ValidationError> Errors { get { return _errors; } }

        public ValidationResult()
        {
            _errors = new List<ValidationError>();
        }

        public void AddError(string fieldName, string errorMessage)
        {
            _errors.Add(new ValidationError(fieldName, errorMessage));
        }

        public bool IsValid()
        {
            return !_errors.Any();
        }
    }

    public static class SqlValidator
    {
        public static void ParseValidationMessages(this SqlException e, ref IValidationResult results)
        {
            if (e.Number != 54321) throw e;

            var errors = e.Message.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var error in errors)
            {
                var parts = error.Split(new[] { ':' }, 2);
                if (parts.Length != 2)
                    throw new Exception("SQL MetaConstraint validation message must be in the form 'fieldName:message'", e);

                results.AddError(parts[0], parts[1]);
            }
        }

        public static T TryExecCatchValidation<T>(Func<T> executeQuery, ref IValidationResult validationResult)
        {
            try
            {
                return executeQuery();
            }
            catch (SqlException e)
            {
                e.ParseValidationMessages(ref validationResult);
                return default(T);
            }
        }
    }
}

using System;
using System.Collections.Generic;

namespace Rikrop.Core.Data.Exceptions
{
    public class ValidationException : ApplicationException
    {
        public ICollection<ValidationExceptionDetail> Details { get; private set; }

        public ValidationException(Exception ex)
            : base("Validation exception", ex)
        {
            Details = new List<ValidationExceptionDetail>();
        }
    }

    public class ValidationExceptionDetail
    {
        public object Entity { get; set; }
        public bool IsValid { get; set; }
        public ICollection<EntityValidationDetails> Errors { get; set; }

        public ValidationExceptionDetail(object entity, bool isValid)
        {
            Entity = entity;
            IsValid = isValid;
            Errors = new List<EntityValidationDetails>();
        }
    }

    public class EntityValidationDetails
    {
        public string PropertyName { get; private set; }
        public string ErrorMessage { get; private set; }

        public EntityValidationDetails(string propertyName, string errorMessage)
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
        }
    }
}

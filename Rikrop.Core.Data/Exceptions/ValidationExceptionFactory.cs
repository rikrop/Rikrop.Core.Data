using System.Data.Entity.Validation;

namespace Rikrop.Core.Data.Exceptions
{
    internal static class ValidationExceptionFactory
    {
        public static ValidationException GetException(DbEntityValidationException exception)
        {
            var res = new ValidationException(exception);
            foreach (var error in exception.EntityValidationErrors)
            {
                var e = new ValidationExceptionDetail(error.Entry, error.IsValid);
                foreach (var details in error.ValidationErrors)
                {
                    e.Errors.Add(new EntityValidationDetails(details.PropertyName, details.ErrorMessage));
                }
                res.Details.Add(e);
            }
            return res;
        }
    }
}

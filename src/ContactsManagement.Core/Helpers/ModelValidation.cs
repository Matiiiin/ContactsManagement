using System.ComponentModel.DataAnnotations;

namespace ContactsManagement.Core.Helpers
{
    internal class ModelValidation
    {
        public static void Validate(object obj)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(obj);
            var isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);
            if (!isValid)
            {
                throw new ArgumentException(validationResults.FirstOrDefault()?.ErrorMessage);
            }
        }

    }
}

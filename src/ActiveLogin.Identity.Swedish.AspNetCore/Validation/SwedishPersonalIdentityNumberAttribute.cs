using System;
using System.ComponentModel.DataAnnotations;

namespace ActiveLogin.Identity.Swedish.AspNetCore.Validation
{
    public class SwedishPersonalIdentityNumberAttribute : ValidationAttribute
    {
        public SwedishPersonalIdentityNumberAttribute()
            : base("{0} is not a valid Swedish personal identity number. It should follow the pattern YYMMDD-XXXX, YYMMDD+XXXX or YYYYMMDDXXXX.")
        { }

        public SwedishPersonalIdentityNumberAttribute(Func<string> errorMessageAccessor) : base(errorMessageAccessor)
        { }

        public SwedishPersonalIdentityNumberAttribute(string errorMessage) : base(errorMessage)
        { }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var valueString = (string) value;
            if (!SwedishPersonalIdentityNumber.TryParse(valueString, out _))
            {
                var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(errorMessage);
            }

            return ValidationResult.Success;
        }
    }
}

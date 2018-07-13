using System;
using System.ComponentModel.DataAnnotations;

namespace ActiveLogin.Identity.Swedish.AspNetCore.Validation
{
    /// <summary>
    /// Validates a Swedish Personal Identity Number using <see cref="SwedishPersonalIdentityNumber"/>.
    /// </summary>
    public class SwedishPersonalIdentityNumberAttribute : ValidationAttribute
    {
        /// <summary>Initializes a new instance of the <see cref="SwedishPersonalIdentityNumberAttribute"></see> class.</summary>
        public SwedishPersonalIdentityNumberAttribute()
            : base("{0} is not a valid Swedish Personal Identity Number. It should follow a valid pattern such as YYMMDD-XXXX, YYMMDD+XXXX or YYYYMMDDXXXX.")
        { }

        /// <summary>Initializes a new instance of the <see cref="SwedishPersonalIdentityNumberAttribute"></see> class by using the function that enables access to validation resources.</summary>
        /// <param name="errorMessageAccessor">The function that enables access to validation resources.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="errorMessageAccessor">errorMessageAccessor</paramref> is null.</exception>
        public SwedishPersonalIdentityNumberAttribute(Func<string> errorMessageAccessor) : base(errorMessageAccessor)
        { }

        /// <summary>Initializes a new instance of the <see cref="SwedishPersonalIdentityNumberAttribute"></see> class by using the error message to associate with a validation control.</summary>
        /// <param name="errorMessage">The error message to associate with a validation control.</param>
        public SwedishPersonalIdentityNumberAttribute(string errorMessage) : base(errorMessage)
        { }

        /// <summary>Validates the specified Swedish Personal Identity Number with respect to the current validation attribute.</summary>
        /// <param name="value">The Swedish Personal Identity Number to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>An instance of the <see cref="System.ComponentModel.DataAnnotations.ValidationResult"></see> class.</returns>
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
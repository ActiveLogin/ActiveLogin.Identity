using System;
using System.ComponentModel.DataAnnotations;

namespace ActiveLogin.Identity.Swedish.AspNetCore.Validation
{
    /// <summary>
    /// Validates a Swedish Coordination Number using <see cref="CoordinationNumber"/>.
    /// </summary>
    public class CoordinationNumberAttribute : ValidationAttribute
    {
        /// <summary>Initializes a new instance of the <see cref="CoordinationNumberAttribute"></see> class.</summary>
        public CoordinationNumberAttribute()
            : base("{0} is not a valid Swedish Coordination Number. It should follow a valid pattern such as YYMMDD-IIIC, YYMMDD+IIIC or YYYYMMDDIIIC.")
        { }

        /// <summary>Initializes a new instance of the <see cref="CoordinationNumberAttribute"></see> class by using the function that enables access to validation resources.</summary>
        /// <param name="errorMessageAccessor">The function that enables access to validation resources.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="errorMessageAccessor">errorMessageAccessor</paramref> is null.</exception>
        public CoordinationNumberAttribute(Func<string> errorMessageAccessor) : base(errorMessageAccessor)
        { }

        /// <summary>Initializes a new instance of the <see cref="CoordinationNumberAttribute"></see> class by using the error message to associate with a validation control.</summary>
        /// <param name="errorMessage">The error message to associate with a validation control.</param>
        public CoordinationNumberAttribute(string errorMessage) : base(errorMessage)
        { }

        /// <summary>Validates the specified Swedish Coordination Number with respect to the current validation attribute.</summary>
        /// <param name="value">The Swedish Coordination Number to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>An instance of the <see cref="System.ComponentModel.DataAnnotations.ValidationResult"></see> class.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var valueString = (string) value;
            if (!CoordinationNumber.TryParse(valueString, out _))
            {
                var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(errorMessage);
            }

            return ValidationResult.Success;
        }
    }
}

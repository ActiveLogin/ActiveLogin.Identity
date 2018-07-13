using System.Collections.Generic;
using ActiveLogin.Identity.Swedish.AspNetCore.Validation;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace ActiveLogin.Identity.Swedish.AspNetCore.Test
{
    public class SwedishPersonalIdentityNumberAttribute_IsValid
    {
        [Fact]
        public void Returns_Valid_When_Valid_Personal_Identity_Number()
        {
            var model = new SampleValidationModel("990913-9801");
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void Returns_Invalif_When_Invalid_Personal_Identity_Number()
        {
            var model = new SampleValidationModel("990913_9801");
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.False(isValid);
            Assert.Single(results);
        }

        private class SampleValidationModel
        {
            public SampleValidationModel(string swedishPersonalIdentityNumber)
            {
                SwedishPersonalIdentityNumber = swedishPersonalIdentityNumber;
            }

            [SwedishPersonalIdentityNumber]
            public string SwedishPersonalIdentityNumber { get; }
        }
    }
}

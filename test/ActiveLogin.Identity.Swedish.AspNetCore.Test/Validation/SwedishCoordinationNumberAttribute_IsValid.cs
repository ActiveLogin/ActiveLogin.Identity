using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ActiveLogin.Identity.Swedish.AspNetCore.Validation;
using Xunit;

namespace ActiveLogin.Identity.Swedish.AspNetCore.Test.Validation
{
    /// <remarks>
    /// Tested with official test Coordination Numbers from Skatteverket:
    /// https://swedish.identityinfo.net/coordinationnumber/testdata
    /// </remarks>
    public class SwedishCoordinationNumberAttribute_IsValid
    {
        [Fact]
        public void Returns_Valid_When_Valid_Coordination_Number()
        {
            var model = new SampleValidationModel("480977-2389");
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void Returns_Invalid_When_Invalid_Coordination_Number()
        {
            var model = new SampleValidationModel("A");
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.False(isValid);
            Assert.Single(results);
        }

        private class SampleValidationModel
        {
            public SampleValidationModel(string swedishCoordinationNumber)
            {
                SwedishCoordinationNumber = swedishCoordinationNumber;
            }

            [CoordinationNumber]
            public string SwedishCoordinationNumber { get; }
        }
    }
}

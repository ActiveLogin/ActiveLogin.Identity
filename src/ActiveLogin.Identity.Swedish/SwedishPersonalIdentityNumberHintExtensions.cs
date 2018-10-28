using System;

namespace ActiveLogin.Identity.Swedish
{
    public static class SwedishPersonalIdentityNumberHintExtensions
    {
        /// <summary>
        /// Gender (juridiskt kön) in Sweden according to the last digit of the birth number in the personal identity number.
        /// Odd number: Male
        /// Even number: Female
        /// </summary>
        public static Gender GetGenderHint(this SwedishPersonalIdentityNumber personalIdentityNumber)
        {
            return GetGender(personalIdentityNumber.BirthNumber);
        }

        private static Gender GetGender(int birthNumber)
        {
            var isBirthNumberEven = birthNumber % 2 == 0;
            if (isBirthNumberEven)
            {
                return Gender.Female;
            }

            return Gender.Male;
        }

        /// <summary>
        /// Date of birth for the person according to the personal identity number.
        /// Not always the actual date of birth due to lmitied amount of personal identity numbers per day.
        /// </summary>
        public static DateTime GetDateOfBirthHint(this SwedishPersonalIdentityNumber personalIdentityNumber)
        {
            return SwedishPersonalIdentityNumberDateCalculations.GetDateOfBirth(personalIdentityNumber.Year, personalIdentityNumber.Month, personalIdentityNumber.Day);
        }

        /// <summary>
        /// Get the age of the person according to the date in the personal identity number.
        /// Not always the actual date of birth due to lmitied amount of personal identity numbers per day.
        /// </summary>
        public static int GetAgeHint(this SwedishPersonalIdentityNumber personalIdentityNumber)
        {
            return GetAgeHint(personalIdentityNumber, DateTime.UtcNow);
        }

        /// <summary>
        /// Get the age of the person according to the date in the personal identity number.
        /// Not always the actual date of birth due to lmitied amount of personal identity numbers per day.
        /// </summary>
        /// <param name="personalIdentityNumber"></param>
        /// <param name="date">The date when to calulate the age.</param>
        /// <returns></returns>
        public static int GetAgeHint(this SwedishPersonalIdentityNumber personalIdentityNumber, DateTime date)
        {
            return SwedishPersonalIdentityNumberDateCalculations.GetAge(personalIdentityNumber.GetDateOfBirthHint(), date);
        }
    }
}

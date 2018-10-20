namespace ActiveLogin.Identity.Swedish
{
    public static class SwedishPersonalIdentityNumberHintExtensions
    {
        /// <summary>
        /// Gender (juridiskt kön) in Sweden according to the last digit of the serial number in the personal identity number.
        /// Odd number: Male
        /// Even number: Female
        /// </summary>
        public static Gender GetGenderHint(this SwedishPersonalIdentityNumber personalIdentityNumber)
        {
            return GetGender(personalIdentityNumber.SerialNumber);
        }

        private static Gender GetGender(int serialNumber)
        {
            var isSerialNumberEven = serialNumber % 2 == 0;
            if (isSerialNumberEven)
            {
                return Gender.Female;
            }

            return Gender.Male;
        }
    }
}

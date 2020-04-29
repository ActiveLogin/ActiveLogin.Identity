using System;
using ActiveLogin.Identity.Swedish;

namespace ExtensionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = "120211+9986";
            var pin = SwedishPersonalIdentityNumber.Parse(input);
            Console.WriteLine($"{input} is a test number: {pin.IsTestNumber()}");
        }
    }
}

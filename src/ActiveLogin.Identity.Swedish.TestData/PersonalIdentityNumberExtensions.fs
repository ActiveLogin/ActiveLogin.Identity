namespace ActiveLogin.Identity.Swedish

open System.Runtime.CompilerServices
open ActiveLogin.Identity.Swedish.TestData

[<Extension>]
/// Checks if a PersonalIdentityNumber is a test number
module PersonalIdentityNumberExtensions =
    [<Extension>]
    let IsTestNumber(pin : PersonalIdentityNumber) =
        PersonalIdentityNumberTestData.IsTestNumber pin

[<AutoOpen>]
module PersonalIdentityNumberFSharpExtensions =
    type PersonalIdentityNumber
        /// Checks if a PersonalIdentityNumber is a test number
        with member this.IsTestNumber() = PersonalIdentityNumberTestData.IsTestNumber this

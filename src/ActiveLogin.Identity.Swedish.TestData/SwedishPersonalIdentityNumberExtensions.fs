namespace ActiveLogin.Identity.Swedish

open System.Runtime.CompilerServices
open ActiveLogin.Identity.Swedish.TestData

[<Extension>]
/// Checks if a SwedishPersonalIdentityNumber is a test number
module SwedishPersonalIdentityNumberExtensions =
    [<Extension>]
    let IsTestNumber(pin : SwedishPersonalIdentityNumber) =
        SwedishPersonalIdentityNumberTestData.IsTestNumber pin

[<AutoOpen>]
module SwedishPersonalIdentityNumberFSharpExtensions =
    type SwedishPersonalIdentityNumber
        /// Checks if a SwedishPersonalIdentityNumber is a test number
        with member this.IsTestNumber() = SwedishPersonalIdentityNumberTestData.IsTestNumber this

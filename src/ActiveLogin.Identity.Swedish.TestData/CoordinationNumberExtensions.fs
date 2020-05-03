namespace ActiveLogin.Identity.Swedish

open System.Runtime.CompilerServices
open ActiveLogin.Identity.Swedish.TestData

[<Extension>]
/// Checks if a CoordinationNumber is a test number
module CoordinationNumberTestDataExtensions =
    [<Extension>]
    let IsTestNumber(pin : CoordinationNumber) =
        CoordinationNumberTestData.IsTestNumber pin

[<AutoOpen>]
module CoordinationNumberFSharpExtensions =
    type CoordinationNumber
        /// Checks if a CoordinationNumber is a test number
        with member this.IsTestNumber() = CoordinationNumberTestData.IsTestNumber this

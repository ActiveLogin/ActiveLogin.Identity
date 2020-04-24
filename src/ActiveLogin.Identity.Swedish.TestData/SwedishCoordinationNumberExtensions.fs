namespace ActiveLogin.Identity.Swedish

open System.Runtime.CompilerServices
open ActiveLogin.Identity.Swedish.TestData

[<Extension>]
/// Checks if a SwedishCoordinationNumber is a test number
module SwedishCoordinationNumberTestDataExtensions =
    [<Extension>]
    let IsTestNumber(pin : SwedishCoordinationNumber) =
        SwedishCoordinationNumberTestData.IsTestNumber pin

[<AutoOpen>]
module SwedishCoordinationNumberFSharpExtensions =
    type SwedishCoordinationNumber
        /// Checks if a SwedishCoordinationNumber is a test number
        with member this.IsTestNumber() = SwedishCoordinationNumberTestData.IsTestNumber this

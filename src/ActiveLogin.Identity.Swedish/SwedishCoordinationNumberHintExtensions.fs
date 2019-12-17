namespace ActiveLogin.Identity.Swedish.Extensions
open System.Runtime.CompilerServices
open ActiveLogin.Identity.Swedish.FSharp
open ActiveLogin.Identity.Swedish.FSharp.Shared
open ActiveLogin.Identity.Swedish


module internal SwedishCoordinationNumberHints =
    let getGenderHint (num: SwedishCoordinationNumberInternal) =
        match num.IndividualNumber.Value with
        | Even -> Gender.Female
        | Odd -> Gender.Male


[<Extension>]
type SwedishCoordinationNumberHintExtensions() =

    /// <summary>
    /// Gender (juridiskt kön) in Sweden according to the last digit of the birth number in the coordination number.
    /// Odd number: Male
    /// Even number: Female
    /// </summary>
    [<Extension>]
    static member GetGenderHint(num : SwedishCoordinationNumber) =
        SwedishCoordinationNumberHints.getGenderHint num.IdentityNumber

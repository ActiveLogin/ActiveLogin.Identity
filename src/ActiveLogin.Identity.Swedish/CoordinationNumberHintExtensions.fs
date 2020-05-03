namespace ActiveLogin.Identity.Swedish.Extensions
open System.Runtime.CompilerServices
open ActiveLogin.Identity.Swedish
open ActiveLogin.Identity.Swedish.Shared


module internal CoordinationNumberHints =
    let getGenderHint (num: CoordinationNumberInternal) =
        match num.IndividualNumber.Value with
        | Even -> Gender.Female
        | Odd -> Gender.Male


[<Extension>]
type CoordinationNumberHintExtensions() =

    /// <summary>
    /// Gender (juridiskt kön) in Sweden according to the last digit of the birth number in the coordination number.
    /// Odd number: Male
    /// Even number: Female
    /// </summary>
    [<Extension>]
    static member GetGenderHint(num : CoordinationNumber) =
        CoordinationNumberHints.getGenderHint num.IdentityNumber

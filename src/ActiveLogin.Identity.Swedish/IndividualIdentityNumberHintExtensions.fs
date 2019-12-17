namespace ActiveLogin.Identity.Swedish.Extensions
open System.Runtime.CompilerServices
open ActiveLogin.Identity.Swedish
open ActiveLogin.Identity.Swedish.FSharp

module internal IndividualIdentityNumberHints =
    let getGenderHint (num: IndividualIdentityNumberInternal) =
        match num with
        | Personal pin ->
            pin |> SwedishPersonalIdentityNumberHints.getGenderHint
        | Coordination num ->
            num |> SwedishCoordinationNumberHints.getGenderHint

[<Extension>]
type IdentityNumberHintExtensions() =

    /// <summary>
    /// Gender (juridiskt kön) in Sweden according to the last digit of the birth number in the identity number.
    /// Odd number: Male
    /// Even number: Female
    /// </summary>
    [<Extension>]
    static member GetGenderHint(num : IndividualIdentityNumber) =
        IndividualIdentityNumberHints.getGenderHint num.IdentityNumber

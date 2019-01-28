namespace ActiveLogin.Identity.Swedish.Extensions
open System
open System.Runtime.CompilerServices
open ActiveLogin.Identity.Swedish
open ActiveLogin.Identity.Swedish.FSharp

[<Extension>]
type SwedishPersonalIdentityNumberCSharpHintExtensions() =

    /// <summary>
    /// Date of birth for the person according to the personal identity number.
    /// Not always the actual date of birth due to the limited quantity of personal identity numbers per day.
    /// </summary>
    [<Extension>]
    static member GetDateOfBirthHint(pin : SwedishPersonalIdentityNumberCSharp) = 
        SwedishPersonalIdentityNumber.Hints.getDateOfBirthHint pin.IdentityNumber

    /// <summary>
    /// Gender (juridiskt kön) in Sweden according to the last digit of the birth number in the personal identity number.
    /// Odd number: Male
    /// Even number: Female
    /// </summary>
    [<Extension>]
    static member GetGenderHint(pin : SwedishPersonalIdentityNumberCSharp) = 
        SwedishPersonalIdentityNumber.Hints.getGenderHint pin.IdentityNumber

    /// <summary>
    /// Get the age of the person according to the date in the personal identity number.
    /// Not always the actual date of birth due to the limited quantity of personal identity numbers per day.
    /// </summary>
    /// <param name="pin"></param>
    /// <param name="date">The date when to calculate the age.</param>
    /// <returns></returns>
    [<Extension>]
    static member GetAgeHint(pin : SwedishPersonalIdentityNumberCSharp, date : DateTime) =
        SwedishPersonalIdentityNumber.Hints.getAgeHintOnDate date pin.IdentityNumber
        |> function
        | None -> invalidArg "pin" "The person is not yet born."
        | Some i -> i

    /// <summary>
    /// Get the age of the person according to the date in the personal identity number.
    /// Not always the actual date of birth due to the limited quantity of personal identity numbers per day.
    /// </summary>
    [<Extension>]
    static member GetAgeHint(pin : SwedishPersonalIdentityNumberCSharp) =
        SwedishPersonalIdentityNumber.Hints.getAgeHint pin.IdentityNumber
        |> function
        | None -> invalidArg "pin" "The person is not yet born."
        | Some i -> i
namespace ActiveLogin.Identity.Swedish.Extensions
open System
open System.Runtime.CompilerServices
open ActiveLogin.Identity.Swedish
open ActiveLogin.Identity.Swedish.FSharp

[<Extension>]
type SwedishCoordinationNumberHintExtensions() =

    /// <summary>
    /// Date of birth for the person according to the coordination number.
    /// Not always the actual date of birth due to the limited quantity of coordination numbers per day.
    /// </summary>
    [<Extension>]
    static member GetDateOfBirthHint(num : SwedishCoordinationNumber) =
        SwedishCoordinationNumber.Hints.getDateOfBirthHint num.IdentityNumber

    /// <summary>
    /// Gender (juridiskt kön) in Sweden according to the last digit of the birth number in the coordination number.
    /// Odd number: Male
    /// Even number: Female
    /// </summary>
    [<Extension>]
    static member GetGenderHint(num : SwedishCoordinationNumber) =
        SwedishCoordinationNumber.Hints.getGenderHint num.IdentityNumber

    /// <summary>
    /// Get the age of the person according to the date in the coordination number.
    /// Not always the actual date of birth due to the limited quantity of coordination numbers per day.
    /// </summary>
    /// <param name="num"></param>
    /// <param name="date">The date when to calculate the age.</param>
    /// <returns></returns>
    [<Extension>]
    static member GetAgeHint(num : SwedishCoordinationNumber, date : DateTime) =
        SwedishCoordinationNumber.Hints.getAgeHintOnDate date num.IdentityNumber
        |> function
        | None -> invalidArg "num" "The person is not yet born."
        | Some i -> i

    /// <summary>
    /// Get the age of the person according to the date in the coordination number.
    /// Not always the actual date of birth due to the limited quantity of coordination numbers per day.
    /// </summary>
    [<Extension>]
    static member GetAgeHint(num : SwedishCoordinationNumber) =
        SwedishCoordinationNumber.Hints.getAgeHint num.IdentityNumber
        |> function
        | None -> invalidArg "num" "The person is not yet born."
        | Some i -> i

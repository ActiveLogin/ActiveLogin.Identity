namespace ActiveLogin.Identity.Swedish.Extensions
open System
open System.Runtime.CompilerServices
open ActiveLogin.Identity.Swedish

[<Extension>]
type IdentityNumberHintExtensions() =

    /// <summary>
    /// Date of birth for the person according to the identity number.
    /// Not always the actual date of birth due to the limited quantity of identity numbers per day.
    /// </summary>
    [<Extension>]
    static member GetDateOfBirthHint(num : IndividualIdentityNumber) =
        IndividualIdentityNumber.Hints.getDateOfBirthHint num.IdentityNumber

    /// <summary>
    /// Gender (juridiskt kön) in Sweden according to the last digit of the birth number in the identity number.
    /// Odd number: Male
    /// Even number: Female
    /// </summary>
    [<Extension>]
    static member GetGenderHint(num : IndividualIdentityNumber) =
        IndividualIdentityNumber.Hints.getGenderHint num.IdentityNumber

    /// <summary>
    /// Get the age of the person according to the date in the identity number.
    /// Not always the actual date of birth due to the limited quantity of identity numbers per day.
    /// </summary>
    /// <param name="num"></param>
    /// <param name="date">The date when to calculate the age.</param>
    /// <returns></returns>
    [<Extension>]
    static member GetAgeHint(num : IndividualIdentityNumber, date : DateTime) =
        IndividualIdentityNumber.Hints.getAgeHintOnDate date num.IdentityNumber
        |> function
        | None -> invalidArg "num" "The person is not yet born."
        | Some i -> i

    /// <summary>
    /// Get the age of the person according to the date in the identity number.
    /// Not always the actual date of birth due to the limited quantity of identity numbers per day.
    /// </summary>
    [<Extension>]
    static member GetAgeHint(num : IndividualIdentityNumber) =
        IndividualIdentityNumber.Hints.getAgeHint num.IdentityNumber
        |> function
        | None -> invalidArg "num" "The person is not yet born."
        | Some i -> i

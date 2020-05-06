namespace ActiveLogin.Identity.Swedish.Extensions
open System
open System.Runtime.CompilerServices
open ActiveLogin.Identity.Swedish
open ActiveLogin.Identity.Swedish.Shared

module Option =
    let fromNullable (n: Nullable<_>) =
        if n.HasValue then
            Some n.Value
        else
            None

type private AgeResult =
    | MissingMonthOrDay
    | NotYetBornError
    | Age of int
module private CoordinationNumberHints =
    let getDateOfBirthHint (num: CoordinationNumberInternal) : Nullable<DateTime> =
        match num.CoordinationMonth.Value, num.CoordinationDay.Value with
        | (0, _) | (_, 0) -> Nullable()
        | (month, day) ->
            DateTime(num.Year.Value, month, day, 0, 0, 0, DateTimeKind.Utc) |> Nullable

    let getAgeHintOnDate date pin =
        getDateOfBirthHint pin
        |> Option.fromNullable
        |> function
        | Some dateOfBirth ->
            if date >= dateOfBirth then
                let months = 12 * (date.Year - dateOfBirth.Year) + (date.Month - dateOfBirth.Month)
                match date.Day < dateOfBirth.Day with
                | true ->
                    let years = (months - 1) / 12
                    years |> Age
                | false -> months / 12 |> Age
            else NotYetBornError
        | None -> MissingMonthOrDay

    let getGenderHint (num: CoordinationNumberInternal) =
        match num.IndividualNumber.Value with
        | Even -> Gender.Female
        | Odd -> Gender.Male


[<Extension>]
type CoordinationNumberHintExtensions() =
    /// <summary>
    /// Date of birth for the person according to the personal identity number.
    /// Not always the actual date of birth due to the limited quantity of personal identity numbers per day.
    /// </summary>
    [<Extension>]
    static member GetDateOfBirthHint(num : CoordinationNumber) =
        CoordinationNumberHints.getDateOfBirthHint num.IdentityNumber

    /// <summary>
    /// Gender (juridiskt kön) in Sweden according to the last digit of the birth number in the coordination number.
    /// Odd number: Male
    /// Even number: Female
    /// </summary>
    [<Extension>]
    static member GetGenderHint(num : CoordinationNumber) =
        CoordinationNumberHints.getGenderHint num.IdentityNumber

    /// <summary>
    /// Get the age of the person according to the date in the personal identity number.
    /// Not always the actual date of birth due to the limited quantity of personal identity numbers per day.
    /// </summary>
    /// <param name="pin"></param>
    /// <param name="date">The date when to calculate the age.</param>
    /// <returns></returns>
    [<Extension>]
    static member GetAgeHint(num : CoordinationNumber, date : DateTime) =
        CoordinationNumberHints.getAgeHintOnDate date num.IdentityNumber
        |> function
        | NotYetBornError -> invalidArg "pin" "The person is not yet born."
        | Age i -> i |> Nullable
        | MissingMonthOrDay -> Nullable()

    /// <summary>
    /// Get the age of the person according to the date in the personal identity number.
    /// Not always the actual date of birth due to the limited quantity of personal identity numbers per day.
    /// </summary>
    [<Extension>]
    static member GetAgeHint(num : CoordinationNumber) =
        CoordinationNumberHintExtensions.GetAgeHint(num, DateTime.UtcNow)

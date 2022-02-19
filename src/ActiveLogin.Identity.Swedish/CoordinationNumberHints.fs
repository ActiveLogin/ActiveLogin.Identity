namespace ActiveLogin.Identity.Swedish.Extensions
open System
open System.Runtime.CompilerServices
open ActiveLogin.Identity.Swedish
open ActiveLogin.Identity.Swedish.Shared
open ActiveLogin.Identity.Swedish.Shared.Hints

[<AutoOpen>]
module internal CoordNum =
    let getDateOfBirthHint (num: CoordinationNumberInternal) : Nullable<DateTime> =
        match num.CoordinationMonth.Value, num.CoordinationDay.RealDay with
        | (0, _) | (_, 0) -> Nullable()
        | (month, day) ->
            DateTime(num.Year.Value, month, day, 0, 0, 0, DateTimeKind.Utc) |> Nullable

    let getAgeHintOnDate date num =
        getDateOfBirthHint num
        |> getAgeFromNullableDOB date

    let getGenderHint (num: CoordinationNumberInternal) =
        match num.IndividualNumber.Value with
        | Even -> Gender.Female
        | Odd -> Gender.Male

[<Extension>]
type CoordinationNumberHintExtensions() =
    /// <summary>
    /// Date of birth for the person according to the coordination number.
    /// Not always the actual date of birth due to the limited quantity of coordination numbers per day.
    /// </summary>
    [<Extension>]
    static member GetDateOfBirthHint(num : CoordinationNumber) = CoordNum.getDateOfBirthHint num.IdentityNumber

    /// <summary>
    /// Gender (juridiskt kön) in Sweden according to the last digit of the birth number in the coordination number.
    /// Odd number: Male
    /// Even number: Female
    /// </summary>
    [<Extension>]
    static member GetGenderHint(num : CoordinationNumber) = CoordNum.getGenderHint num.IdentityNumber

    /// <summary>
    /// Get the age of the person according to the date in the coordination number.
    /// Not always the actual date of birth due to the limited quantity of coordination numbers per day.
    /// </summary>
    /// <param name="pin"></param>
    /// <param name="date">The date when to calculate the age.</param>
    /// <returns></returns>
    [<Extension>]
    static member GetAgeHint(num : CoordinationNumber, date : DateTime) = CoordNum.getAgeHintOnDate date num.IdentityNumber

    /// <summary>
    /// Get the age of the person according to the date in the coordination number.
    /// Not always the actual date of birth due to the limited quantity of coordination numbers per day.
    /// </summary>
    [<Extension>]
    static member GetAgeHint(num : CoordinationNumber) = CoordinationNumberHintExtensions.GetAgeHint(num, DateTime.UtcNow)

namespace ActiveLogin.Identity.Swedish.Extensions
open System
open System.Runtime.CompilerServices
open ActiveLogin.Identity.Swedish
open ActiveLogin.Identity.Swedish.Shared

module internal PersonalIdentityNumberHints =
    let getDateOfBirthHint (pin: PersonalIdentityNumberInternal) =
        DateTime(pin.Year.Value, pin.Month.Value, pin.Day.Value, 0, 0, 0, DateTimeKind.Utc)

    let getAgeHintOnDate date pin =
        let dateOfBirth = getDateOfBirthHint pin
        if date >= dateOfBirth then
            let months = 12 * (date.Year - dateOfBirth.Year) + (date.Month - dateOfBirth.Month)
            match date.Day < dateOfBirth.Day with
            | true ->
                let years = (months - 1) / 12
                years |> Some
            | false -> months / 12 |> Some
        else None

    let getAgeHint = getAgeHintOnDate DateTime.UtcNow

    let getGenderHint pin =
        match pin.BirthNumber.Value with
        | Even -> Gender.Female
        | Odd -> Gender.Male


[<Extension>]
type PersonalIdentityNumberHintExtensions() =
    /// <summary>
    /// Date of birth for the person according to the personal identity number.
    /// Not always the actual date of birth due to the limited quantity of personal identity numbers per day.
    /// </summary>
    [<Extension>]
    static member GetDateOfBirthHint(pin : PersonalIdentityNumber) =
        PersonalIdentityNumberHints.getDateOfBirthHint pin.IdentityNumber

    /// <summary>
    /// Gender (juridiskt kön) in Sweden according to the last digit of the birth number in the personal identity number.
    /// Odd number: Male
    /// Even number: Female
    /// </summary>
    [<Extension>]
    static member GetGenderHint(pin : PersonalIdentityNumber) =
        PersonalIdentityNumberHints.getGenderHint pin.IdentityNumber

    /// <summary>
    /// Get the age of the person according to the date in the personal identity number.
    /// Not always the actual date of birth due to the limited quantity of personal identity numbers per day.
    /// </summary>
    /// <param name="pin"></param>
    /// <param name="date">The date when to calculate the age.</param>
    /// <returns></returns>
    [<Extension>]
    static member GetAgeHint(pin : PersonalIdentityNumber, date : DateTime) =
        PersonalIdentityNumberHints.getAgeHintOnDate date pin.IdentityNumber
        |> function
        | None -> invalidArg "pin" "The person is not yet born."
        | Some i -> i

    /// <summary>
    /// Get the age of the person according to the date in the personal identity number.
    /// Not always the actual date of birth due to the limited quantity of personal identity numbers per day.
    /// </summary>
    [<Extension>]
    static member GetAgeHint(pin : PersonalIdentityNumber) =
        PersonalIdentityNumberHints.getAgeHint pin.IdentityNumber
        |> function
        | None -> invalidArg "pin" "The person is not yet born."
        | Some i -> i

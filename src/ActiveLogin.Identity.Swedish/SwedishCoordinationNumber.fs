module ActiveLogin.Identity.Swedish.FSharp.SwedishCoordinationNumber
open ActiveLogin.Identity.Swedish.FSharp
open System

let create (values : IdentityNumberValues) =
    result {
        let! y = values.Year |> Year.create
        let! m = values.Month |> Month.create
        let! d = values.Day |> CoordinationDay.create y m
        let! s = values.BirthNumber |> BirthNumber.create
        let! c = values.Checksum |> Checksum.create y m (CoordinationDay d) s
        return { SwedishCoordinationNumber.Year = y
                 Month = m
                 CoordinationDay = d
                 BirthNumber = s
                 Checksum = c }
    }

/// <summary>
/// Converts a SwedishPersonalIdentityNumber to its equivalent 10 digit string representation. The total length, including the separator, will be 11 chars.
/// </summary>
/// <param name="serializationYear">
/// The specific year to use when checking if the person has turned / will turn 100 years old.
/// That information changes the delimiter (- or +).
///
/// For more info, see: https://www.riksdagen.se/sv/dokument-lagar/dokument/svensk-forfattningssamling/folkbokforingslag-1991481_sfs-1991-481#P18
/// </param>
/// <param name="pin">A SwedishPersonalIdentityNumber</param>
let to10DigitStringInSpecificYear serializationYear (num: SwedishCoordinationNumber) =
    num
    |> Coordination
    |> StringHelpers.to10DigitStringInSpecificYear serializationYear

/// <summary>
/// Converts a SwedishPersonalIdentityNumber to its equivalent 10 digit string representation. The total length, including the separator, will be 11 chars.
/// </summary>
/// <param name="pin">A SwedishPersonalIdentityNumber</param>
let to10DigitString (num : SwedishCoordinationNumber) =
    num
    |> Coordination
    |> StringHelpers.to10DigitString

/// <summary>
/// Converts the value of the current <see cref="SwedishPersonalIdentityNumber" /> object to its equivalent 12 digit string representation.
/// Format is YYYYMMDDBBBC, for example <example>19908072391</example> or <example>191202119986</example>.
/// </summary>
/// <param name="pin">A SwedishPersonalIdentityNumber</param>
let to12DigitString pin =
    pin
    |> Coordination
    |> StringHelpers.to12DigitString

/// <summary>
/// Converts the string representation of the Swedish personal identity number to its <see cref="SwedishPersonalIdentityNumber"/> equivalent.
/// </summary>
/// <param name="parseYear">
/// The specific year to use when checking if the person has turned / will turn 100 years old.
/// That information changes the delimiter (- or +).
///
/// For more info, see: https://www.riksdagen.se/sv/dokument-lagar/dokument/svensk-forfattningssamling/folkbokforingslag-1991481_sfs-1991-481#P18
/// </param>
/// <param name="str">A string representation of the Swedish personal identity number to parse.</param>
let parseInSpecificYear parseYear str =
    match Parse.parse parseYear str with
    | Ok pinValues -> create pinValues
    | Error error -> Error error
    |> Result.mapError ParsingError.toParsingError

/// <summary>
/// Converts the string representation of the personal identity number to its <see cref="SwedishPersonalIdentityNumber"/> equivalent.
/// </summary>
/// <param name="str">A string representation of the Swedish personal identity number to parse.</param>
let parse str = result { let! year = DateTime.UtcNow.Year |> Year.create
                         return! parseInSpecificYear year str }

module Hints =
    open ActiveLogin.Identity.Swedish

    /// <summary>
    /// Date of birth for the person according to the personal identity number.
    /// Not always the actual date of birth due to the limited quantity of personal identity numbers per day.
    /// </summary>
    /// <param name="pin">A SwedishPersonalIdentityNumber</param>
    let getDateOfBirthHint num = HintsHelper.getDateOfBirthHint (Coordination num)

    /// <summary>
    /// Get the age of the person according to the date in the personal identity number.
    /// Not always the actual date of birth due to the limited quantity of personal identity numbers per day.
    /// </summary>
    /// <param name="date">The date when to calculate the age.</param>
    /// <param name="pin">A SwedishPersonalIdentityNumber</param>
    let getAgeHintOnDate date num = HintsHelper.getAgeHintOnDate date (Coordination num)

    /// <summary>
    /// Get the age of the person according to the date in the personal identity number.
    /// Not always the actual date of birth due to the limited quantity of personal identity numbers per day.
    /// </summary>
    /// <param name="pin">A SwedishPersonalIdentityNumber</param>
    let getAgeHint num = HintsHelper.getAgeHint (Coordination num)

    /// <summary>
    /// Gender (juridiskt kön) in Sweden according to the last digit of the birth number in the personal identity number.
    /// Odd number: Male
    /// Even number: Female
    /// </summary>
    /// <param name="pin">A SwedishPersonalIdentityNumber</param>
    let getGenderHint num = HintsHelper.getGenderHint (Coordination num)

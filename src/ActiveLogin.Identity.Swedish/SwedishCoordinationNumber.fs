module ActiveLogin.Identity.Swedish.FSharp.SwedishCoordinationNumber
open ActiveLogin.Identity.Swedish.FSharp

/// <summary>
/// Creates a <see cref="SwedishCoordinationNumber"/> out of the individual parts.
/// </summary>
/// <param name="values">IdentityNumberValues containing all the number parts</param>
let create (year, month, day, birthNumber, checksum) =
    result {
        let! y = year |> Year.create
        let! m = month |> Month.create
        let! d = day |> CoordinationDay.create y m
        let! s = birthNumber |> BirthNumber.create
        let! c = checksum |> Checksum.create y m (CoordinationDay d) s
        return { SwedishCoordinationNumber.Year = y
                 Month = m
                 CoordinationDay = d
                 BirthNumber = s
                 Checksum = c }
    }

/// <summary>
/// Converts a SwedishCoordinationNumber to its equivalent 10 digit string representation. The total length, including the separator, will be 11 chars.
/// </summary>
/// <param name="serializationYear">
/// The specific year to use when checking if the person has turned / will turn 100 years old.
/// That information changes the delimiter (- or +).
///
/// For more info, see: https://www.riksdagen.se/sv/dokument-lagar/dokument/svensk-forfattningssamling/folkbokforingslag-1991481_sfs-1991-481#P18
/// </param>
/// <param name="num">A SwedishCoordinationNumber</param>
let to10DigitStringInSpecificYear serializationYear (num: SwedishCoordinationNumber) =
    num
    |> Coordination
    |> StringHelpers.to10DigitStringInSpecificYear serializationYear

/// <summary>
/// Converts a SwedishCoordinationNumber to its equivalent 10 digit string representation. The total length, including the separator, will be 11 chars.
/// </summary>
/// <param name="num">A SwedishCoordinationNumber</param>
let to10DigitString (num : SwedishCoordinationNumber) =
    num
    |> Coordination
    |> StringHelpers.to10DigitString

/// <summary>
/// Converts the value of the current <see cref="SwedishCoordinationNumber" /> object to its equivalent 12 digit string representation.
/// Format is YYYYMMDDBBBC, for example <example>199008672397</example> or <example>191202719983</example>.
/// </summary>
/// <param name="num">A SwedishCoordinationNumber</param>
let to12DigitString num =
    num
    |> Coordination
    |> StringHelpers.to12DigitString

/// <summary>
/// Converts the string representation of the Swedish coordination number to its <see cref="SwedishCoordinationNumber"/> equivalent.
/// </summary>
/// <param name="parseYear">
/// The specific year to use when checking if the person has turned / will turn 100 years old.
/// That information changes the delimiter (- or +).
///
/// For more info, see: https://www.riksdagen.se/sv/dokument-lagar/dokument/svensk-forfattningssamling/folkbokforingslag-1991481_sfs-1991-481#P18
/// </param>
/// <param name="str">A string representation of the Swedish coordination number to parse.</param>
let parseInSpecificYear parseYear str = Parse.parseInSpecificYear create parseYear str

/// <summary>
/// Converts the string representation of the Swedish coordination number to its <see cref="SwedishCoordinationNumber"/> equivalent.
/// </summary>
/// <param name="str">A string representation of the Swedish coordination number to parse.</param>
let parse str = Parse.parse create str

module Hints =
    open ActiveLogin.Identity.Swedish

    /// <summary>
    /// Date of birth for the person according to the coordination number.
    /// Not always the actual date of birth due to the limited quantity of coordination numbers per day.
    /// </summary>
    /// <param name="num">A SwedishCoordinationNumber</param>
    let getDateOfBirthHint num = HintsHelper.getDateOfBirthHint (Coordination num)

    /// <summary>
    /// Get the age of the person according to the date in the coordination number.
    /// Not always the actual date of birth due to the limited quantity of coordination numbers per day.
    /// </summary>
    /// <param name="date">The date when to calculate the age.</param>
    /// <param name="num">A SwedishCoordinationNumber</param>
    let getAgeHintOnDate date num = HintsHelper.getAgeHintOnDate date (Coordination num)

    /// <summary>
    /// Get the age of the person according to the date in the coordination number.
    /// Not always the actual date of birth due to the limited quantity of coordination numbers per day.
    /// </summary>
    /// <param name="num">A SwedishCoordinationNumber</param>
    let getAgeHint num = HintsHelper.getAgeHint (Coordination num)

    /// <summary>
    /// Gender (juridiskt kön) in Sweden according to the last digit of the birth number in the coordination number.
    /// Odd number: Male
    /// Even number: Female
    /// </summary>
    /// <param name="num">A SwedishCoordinationNumber</param>
    let getGenderHint num = HintsHelper.getGenderHint (Coordination num)

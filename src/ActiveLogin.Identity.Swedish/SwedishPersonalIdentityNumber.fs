module ActiveLogin.Identity.Swedish.FSharp.SwedishPersonalIdentityNumber


/// <summary>
/// Creates a <see cref="SwedishPersonalIdentityNumber"/> out of the individual parts.
/// </summary>
/// <param name="values">IdentityNumberValues containing all the number parts</param>
let create (values : IdentityNumberValues) =
    result {
        let! y = values.Year |> Year.create
        let! m = values.Month |> Month.create
        let! d = values.Day |> Day.create y m
        let! s = values.BirthNumber |> BirthNumber.create
        let! c = values.Checksum |> Checksum.create y m (Day d) s
        return { SwedishPersonalIdentityNumber.Year = y
                 Month = m
                 Day = d
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
let to10DigitStringInSpecificYear serializationYear pin =
    pin
    |> Personal
    |> StringHelpers.to10DigitStringInSpecificYear serializationYear

/// <summary>
/// Converts a SwedishPersonalIdentityNumber to its equivalent 10 digit string representation. The total length, including the separator, will be 11 chars.
/// </summary>
/// <param name="pin">A SwedishPersonalIdentityNumber</param>
let to10DigitString pin =
    pin
    |> Personal
    |> StringHelpers.to10DigitString

/// <summary>
/// Converts the value of the current <see cref="SwedishPersonalIdentityNumber" /> object to its equivalent 12 digit string representation.
/// Format is YYYYMMDDBBBC, for example <example>199008072390</example> or <example>191202119986</example>.
/// </summary>
/// <param name="pin">A SwedishPersonalIdentityNumber</param>
let to12DigitString pin =
    pin
    |> Personal
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
let parseInSpecificYear parseYear str = Parse.parseInSpecificYear create parseYear str

/// <summary>
/// Converts the string representation of the personal identity number to its <see cref="SwedishPersonalIdentityNumber"/> equivalent.
/// </summary>
/// <param name="str">A string representation of the Swedish personal identity number to parse.</param>
let parse str = Parse.parse create str

module Hints =
    open ActiveLogin.Identity.Swedish

    /// <summary>
    /// Date of birth for the person according to the personal identity number.
    /// Not always the actual date of birth due to the limited quantity of personal identity numbers per day.
    /// </summary>
    /// <param name="pin">A SwedishPersonalIdentityNumber</param>
    let getDateOfBirthHint pin = HintsHelper.getDateOfBirthHint (Personal pin)

    /// <summary>
    /// Get the age of the person according to the date in the personal identity number.
    /// Not always the actual date of birth due to the limited quantity of personal identity numbers per day.
    /// </summary>
    /// <param name="date">The date when to calculate the age.</param>
    /// <param name="pin">A SwedishPersonalIdentityNumber</param>
    let getAgeHintOnDate date pin = HintsHelper.getAgeHintOnDate date (Personal pin)

    /// <summary>
    /// Get the age of the person according to the date in the personal identity number.
    /// Not always the actual date of birth due to the limited quantity of personal identity numbers per day.
    /// </summary>
    /// <param name="pin">A SwedishPersonalIdentityNumber</param>
    let getAgeHint pin = HintsHelper.getAgeHint (Personal pin)

    /// <summary>
    /// Gender (juridiskt kön) in Sweden according to the last digit of the birth number in the personal identity number.
    /// Odd number: Male
    /// Even number: Female
    /// </summary>
    /// <param name="pin">A SwedishPersonalIdentityNumber</param>
    let getGenderHint pin = HintsHelper.getGenderHint (Personal pin)

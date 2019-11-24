module ActiveLogin.Identity.Swedish.FSharp.IndividualIdentityNumber

open ActiveLogin.Identity.Swedish.FSharp


let internal createInternal values =
    match SwedishPersonalIdentityNumber.createInternal values, SwedishCoordinationNumber.createInternal values with
    | Ok pin, Error _ ->
        pin |> Personal |> Ok
    | Error _, Ok num ->
        num |> Coordination |> Ok
    | Error pinError, Error coordError ->
        let msg = sprintf "Not a valid pin or coordination number. PinError: %A, CoordinationError: %A" pinError coordError
        ParsingError.Invalid msg |> ParsingError |> Error
    | Ok _, Ok _ ->
        // A number can't be a valid pin and coordination number at the same time. Ending up here is a bug.
        failwith "This should not have happened! There is an application error for either SwedishPersonalIdentityNumber or SwedishCoordinationNumber"

/// <summary>
/// Creates a <see cref="IdentityNumber"/> out of the individual parts.
/// </summary>
/// <param name="values">IdentityNumberValues containing all the number parts</param>
let create = createInternal >> Error.handle


let internal parseInSpecificYearInternal parseYear str =
    result {
        let! pYear = parseYear |> Year.create
        return! Parse.parseInSpecificYear createInternal pYear str
    }

/// <summary>
/// Converts the string representation of the identity number to its <see cref="IdentityNumber"/> equivalent.
/// </summary>
/// <param name="parseYear">
/// The specific year to use when checking if the person has turned / will turn 100 years old.
/// That information changes the delimiter (- or +).
///
/// For more info, see: https://www.riksdagen.se/sv/dokument-lagar/dokument/svensk-forfattningssamling/folkbokforingslag-1991481_sfs-1991-481#P18
/// </param>
/// <param name="str">A string representation of the identity number to parse.</param>
let parseInSpecificYear parseYear str =
    parseInSpecificYearInternal parseYear str
    |> Error.handle

let tryParseInSpecificYear parseYear str =
    match parseInSpecificYearInternal parseYear str with
    | Ok num -> Some num
    | Error _ -> None

let internal parseInternal str = Parse.parse createInternal str

/// <summary>
/// Converts the string representation of the identity number to its <see cref="IdentityNumber"/> equivalent.
/// </summary>
/// <param name="str">A string representation of the identity number to parse.</param>
let parse = parseInternal >> Error.handle

let tryParse str =
    match parseInternal str with
    | Ok num -> Some num
    | Error _ -> None

/// <summary>
/// Converts a IdentityNumber to its equivalent 10 digit string representation. The total length, including the separator, will be 11 chars.
/// </summary>
/// <param name="serializationYear">
/// The specific year to use when checking if the person has turned / will turn 100 years old.
/// That information changes the delimiter (- or +).
///
/// For more info, see: https://www.riksdagen.se/sv/dokument-lagar/dokument/svensk-forfattningssamling/folkbokforingslag-1991481_sfs-1991-481#P18
/// </param>
/// <param name="num">An IdentityNumber</param>
let to10DigitStringInSpecificYear serializationYear (num: IndividualIdentityNumber) =
    num
    |> StringHelpers.to10DigitStringInSpecificYear serializationYear
    |> Error.handle

/// <summary>
/// Converts a IdentityNumber to its equivalent 10 digit string representation. The total length, including the separator, will be 11 chars.
/// </summary>
/// <param name="num">An IdentityNumber</param>
let to10DigitString (num : IndividualIdentityNumber) =
    num
    |> StringHelpers.to10DigitString
    |> Error.handle

/// <summary>
/// Converts the value of the current <see cref="IdentityNumber" /> object to its equivalent 12 digit string representation.
/// Format is YYYYMMDDBBBC, for example <example>199008672397</example> or <example>191202719983</example>.
/// </summary>
/// <param name="num">An IdentityNumber</param>
let to12DigitString num =
    num
    |> StringHelpers.to12DigitString

module Hints =

    /// <summary>
    /// Date of birth for the person according to the identity number.
    /// Not always the actual date of birth due to the limited quantity of identity numbers per day.
    /// </summary>
    /// <param name="num">An IdentityNumber</param>
    let getDateOfBirthHint num = HintsHelper.getDateOfBirthHint num

    /// <summary>
    /// Get the age of the person according to the date in the identity number.
    /// Not always the actual date of birth due to the limited quantity of identity numbers per day.
    /// </summary>
    /// <param name="date">The date when to calculate the age.</param>
    /// <param name="num">An IdentityNumber</param>
    let getAgeHintOnDate date num = HintsHelper.getAgeHintOnDate date num

    /// <summary>
    /// Get the age of the person according to the date in the identity number.
    /// Not always the actual date of birth due to the limited quantity of identity numbers per day.
    /// </summary>
    /// <param name="num">An IdentityNumber</param>
    let getAgeHint num = HintsHelper.getAgeHint num

    /// <summary>
    /// Gender (juridiskt kön) in Sweden according to the last digit of the birth number in the identity number.
    /// Odd number: Male
    /// Even number: Female
    /// </summary>
    /// <param name="num">An IdentityNumber</param>
    let getGenderHint num = HintsHelper.getGenderHint num

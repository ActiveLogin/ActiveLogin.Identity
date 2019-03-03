module ActiveLogin.Identity.Swedish.FSharp.SwedishPersonalIdentityNumber

open System

/// Creates a <see cref="SwedishPersonalIdentityNumber"/> out of the individual parts.
/// <param name="values">SwedishPersonalIdentityNumberValues containing all the number parts</param>
let create (values : SwedishPersonalIdentityNumberValues) =
    result {
        let! y = values.Year |> Year.create
        let! m = values.Month |> Month.create
        let! d = values.Day |> Day.create y m
        let! s = values.BirthNumber |> BirthNumber.create
        let! c = values.Checksum |> Checksum.create y m d s
        return { SwedishPersonalIdentityNumber.Year = y
                 Month = m
                 Day = d
                 BirthNumber = s
                 Checksum = c }
    }

let private extractValues (pin : SwedishPersonalIdentityNumber) : SwedishPersonalIdentityNumberValues =
    { Year = pin.Year |> Year.value
      Month = pin.Month |> Month.value
      Day = pin.Day |> Day.value
      BirthNumber = pin.BirthNumber |> BirthNumber.value
      Checksum = pin.Checksum |> Checksum.value }

/// Converts a SwedishPersonalIdentityNumber to its equivalent 10 digit string representation. The total length, including the separator, will be 11 chars. 
/// <param name="serializationYear">
/// The specific year to use when checking if the person has turned / will turn 100 years old.
/// That information changes the delimiter (- or +).
///
/// For more info, see: https://www.riksdagen.se/sv/dokument-lagar/dokument/svensk-forfattningssamling/folkbokforingslag-1991481_sfs-1991-481#P18
/// </param>
/// <param name="pin">A SwedishPersonalIdentityNumber</param>
let to10DigitStringInSpecificYear serializationYear (pin : SwedishPersonalIdentityNumber) =
    let delimiter =
        if (serializationYear |> Year.value) - (pin.Year |> Year.value) >= 100 then "+"
        else "-"

    let vs = extractValues pin
    sprintf "%02i%02i%02i%s%03i%1i" (vs.Year % 100) vs.Month vs.Day delimiter vs.BirthNumber vs.Checksum

/// Converts a SwedishPersonalIdentityNumber to its equivalent 10 digit string representation. The total length, including the separator, will be 11 chars. 
/// <param name="pin">A SwedishPersonalIdentityNumber</param>
let to10DigitString (pin : SwedishPersonalIdentityNumber) =
    let year =
        DateTime.UtcNow.Year
        |> Year.create
        |> function
        | Ok y -> y
        | Error _ -> invalidArg "year" "DateTime.Year wasn't a year"
    to10DigitStringInSpecificYear year pin

/// Converts the value of the current <see cref="SwedishPersonalIdentityNumber" /> object to its equivalent 12 digit string representation.
/// Format is YYYYMMDDBBBC, for example <example>19908072391</example> or <example>191202119986</example>.
/// <param name="pin">A SwedishPersonalIdentityNumber</param>
let to12DigitString pin =
    let vs = extractValues pin
    sprintf "%02i%02i%02i%03i%1i" vs.Year vs.Month vs.Day vs.BirthNumber vs.Checksum

let internal toParsingError err = 
    let invalidWithMsg msg i =
        i |> sprintf "%s %i" msg |> Invalid |> ParsingError 
    match err with
    | InvalidYear y ->
        y |> invalidWithMsg "InvalidYear:"
    | InvalidMonth m ->
        m |> invalidWithMsg "Invalid month:" 
    | InvalidDay d | InvalidDayAndCoordinationDay d ->
        d |> invalidWithMsg "Invalid day:"
    | InvalidBirthNumber b ->
        b |> invalidWithMsg "Invalid birthnumber:" 
    | InvalidChecksum c ->
        c |> invalidWithMsg "Invalid checksum:"
    | ParsingError err -> ParsingError err
    | ArgumentError err -> ArgumentError err
    
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
    |> Result.mapError toParsingError

/// <summary>
/// Converts the string representation of the personal identity number to its <see cref="SwedishPersonalIdentityNumber"/> equivalent.
/// </summary>
/// <param name="str">A string representation of the Swedish personal identity number to parse.</param>
let parse str = result { let! year = DateTime.UtcNow.Year |> Year.create
                         return! parseInSpecificYear year str }

module Hints =
    open ActiveLogin.Identity.Swedish

    /// Date of birth for the person according to the personal identity number.
    /// Not always the actual date of birth due to the limited quantity of personal identity numbers per day.
    /// <param name="pin">A SwedishPersonalIdentityNumber</param>
    let getDateOfBirthHint (pin : SwedishPersonalIdentityNumber) =
        DateTime(pin.Year |> Year.value, pin.Month |> Month.value, pin.Day |> Day.value, 0, 0, 0, DateTimeKind.Utc)

    /// Get the age of the person according to the date in the personal identity number.
    /// Not always the actual date of birth due to the limited quantity of personal identity numbers per day.
    /// <param name="date">The date when to calculate the age.</param>
    /// <param name="pin">A SwedishPersonalIdentityNumber</param>
    let getAgeHintOnDate (date : DateTime) pin =
        let dateOfBirth = getDateOfBirthHint pin
        match date >= dateOfBirth with
        | true ->
            let months = 12 * (date.Year - dateOfBirth.Year) + (date.Month - dateOfBirth.Month)
            match date.Day < dateOfBirth.Day with
            | true ->
                let years = (months - 1) / 12
                years |> Some
            | false -> months / 12 |> Some
        | false -> None

    /// Get the age of the person according to the date in the personal identity number.
    /// Not always the actual date of birth due to the limited quantity of personal identity numbers per day.
    /// <param name="pin">A SwedishPersonalIdentityNumber</param>
    let getAgeHint pin = getAgeHintOnDate DateTime.UtcNow pin

    /// Gender (juridiskt kön) in Sweden according to the last digit of the birth number in the personal identity number.
    /// Odd number: Male
    /// Even number: Female
    /// <param name="pin">A SwedishPersonalIdentityNumber</param>
    let getGenderHint (pin : SwedishPersonalIdentityNumber) =
        let isBirthNumberEven = (pin.BirthNumber |> BirthNumber.value) % 2 = 0
        if isBirthNumberEven then Gender.Female
        else Gender.Male

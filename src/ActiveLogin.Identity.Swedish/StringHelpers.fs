module internal ActiveLogin.Identity.Swedish.FSharp.StringHelpers
open ActiveLogin.Identity.Swedish.FSharp
open System

let private validSerializationYear (serializationYear: Year) (pinYear: Year) =
    if serializationYear < pinYear
    then
        "SerializationYear cannot be a year before the person was born"
        |> InvalidSerializationYear
        |> Error

    elif (serializationYear |> Year.value) > ((pinYear |> Year.value) + 199)
    then
        "SerializationYear cannot be a more than 199 years after the person was born"
        |> InvalidSerializationYear
        |> Error
    else
        serializationYear |> Ok

let private parseDay day =
    match day with
    | Day d -> d.Value
    | CoordinationDay cd -> cd.Value


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
let to10DigitStringInSpecificYear serializationYear (num : IdentityNumber) =
    result {
        let! validYear = validSerializationYear serializationYear num.Year
        let delimiter =
            if (validYear |> Year.value) - (num.Year |> Year.value) >= 100 then "+"
            else "-"

        return sprintf "%02i%02i%02i%s%03i%1i"
            (num.Year.Value % 100)
            num.Month.Value
            (num.Day |> parseDay)
            delimiter
            num.BirthNumber.Value
            num.Checksum.Value
    }

/// <summary>
/// Converts a SwedishPersonalIdentityNumber to its equivalent 10 digit string representation. The total length, including the separator, will be 11 chars.
/// </summary>
/// <param name="pin">A SwedishPersonalIdentityNumber</param>
let to10DigitString (num : IdentityNumber) =
    let year =
        DateTime.UtcNow.Year
        |> Year.create
        |> function
        | Ok y -> y
        | Error _ -> invalidArg "year" "DateTime.Year wasn't a valid year"
    to10DigitStringInSpecificYear year num

/// <summary>
/// Converts the value of the current <see cref="SwedishPersonalIdentityNumber" /> object to its equivalent 12 digit string representation.
/// Format is YYYYMMDDBBBC, for example <example>19908072391</example> or <example>191202119986</example>.
/// </summary>
/// <param name="pin">A SwedishPersonalIdentityNumber</param>
let to12DigitString (num: IdentityNumber) =
    sprintf "%02i%02i%02i%03i%1i"
        num.Year.Value
        num.Month.Value
        (num.Day |> parseDay)
        num.BirthNumber.Value
        num.Checksum.Value


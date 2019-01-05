namespace ActiveLogin.Identity.Swedish

open ActiveLogin.Identity.Swedish.FSharp
open SwedishPersonalIdentityNumber
open System
open System.Runtime.InteropServices //for OutAttribute

/// <summary>
/// Represents a Swedish Personal Identity Number (Svenskt Personnummer).
/// https://en.wikipedia.org/wiki/Personal_identity_number_(Sweden)
/// https://sv.wikipedia.org/wiki/Personnummer_i_Sverige
/// </summary>
type SwedishPersonalIdentityNumber(pin : Types.SwedishPersonalIdentityNumber) =
    let identityNumber = pin
    member internal __.IdentityNumber = identityNumber

    /// <summary>
    /// The year for date of birth.
    /// </summary>
    member __.Year = identityNumber.Year |> Year.value

    /// <summary>
    /// The month for date of birth.
    /// </summary>
    member __.Month = identityNumber.Month |> Month.value

    /// <summary>
    /// The day for date of birth.
    /// </summary>
    member __.Day = identityNumber.Day |> Day.value

    /// <summary>
    /// A birth number (födelsenummer) to distinguish people born on the same day.
    /// </summary>
    member __.BirthNumber = identityNumber.BirthNumber |> BirthNumber.value

    /// <summary>
    /// A checksum (kontrollsiffra) used for validation. Last digit in the PIN.
    /// </summary>
    member __.Checksum = identityNumber.Checksum |> Checksum.value

    /// <summary>
    /// Creates an instance of <see cref="SwedishPersonalIdentityNumber"/> out of the individual parts.
    /// </summary>
    /// <param name="year">The year part.</param>
    /// <param name="month">The month part.</param>
    /// <param name="day">The day part.</param>
    /// <param name="birthNumber">The birth number part.</param>
    /// <param name="checksum">The checksum part.</param>
    /// <returns>An instance of <see cref="SwedishPersonalIdentityNumber"/> if all the paramaters are valid by themselfes and in combination.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the arguments are invalid.</exception>
    static member Create(year, month, day, birthNumber, checksum) =
        create { Year = year
                 Month = month
                 Day = day
                 BirthNumber = birthNumber
                 Checksum = checksum }
        |> tryGetResult
        |> SwedishPersonalIdentityNumber

    /// <summary>
    /// Converts the string representation of the personal identity number to its <see cref="SwedishPersonalIdentityNumber"/> equivalent.
    /// </summary>
    /// <param name="personalIdentityNumber">A string representation of the personal identity number to parse.</param>
    /// <param name="parseYear">
    /// The specific year to use when checking if the person has turned / will turn 100 years old.
    /// That information changes the delimiter (- or +).
    ///
    /// For more info, see: https://www.riksdagen.se/sv/dokument-lagar/dokument/svensk-forfattningssamling/folkbokforingslag-1991481_sfs-1991-481#P18
    /// </param>
    static member ParseInSpecificYear((personalIdentityNumber : string), parseYear : int) =
        result { let! year = parseYear |> Year.create
                 return! parseInSpecificYear year personalIdentityNumber }
        |> tryGetResult
        |> SwedishPersonalIdentityNumber

    /// <summary>
    /// Converts the string representation of the personal identity number to its <see cref="SwedishPersonalIdentityNumber"/> equivalent.
    /// </summary>
    static member Parse(personalIdentityNumber) =
        parse personalIdentityNumber
        |> tryGetResult
        |> SwedishPersonalIdentityNumber

    /// <summary>
    /// Converts the string representation of the personal identity number to its <see cref="SwedishPersonalIdentityNumber"/> equivalent  and returns a value that indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="personalIdentityNumber">A string representation of the personal identity number to parse.</param>
    /// <param name="parseYear">
    /// The specific year to use when checking if the person has turned / will turn 100 years old.
    /// That information changes the delimiter (- or +).
    ///
    /// For more info, see: https://www.riksdagen.se/sv/dokument-lagar/dokument/svensk-forfattningssamling/folkbokforingslag-1991481_sfs-1991-481#P18
    /// </param>
    /// <param name="parseResult">If valid, an instance of <see cref="SwedishPersonalIdentityNumber"/></param>
    static member TryParseInSpecificYear((personalIdentityNumber : string), (parseYear : int),
                                         [<Out>] parseResult : SwedishPersonalIdentityNumber byref) =
        let pin = result { let! year = parseYear |> Year.create
                           return! parseInSpecificYear year personalIdentityNumber }
        match pin with
        | Error _ -> false
        | Ok pin ->
            parseResult <- (pin |> SwedishPersonalIdentityNumber)
            true

    /// <summary>
    /// Converts the string representation of the personal identity number to its <see cref="SwedishPersonalIdentityNumber"/> equivalent  and returns a value that indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="personalIdentityNumber">A string representation of the personal identity number to parse.</param>
    /// <param name="parseResult">If valid, an instance of <see cref="SwedishPersonalIdentityNumber"/></param>
    static member TryParse((personalIdentityNumber : string), [<Out>] parseResult : SwedishPersonalIdentityNumber byref) =
        let pin = parse personalIdentityNumber
        match pin with
        | Error _ -> false
        | Ok pin ->
            parseResult <- (pin |> SwedishPersonalIdentityNumber)
            true

    /// <summary>
    /// Converts the value of the current <see cref="SwedishPersonalIdentityNumber" /> object to its equivalent 10 digit string representation. The total length, including the separator, will be 11 chars.
    /// Format is YYMMDDXBBBC, for example <example>990807-2391</example> or <example>120211+9986</example>.
    /// </summary>
    /// <param name="serializationYear">
    /// The specific year to use when checking if the person has turned / will turn 100 years old.
    /// That information changes the delimiter (- or +).
    ///
    /// For more info, see: https://www.riksdagen.se/sv/dokument-lagar/dokument/svensk-forfattningssamling/folkbokforingslag-1991481_sfs-1991-481#P18
    /// </param>
    member __.To10DigitStringInSpecificYear(serializationYear : int) =
        match serializationYear |> Year.create with
        | Error _ -> raise (new ArgumentOutOfRangeException("year", serializationYear, "Invalid year."))
        | Ok year -> to10DigitStringInSpecificYear year identityNumber

    /// <summary>
    /// Converts the value of the current <see cref="SwedishPersonalIdentityNumber" /> object to its equivalent short string representation.
    /// Format is YYMMDDXBBBC, for example <example>990807-2391</example> or <example>120211+9986</example>.
    /// </summary>
    member __.To10DigitString() = to10DigitString identityNumber

    /// <summary>
    /// Converts the value of the current <see cref="SwedishPersonalIdentityNumber" /> object to its equivalent 12 digit string representation.
    /// Format is YYYYMMDDBBBC, for example <example>19908072391</example> or <example>191202119986</example>.
    /// </summary>
    member __.To12DigitString() = to12DigitString identityNumber

    /// <summary>
    /// Converts the value of the current <see cref="SwedishPersonalIdentityNumber" /> object to its equivalent 12 digit string representation.
    /// Format is YYYYMMDDBBBC, for example <example>19908072391</example> or <example>191202119986</example>.
    /// </summary>
    override __.ToString() = __.To12DigitString()

    /// <summary>Returns a value indicating whether this instance is equal to a specified object.</summary>
    /// <param name="value">The object to compare to this instance.</param>
    /// <returns>true if <paramref name="value">value</paramref> is an instance of <see cref="SwedishPersonalIdentityNumber"></see> and equals the value of this instance; otherwise, false.</returns>
    override __.Equals(b) =
        match b with
        | :? SwedishPersonalIdentityNumber as pin -> identityNumber = pin.IdentityNumber
        | _ -> false

    /// <summary>Returns the hash code for this instance.</summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    override __.GetHashCode() = hash identityNumber

    static member op_Equality (left: SwedishPersonalIdentityNumber, right: SwedishPersonalIdentityNumber) =
        left.IdentityNumber = right.IdentityNumber

open System.Runtime.CompilerServices

[<Extension>]
type SwedishPersonalIdentityNumberHintExtensions() =

    /// <summary>
    /// Date of birth for the person according to the personal identity number.
    /// Not always the actual date of birth due to the limited quantity of personal identity numbers per day.
    /// </summary>
    [<Extension>]
    static member GetDateOfBirthHint(pin : SwedishPersonalIdentityNumber) = Hints.getDateOfBirthHint pin.IdentityNumber

    /// <summary>
    /// Gender (juridiskt kön) in Sweden according to the last digit of the birth number in the personal identity number.
    /// Odd number: Male
    /// Even number: Female
    /// </summary>
    [<Extension>]
    static member GetGenderHint(pin : SwedishPersonalIdentityNumber) = Hints.getGenderHint pin.IdentityNumber

    /// <summary>
    /// Get the age of the person according to the date in the personal identity number.
    /// Not always the actual date of birth due to the limited quantity of personal identity numbers per day.
    /// </summary>
    /// <param name="personalIdentityNumber"></param>
    /// <param name="date">The date when to calculate the age.</param>
    /// <returns></returns>
    [<Extension>]
    static member GetAgeHint(pin : SwedishPersonalIdentityNumber, date : DateTime) =
        Hints.getAgeHintOnDate date pin.IdentityNumber
        |> function
        | None -> invalidArg "pin" "The person is not yet born."
        | Some i -> i

    /// <summary>
    /// Get the age of the person according to the date in the personal identity number.
    /// Not always the actual date of birth due to the limited quantity of personal identity numbers per day.
    /// </summary>
    [<Extension>]
    static member GetAgeHint(pin : SwedishPersonalIdentityNumber) =
        Hints.getAgeHint pin.IdentityNumber
        |> function
        | None -> invalidArg "pin" "The person is not yet born."
        | Some i -> i

namespace ActiveLogin.Identity.Swedish

open ActiveLogin.Identity.Swedish.FSharp
open System
open System.Runtime.InteropServices //for OutAttribute


module internal SwedishPersonalIdentityNumber =
    let create (year, month, day, birthNumber, checksum) =
        result {
            let! y = year |> Year.create
            let! m = month |> Month.create
            let! d = day |> Day.create y m
            let! s = birthNumber |> BirthNumber.create
            let! c = checksum |> Checksum.create y m (DayInternal.Day d) s
            return { SwedishPersonalIdentityNumberInternal.Year = y
                     Month = m
                     Day = d
                     BirthNumber = s
                     Checksum = c }
        }

    let to10DigitStringInSpecificYear serializationYear pin =
        pin
        |> Personal
        |> StringHelpers.to10DigitStringInSpecificYear serializationYear
        |> Error.handle

    let to10DigitString pin =
        pin
        |> Personal
        |> StringHelpers.to10DigitString
        |> Error.handle

    let to12DigitString pin =
        pin
        |> Personal
        |> StringHelpers.to12DigitString

    let internal parseInSpecificYearInternal parseYear str =
        result {
            let! pYear = parseYear |> Year.create
            return! Parse.parseInSpecificYear create pYear str
        }

    let parseInSpecificYear parseYear str =
        parseInSpecificYearInternal parseYear str |> Error.handle

    let tryParseInSpecificYear parseYear str =
        match parseInSpecificYearInternal parseYear str with
        | Ok pin -> Some pin
        | Error _ -> None

    let internal parseInternal str = Parse.parse create str

    let parse = parseInternal >> Error.handle

    let tryParse str =
       match parseInternal str with
       | Ok pin -> Some pin
       | Error _ -> None

    module Hints =
        let getDateOfBirthHint pin = HintsHelper.getDateOfBirthHint (Personal pin)

        let getAgeHintOnDate date pin = HintsHelper.getAgeHintOnDate date (Personal pin)

        let getAgeHint pin = HintsHelper.getAgeHint (Personal pin)

        let getGenderHint pin = HintsHelper.getGenderHint (Personal pin)

open SwedishPersonalIdentityNumber

/// <summary>
/// Represents a Swedish Personal Identity Number (Svenskt Personnummer).
/// https://en.wikipedia.org/wiki/Personal_identity_number_(Sweden)
/// https://sv.wikipedia.org/wiki/Personnummer_i_Sverige
/// </summary>
type SwedishPersonalIdentityNumber internal(pin : SwedishPersonalIdentityNumberInternal) =
    /// <summary>
    /// Creates an instance of <see cref="SwedishPersonalIdentityNumber"/> out of the individual parts.
    /// </summary>
    /// <param name="year">The year part.</param>
    /// <param name="month">The month part.</param>
    /// <param name="day">The day part.</param>
    /// <param name="birthNumber">The birth number part.</param>
    /// <param name="checksum">The checksum part.</param>
    /// <returns>An instance of <see cref="SwedishPersonalIdentityNumber"/> if all the paramaters are valid by themselfes and in combination.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the range arguments is invalid.</exception>
    /// <exception cref="ArgumentException">Thrown when checksum is invalid.</exception>
    new(year, month, day, birthNumber, checksum) =
        let pin =
            (year, month, day, birthNumber, checksum)
            |> create
            |> Error.handle
        SwedishPersonalIdentityNumber(pin)

    member internal __.IdentityNumber = pin

    /// <summary>
    /// The year for date of birth.
    /// </summary>
    member __.Year = pin.Year.Value

    /// <summary>
    /// The month for date of birth.
    /// </summary>
    member __.Month = pin.Month.Value

    /// <summary>
    /// The day for date of birth.
    /// </summary>
    member __.Day = pin.Day.Value

    /// <summary>
    /// A birth number (födelsenummer) to distinguish people born on the same day.
    /// </summary>
    member __.BirthNumber = pin.BirthNumber.Value

    /// <summary>
    /// A checksum (kontrollsiffra) used for validation. Last digit in the PIN.
    /// </summary>
    member __.Checksum = pin.Checksum.Value

    /// <summary>
    /// Converts the string representation of the Swedish personal identity number to its <see cref="SwedishPersonalIdentityNumber"/> equivalent.
    /// </summary>
    /// <param name="s">A string representation of the Swedish personal identity number to parse.</param>
    /// <param name="parseYear">
    /// The specific year to use when checking if the person has turned / will turn 100 years old.
    /// That information changes the delimiter (- or +).
    ///
    /// For more info, see: https://www.riksdagen.se/sv/dokument-lagar/dokument/svensk-forfattningssamling/folkbokforingslag-1991481_sfs-1991-481#P18
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when string input is null.</exception>
    /// <exception cref="FormatException">Thrown when string input cannot be recognized as a valid SwedishPersonalIdentityNumber.</exception>
    static member ParseInSpecificYear((s : string), parseYear : int) =
        parseInSpecificYear parseYear s
        |> SwedishPersonalIdentityNumber

    /// <summary>
    /// Converts the string representation of the personal identity number to its <see cref="SwedishPersonalIdentityNumber"/> equivalent.
    /// </summary>
    /// <param name="s">A string representation of the Swedish personal identity number to parse.</param>
    /// <exception cref="ArgumentNullException">Thrown when string input is null.</exception>
    /// <exception cref="FormatException">Thrown when string input cannot be recognized as a valid SwedishPersonalIdentityNumber.</exception>
    static member Parse(s) =
        parse s
        |> SwedishPersonalIdentityNumber

    /// <summary>
    /// Converts the string representation of the personal identity number to its <see cref="SwedishPersonalIdentityNumber"/> equivalent  and returns a value that indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="s">A string representation of the Swedish personal identity number to parse.</param>
    /// <param name="parseYear">
    /// The specific year to use when checking if the person has turned / will turn 100 years old.
    /// That information changes the delimiter (- or +).
    ///
    /// For more info, see: https://www.riksdagen.se/sv/dokument-lagar/dokument/svensk-forfattningssamling/folkbokforingslag-1991481_sfs-1991-481#P18
    /// </param>
    /// <param name="parseResult">If valid, an instance of <see cref="SwedishPersonalIdentityNumber"/></param>
    static member TryParseInSpecificYear((s : string), (parseYear : int),
                                         [<Out>] parseResult : SwedishPersonalIdentityNumber byref) =
        let pin = tryParseInSpecificYear parseYear s
        match pin with
        | Some pin ->
            parseResult <- (pin |> SwedishPersonalIdentityNumber)
            true
        | None -> false

    /// <summary>
    /// Converts the string representation of the personal identity number to its <see cref="SwedishPersonalIdentityNumber"/> equivalent  and returns a value that indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="s">A string representation of the Swedish personal identity number to parse.</param>
    /// <param name="parseResult">If valid, an instance of <see cref="SwedishPersonalIdentityNumber"/></param>
    static member TryParse((s : string), [<Out>] parseResult : SwedishPersonalIdentityNumber byref) =
        match tryParse s with
        | Some pin ->
            parseResult <- (pin |> SwedishPersonalIdentityNumber)
            true
        | None -> false

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
        to10DigitStringInSpecificYear serializationYear pin

    /// <summary>
    /// Converts the value of the current <see cref="SwedishPersonalIdentityNumber" /> object to its equivalent short string representation.
    /// Format is YYMMDDXBBBC, for example <example>990807-2391</example> or <example>120211+9986</example>.
    /// </summary>
    member __.To10DigitString() = to10DigitString pin

    /// <summary>
    /// Converts the value of the current <see cref="SwedishPersonalIdentityNumber" /> object to its equivalent 12 digit string representation.
    /// Format is YYYYMMDDBBBC, for example <example>19908072391</example> or <example>191202119986</example>.
    /// </summary>
    member __.To12DigitString() = to12DigitString pin

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
        | :? SwedishPersonalIdentityNumber as p -> pin = p.IdentityNumber
        | _ -> false

    /// <summary>Returns the hash code for this instance.</summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    override __.GetHashCode() = hash pin

    static member op_Equality (left: SwedishPersonalIdentityNumber, right: SwedishPersonalIdentityNumber) =
        match box left, box right with
        | (null, null) -> true
        | (null, _) | (_, null) -> false
        | _ -> left.IdentityNumber = right.IdentityNumber

    static member op_Inequality (left: SwedishPersonalIdentityNumber, right: SwedishPersonalIdentityNumber) =
        match box left, box right with
        | (null, null) -> false
        | (null, _) | (_, null) -> true
        | _ -> left.IdentityNumber <> right.IdentityNumber

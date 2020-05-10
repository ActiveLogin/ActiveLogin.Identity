namespace ActiveLogin.Identity.Swedish

open ActiveLogin.Identity.Swedish
open ActiveLogin.Identity.Swedish.Shared
open System
open System.Runtime.InteropServices //for OutAttribute


module internal PersonalIdentityNumber =
    let create (year, month, day, birthNumber, checksum) =
            let y = year |> Year.create
            let m = month |> Month.create
            let d = day |> Day.create y m
            let birthNum = birthNumber |> BirthNumber.create
            let c = checksum |> Checksum.create y (Choice1Of2 m) (Choice1Of2 d) (Choice1Of2 birthNum)
            { PersonalIdentityNumberInternal.Year = y
              Month = m
              Day = d
              BirthNumber = birthNum
              Checksum = c }

    let to10DigitStringInSpecificYear serializationYear (pin: PersonalIdentityNumberInternal) =
            let validYear = validSerializationYear serializationYear pin.Year
            let delimiter =
                if validYear - (pin.Year.Value) >= 100 then "+"
                else "-"

            sprintf "%02i%02i%02i%s%03i%1i"
                (pin.Year.Value % 100)
                pin.Month.Value
                pin.Day.Value
                delimiter
                pin.BirthNumber.Value
                pin.Checksum.Value

    let to10DigitString (pin : PersonalIdentityNumberInternal) =
        let year = DateTime.UtcNow.Year |> Year.create
        to10DigitStringInSpecificYear year.Value pin

    let to12DigitString (pin: PersonalIdentityNumberInternal) =
        sprintf "%02i%02i%02i%03i%1i"
            pin.Year.Value
            pin.Month.Value
            pin.Day.Value
            pin.BirthNumber.Value
            pin.Checksum.Value

    let internal parseInSpecificYearInternal parseYear str =
        let pYear = parseYear |> Year.create
        Parse.parseInSpecificYear create pYear str

    let parseInSpecificYear parseYear str =
        parseInSpecificYearInternal parseYear str

    let tryParseInSpecificYear parseYear str =
        try
            parseInSpecificYearInternal parseYear str
            |> Some
        with
            exn -> None

    let parse str = Parse.parse create str


    let tryParse str =
       try
           parse str
           |> Some
       with
           exn -> None


open PersonalIdentityNumber

/// <summary>
/// Represents a Swedish Personal Identity Number (Svenskt Personnummer).
/// https://en.wikipedia.org/wiki/Personal_identity_number_(Sweden)
/// https://sv.wikipedia.org/wiki/Personnummer_i_Sverige
/// </summary>
type PersonalIdentityNumber internal(pin : PersonalIdentityNumberInternal) =
    /// <summary>
    /// Creates an instance of <see cref="SwedishPersonalIdentityNumber"/> out of the individual parts.
    /// </summary>
    /// <param name="year">The year part.</param>
    /// <param name="month">The month part.</param>
    /// <param name="day">The day part.</param>
    /// <param name="birthNumber">The birth number part.</param>
    /// <param name="checksum">The checksum part.</param>
    /// <returns>An instance of <see cref="SwedishPersonalIdentityNumber"/> if all the parameters are valid by themselves and in combination.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the range arguments is invalid.</exception>
    /// <exception cref="ArgumentException">Thrown when checksum is invalid.</exception>
    new(year, month, day, birthNumber, checksum) =
        let pin =
            (year, month, day, birthNumber, checksum)
            |> create
        PersonalIdentityNumber(pin)

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
    /// Converts the string representation of the Swedish personal identity number to its <see cref="PersonalIdentityNumber"/> equivalent.
    /// </summary>
    /// <param name="s">A string representation of the Swedish personal identity number to parse.</param>
    /// <param name="parseYear">
    /// The specific year to use when checking if the person has turned / will turn 100 years old.
    /// That information changes the delimiter (- or +).
    ///
    /// For more info, see: https://www.riksdagen.se/sv/dokument-lagar/dokument/svensk-forfattningssamling/folkbokforingslag-1991481_sfs-1991-481#P18
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when string input is null.</exception>
    /// <exception cref="FormatException">Thrown when string input cannot be recognized as a valid PersonalIdentityNumber.</exception>
    static member ParseInSpecificYear((s : string), parseYear : int) =
        parseInSpecificYear parseYear s
        |> PersonalIdentityNumber

    /// <summary>
    /// Converts the string representation of the personal identity number to its <see cref="PersonalIdentityNumber"/> equivalent.
    /// </summary>
    /// <param name="s">A string representation of the Swedish personal identity number to parse.</param>
    /// <exception cref="ArgumentNullException">Thrown when string input is null.</exception>
    /// <exception cref="FormatException">Thrown when string input cannot be recognized as a valid PersonalIdentityNumber.</exception>
    static member Parse(s) =
        parse s
        |> PersonalIdentityNumber

    /// <summary>
    /// Converts the string representation of the personal identity number to its <see cref="PersonalIdentityNumber"/> equivalent  and returns a value that indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="s">A string representation of the Swedish personal identity number to parse.</param>
    /// <param name="parseYear">
    /// The specific year to use when checking if the person has turned / will turn 100 years old.
    /// That information changes the delimiter (- or +).
    ///
    /// For more info, see: https://www.riksdagen.se/sv/dokument-lagar/dokument/svensk-forfattningssamling/folkbokforingslag-1991481_sfs-1991-481#P18
    /// </param>
    /// <param name="parseResult">If valid, an instance of <see cref="PersonalIdentityNumber"/></param>
    static member TryParseInSpecificYear((s : string), (parseYear : int),
                                         [<Out>] parseResult : PersonalIdentityNumber byref) =
        let pin = tryParseInSpecificYear parseYear s
        match pin with
        | Some pin ->
            parseResult <- PersonalIdentityNumber pin
            true
        | None -> false

    /// <summary>
    /// Converts the string representation of the personal identity number to its <see cref="PersonalIdentityNumber"/> equivalent  and returns a value that indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="s">A string representation of the Swedish personal identity number to parse.</param>
    /// <param name="parseResult">If valid, an instance of <see cref="PersonalIdentityNumber"/></param>
    static member TryParse((s : string), [<Out>] parseResult : PersonalIdentityNumber byref) =
        match tryParse s with
        | Some pin ->
            parseResult <- PersonalIdentityNumber pin
            true
        | None -> false

    /// <summary>
    /// Converts the value of the current <see cref="PersonalIdentityNumber" /> object to its equivalent 10 digit string representation. The total length, including the separator, will be 11 chars.
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
    /// Converts the value of the current <see cref="PersonalIdentityNumber" /> object to its equivalent short string representation.
    /// Format is YYMMDDXBBBC, for example <example>990807-2391</example> or <example>120211+9986</example>.
    /// </summary>
    member __.To10DigitString() = to10DigitString pin

    /// <summary>
    /// Converts the value of the current <see cref="PersonalIdentityNumber" /> object to its equivalent 12 digit string representation.
    /// Format is YYYYMMDDBBBC, for example <example>19908072391</example> or <example>191202119986</example>.
    /// </summary>
    member __.To12DigitString() = to12DigitString pin

    /// <summary>
    /// Converts the value of the current <see cref="PersonalIdentityNumber" /> object to its equivalent 12 digit string representation.
    /// Format is YYYYMMDDBBBC, for example <example>19908072391</example> or <example>191202119986</example>.
    /// </summary>
    override __.ToString() = __.To12DigitString()

    /// <summary>Returns a value indicating whether this instance is equal to a specified object.</summary>
    /// <param name="value">The object to compare to this instance.</param>
    /// <returns>true if <paramref name="value">value</paramref> is an instance of <see cref="PersonalIdentityNumber"></see> and equals the value of this instance; otherwise, false.</returns>
    override __.Equals(b) =
        match b with
        | :? PersonalIdentityNumber as p -> pin = p.IdentityNumber
        | _ -> false

    /// <summary>Returns the hash code for this instance.</summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    override __.GetHashCode() = hash pin

    static member op_Equality (left: PersonalIdentityNumber, right: PersonalIdentityNumber) =
        match box left, box right with
        | (null, null) -> true
        | (null, _) | (_, null) -> false
        | _ -> left.IdentityNumber = right.IdentityNumber

    static member op_Inequality (left: PersonalIdentityNumber, right: PersonalIdentityNumber) =
        match box left, box right with
        | (null, null) -> false
        | (null, _) | (_, null) -> true
        | _ -> left.IdentityNumber <> right.IdentityNumber

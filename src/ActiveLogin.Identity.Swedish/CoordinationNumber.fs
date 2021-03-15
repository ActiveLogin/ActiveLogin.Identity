namespace ActiveLogin.Identity.Swedish

open ActiveLogin.Identity.Swedish
open System
open System.Runtime.InteropServices //for OutAttribute
open ActiveLogin.Identity.Swedish.Shared


module internal CoordinationNumber =
    let internal create (year, month, day, individualNumber, checksum) =
        let y = year |> Year.create
        let m = month |> CoordinationMonth.create
        let d = day |> CoordinationDay.create y m
        let s = individualNumber |> IndividualNumber.create
        let c = checksum |> Checksum.create y (Choice2Of2 m) (Choice2Of2 d) (Choice2Of2 s)
        { Year = y
          CoordinationMonth = m
          CoordinationDay = d
          IndividualNumber = s
          Checksum = c }

    let to10DigitStringInSpecificYear serializationYear (num: CoordinationNumberInternal) =
        let validYear = validSerializationYear serializationYear num.Year
        let delimiter =
            if validYear - (num.Year.Value) >= 100 then "+"
            else "-"

        sprintf "%02i%02i%02i%s%03i%1i"
            (num.Year.Value % 100)
            num.CoordinationMonth.Value
            num.CoordinationDay.Value
            delimiter
            num.IndividualNumber.Value
            num.Checksum.Value

    let to10DigitString (pin : CoordinationNumberInternal) =
        let year =
            DateTime.UtcNow.Year
            |> Year.create
        to10DigitStringInSpecificYear year.Value pin

    let to12DigitString pin =
        sprintf "%02i%02i%02i%03i%1i"
            pin.Year.Value
            pin.CoordinationMonth.Value
            pin.CoordinationDay.Value
            pin.IndividualNumber.Value
            pin.Checksum.Value

    let internal parseInSpecificYearInternal strictMode parseYear str =
        let pYear = Year.create parseYear
        Parse.parseInSpecificYear create strictMode pYear str

    let parseInSpecificYear strictMode parseYear str =
        parseInSpecificYearInternal strictMode parseYear str

    let tryParseInSpecificYear strictMode parseYear str =
        try
            parseInSpecificYearInternal strictMode parseYear str
            |> Some
        with
            exn -> None

    let parse strictMode str = Parse.parse create strictMode str

    let tryParse strictMode str =
        try
            parse strictMode str |> Some
        with
            exn -> None

open CoordinationNumber


/// <summary>
/// Represents a Swedish Coordination Number (Samordningsnummer).
/// https://en.wikipedia.org/wiki/Personal_identity_number_(Sweden)
/// https://sv.wikipedia.org/wiki/Personnummer_i_Sverige
/// </summary>
type CoordinationNumber internal(num : CoordinationNumberInternal) =

    /// <summary>
    /// Creates an instance of <see cref="CoordinationNumber"/> out of the individual parts.
    /// </summary>
    /// <param name="year">The year part.</param>
    /// <param name="month">The month part.</param>
    /// <param name="day">The day part.</param>
    /// <param name="birthNumber">The birth number part.</param>
    /// <param name="checksum">The checksum part.</param>
    /// <returns>An instance of <see cref="CoordinationNumber"/> if all the parameters are valid by themselves and in combination.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the range arguments is invalid.</exception>
    /// <exception cref="ArgumentException">Thrown when checksum is invalid.</exception>
    new(year, month, day, birthNumber, checksum) =
        let idNum =
            (year, month, day, birthNumber, checksum)
            |> create

        CoordinationNumber(idNum)

    member internal __.IdentityNumber = num

    /// <summary>
    /// The year for date of birth.
    /// </summary>
    member __.Year = num.Year.Value

    /// <summary>
    /// The month for date of birth. Can be 0 if month is unknown.
    /// </summary>
    member __.Month = num.CoordinationMonth.Value

    /// <summary>
    /// The coordination day. This is defined as the day for date of birth + 60.
    /// </summary>
    member __.Day = num.CoordinationDay.Value

    /// <summary>
    /// The real day of date of birth. This is the coordination day - 60. Since 60 is a valid coordination day, the "RealDay" can be 0, when the day of date of birth is unknown.
    /// </summary>
    member __.RealDay = num.CoordinationDay.RealDay

    /// <summary>
    /// Converts the string representation of the Swedish coordination number to its <see cref="CoordinationNumber"/> equivalent.
    /// </summary>
    /// <param name="s">A string representation of the Swedish coordination number to parse.</param>
    /// <param name="parseYear">
    /// The specific year to use when checking if the person has turned / will turn 100 years old.
    /// That information changes the delimiter (- or +).
    ///
    /// For more info, see: https://www.riksdagen.se/sv/dokument-lagar/dokument/svensk-forfattningssamling/folkbokforingslag-1991481_sfs-1991-481#P18
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when string input is null.</exception>
    /// <exception cref="FormatException">Thrown when string input cannot be recognized as a valid CoordinationNumber.</exception>
    static member ParseInSpecificYear((s : string), parseYear : int) =
        parseInSpecificYear StrictModeInternal.TenOrTwelveDigits parseYear s
        |> CoordinationNumber

    /// <summary>
    /// Converts the string representation of the Swedish coordination number to its <see cref="CoordinationNumber"/> equivalent.
    /// </summary>
    /// <param name="s">A string representation of the Swedish coordination number to parse.</param>
    /// <param name="parseYear">
    /// <param name="strictMode">cref="StrictMode" enum indicating how strict to parse the identity number</param>
    /// The specific year to use when checking if the person has turned / will turn 100 years old.
    /// That information changes the delimiter (- or +).
    ///
    /// For more info, see: https://www.riksdagen.se/sv/dokument-lagar/dokument/svensk-forfattningssamling/folkbokforingslag-1991481_sfs-1991-481#P18
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when string input is null.</exception>
    /// <exception cref="ArgumentException">Thrown when StrictMode is invalid.</exception>
    /// <exception cref="FormatException">Thrown when string input cannot be recognized as a valid CoordinationNumber.</exception>
    static member ParseInSpecificYear(s, parseYear : int, strictMode: StrictMode) : CoordinationNumber =
        parseInSpecificYear (StrictModeInternal.Create(strictMode)) parseYear s
        |> CoordinationNumber

    /// <summary>
    /// Converts the string representation of the Swedish coordination number to its <see cref="CoordinationNumber"/> equivalent.
    /// </summary>
    /// <param name="s">A string representation of the Swedish coordination number to parse.</param>
    /// <exception cref="ArgumentNullException">Thrown when string input is null.</exception>
    /// <exception cref="FormatException">Thrown when string input cannot be recognized as a valid CoordinationNumber.</exception>
    static member Parse(s) =
        parse StrictModeInternal.TenOrTwelveDigits s
        |> CoordinationNumber

    /// <summary>
    /// Converts the string representation of the Swedish coordination number to its <see cref="CoordinationNumber"/> equivalent.
    /// </summary>
    /// <param name="s">A string representation of the Swedish coordination number to parse.</param>
    /// <param name="strictMode">cref="StrictMode" enum indicating how strict to parse the identity number</param>
    /// <exception cref="ArgumentNullException">Thrown when string input is null.</exception>
    /// <exception cref="ArgumentException">Thrown when StrictMode is invalid.</exception>
    /// <exception cref="FormatException">Thrown when string input cannot be recognized as a valid CoordinationNumber.</exception>
    static member Parse(s, strictMode: StrictMode) : CoordinationNumber =
        parse (StrictModeInternal.Create(strictMode)) s
        |> CoordinationNumber

    /// <summary>
    /// Converts the string representation of the coordination number to its <see cref="CoordinationNumber"/>
    /// equivalent and returns a value that indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="s">A string representation of the Swedish coordination number to parse.</param>
    /// <param name="parseYear">
    /// The specific year to use when checking if the person has turned / will turn 100 years old.
    /// That information changes the delimiter (- or +).
    ///
    /// For more info, see: https://www.riksdagen.se/sv/dokument-lagar/dokument/svensk-forfattningssamling/folkbokforingslag-1991481_sfs-1991-481#P18
    /// </param>
    /// <param name="parseResult">If valid, an instance of <see cref="CoordinationNumber"/></param>
    static member TryParseInSpecificYear((s : string), (parseYear : int),
                                         [<Out>] parseResult : CoordinationNumber byref) =
        match tryParseInSpecificYear StrictModeInternal.TenOrTwelveDigits parseYear s with
        | Some num ->
            parseResult <- (num |> CoordinationNumber)
            true
        | None -> false

    /// <summary>
    /// Converts the string representation of the coordination number to its <see cref="CoordinationNumber"/>
    /// equivalent and returns a value that indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="s">A string representation of the Swedish coordination number to parse.</param>
    /// <param name="parseYear">
    /// <param name="strictMode">cref="StrictMode" enum indicating how strict to parse the identity number</param>
    /// The specific year to use when checking if the person has turned / will turn 100 years old.
    /// That information changes the delimiter (- or +).
    ///
    /// For more info, see: https://www.riksdagen.se/sv/dokument-lagar/dokument/svensk-forfattningssamling/folkbokforingslag-1991481_sfs-1991-481#P18
    /// </param>
    /// <param name="parseResult">If valid, an instance of <see cref="CoordinationNumber"/></param>
    /// <exception cref="ArgumentException">Thrown when StrictMode is invalid.</exception>
    static member TryParseInSpecificYear((s : string), (parseYear : int), strictMode: StrictMode,
                                         [<Out>] parseResult : CoordinationNumber byref) =
        let pin = tryParseInSpecificYear (StrictModeInternal.Create(strictMode)) parseYear s
        match pin with
        | Some pin ->
            parseResult <- CoordinationNumber pin
            true
        | None -> false

    /// <summary>
    /// Converts the string representation of the coordination number to its <see cref="CoordinationNumber"/>
    /// equivalent and returns a value that indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="s">A string representation of the Swedish coordination number to parse.</param>
    /// <param name="strictMode">cref="StrictMode" enum indicating how strict to parse the identity number</param>
    /// <param name="parseResult">If valid, an instance of <see cref="CoordinationNumber"/></param>
    /// <exception cref="ArgumentException">Thrown when StrictMode is invalid.</exception>
    static member TryParse((s : string), strictMode: StrictMode, [<Out>] parseResult : CoordinationNumber byref) =
        match tryParse (StrictModeInternal.Create(strictMode)) s with
        | Some pin ->
            parseResult <- CoordinationNumber pin
            true
        | None -> false

    /// <summary>
    /// Converts the string representation of the coordination number to its <see cref="CoordinationNumber"/>
    /// equivalent and returns a value that indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="s">A string representation of the Swedish coordination number to parse.</param>
    /// <param name="parseResult">If valid, an instance of <see cref="CoordinationNumber"/></param>
    static member TryParse((s : string), [<Out>] parseResult : CoordinationNumber byref) =
        match tryParse StrictModeInternal.TenOrTwelveDigits s with
        | Some num ->
            parseResult <- (num |> CoordinationNumber)
            true
        | None -> false

    /// <summary>
    /// Converts the value of the current <see cref="CoordinationNumber" /> object to its equivalent 10 digit string representation. The total length, including the separator, will be 11 chars.
    /// Format is YYMMDDXBBBC, for example <example>990807-2391</example> or <example>120211+9986</example>.
    /// </summary>
    /// <param name="serializationYear">
    /// The specific year to use when checking if the person has turned / will turn 100 years old.
    /// That information changes the delimiter (- or +).
    ///
    /// For more info, see: https://www.riksdagen.se/sv/dokument-lagar/dokument/svensk-forfattningssamling/folkbokforingslag-1991481_sfs-1991-481#P18
    /// </param>
    member __.To10DigitStringInSpecificYear(serializationYear : int) =
        to10DigitStringInSpecificYear serializationYear num

    /// <summary>
    /// Converts the value of the current <see cref="CoordinationNumber" /> object to its equivalent short string representation.
    /// Format is YYMMDDXBBBC, for example <example>990807-2391</example> or <example>120211+9986</example>.
    /// </summary>
    member __.To10DigitString() = to10DigitString num

    /// <summary>
    /// Converts the value of the current <see cref="CoordinationNumber" /> object to its equivalent 12 digit string representation.
    /// Format is YYYYMMDDBBBC, for example <example>19908072391</example> or <example>191202119986</example>.
    /// </summary>
    member __.To12DigitString() = to12DigitString num

    /// <summary>
    /// Converts the value of the current <see cref="CoordinationNumber" /> object to its equivalent 12 digit string representation.
    /// Format is YYYYMMDDBBBC, for example <example>19908072391</example> or <example>191202119986</example>.
    /// </summary>
    override __.ToString() = __.To12DigitString()

    /// <summary>Returns a value indicating whether this instance is equal to a specified object.</summary>
    /// <param name="b">The object to compare to this instance.</param>
    /// <returns>true if <paramref name="b">value</paramref> is an instance of <see cref="CoordinationNumber"></see> and equals the value of this instance; otherwise, false.</returns>
    override __.Equals(b) =
        match b with
        | :? CoordinationNumber as n -> num = n.IdentityNumber
        | _ -> false

    /// <summary>Returns the hash code for this instance.</summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    override __.GetHashCode() = hash num

    static member op_Equality (left: CoordinationNumber, right: CoordinationNumber) =
        match box left, box right with
        | (null, null) -> true
        | (null, _) | (_, null) -> false
        | _ -> left.IdentityNumber = right.IdentityNumber

    static member op_Inequality (left: CoordinationNumber, right: CoordinationNumber) =
        match box left, box right with
        | (null, null) -> false
        | (null, _) | (_, null) -> true
        | _ -> left.IdentityNumber <> right.IdentityNumber

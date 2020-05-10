namespace ActiveLogin.Identity.Swedish

open System
open System.Runtime.InteropServices //for OutAttribute


module internal IndividualIdentityNumber =
    let internal create values =
        try
            PersonalIdentityNumber.create values |> Personal
        with
            pinException ->
                try
                    CoordinationNumber.create values |> Coordination
                with
                    coordnumException ->
                        let msg = sprintf "Not a valid pin or coordination number. PinError: %s, CoordinationError: %s" pinException.Message coordnumException.Message
                        ArgumentException(sprintf "String was not recognized as a valid IndividualIdentityNumber. %s" msg) |> raise


    let internal parseInSpecificYearInternal parseYear str =
        let pYear = parseYear |> Year.create
        Parse.parseInSpecificYear create pYear str

    let parseInSpecificYear parseYear str =
        parseInSpecificYearInternal parseYear str

    let tryParseInSpecificYear parseYear str =
        try
            parseInSpecificYearInternal parseYear str |> Some
        with
            exn -> None

    let parse str = Parse.parse create str

    let tryParse str =
        try
            parse str |> Some
        with
            exn -> None

    let to10DigitStringInSpecificYear serializationYear (num: IndividualIdentityNumberInternal) =
        match num with
        | Personal pin ->
            pin |> PersonalIdentityNumber.to10DigitStringInSpecificYear serializationYear
        | Coordination num ->
            num |> CoordinationNumber.to10DigitStringInSpecificYear serializationYear

    let to10DigitString (num : IndividualIdentityNumberInternal) =
        match num with
        | Personal pin ->
            pin |> PersonalIdentityNumber.to10DigitString
        | Coordination num ->
            num |> CoordinationNumber.to10DigitString

    let to12DigitString num =
        match num with
        | Personal pin ->
            pin |> PersonalIdentityNumber.to12DigitString
        | Coordination num ->
            num |> CoordinationNumber.to12DigitString

open IndividualIdentityNumber

/// <summary>
/// Represents a Swedish Identity Number.
/// https://en.wikipedia.org/wiki/Personal_identity_number_(Sweden)
/// https://sv.wikipedia.org/wiki/Personnummer_i_Sverige
/// </summary>
type IndividualIdentityNumber private(num: IndividualIdentityNumberInternal) =

    /// <summary>
    /// Creates an instance of a <see cref="IdentityNumber"/> out of the individual parts.
    /// </summary>
    /// <param name="year">The year part.</param>
    /// <param name="month">The month part.</param>
    /// <param name="day">The day part.</param>
    /// <param name="birthNumber">The birth number part.</param>
    /// <param name="checksum">The checksum part.</param>
    /// <returns>An instance of <see cref="IdentityNumber"/> if all the parameters are valid by themselves and in combination.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the range arguments is invalid.</exception>
    /// <exception cref="ArgumentException">Thrown when checksum is invalid.</exception>
    new(year, month, day, birthNumber, checksum) =
        let idNum = (year, month, day, birthNumber, checksum) |> create
        IndividualIdentityNumber(idNum)

    member this.PersonalIdentityNumber =
        match num with
        | Personal pin -> pin |> PersonalIdentityNumber
        | _ -> Unchecked.defaultof<PersonalIdentityNumber>

    member this.CoordinationNumber =
        match num with
        | Coordination num -> num |> CoordinationNumber
        | _ -> Unchecked.defaultof<CoordinationNumber>

    /// <summary>Returns a value indicating whether this instance is a PersonalIdentityNumber.</summary>
    member __.IsPersonalIdentityNumber = num.IsPersonalIdentityNumber

    /// <summary>Returns a value indicating whether this instance is a CoordinationNumber.</summary>
    member __.IsCoordinationNumber = num.IsCoordinationNumber

    /// <summary>
    /// Converts the string representation of the Swedish identity number to its <see cref="IdentityNumber"/> equivalent.
    /// </summary>
    /// <param name="s">A string representation of the Swedish identity number to parse.</param>
    /// <param name="parseYear">
    /// The specific year to use when checking if the person has turned / will turn 100 years old.
    /// That information changes the delimiter (- or +).
    ///
    /// For more info, see: https://www.riksdagen.se/sv/dokument-lagar/dokument/svensk-forfattningssamling/folkbokforingslag-1991481_sfs-1991-481#P18
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when string input is null.</exception>
    /// <exception cref="FormatException">Thrown when string input cannot be recognized as a valid IdentityNumber.</exception>
    static member ParseInSpecificYear((s : string), parseYear : int) =
        parseInSpecificYear parseYear s
        |> IndividualIdentityNumber

    member internal __.IdentityNumber = num

    /// <summary>
    /// Converts the string representation of the Swedish coordination number to its <see cref="IdentityNumber"/> equivalent.
    /// </summary>
    /// <param name="s">A string representation of the Swedish coordination number to parse.</param>
    /// <exception cref="ArgumentNullException">Thrown when string input is null.</exception>
    /// <exception cref="FormatException">Thrown when string input cannot be recognized as a valid IdentityNumber.</exception>
    static member Parse(s) =
        parse s
        |> IndividualIdentityNumber

    /// <summary>
    /// Converts the string representation of the coordination number to its <see cref="IdentityNumber"/>
    /// equivalent and returns a value that indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="s">A string representation of the Swedish coordination number to parse.</param>
    /// <param name="parseYear">
    /// The specific year to use when checking if the person has turned / will turn 100 years old.
    /// That information changes the delimiter (- or +).
    ///
    /// For more info, see: https://www.riksdagen.se/sv/dokument-lagar/dokument/svensk-forfattningssamling/folkbokforingslag-1991481_sfs-1991-481#P18
    /// </param>
    /// <param name="parseResult">If valid, an instance of <see cref="IdentityNumber"/></param>
    static member TryParseInSpecificYear((s : string), (parseYear : int),
                                         [<Out>] parseResult : IndividualIdentityNumber byref) =
        match tryParseInSpecificYear parseYear s with
        | Some num ->
            parseResult <- (num |> IndividualIdentityNumber)
            true
        | None -> false

    /// <summary>
    /// Converts the string representation of the coordination number to its <see cref="IdentityNumber"/>
    /// equivalent and returns a value that indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="s">A string representation of the Swedish coordination number to parse.</param>
    /// <param name="parseResult">If valid, an instance of <see cref="IdentityNumber"/></param>
    static member TryParse((s : string), [<Out>] parseResult : IndividualIdentityNumber byref) =
        match tryParse s with
        | Some num ->
            parseResult <- (num |> IndividualIdentityNumber)
            true
        | None -> false

    /// <summary>
    /// Creates an instance of a <see cref="IdentityNumber"/> out of a swedish personal identity number.
    /// </summary>
    /// <param name="pin">The PersonalIdentityNumber.</param>
    /// <returns>An instance of <see cref="IdentityNumber"/></returns>
    static member FromPersonalIdentityNumber(pin: PersonalIdentityNumber) =
        IndividualIdentityNumber(Personal pin.IdentityNumber)

    /// <summary>
    /// Creates an instance of a <see cref="IdentityNumber"/> out of a swedish coordination number.
    /// </summary>
    /// <param name="pin">The CoordinationNumber.</param>
    /// <returns>An instance of <see cref="IdentityNumber"/></returns>
    static member FromCoordinationNumber(num: CoordinationNumber) =
        IndividualIdentityNumber(Coordination num.IdentityNumber)

    /// <summary>
    /// Converts the value of the current <see cref="IdentityNumber" /> object to its equivalent 10 digit string representation. The total length, including the separator, will be 11 chars.
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
    /// Converts the value of the current <see cref="IdentityNumber" /> object to its equivalent short string representation.
    /// Format is YYMMDDXBBBC, for example <example>990807-2391</example> or <example>120211+9986</example>.
    /// </summary>
    member __.To10DigitString() = to10DigitString num

    /// <summary>
    /// Converts the value of the current <see cref="IdentityNumber" /> object to its equivalent 12 digit string representation.
    /// Format is YYYYMMDDBBBC, for example <example>19908072391</example> or <example>191202119986</example>.
    /// </summary>
    member __.To12DigitString() = to12DigitString num

    /// <summary>
    /// Converts the value of the current <see cref="IdentityNumber" /> object to its equivalent 12 digit string representation.
    /// Format is YYYYMMDDBBBC, for example <example>19908072391</example> or <example>191202119986</example>.
    /// </summary>
    override __.ToString() = __.To12DigitString()

    /// <summary>Returns a value indicating whether this instance is equal to a specified object.</summary>
    /// <param name="value">The object to compare to this instance.</param>
    /// <returns>true if <paramref name="value">value</paramref> is an instance of <see cref="IdentityNumber"></see> and equals the value of this instance; otherwise, false.</returns>
    override __.Equals(b) =
        match b with
        | :? IndividualIdentityNumber as n -> num = n.IdentityNumber
        | _ -> false

    /// <summary>Returns the hash code for this instance.</summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    override __.GetHashCode() = hash num

    static member op_Equality (left: IndividualIdentityNumber, right: IndividualIdentityNumber) =
        match box left, box right with
        | (null, null) -> true
        | (null, _) | (_, null) -> false
        | _ -> left.IdentityNumber = right.IdentityNumber

    static member op_Inequality (left: IndividualIdentityNumber, right: IndividualIdentityNumber) =
        match box left, box right with
        | (null, null) -> false
        | (null, _) | (_, null) -> true
        | _ -> left.IdentityNumber <> right.IdentityNumber

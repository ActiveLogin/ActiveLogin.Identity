namespace ActiveLogin.Identity.Swedish
open System
open FSharp.ActiveLogin.Identity.Swedish
open SwedishPersonalIdentityNumber
open System.Runtime.InteropServices //for OutAttribute


type SwedishPersonalIdentityNumber (pin: Types.SwedishPersonalIdentityNumber) =
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
    /// A birth number to distinguish people born on the same day. 
    /// </summary>
    member __.BirthNumber = identityNumber.BirthNumber |> BirthNumber.value

    /// <summary>
    /// A checksum (last digit in personal identity number) used for validation.
    /// </summary>
    member __.Checksum = identityNumber.Checksum |> Checksum.value


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
        SwedishPersonalIdentityNumber.create year month day birthNumber checksum 
        |> tryGetResult
        |> SwedishPersonalIdentityNumber

    /// <summary>
    /// Converts the string representation of the personal identity number to its <see cref="SwedishPersonalIdentityNumber"/> equivalent.
    /// </summary>
    /// <param name="personalIdentityNumber">A string representation of the personal identity number to parse.</param>
    /// <param name="date">The date to decide wheter the person is older than 100 years. That decides the delimiter (- or +).</param>
    static member Parse((personalIdentityNumber:string), date: DateTime) =
        match date.Year |> Year.create with
        | Error _ -> raise (new ArgumentOutOfRangeException("year", date.Year, "Invalid year."))
        | Ok currentYear ->
            SwedishPersonalIdentityNumber.parse currentYear personalIdentityNumber
            |> tryGetResult
            |> SwedishPersonalIdentityNumber

    /// <summary>
    /// Converts the string representation of the personal identity number to its <see cref="SwedishPersonalIdentityNumber"/> equivalent.
    /// </summary>
    /// <param name="personalIdentityNumber">A string representation of the personal identity number to parse.</param>
    static member Parse(personalIdentityNumber) = SwedishPersonalIdentityNumber.Parse(personalIdentityNumber, DateTime.UtcNow)

    /// <summary>
    /// Converts the string representation of the personal identity number to its <see cref="SwedishPersonalIdentityNumber"/> equivalent  and returns a value that indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="personalIdentityNumber">A string representation of the personal identity number to parse.</param>
    /// <param name="date">The date to decide whether the person is has turned / will turn 100 years old that year. That decides the delimiter (- or +).</param>
    /// <param name="result">If valid, an instance of <see cref="SwedishPersonalIdentityNumber"/></param>
    static member TryParse((personalIdentityNumber:string), (date:DateTime), [<Out>] result : SwedishPersonalIdentityNumber byref) =
        match date.Year |> Year.create with
        | Error _ -> raise (new ArgumentOutOfRangeException("year", date.Year, "Invalid year."))
        | Ok currentYear ->
            match SwedishPersonalIdentityNumber.parse currentYear personalIdentityNumber with
            | Ok pin -> 
                result <- (pin |> SwedishPersonalIdentityNumber)
                true
            | Error _ -> false

    /// <summary>
    /// Converts the string representation of the personal identity number to its <see cref="SwedishPersonalIdentityNumber"/> equivalent  and returns a value that indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="personalIdentityNumber">A string representation of the personal identity number to parse.</param>
    /// <param name="result">If valid, an instance of <see cref="SwedishPersonalIdentityNumber"/></param>
    static member TryParse((personalIdentityNumber:string), [<Out>] result : SwedishPersonalIdentityNumber byref) = 
        SwedishPersonalIdentityNumber.TryParse(personalIdentityNumber, DateTime.UtcNow, &result)
    
    /// <summary>
    /// Converts the value of the current <see cref="SwedishPersonalIdentityNumber" /> object to its equivalent short string representation.
    /// Format is YYMMDDXSSSC, for example <example>990807-2391</example> or <example>120211+9986</example>.
    /// </summary>
    /// <param name="date">The date to decide wheter the person is older than 100 years. That decides the delimiter (- or +).</param>
    member __.To10DigitString(date: DateTime) =
        match date.Year |> Year.create with
        | Error _ -> raise (new ArgumentOutOfRangeException("year", date.Year, "Invalid year."))
        | Ok currentYear ->
            to10DigitString currentYear identityNumber

    /// <summary>
    /// Converts the value of the current <see cref="SwedishPersonalIdentityNumber" /> object to its equivalent short string representation.
    /// Format is YYMMDDXSSSC, for example <example>990807-2391</example> or <example>120211+9986</example>.
    /// </summary>
    member __.To10DigitString() = __.To10DigitString(DateTime.UtcNow)



    /// Converts the value of the current <see cref="SwedishPersonalIdentityNumber" /> object to its equivalent long string representation.
    /// Format is YYYYMMDDSSSC, for example <example>19908072391</example> or <example>191202119986</example>.
    /// </summary>
    /// 
    member __.To12DigitString() = to12DigitString identityNumber

    /// <summary>
    /// Converts the value of the current <see cref="SwedishPersonalIdentityNumber" /> object to its equivalent short string representation.
    /// Format is YYMMDDXSSSC, for example <example>990807-2391</example> or <example>120211+9986</example>.
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
    /// Not always the actual date of birth due to limited amount of personal identity numbers per day.
    /// </summary>
    [<Extension>]
    static member GetDateOfBirthHint(pin:SwedishPersonalIdentityNumber) = Hints.getDateOfBirthHint pin.IdentityNumber

    /// <summary>
    /// Gender (juridiskt kön) in Sweden according to the last digit of the birth number in the personal identity number.
    /// Odd number: Male
    /// Even number: Female
    /// </summary>
    [<Extension>]
    static member GetGenderHint(pin:SwedishPersonalIdentityNumber) = Hints.getGenderHint pin.IdentityNumber

    /// <summary>
    /// Get the age of the person according to the date in the personal identity number.
    /// Not always the actual date of birth due to limited amount of personal identity numbers per day.
    /// </summary>
    /// <param name="personalIdentityNumber"></param>
    /// <param name="date">The date when to calculate the age.</param>
    /// <returns></returns>
    [<Extension>]
    static member GetAgeHint(pin:SwedishPersonalIdentityNumber, date:DateTime) = 
        Hints.getAgeHintOnDate date pin.IdentityNumber
        |> function
        | i when i < 0 -> invalidArg "pin" "The person is not yet born."
        | i -> i

    /// <summary>
    /// Get the age of the person according to the date in the personal identity number.
    /// Not always the actual date of birth due to limited amount of personal identity numbers per day.
    /// </summary>
    /// <param name="personalIdentityNumber"></param>
    /// <returns></returns>
    [<Extension>]
    static member GetAgeHint(pin:SwedishPersonalIdentityNumber) = 
        Hints.getAgeHintOnDate DateTime.UtcNow pin.IdentityNumber
        |> function
        | i when i < 0 -> invalidArg "pin" "The person is not yet born."
        | i -> i
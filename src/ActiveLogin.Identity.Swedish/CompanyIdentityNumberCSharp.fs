namespace ActiveLogin.Identity.Swedish

open ActiveLogin.Identity.Swedish.FSharp
open System.Runtime.InteropServices //for OutAttribute


/// <summary>
/// Represents a Swedish Company Registration Number (organisationsnummer).
/// https://en.wikipedia.org/wiki/Personal_identity_number_(Sweden)
/// https://sv.wikipedia.org/wiki/Personnummer_i_Sverige
/// </summary>
[<CompiledName("CompanyIdentityNumber")>]
type CompanyIdentityNumberCSharp internal(num :CompanyIdentityNumber) =

    /// <summary>
    /// Creates an instance of <see cref="SwedishCompanyRegistrationNumber"/> out of the individual parts.
    /// </summary>
    /// <param name="year">The year part.</param>
    /// <param name="month">The month part.</param>
    /// <param name="day">The day part.</param>
    /// <param name="birthNumber">The birth number part.</param>
    /// <param name="checksum">The checksum part.</param>
    /// <returns>An instance of <see cref="SwedishCompanyRegistrationNumber"/> if all the paramaters are valid by themselfes and in combination.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the range arguments is invalid.</exception>
    /// <exception cref="ArgumentException">Thrown when checksum is invalid.</exception>
    new(year, month, day, birthNumber, checksum) =
        let idNum = CompanyIdentityNumber.create (year, month, day, birthNumber, checksum) |> Error.handle

        CompanyIdentityNumberCSharp(idNum)

    member internal __.IdentityNumber = num

    member this.SwedishCompanyRegistrationNumber =
        match num with
        | Company num -> num |> SwedishCompanyRegistrationNumberCSharp
        | _ -> Unchecked.defaultof<SwedishCompanyRegistrationNumberCSharp>

    member this.SwedishPersonalIdentityNumber =
        match num with
        | Individual (Personal pin) -> pin |> SwedishPersonalIdentityNumberCSharp
        | _ -> Unchecked.defaultof<SwedishPersonalIdentityNumberCSharp>

    member this.SwedishCoordinationNumber =
        match num with
        | Individual (Coordination num) -> num |> SwedishCoordinationNumberCSharp
        | _ -> Unchecked.defaultof<SwedishCoordinationNumberCSharp>

    /// <summary>
    /// Converts the string representation of the Swedish company registration number to its <see cref="SwedishCompanyRegistrationNumber"/> equivalent.
    /// </summary>
    /// <param name="s">A string representation of the Swedish company registration number to parse.</param>
    /// <exception cref="ArgumentNullException">Thrown when string input is null.</exception>
    /// <exception cref="FormatException">Thrown when string input cannot be recognized as a valid SwedishCompanyRegistrationNumber.</exception>
    static member Parse(s) =
        CompanyIdentityNumber.parse s
        |> Error.handle
        |> CompanyIdentityNumberCSharp

    /// <summary>
    /// Converts the string representation of the company registration number to its <see cref="SwedishCompanyRegistrationNumber"/>
    /// equivalent and returns a value that indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="s">A string representation of the Swedish company registration number to parse.</param>
    /// <param name="parseResult">If valid, an instance of <see cref="SwedishCompanyRegistrationNumber"/></param>
    static member TryParse((s : string), [<Out>] parseResult : CompanyIdentityNumberCSharp byref) =
        let num = CompanyIdentityNumber.parse s
        match num with
        | Error _ -> false
        | Ok num ->
            parseResult <- (num |> CompanyIdentityNumberCSharp)
            true

    /// <summary>
    /// Converts the value of the current <see cref="IdentityNumber" /> object to its equivalent 12 digit string representation.
    /// Format is YYYYMMDDBBBC, for example <example>19908072391</example> or <example>191202119986</example>.
    /// </summary>
    member __.To12DigitString() = CompanyIdentityNumber.to12DigitString num

    /// <summary>
    /// Converts the value of the current <see cref="IdentityNumber" /> object to its equivalent 12 digit string representation.
    /// Format is YYYYMMDDBBBC, for example <example>19908072391</example> or <example>191202119986</example>.
    /// </summary>
    override __.ToString() = __.To12DigitString()


    /// <summary>Returns a value indicating whether this instance is equal to a specified object.</summary>
    /// <param name="value">The object to compare to this instance.</param>
    /// <returns>true if <paramref name="value">value</paramref> is an instance of <see cref="SwedishCompanyRegistrationNumber"></see> and equals the value of this instance; otherwise, false.</returns>
    override __.Equals(b) =
        match b with
        | :? CompanyIdentityNumberCSharp as n -> num = n.IdentityNumber
        | _ -> false

    /// <summary>Returns the hash code for this instance.</summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    override __.GetHashCode() = hash num

    static member op_Equality (left: CompanyIdentityNumberCSharp, right: CompanyIdentityNumberCSharp) =
        match box left, box right with
        | (null, null) -> true
        | (null, _) | (_, null) -> false
        | _ -> left.IdentityNumber = right.IdentityNumber

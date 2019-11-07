namespace ActiveLogin.Identity.Swedish

open ActiveLogin.Identity.Swedish.FSharp
open SwedishCompanyRegistrationNumber
open System.Runtime.InteropServices //for OutAttribute


/// <summary>
/// Represents a Swedish Company Registration Number (organisationsnummer).
/// https://en.wikipedia.org/wiki/Personal_identity_number_(Sweden)
/// https://sv.wikipedia.org/wiki/Personnummer_i_Sverige
/// </summary>
[<CompiledName("SwedishCompanyRegistrationNumber")>]
type SwedishCompanyRegistrationNumberCSharp internal(num : SwedishCompanyRegistrationNumber) =

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
        let idNum = create (year, month, day, birthNumber, checksum) |> Error.handle

        SwedishCompanyRegistrationNumberCSharp(idNum)

    member internal __.IdentityNumber = num

    /// <summary>
    /// Converts the string representation of the Swedish company registration number to its <see cref="SwedishCompanyRegistrationNumber"/> equivalent.
    /// </summary>
    /// <param name="s">A string representation of the Swedish company registration number to parse.</param>
    /// <exception cref="ArgumentNullException">Thrown when string input is null.</exception>
    /// <exception cref="FormatException">Thrown when string input cannot be recognized as a valid SwedishCompanyRegistrationNumber.</exception>
    static member Parse(s) =
        parse s
        |> Error.handle
        |> SwedishCompanyRegistrationNumberCSharp

    /// <summary>
    /// Converts the string representation of the company registration number to its <see cref="SwedishCompanyRegistrationNumber"/>
    /// equivalent and returns a value that indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="s">A string representation of the Swedish company registration number to parse.</param>
    /// <param name="parseResult">If valid, an instance of <see cref="SwedishCompanyRegistrationNumber"/></param>
    static member TryParse((s : string), [<Out>] parseResult : SwedishCompanyRegistrationNumberCSharp byref) =
        let num = parse s
        match num with
        | Error _ -> false
        | Ok num ->
            parseResult <- (num |> SwedishCompanyRegistrationNumberCSharp)
            true

    /// <summary>Returns a value indicating whether this instance is equal to a specified object.</summary>
    /// <param name="value">The object to compare to this instance.</param>
    /// <returns>true if <paramref name="value">value</paramref> is an instance of <see cref="SwedishCompanyRegistrationNumber"></see> and equals the value of this instance; otherwise, false.</returns>
    override __.Equals(b) =
        match b with
        | :? SwedishCompanyRegistrationNumberCSharp as n -> num = n.IdentityNumber
        | _ -> false

    /// <summary>Returns the hash code for this instance.</summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    override __.GetHashCode() = hash num

    static member op_Equality (left: SwedishCompanyRegistrationNumberCSharp, right: SwedishCompanyRegistrationNumberCSharp) =
        match box left, box right with
        | (null, null) -> true
        | (null, _) | (_, null) -> false
        | _ -> left.IdentityNumber = right.IdentityNumber

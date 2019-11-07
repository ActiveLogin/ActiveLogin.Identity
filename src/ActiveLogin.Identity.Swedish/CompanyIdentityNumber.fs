module ActiveLogin.Identity.Swedish.FSharp.CompanyIdentityNumber
open ActiveLogin.Identity.Swedish.FSharp

/// <summary>
/// Creates a <see cref="SwedishCompanyRegistrationNumber"/> out of the individual parts.
/// </summary>
/// <param name="values">IdentityNumberValues containing all the number parts</param>
let create values =
    // maybe this should be applicative instead of monadic?
    result {
        match SwedishCompanyRegistrationNumber.create values with
        | Ok num -> return Company num
        | Error _ ->
            match SwedishPersonalIdentityNumber.create values with
            | Ok num -> return num |> Personal |> Individual
            | Error _ ->
                match SwedishCoordinationNumber.create values with
                | Ok num -> return num |> Coordination |> Individual
                | Error _ -> return! "Not a valid company registration number" |> ParsingError.Invalid |> ParsingError |> Error
    }

/// <summary>
/// Converts the value of the current <see cref="CompanyIdentityNumber" /> object to its equivalent 12 digit string representation.
/// Format is YYYYMMDDBBBC, for example <example>199008672397</example> or <example>191202719983</example>.
/// </summary>
/// <param name="num">A SwedishCompanyRegistrationNumber</param>
let to12DigitString (num: CompanyIdentityNumber) =
    match num with
    | Company num -> SwedishCompanyRegistrationNumber.to12DigitString num
    | Individual (Personal pin) -> SwedishPersonalIdentityNumber.to12DigitString pin
    | Individual (Coordination num) -> SwedishCoordinationNumber.to12DigitString num

/// <summary>
/// Converts the string representation of the Swedish company registration number to its <see cref="SwedishCompanyRegistrationNumber"/> equivalent.
/// </summary>
/// <param name="str">A string representation of the Swedish company registration number to parse.</param>
let parse str = Parse.parse create str


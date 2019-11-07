module ActiveLogin.Identity.Swedish.FSharp.SwedishCompanyRegistrationNumber
open ActiveLogin.Identity.Swedish.FSharp

let create (x, y, z, q, checksum) =
    result {
        let digits =
            if x < 99
            then
                sprintf "%02i%02i%02i%03i" x y z q
            else
                sprintf "%04i%02i%02i%03i" x y z q
            |> (fun str -> str |> Seq.map (fun s -> s.ToString() |> int))
        let! x = x |> X.create
        let! y = y |> Y.create
        let! z = z |> Z.create
        let! q = q |> Q.create
        let! c = checksum |> Checksum.createFromDigits digits
        return { SwedishCompanyRegistrationNumber.X = x
                 Y = y
                 Z = z
                 Q = q
                 Checksum = c }
    }

/// <summary>
/// Converts a SwedishCompanyRegistrationNumber to its equivalent 10 digit string representation. The total length, including the separator, will be 11 chars.
/// </summary>
/// <param name="num">A SwedishCompanyRegistrationNumber</param>
let to10DigitString (num : SwedishCompanyRegistrationNumber) =
    result {
        let delimiter = "-"

        return sprintf "%02i%02i%02i%s%03i%1i"
            (num.X.Value % 100)
            num.Y.Value
            num.Z.Value
            delimiter
            num.Q.Value
            num.Checksum.Value
    }

/// <summary>
/// Converts the value of the current <see cref="SwedishCompanyRegistrationNumber" /> object to its equivalent 12 digit string representation.
/// Format is YYYYMMDDBBBC, for example <example>199008672397</example> or <example>191202719983</example>.
/// </summary>
/// <param name="num">A SwedishCompanyRegistrationNumber</param>
let to12DigitString (num: SwedishCompanyRegistrationNumber) =
    sprintf "%02i%02i%02i%03i%1i"
        num.X.Value
        num.Y.Value
        num.Z.Value
        num.Q.Value
        num.Checksum.Value

/// <summary>
/// Converts the string representation of the Swedish company registration number to its <see cref="SwedishCompanyRegistrationNumber"/> equivalent.
/// </summary>
/// <param name="str">A string representation of the Swedish company registration number to parse.</param>
let parse str = Parse.parse create str


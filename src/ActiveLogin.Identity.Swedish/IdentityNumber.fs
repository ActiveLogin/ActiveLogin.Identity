module ActiveLogin.Identity.Swedish.FSharp.IdentityNumber

open ActiveLogin.Identity.Swedish.FSharp
open System

/// <summary>
/// Creates a <see cref="SwedishPersonalIdentityNumber"/> out of the individual parts.
/// </summary>
/// <param name="values">SwedishPersonalIdentityNumberValues containing all the number parts</param>
let create (values : IdentityNumberValues) =
    result {
        match SwedishPersonalIdentityNumber.create values with
        | Ok p -> return p |> Personal
        | Error _ ->
            match SwedishCoordinationNumber.create values with
            | Ok coordNum -> return coordNum |> Coordination
            | Error _ -> return! ParsingError.Invalid "Not a pin or coordination number" |> ParsingError |> Error
    }

/// <summary>
/// Converts the string representation of the Swedish personal identity number to its <see cref="SwedishPersonalIdentityNumber"/> equivalent.
/// </summary>
/// <param name="parseYear">
/// The specific year to use when checking if the person has turned / will turn 100 years old.
/// That information changes the delimiter (- or +).
///
/// For more info, see: https://www.riksdagen.se/sv/dokument-lagar/dokument/svensk-forfattningssamling/folkbokforingslag-1991481_sfs-1991-481#P18
/// </param>
/// <param name="str">A string representation of the Swedish personal identity number to parse.</param>
let parseInSpecificYear parseYear str = Parse.parseInSpecificYear create parseYear str

/// <summary>
/// Converts the string representation of the personal identity number to its <see cref="SwedishPersonalIdentityNumber"/> equivalent.
/// </summary>
/// <param name="str">A string representation of the Swedish personal identity number to parse.</param>
let parse str = Parse.parse create str

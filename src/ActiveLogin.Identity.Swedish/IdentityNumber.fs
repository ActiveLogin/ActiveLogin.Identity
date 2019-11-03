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


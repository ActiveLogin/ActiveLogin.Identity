[<AutoOpen>]
module ActiveLogin.Identity.Swedish.FSharp.TestExtensions


module Checksum =

    // copied from production code :(
    let getChecksum (year: int) (month: int) day' (birth: int) =
        let twoDigitYear = year % 100
        let numberStr = sprintf "%02i%02i%02i%03i" twoDigitYear month day' birth
        let digits = numberStr |> Seq.map (fun s -> s.ToString() |> int)
        digits
        |> Seq.rev
        |> Seq.mapi (fun (i : int) (d : int) ->
               if i % 2 = 0 then d * 2
               else d)
        |> Seq.rev
        |> Seq.sumBy (fun (d : int) ->
               if d > 9 then d - 9
               else d)
        |> fun x -> (x * 9) % 10

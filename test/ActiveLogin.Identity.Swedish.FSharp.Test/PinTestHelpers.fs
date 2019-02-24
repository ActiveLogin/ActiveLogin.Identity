module ActiveLogin.Identity.Swedish.FSharp.Test.PinTestHelpers
open ActiveLogin.Identity.Swedish.FSharp

let quickParse (str:string) = 
    let values = 
        { Year = str.[ 0..3 ] |> int
          Month = str.[ 4..5 ] |> int 
          Day = str.[ 6..7 ] |> int
          BirthNumber = str.[ 8..10 ] |> int
          Checksum = str.[ 11..11 ] |> int }
    match SwedishPersonalIdentityNumber.create values with 
    | Ok p -> Some p 
    | Error _ -> None
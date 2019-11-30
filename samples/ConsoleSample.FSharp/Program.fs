module ConsoleSample.FSharp

open System
open ActiveLogin.Identity.Swedish
open ActiveLogin.Identity.Swedish.FSharp.TestData

let sampleStrings = [ "990913+9801"; "120211+9986"; "990807-2391"; "180101-2392"; "180101.2392" ]
let sampleCoordinationNumbers = [ "900778-2395" ]
let sampleInvalidNumbers = [ "ABC" ]


[<EntryPoint>]
let main argv =
    // TODO
    0 // return an integer exit code

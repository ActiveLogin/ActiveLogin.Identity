open System
open BenchmarkDotNet.Running
open ActiveLogin.Identity.Swedish.Benchmark

[<EntryPoint>]
let main argv =
    let summary = BenchmarkRunner.Run<BenchmarkPinAccess>()
    0 // return an integer exit code

// To run this script, make sure fake is installed, or install it:
// "dotnet tool install fake-cli -g" or see here for more options: https://fake.build/fake-gettingstarted.html
// to run the script:
// "fake run FetchCoordinationTestData.fsx"
// It will fetch the dependencies list below using paket and run the script.
// If you are updating/adding any dependencies in the list below, remove the ".fake"-folder and the
// FetchCoordinationTestData.fsx.lock-file and run the script again to download the new dependencies.
#r "paket:
nuget FSharp.Core 4.5.4
nuget FSharp.Data //"
#load "./.fake/FetchTestData.fsx/intellisense.fsx"
#load "./FetchCommon.fs"

open FSharp.Data
open System.Text
open System.IO
open FetchCommon

type TestData = CsvProvider<"CoordinationTestDataTemplate.csv">

let getCoordNums url =
    async {
        let! nums = TestData.AsyncLoad url
        return
            nums.Rows
            |> Seq.map (fun r -> r.TestSamordningsnummer)
    }

let header = """// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
module internal ActiveLogin.Identity.Swedish.TestData.AllCoordNums
    let allCoordNums =
        [|
"""

let footer = """        |]"""

let sb = StringBuilder()

let nums =
    let validCoordinationNum numStr =
        // see issue (https://github.com/ActiveLogin/ActiveLogin.Identity/issues/100)
        let validMonth (numStr:string) =
            numStr.[4..5] <> "00"
        let validCoordinationDay (numStr:string) =
            let day = numStr.[6..7] |> int
            day > 60 && day < 89 // yes, the day *can* be 89 or greater, but see the issue linked above.
        validMonth numStr && validCoordinationDay numStr

    [ "https://skatteverket.entryscape.net/store/9/resource/154" ]
    |> List.map getCoordNums
    |> Async.Parallel
    |> Async.RunSynchronously
    |> Seq.concat
    |> Seq.sort
    |> Seq.map string
    |> Seq.filter validCoordinationNum


sb.Clear()
sb.Append(header)

nums
|> Seq.iter (toStringTuple >> appendLine sb)
sb.Append(footer)
File.WriteAllText("AllCoordNums.fs", sb.ToString())
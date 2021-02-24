module ConsoleSample.FSharp

open ActiveLogin.Identity.Swedish
open ActiveLogin.Identity.Swedish.Extensions

let samplePins = [ "990913+9801"; "120211+9986"; "990807-2391"; "180101-2392"; "180101.2392" ]
let sampleCoordinationNumbers = [ "900778-2395" ]
let sampleInvalidNumbers = [ "ABC" ]

module SwedishPersonalIdentityNumber =
    let tryParse input =
        match PersonalIdentityNumber.TryParse input with
        | true, num -> Some num
        | false, _ -> None

    let print (num: PersonalIdentityNumber) =
        printfn "   .ToString(): %O" num
        printfn "   .To10DigitString(): %s" (num.To10DigitString())
        printfn "   .To12DigitString(): %s" (num.To12DigitString())
        printfn "   .Year: %O" num.Year
        printfn "   .Month: %O" num.Month
        printfn "   .Day: %O" num.Day
        printfn "   .BirthNumber: %O" num.BirthNumber
        printfn "   .Checksum: %O" num.Checksum
        printfn "   .GetDateOfBirthHint(): %s" (num.GetDateOfBirthHint().ToShortDateString())
        printfn "   .GetAgeHint(): %O" (num.GetAgeHint())
        printfn "   .GetGenderHint(): %O" (num.GetGenderHint())
        printfn "   .IsTestNumber(): %O" (num.IsTestNumber)

let sampleStrings = samplePins @ sampleCoordinationNumbers @ sampleInvalidNumbers

let printSection withTopSpace header =
    if withTopSpace then printfn ""
    printfn "%s" header
    printfn "###################################################################"
    printfn ""

let printKeyValueInfo = printfn "%s: %s"

let printSpace() = printfn ""; printfn ""

let printHeader withTopSpace header args =
    if withTopSpace then printfn ""
    printfn header args
    printfn "---------------------------------------"

let parseAndPrint parse print rawString =
    printHeader true "Input: %s" rawString
    rawString
    |> parse
    |> function
    | Some num -> print num
    | None ->
        eprintfn "Unable to parse the input as a PersonalIdentityNumber"
        printSpace()


[<EntryPoint>]
let main argv =
    printSection false "Sample for SwedishPersonalIdentityNumber."
    printSection true "Parse Swedish personal identity numbers"
    sampleStrings
    |> List.iter (parseAndPrint SwedishPersonalIdentityNumber.tryParse SwedishPersonalIdentityNumber.print)

    0 // return an integer exit code

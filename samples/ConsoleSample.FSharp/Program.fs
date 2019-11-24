module ConsoleSample.FSharp

open System
open ActiveLogin.Identity.Swedish.FSharp
open ActiveLogin.Identity.Swedish.FSharp.TestData

let sampleStrings = [ "990913+9801"; "120211+9986"; "990807-2391"; "180101-2392"; "180101.2392" ]
let sampleCoordinationNumbers = [ "199008672397" ]
let sampleInvalidNumbers = [ "ABC" ]

let parseAndPrintIndividualIdentityNumber str =
    let printHeader str =
        str
        |> printfn "Input: %s\n----------------------"

    let print10DigitString num =
        num
        |> IndividualIdentityNumber.to10DigitString
        |> printfn "IdentityNumber.to10DigitString: %A"

    let print12DigitString num =
        num
        |> IndividualIdentityNumber.to12DigitString
        |> printfn "IdentityNumber.to12DigitString: %s"

    let printDateOfBirthHint num =
        let date = num |> IndividualIdentityNumber.Hints.getDateOfBirthHint
        date.ToShortDateString() |> printfn "IdentityNumber.Hints.getDateOfBirthHint: %s"

    let printAgeHint num =
        num
        |> IndividualIdentityNumber.Hints.getAgeHintOnDate DateTime.UtcNow
        |> Option.defaultValue 0
        |> printfn "IdentityNumber.Hints.getAgeHintOnDate: %i"

    let printGenderHint pin =
        let gender = pin |> IndividualIdentityNumber.Hints.getGenderHint
        gender.ToString() |> printfn "IdentityNumber.Hints.getGenderHint: %s"

    let printIsTestNumber num =
        // isTestNumber is an extension from the package ActiveLogin.Identity.Swedish.FSharp.TestData
        match num with
        | Personal pin ->
            pin
            |> SwedishPersonalIdentityNumber.isTestNumber
            |> printfn "SwedishPersonalIdentityNumber.isTestNumber: %b"
        | Coordination _ -> printfn "Testnumber check is not implemented for coordination numbers"

    printHeader str
    match IndividualIdentityNumber.tryParse str  with
    | Some num ->
        match num with
        | Personal pin ->
            printfn "SwedishPersonalIdentityNumber:"
            printfn "%A" pin
        | Coordination cNum ->
            printfn "SwedishCoordinationNumber:"
            printfn "%A" cNum
        print10DigitString num
        print12DigitString num
        printDateOfBirthHint num
        printAgeHint num
        printGenderHint num
        printIsTestNumber num
    | None -> printfn "%s: Unable to parse the input as an IndividualIdentityNumber." str
    printf "\n\n"

[<EntryPoint>]
let main argv =
    printfn "Sample showing possible uses of SwedishPersonalIdentityNumber."
    printf "\n\n"

    sampleStrings @ sampleCoordinationNumbers @ sampleInvalidNumbers |> List.iter parseAndPrintIndividualIdentityNumber

    printfn "Here is a valid 10 digit string personal identity number that can be used for testing:\n----------------------"
    SwedishPersonalIdentityNumberTestData.getRandom()
    |> SwedishPersonalIdentityNumber.to10DigitString
    |> printfn "%A"
    printf "\n\n"

    printfn "Here is a valid 12 digit personal identity number string that can be used for testing:\n----------------------"
    SwedishPersonalIdentityNumberTestData.getRandom()
    |> SwedishPersonalIdentityNumber.to12DigitString
    |> printfn "%s"
    printf "\n\n"

    printfn "Here is a personal identity number that can be used for testing:\n----------------------"
    let pin = SwedishPersonalIdentityNumberTestData.getRandom()
    printfn "%A" pin
    pin
    |> SwedishPersonalIdentityNumber.isTestNumber
    |> printfn "Is it a test number? %b!"
    printf "\n\n"

    printfn "What is your (Swedish) Identity Number?"
    let userInput = Console.ReadLine()
    parseAndPrintIndividualIdentityNumber userInput
    Console.ReadLine() |> ignore
    0 // return an integer exit code

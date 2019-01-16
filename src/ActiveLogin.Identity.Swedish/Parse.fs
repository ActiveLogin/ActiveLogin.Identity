module internal ActiveLogin.Identity.Swedish.FSharp.Parse

open System

type Delimiter =
    | Plus
    | Hyphen

type NumberParts =
    { FullYear : int option
      ShortYear : int option
      Month : int
      Day : int
      Delimiter : Delimiter option
      BirthNumber : int
      Checksum : int }

module NumberParts =
    type internal DigitCount = TwelveDigits of char list | TenDigits of char list
    
    let requireNotEmpty str =
        match String.IsNullOrWhiteSpace str with
        | false -> str |> Ok
        | true when str = null ->
            Null |> ArgumentError |> Error
        | true ->
            Empty |> ArgumentError |> Error

    let requireStringLength (str:string) =
        if str.Length < 200 then str |> Ok
        else Length |> ArgumentError |> Error

    let requireDigitCount (str:string) =
        let list = str.ToCharArray() |> List.ofArray
        let length = 
            list
            |> List.filter Char.IsDigit
            |> List.length
        match length with
        | 10 -> (list |> TenDigits) |> Ok
        | 12 -> (list |> TwelveDigits) |> Ok
        | _ -> Length |> ArgumentError |> Error
        
    let clean numberType =
        let (|IsDigit|IsPlus|NotDigitOrPlus|) char =
            match char |> Char.IsDigit with
            | true -> IsDigit
            | false when char = '+' -> IsPlus
            | false -> NotDigitOrPlus

        let folder char state =
            match state |> List.length, char with
            | 4, IsPlus -> char :: state
            | 4, IsDigit -> char :: ('-' :: state)
            | _, IsDigit -> char :: state
            | _ -> state
        
        match numberType with
        | TenDigits chars -> (chars, []) ||> List.foldBack folder |> TenDigits
        | TwelveDigits chars -> chars |> List.filter Char.IsDigit |> TwelveDigits

    let parseNumberParts (numberType) =
        let parseDelimiter str =
            match str with
            | '-' -> Hyphen
            | '+' -> Plus
            | _ -> failwith "Internal Error, should not happen"

        match numberType with
        | TwelveDigits chars ->
            let str = chars |> Array.ofList |> String
            { FullYear = str.[0..3] |> int |> Some 
              ShortYear = None 
              Month = str.[4..5] |> int
              Day = str.[6..7] |> int
              Delimiter = None
              BirthNumber = str.[8..10] |> int
              Checksum = str.[11..11] |> int }
        | TenDigits chars -> 
            let str = chars |> Array.ofList |> String
            { FullYear = None
              ShortYear = str.[0..1] |> int |> Some 
              Month = str.[2..3] |> int
              Day = str.[4..5] |> int
              Delimiter = str.[6] |> parseDelimiter |> Some
              BirthNumber = str.[7..9] |> int
              Checksum = str.[10..10] |> int }

        
    let create str = 
        str
        |> requireNotEmpty
        |> Result.bind requireStringLength
        |> Result.bind requireDigitCount
        |> Result.map clean
        |> Result.map parseNumberParts

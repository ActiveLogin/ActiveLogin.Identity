module internal ActiveLogin.Identity.Swedish.FSharp.Parse

open System

type private PinType<'T> =
    | TwelveDigits of 'T
    | TenDigits of 'T

let parse parseYear =
    let toChars str = [ for c in str -> c ]
    let toString = Array.ofList >> String

    let requireNotEmpty str =
        match String.IsNullOrWhiteSpace str with
        | false -> str |> Ok
        | true when str = null ->
            Null
            |> ArgumentError
            |> Error
        | true ->
            Empty
            |> ArgumentError
            |> Error

    let requireDigitCount (str : string) =
        let chars = str |> toChars

        let numDigits =
            chars
            |> List.filter Char.IsDigit
            |> List.length
        match numDigits with
        | 10 -> (chars |> TenDigits) |> Ok
        | 12 -> (chars |> TwelveDigits) |> Ok
        | _ ->
            Length
            |> ArgumentError
            |> Error

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
        | TwelveDigits chars ->
            chars
            |> List.filter Char.IsDigit
            |> toString
            |> TwelveDigits
        | TenDigits chars ->
            (chars, [])
            ||> List.foldBack folder
            |> toString
            |> TenDigits

    let parseNumberValues (numberType : PinType<string>) =
        result {
            match numberType with
            | TwelveDigits str ->
                // YYYYMMDDbbbc
                // 012345678901
                return { Year = str.[0..3] |> int
                         Month = str.[4..5] |> int
                         Day = str.[6..7] |> int
                         BirthNumber = str.[8..10] |> int
                         Checksum = str.[11..11] |> int }
            | TenDigits str ->
                // YYMMDD-bbbc or YYMMDD+bbbc
                // 01234567890    01234567890
                let shortYear = (str.[0..1] |> int)
                let getCentury (year : int) = (year / 100) * 100
                let parseYear = Year.value parseYear
                let parseCentury = getCentury parseYear
                let fullYearGuess = parseCentury + shortYear
                let lastDigitsParseYear = parseYear % 100

                let! fullYear =
                    match str.[6..6] with
                    | "-" when shortYear <= lastDigitsParseYear -> fullYearGuess |> Ok
                    | "-" -> fullYearGuess - 100 |> Ok
                    | "+" when shortYear <= lastDigitsParseYear -> fullYearGuess - 100 |> Ok
                    | "+" -> fullYearGuess - 200 |> Ok
                    | _ -> "delimiter" |> Invalid |> ArgumentError |> Error
                return { Year = fullYear
                         Month = str.[2..3] |> int
                         Day = str.[4..5] |> int
                         BirthNumber = str.[7..9] |> int
                         Checksum = str.[10..10] |> int }
        }

    requireNotEmpty
    >> Result.bind requireDigitCount
    >> Result.map clean
    >> Result.bind parseNumberValues

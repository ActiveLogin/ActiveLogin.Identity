module internal ActiveLogin.Identity.Swedish.FSharp.Parse

open System

type private PinType<'T> =
    | TwelveDigits of 'T
    | TenDigits of 'T

type private IdNumberType =
    | Personal of ParseYear: Year
    | CompanyNumber


let private (|IsDigit|IsPlus|NotDigitOrPlus|) char =
    match char |> Char.IsDigit with
    | true -> IsDigit
    | false when char = '+' -> IsPlus
    | false -> NotDigitOrPlus

let private parseInternal (numberType: IdNumberType) =
    let toChars str = [ for c in str -> c ]
    let toString = Array.ofList >> String

    let requireNotEmpty str =
        match String.IsNullOrWhiteSpace str with
        | false -> str |> Ok
        | true when isNull str ->
            ArgumentNullError
            |> Error
        | true ->
            Empty
            |> ParsingError
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
            |> ParsingError
            |> Error

    let clean numDigits =
        let folder char state =
            match state |> List.length, char with
            | 4, IsPlus -> char :: state
            | 4, IsDigit -> char :: ('-' :: state)
            | _, IsDigit -> char :: state
            | _ -> state

        match numDigits with
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

    let parseNumberValues (numDigits : PinType<string>) =
        result {
            match numDigits with
            | TwelveDigits str ->
                // YYYYMMDDbbbc
                // 012345678901
                let year = str.[0..3] |> int
                let month = str.[4..5] |> int
                let day = str.[6..7] |> int
                let birthNumber = str.[8..10] |> int
                let checksum = str.[11..11] |> int
                return (year, month, day, birthNumber, checksum)
            | TenDigits str ->
                match numberType with
                | Personal parseYear ->
                    // YYMMDD-bbbc or YYMMDD+bbbc
                    // 01234567890    01234567890
                    let shortYear = (str.[0..1] |> int)
                    let getCentury (year : int) = (year / 100) * 100
                    let parseYear = Year.value parseYear
                    let parseCentury = getCentury parseYear
                    let fullYearGuess = parseCentury + shortYear
                    let lastDigitsParseYear = parseYear % 100
                    let delimiter = str.[6]

                    let! fullYear =
                        match delimiter with
                        | '-' when shortYear <= lastDigitsParseYear -> fullYearGuess |> Ok
                        | '-' -> fullYearGuess - 100 |> Ok
                        | '+' when shortYear <= lastDigitsParseYear -> fullYearGuess - 100 |> Ok
                        | '+' -> fullYearGuess - 200 |> Ok
                        | _ -> "delimiter" |> Invalid |> ParsingError |> Error
                    let month = str.[2..3] |> int
                    let day = str.[4..5] |> int
                    let birthNumber = str.[7..9] |> int
                    let checksum = str.[10..10] |> int

                    return (fullYear, month, day, birthNumber, checksum)
                | CompanyNumber ->
                    // XXYYZZ-QQQC
                    // 01234567890
                    let delimiter = str.[6]
                    match delimiter with
                    | '-' ->
                        let x = "16" + str.[0..1] |> int // TODO remove magic number
                        let y = str.[2..3] |> int
                        let z = str.[4..5] |> int
                        let q = str.[7..9] |> int
                        let checksum = str.[10..10] |> int
                        return (x, y, z, q, checksum)
                    | _ -> return! "delimiter" |> Invalid |> ParsingError |> Error
        }

    requireNotEmpty
    >> Result.bind requireDigitCount
    >> Result.map clean
    >> Result.bind parseNumberValues

let parseInSpecificYear createFunc parseYear str =
    parseInternal (Personal parseYear) str
    |> Result.bind createFunc
    |> Result.mapError ParsingError.toParsingError

let parse createFunc str = result { let! year = DateTime.UtcNow.Year |> Year.create
                                    return! parseInSpecificYear createFunc year str }

let parseCompanyNumber createFunc str =
    parseInternal IdNumberType.CompanyNumber str
    |> Result.bind createFunc
    |> Result.mapError ParsingError.toParsingError

module internal ActiveLogin.Identity.Swedish.FSharp.Parse

open System

type private PinType<'T> =
    | TwelveDigits of 'T
    | TenDigits of 'T

let private parseInternal parseYear =
    let toChars str = [ for c in str -> c ]
    let toString = Array.ofList >> String
    let requireNotEmpty str =
        match String.IsNullOrWhiteSpace str with
        | false -> str
        | true when isNull str ->
            ArgumentNullException("input")
            |> raise
        | true ->
            FormatException("String was not recognized as a valid IdentityNumber. Cannot be empty string or whitespace.")
            |> raise

    let requireDigitCount (str : string) =
        let chars = str |> toChars
        let numDigits =
            chars
            |> List.filter Char.IsDigit
            |> List.length
        match numDigits with
        | 10 -> (chars |> TenDigits)
        | 12 -> (chars |> TwelveDigits)
        | _ -> FormatException("String was not recognized as a valid IdentityNumber.") |> raise

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
        match numberType with
        | TwelveDigits str ->
            // YYYYMMDDbbbc
            // 012345678901
            let year = str.[0..3] |> int
            let month = str.[4..5] |> int
            let day = str.[6..7] |> int
            let birthNumber = str.[8..10] |> int
            let checksum = str.[11..11] |> int
            (year, month, day, birthNumber, checksum)
        | TenDigits str ->
            // YYMMDD-bbbc or YYMMDD+bbbc
            // 01234567890    01234567890
            let shortYear = (str.[0..1] |> int)
            let getCentury (year : int) = (year / 100) * 100
            let parseYear = Year.value parseYear
            let parseCentury = getCentury parseYear
            let fullYearGuess = parseCentury + shortYear
            let lastDigitsParseYear = parseYear % 100
            let delimiter = str.[6]

            let fullYear =
                match delimiter with
                | '-' when shortYear <= lastDigitsParseYear -> fullYearGuess
                | '-' -> fullYearGuess - 100
                | '+' when shortYear <= lastDigitsParseYear -> fullYearGuess - 100
                | '+' -> fullYearGuess - 200
                | _ -> FormatException(sprintf "String was not recognized as a valid SwedishPersonalIdentityNumber. delimiter") |> raise

            let month = str.[2..3] |> int
            let day = str.[4..5] |> int
            let birthNumber = str.[7..9] |> int
            let checksum = str.[10..10] |> int
            (fullYear, month, day, birthNumber, checksum)

    requireNotEmpty
    >> requireDigitCount
    >> clean
    >> parseNumberValues

let parseInSpecificYear createFunc parseYear str =
    try
        parseInternal parseYear str
        |> createFunc
    with
        | :? ArgumentOutOfRangeException as ex ->
            FormatException(sprintf "String was not recognized as a valid IdentityNumber. %s" ex.Message, ex) |> raise
        | :? ArgumentNullException -> reraise()
        | :? ArgumentException as ex ->
            FormatException(sprintf "String was not recognized as a valid IdentityNumber. %s" ex.Message, ex) |> raise

let parse createFunc str =
    let year = DateTime.UtcNow.Year |> Year.create
    parseInSpecificYear createFunc year str

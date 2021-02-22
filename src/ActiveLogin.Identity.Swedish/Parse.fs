module internal ActiveLogin.Identity.Swedish.Parse

open System

type private PinType<'T> =
    | TwelveDigits of 'T
    | TenDigits of 'T

module private PinType =
    let map f pt =
        match pt with
        | TenDigits v -> TenDigits (f v)
        | TwelveDigits v -> TwelveDigits (f v)

module private Helpers =
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

    let requireDigitCount strictMode (str : string) =
        let chars = str |> toChars

        match strictMode with
        | StrictMode.Off ->
            match chars |> List.filter Char.IsDigit |> List.length with
            | 10 ->
                chars |> TenDigits
            | 12 ->
                chars |> TwelveDigits
            | _ ->
                FormatException("String was not recognized as a ten or twelve digit IdentityNumber.") |> raise
        | StrictMode.TenOrTwelveDigits ->
            match List.length chars with
            | 10 ->
                chars |> TenDigits
            | 11 when List.contains '-' chars || List.contains '+' chars ->
                chars |> TenDigits
            | 12 ->
                chars |> TwelveDigits
            | _ ->
                FormatException("String was not recognized as a ten or twelve digit IdentityNumber.") |> raise
        | StrictMode.TenDigits ->
            match List.length chars with
            | 10 ->
                chars |> TenDigits
            | 11 when List.contains '-' chars || List.contains '+' chars ->
                chars |> TenDigits
            | _ ->
                FormatException("String was not recognized as a ten digit IdentityNumber.") |> raise
        | StrictMode.TwelveDigits ->
            if chars |> List.length = 12 then
               chars |> TwelveDigits
            else
                FormatException("String was not recognized as a twelve digit IdentityNumber.") |> raise
        | x ->
            invalidArg "StrictMode" (sprintf "%A is not a valid StrictMode" x)


    let clean strictMode numberType =
        let (|IsDigit|IsPlus|NotDigitOrPlus|) char =
            match char |> Char.IsDigit with
            | true -> IsDigit
            | false when char = '+' -> IsPlus
            | false -> NotDigitOrPlus

        let folder char state =
            match state |> List.length, char with
            | 4, IsPlus
            | _, IsDigit -> char :: state
            | _ -> state

        match numberType with
        | TwelveDigits chars ->
            match strictMode with
            | StrictMode.Off ->
                chars
                |> List.filter Char.IsDigit
                |> toString
                |> TwelveDigits
            | StrictMode.TwelveDigits | StrictMode.TenOrTwelveDigits ->
                chars
                |> toString
                |> TwelveDigits
            | StrictMode.TenDigits ->
                failwith "programmer error, mismatching digit count"
            | _ ->
                invalidArg "StrictMode" "Invalid strict mode"
        | TenDigits chars ->
            match strictMode with
            | StrictMode.Off ->
                (chars, [])
                ||> List.foldBack folder
                |> toString
                |> TenDigits
            | StrictMode.TenDigits | StrictMode.TenOrTwelveDigits ->
                chars
                |> toString
                |> TenDigits
            | StrictMode.TwelveDigits ->
                failwith "programmer error, mismatching digit count"
            | _ ->
                invalidArg "StrictMode" "Invalid strict mode"

    let addImplicitHyphen (pin: PinType<string>) =
        match pin with
        | TenDigits str ->
            let delimiter = str.[6]
            if Char.IsDigit delimiter then
                str.[0..5] + "-" + str.[6..9]
                |> TenDigits
            else
                str
                |> TenDigits
        | TwelveDigits str -> TwelveDigits str

    let parseNumberValues parseYear (numberType : PinType<string>) =
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
                | _ -> FormatException(sprintf "String was not recognized as a valid PersonalIdentityNumber. Pin %s, has an invalid delimiter (%c)" str delimiter) |> raise

            let month = str.[2..3] |> int
            let day = str.[4..5] |> int
            let birthNumber = str.[7..9] |> int
            let checksum = str.[10..10] |> int
            (fullYear, month, day, birthNumber, checksum)

let tee f x = f x; x
let parseInSpecificYear createFunc strictMode parseYear str =
    try
        str
        |> Helpers.requireNotEmpty
        |> Helpers.requireDigitCount strictMode
        |> Helpers.clean strictMode
        |> Helpers.addImplicitHyphen
        |> Helpers.parseNumberValues parseYear
        |> createFunc
    with
        | :? ArgumentOutOfRangeException as ex ->
            FormatException(sprintf "String was not recognized as a valid IdentityNumber. %s" ex.Message, ex) |> raise
        | :? ArgumentNullException -> reraise()
        | :? ArgumentException as ex ->
            FormatException(sprintf "String was not recognized as a valid IdentityNumber. %s" ex.Message, ex) |> raise

let parse createFunc strictMode str =
    let year = DateTime.UtcNow.Year |> Year.create
    parseInSpecificYear createFunc strictMode year str

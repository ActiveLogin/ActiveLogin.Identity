module ActiveLogin.Identity.Swedish.FSharp.SwedishPersonalIdentityNumber

open System

let create (values : SwedishPersonalIdentityNumberValues) =
    result {
        let! y = values.Year |> Year.create
        let! m = values.Month |> Month.create
        let! d = values.Day |> Day.create y m
        let! s = values.BirthNumber |> BirthNumber.create
        let! c = values.Checksum |> Checksum.create y m d s
        return { SwedishPersonalIdentityNumber.Year = y
                 Month = m
                 Day = d
                 BirthNumber = s
                 Checksum = c }
    }

let private extractValues (pin : SwedishPersonalIdentityNumber) : SwedishPersonalIdentityNumberValues =
    { Year = pin.Year |> Year.value
      Month = pin.Month |> Month.value
      Day = pin.Day |> Day.value
      BirthNumber = pin.BirthNumber |> BirthNumber.value
      Checksum = pin.Checksum |> Checksum.value }

let to10DigitStringInSpecificYear serializationYear (pin : SwedishPersonalIdentityNumber) =
    let delimiter =
        if (serializationYear |> Year.value) - (pin.Year |> Year.value) >= 100 then "+"
        else "-"

    let vs = extractValues pin
    sprintf "%02i%02i%02i%s%03i%1i" (vs.Year % 100) vs.Month vs.Day delimiter vs.BirthNumber vs.Checksum

let to10DigitString (pin : SwedishPersonalIdentityNumber) =
    let year =
        DateTime.UtcNow.Year
        |> Year.create
        |> function
        | Ok y -> y
        | Error e -> invalidArg "year" "DateTime.Year wasn't a year"
    to10DigitStringInSpecificYear year pin

let to12DigitString pid =
    let vs = extractValues pid
    sprintf "%02i%02i%02i%03i%1i" vs.Year vs.Month vs.Day vs.BirthNumber vs.Checksum

let internal handleError e =
    match e with
    | InvalidYear y -> raise (new ArgumentOutOfRangeException("year", y, "Invalid year."))
    | InvalidMonth m ->
        raise (new ArgumentOutOfRangeException("month", m, "Invalid month. Must be in the range 1 to 12."))
    | InvalidDay d ->
        raise
            (new ArgumentOutOfRangeException("day", d, "Invalid day of month. It might be a valid co-ordination number."))
    | InvalidDayAndCoordinationDay d -> raise (new ArgumentOutOfRangeException("day", d, "Invalid day of month."))
    | InvalidBirthNumber s ->
        raise
            (new ArgumentOutOfRangeException("birthNumber", s, "Invalid birth number. Must be in the range 0 to 999."))
    | InvalidChecksum _ -> raise (new ArgumentException("Invalid checksum.", "checksum"))
    | ArgumentError a ->
        match a with
        | Null ->
            raise
                (new ArgumentNullException("personalIdentityNumber"))
        | Empty ->
            raise
                (new ArgumentException("Invalid personalIdentityNumber. Cannot be empty string or whitespace.", "personalIdentityNumber"))
        | Length ->
            raise
                (new ArgumentException("Invalid personalIdentityNumber.", "personalIdentityNumber"))
    | ParsingError -> invalidArg "str" "Invalid Swedish personal identity number."

let tryGetResult (pin : Result<SwedishPersonalIdentityNumber, Error>) =
    match pin with
    | Ok p -> p
    | Error e -> handleError e

open Parse

let parseInSpecificYear parseYear str =
    let fromNumberParts parseYear parsed =
        match parsed.FullYear, parsed.ShortYear, parsed.Month, parsed.Day, parsed.Delimiter, parsed.BirthNumber,
              parsed.Checksum with
        | Some fullYear, None, month, day, Hyphen, birthNumber, checksum ->
            create { Year = fullYear
                     Month = month
                     Day = day
                     BirthNumber = birthNumber
                     Checksum = checksum }
        | None, Some shortYear, month, day, delimiter, birthNumber, checksum ->
            let getCentury (year : int) = (year / 100) * 100
            let parseYear = Year.value parseYear
            let parseCentury = getCentury parseYear
            let fullYearGuess = parseCentury + shortYear
            let lastDigitsParseYear = parseYear % 100

            let fullYear =
                match delimiter with
                | Hyphen when shortYear <= lastDigitsParseYear -> fullYearGuess
                | Hyphen -> fullYearGuess - 100
                | Plus when shortYear <= lastDigitsParseYear -> fullYearGuess - 100
                | Plus -> fullYearGuess - 200
            create { Year = fullYear
                     Month = month
                     Day = day
                     BirthNumber = birthNumber
                     Checksum = checksum }
        | _ -> ParsingError |> Error
    match ParsableString.create str with
    | Error error -> Error error
    | Ok(SwedishIdentityNumber parts) -> fromNumberParts parseYear parts
    | Ok(_) -> ParsingError |> Error

let parse str = result { let! year = DateTime.UtcNow.Year |> Year.create
                         return! parseInSpecificYear year str }

module Hints =
    open ActiveLogin.Identity.Swedish

    let getDateOfBirthHint (pin : SwedishPersonalIdentityNumber) =
        DateTime(pin.Year |> Year.value, pin.Month |> Month.value, pin.Day |> Day.value, 0, 0, 0, DateTimeKind.Utc)

    let getAgeHintOnDate (date : DateTime) pin =
        let dateOfBirth = getDateOfBirthHint pin
        match date >= dateOfBirth with
        | true ->
            let months = 12 * (date.Year - dateOfBirth.Year) + (date.Month - dateOfBirth.Month)
            match date.Day < dateOfBirth.Day with
            | true ->
                let years = (months - 1) / 12
                years |> Some
            | false -> months / 12 |> Some
        | false -> None

    let getAgeHint pin = getAgeHintOnDate DateTime.UtcNow pin

    let getGenderHint (pin : SwedishPersonalIdentityNumber) =
        let isBirthNumberEven = (pin.BirthNumber |> BirthNumber.value) % 2 = 0
        if isBirthNumberEven then Gender.Female
        else Gender.Male

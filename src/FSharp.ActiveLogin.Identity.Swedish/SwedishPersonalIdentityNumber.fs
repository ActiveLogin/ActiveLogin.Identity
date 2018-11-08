module FSharp.ActiveLogin.Identity.Swedish.SwedishPersonalIdentityNumber
open System

let create year month day birthNumber checksum =
    result {
        let! y = year |> Year.create
        let! m = month |> Month.create
        let! d = day |> Day.create y m
        let! s = birthNumber |> BirthNumber.create
        let! c = checksum |> Checksum.create y m d s

        return  
            { SwedishPersonalIdentityNumber.Year = y
              Month = m
              Day = d
              BirthNumber = s
              Checksum = c }
    } 

type private IdentityNumberValues = 
    { Year: int
      Month: int
      Day: int
      BirthNumber: int
      Checksum: int }
let private extractValues (pin:SwedishPersonalIdentityNumber) =
    { IdentityNumberValues.Year = pin.Year |> Year.value
      Month = pin.Month |> Month.value
      Day = pin.Day |> Day.value
      BirthNumber = pin.BirthNumber |> BirthNumber.value
      Checksum = pin.Checksum |> Checksum.value }

let to10DigitString currentYear (pin:SwedishPersonalIdentityNumber) =
    let delimiter = if (currentYear |> Year.value) - (pin.Year |> Year.value) >= 100 then "+" else "-"
    let vs = extractValues pin
    sprintf "%02i%02i%02i%s%03i%1i" (vs.Year % 100) vs.Month vs.Day delimiter vs.BirthNumber vs.Checksum

let to12DigitString  pid =
    let vs = extractValues pid
    sprintf "%02i%02i%02i%03i%1i" vs.Year vs.Month vs.Day vs.BirthNumber vs.Checksum

let internal handleError e =
    match e with
    | InvalidYear y -> raise (new ArgumentOutOfRangeException("year", y, "Invalid year."))
    | InvalidMonth m -> raise (new ArgumentOutOfRangeException("month", m, "Invalid month. Must be in the range 1 to 12."))
    | InvalidDay d -> raise (new ArgumentOutOfRangeException("day", d, "Invalid day of month. It might be a valid co-ordination number."))
    | InvalidDayAndCoordinationDay d -> raise (new ArgumentOutOfRangeException("day", d, "Invalid day of month."))
    | InvalidBirthNumber s -> raise (new ArgumentOutOfRangeException("birthNumber", s, "Invalid birth number. Must be in the range 0 to 999."))
    | InvalidChecksum _ -> raise (new ArgumentException("Invalid checksum.", "checksum"))
    | ParsingError -> invalidArg "str" "Invalid Swedish personal identity number."

let tryGetResult (pin:Result<SwedishPersonalIdentityNumber,Error>) =
    match pin with 
    | Ok p -> p 
    | Error e -> handleError e

open Parse
let parse currentYear str = 
    let fromNumberParts currentYear parsed =
        match parsed.FullYear, parsed.ShortYear, parsed.Month, parsed.Day, parsed.Delimiter, parsed.BirthNumber, parsed.Checksum with
        | Some fullYear, None, month, day, None, birthNumber, checksum ->
            create fullYear month day birthNumber checksum
        | None, Some shortYear, month, day, delimiter, birthNumber, checkSum ->
            let getCentury (year: int) = (year / 100) * 100
            let currentYear = Year.value currentYear
            let currentCentury = getCentury currentYear
            let fullYearGuess = currentCentury + shortYear
            let lastDigitsCurrentYear = currentYear % 100
            let fullYear =
                match delimiter with
                | Some Hyphen | None when shortYear <= lastDigitsCurrentYear -> fullYearGuess
                | Some Hyphen | None -> fullYearGuess - 100
                | Some Plus when shortYear <= lastDigitsCurrentYear -> fullYearGuess - 100
                | Some Plus -> fullYearGuess - 200
            create fullYear month day birthNumber checkSum
        | _ -> ParsingError |> Error

    match str with
    | SwedishIdentityNumber parts -> fromNumberParts currentYear parts
    | _ -> ParsingError |> Error

module Hints =
    open ActiveLogin.Identity.Swedish

    let getDateOfBirthHint (pin:SwedishPersonalIdentityNumber) =
        DateTime(pin.Year |> Year.value, pin.Month |> Month.value, pin.Day |> Day.value, 0, 0, 0, DateTimeKind.Utc)

    let getAgeHintOnDate (date:DateTime) pin =
        let dateOfBirth = getDateOfBirthHint pin
        let age = date.Year - dateOfBirth.Year
        if date.DayOfYear < dateOfBirth.DayOfYear then age - 1 else age

    let getGenderHint (pin:SwedishPersonalIdentityNumber) =
        let isBirthNumberEven = (pin.BirthNumber |> BirthNumber.value) % 2 = 0
        if isBirthNumberEven then Gender.Female else Gender.Male
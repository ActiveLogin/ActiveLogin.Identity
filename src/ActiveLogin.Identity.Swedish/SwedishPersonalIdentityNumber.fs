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
        | Error _ -> invalidArg "year" "DateTime.Year wasn't a year"
    to10DigitStringInSpecificYear year pin

let to12DigitString pid =
    let vs = extractValues pid
    sprintf "%02i%02i%02i%03i%1i" vs.Year vs.Month vs.Day vs.BirthNumber vs.Checksum

let internal toParsingError err = 
    let invalidWithMsg msg i =
        i |> sprintf "%s %i" msg |> Invalid |> ParsingError 
    match err with
    | InvalidYear y ->
        y |> invalidWithMsg "InvalidYear:"
    | InvalidMonth m ->
        m |> invalidWithMsg "Invalid month:" 
    | InvalidDay d | InvalidDayAndCoordinationDay d ->
        d |> invalidWithMsg "Invalid day:"
    | InvalidBirthNumber b ->
        b |> invalidWithMsg "Invalid birthnumber:" 
    | InvalidChecksum c ->
        c |> invalidWithMsg "Invalid checksum:"
    | ParsingError err -> ParsingError err
    | ArgumentError err -> ArgumentError err
    
let parseInSpecificYear parseYear str =
    match Parse.parse parseYear str with
    | Ok pinValues -> create pinValues 
    | Error error -> Error error
    |> Result.mapError toParsingError

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

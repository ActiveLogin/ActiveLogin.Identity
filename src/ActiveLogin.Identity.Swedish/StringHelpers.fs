module internal ActiveLogin.Identity.Swedish.FSharp.StringHelpers
open ActiveLogin.Identity.Swedish.FSharp
open System


let to10DigitStringInSpecificYear serializationYear (num : IndividualIdentityNumberInternal) =

    let validSerializationYear (serializationYear: int) (pinYear: int) =
        if serializationYear < pinYear
        then
            ArgumentOutOfRange(
                "serializationYear",
                serializationYear,
                "SerializationYear cannot be a year before the person was born")
            |> Error

        elif serializationYear > (pinYear + 199)
        then
            ArgumentOutOfRange(
                "serializationYear",
                serializationYear,
                "SerializationYear cannot be a more than 199 years after the person was born")
            |> Error
        else
            serializationYear |> Ok

    result {
        let! validYear = validSerializationYear serializationYear num.Year
        let delimiter =
            if validYear - (num.Year) >= 100 then "+"
            else "-"

        return sprintf "%02i%02i%02i%s%03i%1i"
            (num.Year % 100)
            num.Month
            num.Day
            delimiter
            num.BirthNumber
            num.Checksum
    }

let to10DigitString (num : IndividualIdentityNumberInternal) =
    let year =
        DateTime.UtcNow.Year
        |> Year.create
        |> function
        | Ok y -> y
        | Error _ -> invalidArg "year" "DateTime.Year wasn't a valid year"
    to10DigitStringInSpecificYear year.Value num

let to12DigitString (num: IndividualIdentityNumberInternal) =
    sprintf "%02i%02i%02i%03i%1i"
        num.Year
        num.Month
        num.Day
        num.BirthNumber
        num.Checksum


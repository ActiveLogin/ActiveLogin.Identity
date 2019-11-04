module internal ActiveLogin.Identity.Swedish.FSharp.StringHelpers
open ActiveLogin.Identity.Swedish.FSharp
open System

let private validSerializationYear (serializationYear: Year) (pinYear: Year) =
    if serializationYear < pinYear
    then
        "SerializationYear cannot be a year before the person was born"
        |> InvalidSerializationYear
        |> Error

    elif (serializationYear |> Year.value) > ((pinYear |> Year.value) + 199)
    then
        "SerializationYear cannot be a more than 199 years after the person was born"
        |> InvalidSerializationYear
        |> Error
    else
        serializationYear |> Ok

let private parseDay day =
    match day with
    | Day d -> d.Value
    | CoordinationDay cd -> cd.Value


let to10DigitStringInSpecificYear serializationYear (num : IdentityNumber) =
    result {
        let! validYear = validSerializationYear serializationYear num.Year
        let delimiter =
            if (validYear |> Year.value) - (num.Year |> Year.value) >= 100 then "+"
            else "-"

        return sprintf "%02i%02i%02i%s%03i%1i"
            (num.Year.Value % 100)
            num.Month.Value
            (num.Day |> parseDay)
            delimiter
            num.BirthNumber.Value
            num.Checksum.Value
    }

let to10DigitString (num : IdentityNumber) =
    let year =
        DateTime.UtcNow.Year
        |> Year.create
        |> function
        | Ok y -> y
        | Error _ -> invalidArg "year" "DateTime.Year wasn't a valid year"
    to10DigitStringInSpecificYear year num

let to12DigitString (num: IdentityNumber) =
    sprintf "%02i%02i%02i%03i%1i"
        num.Year.Value
        num.Month.Value
        (num.Day |> parseDay)
        num.BirthNumber.Value
        num.Checksum.Value


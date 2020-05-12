module internal ActiveLogin.Identity.Swedish.Shared
open System


let validSerializationYear (serializationYear: int) (pinYear: Year) =
    if serializationYear < pinYear.Value
    then
        ArgumentOutOfRangeException(
            "serializationYear",
            serializationYear,
            "SerializationYear cannot be a year before the person was born")
        |> raise

    elif serializationYear > (pinYear.Value + 199)
    then
        ArgumentOutOfRangeException(
            "serializationYear",
            serializationYear,
            "SerializationYear cannot be a more than 199 years after the person was born")
        |> raise
    else
        serializationYear

let (|Even|Odd|) value =
    if value % 2 = 0 then Even
    else Odd


module internal Hints =
    module private Option =
        let fromNullable (n: Nullable<_>) =
            if n.HasValue then
                Some n.Value
            else
                None

    type internal AgeResult =
        | MissingMonthOrDay
        | NotYetBornError
        | Age of int

    module AgeResult =
        let toNullableInt result =
            match result with
            | Age i -> i |> Nullable
            | NotYetBornError -> invalidArg "num" "The person is not yet born."
            | MissingMonthOrDay -> Nullable()
        let toInt result =
            match result with
            | Age i -> i
            | NotYetBornError -> invalidArg "num" "The person is not yet born."
            | MissingMonthOrDay -> invalidArg "num" "The number contains values that can not be correctly represented as date"

    let getAge (date: DateTime) (dateOfBirth: DateTime) =
        if date >= dateOfBirth then
            let months = 12 * (date.Year - dateOfBirth.Year) + (date.Month - dateOfBirth.Month)
            match date.Day < dateOfBirth.Day with
            | true ->
                let years = (months - 1) / 12
                years |> Age
            | false -> months / 12 |> Age
        else NotYetBornError

    let getAgeFromNullableDOB (date: DateTime) (dateOfBirth: Nullable<DateTime>) =
        dateOfBirth
        |> Option.fromNullable
        |> Option.map (fun dateOfBirth -> getAge date dateOfBirth)
        |> Option.defaultValue MissingMonthOrDay
        |> AgeResult.toNullableInt

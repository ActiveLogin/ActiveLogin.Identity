module internal ActiveLogin.Identity.Swedish.FSharp.Shared
open ActiveLogin.Identity.Swedish.FSharp


let validSerializationYear (serializationYear: int) (pinYear: Year) =
    if serializationYear < pinYear.Value
    then
        ArgumentOutOfRange(
            "serializationYear",
            serializationYear,
            "SerializationYear cannot be a year before the person was born")
        |> Error

    elif serializationYear > (pinYear.Value + 199)
    then
        ArgumentOutOfRange(
            "serializationYear",
            serializationYear,
            "SerializationYear cannot be a more than 199 years after the person was born")
        |> Error
    else
        serializationYear |> Ok

let (|Even|Odd|) value =
    if value % 2 = 0 then Even
    else Odd

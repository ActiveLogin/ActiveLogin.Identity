module internal ActiveLogin.Identity.Swedish.FSharp.HintsHelper

open System
open ActiveLogin.Identity.Swedish

let getDateOfBirthHint (num : IndividualIdentityNumber) =
    let day =
        match num with
        | Personal pin -> pin.Day.Value
        | Coordination num -> num.RealDay
    DateTime(num.Year.Value, num.Month.Value, day, 0, 0, 0, DateTimeKind.Utc)

let getAgeHintOnDate (date : DateTime) num =
    let dateOfBirth = getDateOfBirthHint num
    if date >= dateOfBirth then
        let months = 12 * (date.Year - dateOfBirth.Year) + (date.Month - dateOfBirth.Month)
        match date.Day < dateOfBirth.Day with
        | true ->
            let years = (months - 1) / 12
            years |> Some
        | false -> months / 12 |> Some
    else None

let getAgeHint num = getAgeHintOnDate DateTime.UtcNow num

let getGenderHint (num : IndividualIdentityNumber) =
    let isBirthNumberEven = (num.BirthNumber |> BirthNumber.value) % 2 = 0
    if isBirthNumberEven then Gender.Female
    else Gender.Male


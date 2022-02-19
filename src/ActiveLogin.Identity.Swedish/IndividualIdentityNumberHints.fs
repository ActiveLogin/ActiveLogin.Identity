namespace ActiveLogin.Identity.Swedish.Extensions
open System
open System.Runtime.CompilerServices
open ActiveLogin.Identity.Swedish
open ActiveLogin.Identity.Swedish.Shared.Hints

[<AutoOpen>]
module private IndividualNum =
    let private runEither fPin fCoordNum num =
        match num with
        | Personal pin -> fPin pin
        | Coordination num -> fCoordNum num

    let getDateOfBirthHint =
        runEither (Pin.getDateOfBirthHint >> Nullable) CoordNum.getDateOfBirthHint

    let getGenderHint =
        runEither Pin.getGenderHint CoordNum.getGenderHint

    let getAgeHintOnDate date num =
        getDateOfBirthHint num
        |> getAgeFromNullableDOB date

[<Extension>]
type IndividualIdentityNumberHintExtensions() =
    /// <summary>
    /// Date of birth for the person according to the  identity number.
    /// Not always the actual date of birth due to the limited quantity of identity numbers per day.
    /// </summary>
    [<Extension>]
    static member GetDateOfBirthHint(num : IndividualIdentityNumber) = IndividualNum.getDateOfBirthHint num.IdentityNumber

    /// <summary>
    /// Get the age of the person according to the date in the identity number.
    /// Not always the actual date of birth due to the limited quantity of identity numbers per day.
    /// </summary>
    /// <param name="num"></param>
    /// <param name="date">The date when to calculate the age.</param>
    /// <returns></returns>
    [<Extension>]
    static member GetAgeHint(num : IndividualIdentityNumber, date : DateTime) =
        IndividualNum.getAgeHintOnDate date num.IdentityNumber

    /// <summary>
    /// Get the age of the person according to the date in the identity number.
    /// Not always the actual date of birth due to the limited quantity of identity numbers per day.
    /// </summary>
    [<Extension>]
    static member GetAgeHint(num : IndividualIdentityNumber) =
        IndividualIdentityNumberHintExtensions.GetAgeHint(num, DateTime.UtcNow)

    /// <summary>
    /// Gender (juridiskt kön) in Sweden according to the last digit of the birth number in the identity number.
    /// Odd number: Male
    /// Even number: Female
    /// </summary>
    [<Extension>]
    static member GetGenderHint(num : IndividualIdentityNumber) =
        IndividualNum.getGenderHint num.IdentityNumber

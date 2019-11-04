[<AutoOpen>]
module ActiveLogin.Identity.Swedish.FSharp.Types

open System

type ParsingError =
    | Empty
    | Length
    | Invalid of string


type Error =
    | InvalidYear of int
    | InvalidMonth of int
    | InvalidDayAndCoordinationDay of int //TODO remove this type, with CoordinationNumber support this should not be used
    | InvalidDay of int
    | InvalidBirthNumber of int
    | InvalidChecksum of int
    | ArgumentNullError
    | ParsingError of ParsingError
    | InvalidSerializationYear of string

module ParsingError =
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
        | ArgumentNullError -> ArgumentNullError
        | InvalidSerializationYear msg -> InvalidSerializationYear msg


module Error =
    /// This function will raise the most fitting Exceptions for the Error type provided.
    let handle result =
        match result with
        | Ok res -> res
        | Error e ->
            match e with
            | InvalidYear y -> raise (ArgumentOutOfRangeException("year", y, "Invalid year."))
            | InvalidMonth m ->
                raise (ArgumentOutOfRangeException("month", m, "Invalid month. Must be in the range 1 to 12."))
            | InvalidDay d ->
                raise
                    (ArgumentOutOfRangeException("day", d, "Invalid day of month. It might be a valid co-ordination number."))
            | InvalidDayAndCoordinationDay d -> raise (ArgumentOutOfRangeException("day", d, "Invalid day of month."))
            | InvalidBirthNumber s ->
                raise
                    (ArgumentOutOfRangeException("birthNumber", s, "Invalid birth number. Must be in the range 0 to 999."))
            | InvalidChecksum _ -> raise (ArgumentException("Invalid checksum.", "checksum"))
            | ArgumentNullError ->
                raise
                    (ArgumentNullException("personalIdentityNumber"))
            | ParsingError p ->
                match p with
                | Empty ->
                    raise
                        (FormatException("String was not recognized as a valid SwedishPersonalIdentityNumber. Cannot be empty string or whitespace."))
                | Length ->
                    raise
                        (FormatException("String was not recognized as a valid SwedishPersonalIdentityNumber."))
                | Invalid msg ->
                    raise
                        (FormatException(sprintf "String was not recognized as a valid SwedishPersonalIdentityNumber. %s" msg))
            | InvalidSerializationYear msg -> raise (ArgumentOutOfRangeException msg)

type Year = private Year of int

module Year =
    let create year =
        let isValidYear = year >= DateTime.MinValue.Year && year <= DateTime.MaxValue.Year
        if isValidYear then
            year
            |> Year
            |> Ok
        else
            year
            |> InvalidYear
            |> Error

    let value (Year year) = year

type Year with
    member this.Value = Year.value this

type Month = private Month of int

module Month =
    let create month =
        let isValidMonth = month >= 1 && month <= 12
        if isValidMonth then
            month
            |> Month
            |> Ok
        else
            month
            |> InvalidMonth
            |> Error

    let value (Month month) = month

type Month with
    member this.Value = Month.value this

type Day = private Day of int

module Day =
    let create (Year inYear) (Month inMonth) day =
        let coordinationNumberDaysAdded = 60
        let daysInMonth = DateTime.DaysInMonth(inYear, inMonth)
        let isValidDay = day >= 1 && day <= daysInMonth

        let isCoordinationDay d =
            let dayWithoutCoordinationAddon = d - coordinationNumberDaysAdded
            dayWithoutCoordinationAddon >= 1 && dayWithoutCoordinationAddon <= daysInMonth
        match isValidDay with
        | true ->
            day
            |> Day
            |> Ok
        | false when isCoordinationDay day ->
            day
            |> InvalidDay
            |> Error
        | false ->
            day
            |> InvalidDayAndCoordinationDay
            |> Error

    let value (Day day) = day

type Day with
    member this.Value = Day.value this

type CoordinationDay = CoordinationDay of int

module CoordinationDay =
    let create (Year inYear) (Month inMonth) day =
        let coordinationNumberDaysAdded = 60
        let daysInMonth = DateTime.DaysInMonth(inYear, inMonth)

        let isCoordinationDay d =
            let dayWithoutCoordinationAddon = d - coordinationNumberDaysAdded
            dayWithoutCoordinationAddon >= 1 && dayWithoutCoordinationAddon <= daysInMonth

        match isCoordinationDay day with
        | true ->
            day
            |> CoordinationDay
            |> Ok
        | false ->
            day
            |> InvalidDayAndCoordinationDay
            |> Error

    let value (CoordinationDay day) = day

type CoordinationDay with
    member this.Value = this |> CoordinationDay.value
    member this.RealDay = this.Value - 60

type DayInternal =
    | Day of Day
    | CoordinationDay of CoordinationDay

type BirthNumber = private BirthNumber of int

module BirthNumber =
    let create num =
        let isValidBirthNumber = num >= 1 && num <= 999
        if isValidBirthNumber then
            num
            |> BirthNumber
            |> Ok
        else
            num
            |> InvalidBirthNumber
            |> Error

    let value (BirthNumber num) = num

type BirthNumber with
    member this.Value = BirthNumber.value this

type Checksum = private Checksum of int

module Checksum =
    let private create' (Year year) (Month month) day (BirthNumber birth) checksum =
        let isValidChecksum =
            let calculatedChecksum =
                let twoDigitYear = year % 100
                let numberStr = sprintf "%02i%02i%02i%03i" twoDigitYear month day birth
                let digits = numberStr |> Seq.map (fun s -> s.ToString() |> int)
                digits
                |> Seq.rev
                |> Seq.mapi (fun (i : int) (d : int) ->
                       if i % 2 = 0 then d * 2
                       else d)
                |> Seq.rev
                |> Seq.sumBy (fun (d : int) ->
                       if d > 9 then d - 9
                       else d)
                |> fun x -> (x * 9) % 10
            calculatedChecksum = checksum

        if isValidChecksum then
            checksum
            |> Checksum
            |> Ok
        else
            checksum
            |> InvalidChecksum
            |> Error

    let create y m (day: DayInternal) b c =
        let day =
            match day with
            | Day (day) -> day.Value
            | CoordinationDay (day) -> day.Value
        create' y m day b c

    let value (Checksum sum) = sum

type Checksum with
    member this.Value = this |> Checksum.value

/// Represents a Swedish Personal Identity Number (Svenskt Personnummer).
/// https://en.wikipedia.org/wiki/Personal_identity_number_(Sweden)
/// https://sv.wikipedia.org/wiki/Personnummer_i_Sverige
type SwedishPersonalIdentityNumber =
    { /// The year for date of birth.
      Year : Year
      /// The month for date of birth.
      Month : Month
      /// The day for date of birth.
      Day : Day
      /// A birth number (födelsenummer) to distinguish people born on the same day.
      BirthNumber : BirthNumber
      /// A checksum (kontrollsiffra) used for validation. Last digit in the PIN.
      Checksum : Checksum }
    /// <summary>
    /// Converts the value of the current <see cref="SwedishPersonalIdentityNumber" /> object to its equivalent 12 digit string representation.
    /// Format is YYYYMMDDBBBC, for example <example>19908072391</example> or <example>191202119986</example>.
    /// </summary>
    override this.ToString() = sprintf "%A" this


/// Represents a Swedish Coordination Identity Number (Samordningsnummer).
type SwedishCoordinationNumber =
    { /// The year for date of birth.
      Year : Year
      /// The month for date of birth.
      Month : Month
      /// The day for date of birth + 60.
      CoordinationDay : CoordinationDay
      /// A birth number (födelsenummer) to distinguish people born on the same day.
      BirthNumber : BirthNumber
      /// A checksum (kontrollsiffra) used for validation. Last digit in the PIN.
      Checksum : Checksum }
    /// <summary>
    /// Converts the value of the current <see cref="SwedishCoordinationNumber" /> object to its equivalent 12 digit string representation.
    /// Format is YYYYMMDDBBBC, for example <example>???</example> or <example>???</example>.
    /// </summary>
    override this.ToString() = sprintf "%A" this
    member this.RealDay = this.CoordinationDay.RealDay


type IdentityNumberValues =
    { Year : int
      Month : int
      Day : int
      BirthNumber : int
      Checksum : int }

/// Represents a Swedish Identity Number.
type IdentityNumber =
    | Personal of SwedishPersonalIdentityNumber
    | Coordination of SwedishCoordinationNumber

    /// The year for date of birth.
    member this.Year =
        match this with
        | Personal p -> p.Year
        | Coordination c -> c.Year

    /// The month for date of birth.
    member this.Month =
        match this with
        | Personal p -> p.Month
        | Coordination c -> c.Month

    /// The day for date of birth.
    member this.Day =
        match this with
        | Personal p -> p.Day |> Day
        | Coordination c -> c.CoordinationDay |> CoordinationDay

    /// A birth number (födelsenummer) to distinguish people born on the same day.
    member this.BirthNumber =
        match this with
        | Personal p -> p.BirthNumber
        | Coordination c -> c.BirthNumber

    /// A checksum (kontrollsiffra) used for validation. Last digit in the PIN.
    member this.Checksum =
        match this with
        | Personal p -> p.Checksum
        | Coordination c -> c.Checksum

    /// Returns a value indicating whether this is a SwedishPersonalIdentityNumber.
    member this.IsSwedishPersonalIdentityNumber =
        match this with
        | Personal _ -> true
        | _ -> false

    /// Returns a value indicating whether this is a SwedishCoordinationNumber.
    member this.IsSwedishCoordinationNumber =
        match this with
        | Coordination _ -> true
        | _ -> false


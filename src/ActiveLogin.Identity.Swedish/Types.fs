[<AutoOpen>]
module ActiveLogin.Identity.Swedish.FSharp.Types

open System

type ArgumentError =
    | Null

type ParsingError =
    | Empty
    | Length
    | Invalid of string

type Error =
    | InvalidYear of int
    | InvalidMonth of int
    | InvalidDayAndCoordinationDay of int
    | InvalidDay of int
    | InvalidBirthNumber of int
    | InvalidChecksum of int
    | ArgumentError of ArgumentError
    | ParsingError of ParsingError

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
            | ArgumentError a ->
                match a with
                | Null ->
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

type Checksum = private Checksum of int

module Checksum =
    let create (Year year) (Month month) (Day day) (BirthNumber birth) checksum =
        let isValidChecksum =
            let getCheckSum digits =
                let checksum =
                    digits
                    |> Seq.rev
                    |> Seq.mapi (fun (i : int) (d : int) ->
                           if i % 2 = 0 then d * 2
                           else d)
                    |> Seq.rev
                    |> Seq.sumBy (fun (d : int) ->
                           if d > 9 then d - 9
                           else d)
                (checksum * 9) % 10

            let twoDigitYear = year % 100
            let pNum = sprintf "%02i%02i%02i%03i" twoDigitYear month day birth
            let digits = Seq.map (fun s -> Int32.Parse <| s.ToString()) pNum
            let calculated = digits |> getCheckSum
            calculated = checksum
        if isValidChecksum then
            checksum
            |> Checksum
            |> Ok
        else
            checksum
            |> InvalidChecksum
            |> Error

    let value (Checksum sum) = sum

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
    /// Converts the value of the current <see cref="SwedishPersonalIdentityNumber" /> object to its equivalent 12 digit string representation.
    /// Format is YYYYMMDDBBBC, for example <example>19908072391</example> or <example>191202119986</example>.
    override this.ToString() = sprintf "%A" this

type SwedishPersonalIdentityNumberValues =
    { Year : int
      Month : int
      Day : int
      BirthNumber : int
      Checksum : int }

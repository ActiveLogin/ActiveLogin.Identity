[<AutoOpen>]
module internal ActiveLogin.Identity.Swedish.FSharp.Types

open System

type ParsingError =
    | Empty
    | Length
    | Invalid of string
type Error =
    | ArgumentOutOfRange of parameter: string * value: int * message: string
    | ArgumentError of parameter: string * message: string
    | ArgumentNullError
    | ParsingError of ParsingError
type Year = private Year of int
type Month = private Month of int
type Day = private Day of int
type CoordinationDay = private CoordinationDay of int
[<RequireQualifiedAccess>]
type DayInternal =
    | Day of Day
    | CoordinationDay of CoordinationDay
type BirthNumber = private BirthNumber of int
type Checksum = private Checksum of int

type SwedishPersonalIdentityNumberInternal =
    { Year : Year
      Month : Month
      Day : Day
      BirthNumber : BirthNumber
      Checksum : Checksum }

type SwedishCoordinationNumberInternal =
    { Year : Year
      Month : Month
      CoordinationDay : CoordinationDay
      BirthNumber : BirthNumber
      Checksum : Checksum }

type IndividualIdentityNumberInternal =
    | Personal of SwedishPersonalIdentityNumberInternal
    | Coordination of SwedishCoordinationNumberInternal

module ParsingError =
    let toParsingError err =
        match err with
        | ArgumentOutOfRange (value = v; message = msg) ->
            sprintf "%s %i" msg v |> ParsingError.Invalid |> ParsingError
        | ArgumentError (parameter = name; message = msg) ->
            sprintf "%s: %s" name msg |> ParsingError.Invalid |> ParsingError
        | ParsingError err -> ParsingError err
        | ArgumentNullError -> ArgumentNullError

module Error =
    let handle result =
        match result with
        | Ok res -> res
        | Error e ->
            match e with
            | ArgumentOutOfRange (parameter = name; value = v; message = msg) ->
                raise (ArgumentOutOfRangeException(name, v , msg))
            | ArgumentError (parameter = name; message = msg) ->
                raise (ArgumentException(msg, name))
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

module Year =
    let create year =
        let isValidYear = year >= DateTime.MinValue.Year && year <= DateTime.MaxValue.Year
        if isValidYear then
            year
            |> Year
            |> Ok
        else
            ArgumentOutOfRange("year", year, "Invalid year.")
            |> Error

    let value (Year year) = year

type Year with
    member this.Value = Year.value this

module Month =
    let create month =
        let isValidMonth = month >= 1 && month <= 12
        if isValidMonth then
            month
            |> Month
            |> Ok
        else

            ArgumentOutOfRange("month", month, "Invalid month.")
            |> Error

    let value (Month month) = month

type Month with
    member this.Value = Month.value this

module Day =
    let create (Year inYear) (Month inMonth) day =
        let daysInMonth = DateTime.DaysInMonth(inYear, inMonth)
        let isValidDay = day >= 1 && day <= daysInMonth

        if isValidDay then
            day |> Day |> Ok
        else

            ArgumentOutOfRange("day", day, "Invalid day of month.")
            |> Error

    let value (Day day) = day

type Day with
    member this.Value = Day.value this

module CoordinationDay =
    let create (Year inYear) (Month inMonth) day =
        let coordinationNumberDaysAdded = 60
        let daysInMonth = DateTime.DaysInMonth(inYear, inMonth)

        let isValidCoordinationDay =
            let dayWithoutCoordinationAddon = day - coordinationNumberDaysAdded
            dayWithoutCoordinationAddon >= 1 && dayWithoutCoordinationAddon <= daysInMonth

        if isValidCoordinationDay then
            day
            |> CoordinationDay
            |> Ok
        else

            ArgumentOutOfRange("day", day, "Invalid coordination day.")
            |> Error

    let value (CoordinationDay day) = day

type CoordinationDay with
    member this.Value = this |> CoordinationDay.value
    member this.RealDay = this.Value - 60

module BirthNumber =
    let create num =
        let isValidBirthNumber = num >= 1 && num <= 999
        if isValidBirthNumber then
            num
            |> BirthNumber
            |> Ok
        else
            ArgumentOutOfRange("num", num, "Invalid birth number.")
            |> Error

    let value (BirthNumber num) = num

type BirthNumber with
    member this.Value = BirthNumber.value this

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
            ArgumentError(parameter = "checksum", message = "Invalid checksum.")
            |> Error

    let create y m (day: DayInternal) b c =
        let day =
            match day with
            | DayInternal.Day (day) -> day.Value
            | DayInternal.CoordinationDay (day) -> day.Value
        create' y m day b c

    let value (Checksum sum) = sum

type Checksum with
    member this.Value = this |> Checksum.value

type IndividualIdentityNumberInternal with
    member this.Year =
        match this with
        | Personal p -> p.Year.Value
        | Coordination c -> c.Year.Value

    member this.Month =
        match this with
        | Personal p -> p.Month.Value
        | Coordination c -> c.Month.Value

    member this.Day =
        match this with
        | Personal p -> p.Day.Value
        | Coordination c -> c.CoordinationDay.Value

    member this.BirthNumber =
        match this with
        | Personal p -> p.BirthNumber.Value
        | Coordination c -> c.BirthNumber.Value

    member this.Checksum =
        match this with
        | Personal p -> p.Checksum.Value
        | Coordination c -> c.Checksum.Value

    member this.IsSwedishPersonalIdentityNumber =
        match this with
        | Personal _ -> true
        | _ -> false

    member this.IsSwedishCoordinationNumber =
        match this with
        | Coordination _ -> true
        | _ -> false

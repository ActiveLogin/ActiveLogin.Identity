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
type BirthNumber = private BirthNumber of int
type Checksum = private Checksum of int
type CoordinationMonth = private CoordinationMonth of int
type CoordinationDay = private CoordinationDay of int
type IndividualNumber = private IndividualNumber of int

type SwedishPersonalIdentityNumberInternal =
    { Year : Year
      Month : Month
      Day : Day
      BirthNumber : BirthNumber
      Checksum : Checksum }

type SwedishCoordinationNumberInternal =
    { Year : Year
      CoordinationMonth : CoordinationMonth
      CoordinationDay : CoordinationDay
      IndividualNumber : IndividualNumber
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
                raise (ArgumentNullException("input"))
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

module CoordinationMonth =
    let create month =
        if month < 0 || month > 12 then
            ArgumentOutOfRange("coordination month", month, "Invalid month for coordination number")
            |> Error
        else
            month |> CoordinationMonth |> Ok

    let value (CoordinationMonth month) = month

type CoordinationMonth with
    member this.Value = this |> CoordinationMonth.value

module CoordinationDay =
    let create day =

        if day < 60 || day > 91 then
            ArgumentOutOfRange("day", day, "Invalid coordination day.")
            |> Error
        else
            day
            |> CoordinationDay
            |> Ok

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

module IndividualNumber =
    let create num =
        let isValidIndividualNumber = num >= 1 && num <= 999
        if isValidIndividualNumber then
            num
            |> IndividualNumber
            |> Ok
        else
            ArgumentOutOfRange("num", num, "Invalid individual number.")
            |> Error

    let value (IndividualNumber num) = num

type IndividualNumber with
    member this.Value = this |> IndividualNumber.value

module Checksum =
    let private create' year month day birth checksum =
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

    let create
        (Year y)
        (month: Choice<Month, CoordinationMonth>)
        (day: Choice<Day, CoordinationDay>)
        (num: Choice<BirthNumber, IndividualNumber>)
        sum =
        let m =
            match month with
            | Choice1Of2 m -> m.Value
            | Choice2Of2 m -> m.Value
        let d =
            match day with
            | Choice1Of2 day -> day.Value
            | Choice2Of2 coordinationDay -> coordinationDay.Value
        let num =
            match num with
            | Choice1Of2 birthNumber -> birthNumber.Value
            | Choice2Of2 individualNumber -> individualNumber.Value

        create' y m d num sum

    let value (Checksum sum) = sum

type Checksum with
    member this.Value = this |> Checksum.value

type IndividualIdentityNumberInternal with
    member this.IsSwedishPersonalIdentityNumber =
        match this with
        | Personal _ -> true
        | _ -> false

    member this.IsSwedishCoordinationNumber =
        match this with
        | Coordination _ -> true
        | _ -> false

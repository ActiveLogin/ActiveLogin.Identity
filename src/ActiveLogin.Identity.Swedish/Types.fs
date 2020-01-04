[<AutoOpen>]
module internal ActiveLogin.Identity.Swedish.FSharp.Types

open System

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

module Year =
    let create year =
        let isValidYear = year >= DateTime.MinValue.Year && year <= DateTime.MaxValue.Year
        if isValidYear then
            year
            |> Year
        else
            ArgumentOutOfRangeException("year", year, "Invalid year.") |> raise

    let value (Year year) = year

type Year with
    member this.Value = Year.value this

module Month =
    let create month =
        let isValidMonth = month >= 1 && month <= 12
        if isValidMonth then
            month
            |> Month
        else

            ArgumentOutOfRangeException("month", month, "Invalid month.")
            |> raise

    let value (Month month) = month

type Month with
    member this.Value = Month.value this

module Day =
    let create (Year inYear) (Month inMonth) day =
        let daysInMonth = DateTime.DaysInMonth(inYear, inMonth)
        let isValidDay = day >= 1 && day <= daysInMonth

        if isValidDay then
            day |> Day
        else

            ArgumentOutOfRangeException("day", day, "Invalid day of month.")
            |> raise

    let value (Day day) = day

type Day with
    member this.Value = Day.value this

module CoordinationMonth =
    let create month =
        if month < 0 || month > 12 then
            ArgumentOutOfRangeException("coordination month", month, "Invalid month for coordination number")
            |> raise
        else
            month |> CoordinationMonth

    let value (CoordinationMonth month) = month

type CoordinationMonth with
    member this.Value = this |> CoordinationMonth.value

module CoordinationDay =
    let create day =

        if day < 60 || day > 91 then
            ArgumentOutOfRangeException("day", day, "Invalid coordination day.")
            |> raise
        else
            day
            |> CoordinationDay

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
        else
            ArgumentOutOfRangeException("num", num, "Invalid birth number.")
            |> raise

    let value (BirthNumber num) = num

type BirthNumber with
    member this.Value = BirthNumber.value this

module IndividualNumber =
    let create num =
        let isValidIndividualNumber = num >= 1 && num <= 999
        if isValidIndividualNumber then
            num
            |> IndividualNumber
        else
            ArgumentOutOfRangeException("num", num, "Invalid individual number.")
            |> raise

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
        else
            ArgumentException("checksum", "Invalid checksum.")
            |> raise

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

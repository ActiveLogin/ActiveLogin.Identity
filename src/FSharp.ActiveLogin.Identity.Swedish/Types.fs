[<AutoOpen>]
module FSharp.ActiveLogin.Identity.Swedish.Types

open System


type Error =
    | InvalidYear of int
    | InvalidMonth of int
    | InvalidDayAndCoordinationDay of int
    | InvalidDay of int
    | InvalidBirthNumber of int
    | InvalidChecksum of int
    | ParsingError

type Year = private Year of int
module Year =
    let create year =
        let isValidYear = 
            year >= DateTime.MinValue.Year && year <= DateTime.MaxValue.Year

        if isValidYear then year |> Year |> Ok
        else year |> InvalidYear |> Error
    let value (Year year) = year

type Month = private Month of int
module Month =
    let create month =
        let isValidMonth =
            month >= 1 && month <= 12
        
        if isValidMonth then month |> Month |> Ok
        else month |> InvalidMonth |> Error

    let value (Month month) = month

type Day = private Day of int
module Day =
    let create (Year inYear) (Month inMonth) day =
        let coordinationNumberDaysAdded = 60
        let daysInMonth = DateTime.DaysInMonth(inYear, inMonth)

        let isValidDay =
            day >= 1 && day <= daysInMonth

        let isCoordinationDay d =
            let dayWithoutCoordinationAddon = d - coordinationNumberDaysAdded
            dayWithoutCoordinationAddon >= 1 && dayWithoutCoordinationAddon <= daysInMonth 

        match isValidDay with
        | true -> day |> Day |> Ok
        | false when isCoordinationDay day -> day |> InvalidDay |> Error
        | false -> day |> InvalidDayAndCoordinationDay |> Error

    let value (Day day) = day

type BirthNumber = private BirthNumber of int
module BirthNumber =
    let create num =
        let isValidBirthNumber =
            num >= 1 && num <= 999  
        
        if isValidBirthNumber then num |> BirthNumber |> Ok
        else num |> InvalidBirthNumber |> Error

    let value (BirthNumber num) = num

type Checksum = private Checksum of int
module Checksum =
    let create (Year year) (Month month) (Day day) (BirthNumber birth) checksum =
        let isValidChecksum = 
            let getCheckSum digits =
                let checksum =
                    digits
                    |> Seq.rev
                    |> Seq.mapi (fun (i:int) (d:int) -> if i % 2 = 0 then d * 2 else d) 
                    |> Seq.rev
                    |> Seq.sumBy (fun (d:int) -> if d > 9 then d - 9 else d)

                (checksum * 9) % 10

            let twoDigitYear = year % 100
            let pNum = sprintf "%02i%02i%02i%03i" twoDigitYear month day birth
            let digits = Seq.map (fun s -> Int32.Parse <| s.ToString()) pNum

            let calculated = digits |> getCheckSum
            calculated = checksum

        if isValidChecksum then checksum |> Checksum |> Ok
        else checksum |> InvalidChecksum |> Error

    let value (Checksum sum) = sum

type SwedishPersonalIdentityNumber = 
    { Year: Year
      Month: Month
      Day: Day
      BirthNumber: BirthNumber
      Checksum: Checksum }
      override this.ToString() = sprintf "%A" this
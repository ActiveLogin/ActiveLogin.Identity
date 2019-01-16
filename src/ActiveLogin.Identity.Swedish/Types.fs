[<AutoOpen>]
module ActiveLogin.Identity.Swedish.FSharp.Types

open System

type ArgumentError =
    | Length
    | Empty
    | Null

type Error =
    | InvalidYear of int
    | InvalidMonth of int
    | InvalidDayAndCoordinationDay of int
    | InvalidDay of int
    | InvalidBirthNumber of int
    | InvalidChecksum of int
    | ArgumentError of ArgumentError
    | ParsingError

type internal ParsableString = private ParsableString of string

module internal ParsableString =
    let cleanAllButDigitsAndPlus (chars:char[]) =
        chars
            |> Array.filter(fun x -> Char.IsDigit(x) || x = '+')
        
    let cleanAllButDigits (chars:char[]) =
        chars
            |> Array.filter Char.IsDigit

    let hasPlusDelimiter (chars:char[]) =
        let revChars = chars
                       |> Array.rev
        revChars.[4] = '+'

    let getCharsWithDelimiter (delimiter:char) (chars:char[]) =
        Array.concat [| chars.[..chars.Length-5] ; [| delimiter |] ;  chars.[chars.Length-4..] |]

    let clean (str:string) =
        let digitsAndPlus = str.ToCharArray()
                            |> cleanAllButDigitsAndPlus
        let digits = digitsAndPlus
                     |> cleanAllButDigits

        match Array.length digits with
        | 10 -> match hasPlusDelimiter digitsAndPlus with
                | true -> digits |> getCharsWithDelimiter '+' |> String |> Ok
                | false -> digits |> getCharsWithDelimiter '-' |> String |> Ok             
        | 12 -> digits |> String |> Ok
        | _ -> Length |> ArgumentError |> Error

    let create str = 
        match String.IsNullOrWhiteSpace str with
        | false ->
            str
            |> clean
            |> Result.map ParsableString
        | true when str = null ->
            Null |> ArgumentError |> Error
        | true ->
            Empty |> ArgumentError |> Error

    let value (ParsableString str) = str

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

type SwedishPersonalIdentityNumber =
    { Year : Year
      Month : Month
      Day : Day
      BirthNumber : BirthNumber
      Checksum : Checksum }
    override this.ToString() = sprintf "%A" this

type SwedishPersonalIdentityNumberValues =
    { Year : int
      Month : int
      Day : int
      BirthNumber : int
      Checksum : int }

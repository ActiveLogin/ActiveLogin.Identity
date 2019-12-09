[<AutoOpen>]
module ActiveLogin.Identity.Finnish.FSharp.Types

open ActiveLogin.Identity.Common.FSharp

module Day =
    type Day = private Day of int

    let create day =
        if day < 31 && day > 0
        then
            Day day
            |> Ok
        else
            Error "Invalid day"

    let value (Day day) = day

    type Day with
        member this.Value =
            this
            |> value

module Month =
    type Month = private Month of int

    let create month =
        if month < 13 && month > 0
        then
            Month month
            |> Ok
        else
            Error "Invalid month"

    let value (Month month) = month

    type Month with
        member this.Value =
            this
            |> value

module Year =
    type Year = private Year of int

    let create year =
        if year < 2100 && year > 1799
        then
            Year year
            |> Ok
        else
            Error "Invalid year"
    let value (Year year) = year

    type Year with
        member this.Value =
            this
            |> value

module CenturySign =
    type CenturySign =
        private
        | C18xx
        | C19xx
        | C20xx

    let create centurySign =
        match centurySign with
        | "+" -> C18xx |> Ok
        | "-" -> C19xx |> Ok
        | "A" -> C20xx |> Ok
        | _ -> Error "Invalid century sign"

    let value centurySign =
        match centurySign with
        | C18xx -> "+"
        | C19xx -> "-"
        | C20xx -> "A"

    type CenturySign with
        member this.Value =
            this
            |> value

module BirthNumber =
    type BirthNumber =
        private
        | BirthNumber of int
        | TemporaryNumber of int

    let create birthNumber =
        if birthNumber < 900 && birthNumber > 1
        then
            BirthNumber birthNumber
            |> Ok
        elif birthNumber < 1000 && birthNumber > 899
        then
            TemporaryNumber birthNumber
            |> Ok
        else
            Error "Invalid month"

    let value birthNumber =
        match birthNumber with
        | BirthNumber birthNumber -> birthNumber
        | TemporaryNumber birthNumber -> birthNumber

    type BirthNumber with
        member this.Value =
            this
            |> value

[<Literal>]
let CorrespondingCharacters = "0123456789ABCDEFHJKLMNPRSTUVWXY"

module CheckSum =
    type CheckSum = private CheckSum of int

    open Day
    open Month
    open Year
    open BirthNumber
    let create (day : Day) (month : Month) (year : Year) (birthNumber : BirthNumber) =
        let number =
            sprintf "%02i%02i%02i%03i" day.Value month.Value year.Value birthNumber.Value
            //|> (fun n -> printfn "%s" n ; n)
            |> uint64
        //printfn "%i" number
        let divided = (number |> double) / (31 |> double)
        //printfn "%f" divided
        let zerofied = divided - ( (number / (31 |> uint64 ) ) |> double)
        //printfn "%f" zerofied
        let multiplied = zerofied * (31 |> double)
        //printfn "%f" multiplied
        let rounded = System.Math.Round(multiplied, 0) |> int
        //printfn "%i" rounded
        if (rounded <= CorrespondingCharacters.Length)
        then
            CheckSum rounded |> Ok
            //CheckSum 1 |> Ok
        else
            Error "Result number is out of corresponding characters range"
    let value (CheckSum checkSum) = CorrespondingCharacters.[checkSum - 1]

    type CheckSum with
        member this.Value =
            this
            |> value


// fake
let day = Day.create 13
let month = Month.create 10
let year = Year.create 1952
let bn = BirthNumber.create 308
let cs = result {
            let! day = day
            let! month = month
            let! year = year
            let! bn = bn
            return! CheckSum.create day month year bn
}

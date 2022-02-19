namespace ActiveLogin.Identity.Swedish

open System

//-----------------------------------------------------------------------------
// Domain - Types
//-----------------------------------------------------------------------------

type internal Year = private Year of int
type internal Month = private Month of int
type internal Day = private Day of int
type internal BirthNumber = private BirthNumber of int
type internal Checksum = private Checksum of int
type internal CoordinationMonth = private CoordinationMonth of int
type internal CoordinationDay = private CoordinationDay of int
type internal IndividualNumber = private IndividualNumber of int

type StrictMode =
    | Off = 0
    | TenDigits = 1
    | TwelveDigits = 2
    | TenOrTwelveDigits = 3
type internal StrictModeInternal =
    | Off
    | TenDigits
    | TwelveDigits
    | TenOrTwelveDigits
    with
        static member Create(mode: StrictMode) =
            match mode with
            | StrictMode.Off -> StrictModeInternal.Off
            | StrictMode.TenDigits -> StrictModeInternal.TenDigits
            | StrictMode.TwelveDigits -> StrictModeInternal.TwelveDigits
            | StrictMode.TenOrTwelveDigits -> StrictModeInternal.TenOrTwelveDigits
            | _ -> invalidArg "strictMode" "Not a valid StrictMode"



type internal PersonalIdentityNumberInternal =
    { Year : Year
      Month : Month
      Day : Day
      BirthNumber : BirthNumber
      Checksum : Checksum }

type internal CoordinationNumberInternal =
    { Year : Year
      CoordinationMonth : CoordinationMonth
      CoordinationDay : CoordinationDay
      IndividualNumber : IndividualNumber
      Checksum : Checksum }

type internal IndividualIdentityNumberInternal =
    | Personal of PersonalIdentityNumberInternal
    | Coordination of CoordinationNumberInternal

//-----------------------------------------------------------------------------
// Domain - Modules
//-----------------------------------------------------------------------------

module internal Year =
    let create year =
        let isValidYear = year >= DateTime.MinValue.Year && year <= DateTime.MaxValue.Year
        if isValidYear then
            year
            |> Year
        else
            ArgumentOutOfRangeException("year", year, "Invalid year.") |> raise

    let value (Year year) = year

module internal Month =
    let create month =
        let isValidMonth = month >= 1 && month <= 12
        if isValidMonth then
            month
            |> Month
        else

            ArgumentOutOfRangeException("month", month, "Invalid month.")
            |> raise

    let value (Month month) = month

module internal Day =
    let create (Year inYear) (Month inMonth) day =
        let daysInMonth = DateTime.DaysInMonth(inYear, inMonth)
        let isValidDay = day >= 1 && day <= daysInMonth

        if isValidDay then
            day |> Day
        else

            ArgumentOutOfRangeException("day", day, "Invalid day of month.")
            |> raise

    let value (Day day) = day

module internal CoordinationMonth =
    let create month =
        if month < 0 || month > 12 then
            ArgumentOutOfRangeException("coordination month", month, "Invalid month for coordination number")
            |> raise
        else
            month |> CoordinationMonth

    let value (CoordinationMonth month) = month

module internal CoordinationDay =
    let [<Literal>] DayOffset = 60
    let create (Year inYear) (CoordinationMonth inMonth) day =
        if day = DayOffset then
            CoordinationDay DayOffset // Real day = 0, this is valid when the day of date of birth is unknown.
        else
            let daysInMonth =
                if inMonth = 0 then 31 else DateTime.DaysInMonth(inYear, inMonth)

            if day < DayOffset || day > (daysInMonth + DayOffset) then
                ArgumentOutOfRangeException("day", day, "Invalid coordination day.")
                |> raise
            else
                CoordinationDay day

    let value (CoordinationDay day) = day

module internal BirthNumber =
    let create num =
        let isValidBirthNumber = num >= 1 && num <= 999
        if isValidBirthNumber then
            num
            |> BirthNumber
        else
            ArgumentOutOfRangeException("num", num, "Invalid birth number.")
            |> raise

    let value (BirthNumber num) = num

module internal IndividualNumber =
    let create num =
        let isValidIndividualNumber = num >= 1 && num <= 999
        if isValidIndividualNumber then
            num
            |> IndividualNumber
        else
            ArgumentOutOfRangeException("num", num, "Invalid individual number.")
            |> raise

    let value (IndividualNumber num) = num

module internal Checksum =
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
            | Choice1Of2 m -> Month.value m
            | Choice2Of2 m -> CoordinationMonth.value m
        let d =
            match day with
            | Choice1Of2 day -> Day.value day
            | Choice2Of2 coordinationDay -> CoordinationDay.value coordinationDay
        let num =
            match num with
            | Choice1Of2 birthNumber -> BirthNumber.value birthNumber
            | Choice2Of2 individualNumber -> IndividualNumber.value individualNumber

        create' y m d num sum

    let value (Checksum sum) = sum

//-----------------------------------------------------------------------------
// Domain - Type Extensions
//-----------------------------------------------------------------------------

type internal Year with member this.Value = Year.value this
type internal Month with member this.Value = Month.value this
type internal Day with member this.Value = Day.value this
type internal CoordinationMonth with member this.Value = this |> CoordinationMonth.value
type internal CoordinationDay with
    member this.Value = this |> CoordinationDay.value
    member this.RealDay = this.Value - CoordinationDay.DayOffset // TODO handle when RealDay = 0. Throw? Null?
type internal BirthNumber with member this.Value = BirthNumber.value this
type internal IndividualNumber with member this.Value = this |> IndividualNumber.value
type internal Checksum with member this.Value = this |> Checksum.value

type internal IndividualIdentityNumberInternal with
    member this.IsPersonalIdentityNumber =
        match this with
        | Personal _ -> true
        | _ -> false
    member this.IsCoordinationNumber =
        match this with
        | Coordination _ -> true
        | _ -> false

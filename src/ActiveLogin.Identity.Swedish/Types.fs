[<AutoOpen>]
module ActiveLogin.Identity.Swedish.FSharp.Types

open System

type internal ParsingError =
    | Empty
    | Length
    | Invalid of string
type internal Error =
    | ArgumentOutOfRange of parameter: string * value: int * message: string
    | ArgumentError of parameter: string * message: string
    | ArgumentNullError
    | ParsingError of ParsingError
type internal Year = private Year of int
type internal Month = private Month of int
type internal Day = private Day of int
type internal CoordinationDay = private CoordinationDay of int
[<RequireQualifiedAccess>]
type internal DayInternal =
    | Day of Day
    | CoordinationDay of CoordinationDay
type internal BirthNumber = private BirthNumber of int
type internal Checksum = private Checksum of int

/// Represents a Swedish Personal Identity Number (Svenskt Personnummer).
/// https://en.wikipedia.org/wiki/Personal_identity_number_(Sweden)
/// https://sv.wikipedia.org/wiki/Personnummer_i_Sverige
type SwedishPersonalIdentityNumber =
    internal
        { _Year : Year
          _Month : Month
          _Day : Day
          _BirthNumber : BirthNumber
          _Checksum : Checksum }
    /// <summary>
    /// Converts the value of the current <see cref="SwedishPersonalIdentityNumber" /> object to its equivalent 12 digit string representation.
    /// Format is YYYYMMDDBBBC, for example <example>19908072391</example> or <example>191202119986</example>.
    /// </summary>
    override this.ToString() =
        let toString
            { _Year = (Year year)
              _Month = (Month month)
              _Day = (Day day)
              _BirthNumber = (BirthNumber birthNumber)
              _Checksum = (Checksum checksum) } =
                sprintf "%04i%02i%02i%03i%01i" year month day birthNumber checksum
        this |> toString

/// Represents a Swedish Coordination Identity Number (Samordningsnummer).
type SwedishCoordinationNumber =
    internal
        { _Year : Year
          _Month : Month
          _CoordinationDay : CoordinationDay
          _BirthNumber : BirthNumber
          _Checksum : Checksum }
    /// <summary>
    /// Converts the value of the current <see cref="SwedishCoordinationNumber" /> object to its equivalent 12 digit string representation.
    /// Format is YYYYMMDDBBBC, for example <example>???</example> or <example>???</example>.
    /// </summary>
    override this.ToString() =
        let toString
            { _Year = (Year year)
              _Month = (Month month)
              _CoordinationDay = (CoordinationDay day)
              _BirthNumber = (BirthNumber birthNumber)
              _Checksum = (Checksum checksum) } =
                sprintf "%04i%02i%02i%03i%01i" year month day birthNumber checksum
        this |> toString

/// Represents a Swedish Identity Number.
type IndividualIdentityNumber =
    | Personal of SwedishPersonalIdentityNumber
    | Coordination of SwedishCoordinationNumber

module ParsingError =
    let internal toParsingError err =
        match err with
        | ArgumentOutOfRange (value = v; message = msg) ->
            sprintf "%s %i" msg v |> ParsingError.Invalid |> ParsingError
        | ArgumentError (parameter = name; message = msg) ->
            sprintf "%s: %s" name msg |> ParsingError.Invalid |> ParsingError
        | ParsingError err -> ParsingError err
        | ArgumentNullError -> ArgumentNullError

module internal Error =
    /// This function will raise the most fitting Exceptions for the Error type provided.
    let handle result =
        match result with
        | Ok res -> res
        | Error e ->
            match e with
            | ArgumentOutOfRange (parameter = name; value = v; message = msg) ->
                raise (ArgumentOutOfRangeException(name, v , msg))
            | ArgumentError (parameter = name; message = msg) ->
                raise (ArgumentException(msg, name))
//            | InvalidYear y -> raise (ArgumentOutOfRangeException("year", y, "Invalid year."))
//            | InvalidMonth m ->
//                raise (ArgumentOutOfRangeException("month", m, "Invalid month. Must be in the range 1 to 12."))
//            | InvalidDay d ->
//                raise
//                    (ArgumentOutOfRangeException("day", d, "Invalid day of month. It might be a valid co-ordination number."))
//            | InvalidDayAndCoordinationDay d -> raise (ArgumentOutOfRangeException("day", d, "Invalid day of month."))
//            | InvalidBirthNumber s ->
//                raise
//                    (ArgumentOutOfRangeException("birthNumber", s, "Invalid birth number. Must be in the range 0 to 999."))
//            | InvalidChecksum _ -> raise (ArgumentException("Invalid checksum.", "checksum"))
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
//            | InvalidSerializationYear msg -> raise (ArgumentOutOfRangeException msg)

module internal Year =
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

type internal Year with
    member this.Value = Year.value this

module internal Month =
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

type internal Month with
    member this.Value = Month.value this

module internal Day =
    let create (Year inYear) (Month inMonth) day =
        let daysInMonth = DateTime.DaysInMonth(inYear, inMonth)
        let isValidDay = day >= 1 && day <= daysInMonth

        if isValidDay then
            day |> Day |> Ok
        else

            ArgumentOutOfRange("day", day, "Invalid day of month.")
            |> Error

    let value (Day day) = day

type internal Day with
    member this.Value = Day.value this

module internal CoordinationDay =
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

type internal CoordinationDay with
    member this.Value = this |> CoordinationDay.value
    member this.RealDay = this.Value - 60

module internal BirthNumber =
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

type internal BirthNumber with
    member this.Value = BirthNumber.value this

module internal Checksum =
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

type internal Checksum with
    member this.Value = this |> Checksum.value

type SwedishPersonalIdentityNumber with
    /// The year for date of birth.
    member this.Year = this._Year.Value
    /// The month for date of birth.
    member this.Month = this._Month.Value
    /// The day for date of birth.
    member this.Day = this._Day.Value
    /// A birth number (födelsenummer) to distinguish people born on the same day.
    member this.BirthNumber = this._BirthNumber.Value
    /// A checksum (kontrollsiffra) used for validation. Last digit in the PIN.
    member this.Checksum = this._Checksum.Value

type SwedishCoordinationNumber with
    /// The year for date of birth.
    member this.Year = this._Year.Value
    /// The month for date of birth.
    member this.Month = this._Month.Value
    /// The day for date of birth + 60.
    member this.CoordinationDay = this._CoordinationDay.Value
    /// The day for date of birth.
    member this.RealDay = this._CoordinationDay.RealDay
    /// A birth number (födelsenummer) to distinguish people born on the same day.
    member this.BirthNumber = this._BirthNumber.Value
    /// A checksum (kontrollsiffra) used for validation. Last digit in the PIN.
    member this.Checksum = this._Checksum.Value

type IndividualIdentityNumber with
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
        | Personal p -> p.Day
        | Coordination c -> c.CoordinationDay

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

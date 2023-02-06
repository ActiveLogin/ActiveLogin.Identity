namespace ActiveLogin.Identity.Swedish.TestData
open ActiveLogin.Identity.Swedish
open System
open System.Threading


module Random =
    let rng =
        lazy
            // this thread-safe implementation is required to handle running lots of invocations of getRandom in parallel
            let seedGenerator = Random()
            let localGenerator = new ThreadLocal<Random>(fun _ ->
                lock seedGenerator (fun _ ->
                    let seed = seedGenerator.Next()
                    Random(seed)))

            fun (min, max) -> localGenerator.Value.Next(min, max)

    let random _ = rng.Value (Int32.MinValue, Int32.MaxValue)

module PersonalIdentityNumberTestDataInternal =
    open ActiveLogin.Identity.Swedish.TestData.AllPins

    let internal shuffledPins() = allPins |> Array.sortBy Random.random

    let raw12DigitStrings =
        allPins
        |> Array.map (fun (year, month, day, birthNumber, checksum) -> sprintf "%04i%02i%02i%03i%i" year month day birthNumber checksum)

    let allPinsByDateDesc() = seq { for pin in allPins do yield PersonalIdentityNumber pin }

    let allPinsShuffled() = seq { for pin in shuffledPins() do yield PersonalIdentityNumber pin }

    let getRandom() =
        let index = Random.rng.Value(0, Array.length allPins - 1)
        allPins.[index]
        |> PersonalIdentityNumber

    let getRandomWithCount count = allPinsShuffled() |> Seq.take count

    let internal isTestNumberTuple values =
        allPins |> Array.contains values

    let isTestNumber (pin: PersonalIdentityNumber) =
        (pin.Year, pin.Month, pin.Day, pin.BirthNumber, pin.Checksum)
        |> isTestNumberTuple

open PersonalIdentityNumberTestDataInternal

/// A class that provides easy access to the official test numbers for Swedish Personal Identity Number (Personnummer)
/// from Skatteverket
type PersonalIdentityNumberTestData() =
    static let toPin (pin: PersonalIdentityNumber) =
        PersonalIdentityNumber(pin.Year, pin.Month, pin.Day, pin.BirthNumber, pin.Checksum)

    /// All the testdata from Skatteverket presented as an array of 12 digit strings.
    static member Raw12DigitStrings = raw12DigitStrings
    /// A random test number
    static member GetRandom() = getRandom() |> toPin
    /// <summary>
    /// Returns a sequence of length specified by count, of unique random test numbers. If it is not important that the
    /// sequence of numbers is unique it is more efficient to call getRandom() repeatedly
    /// </summary>
    /// <param name="count">The number of numbers to return</param>
    static member GetRandom(count) =
        getRandomWithCount(count)
        |> List.ofSeq
        |> Seq.map toPin
    /// A seqence of all test numbers ordered by date descending
    static member AllPinsByDateDesc() = allPinsByDateDesc() |> Seq.map toPin
    /// A sequence of all test numbers in random order
    static member AllPinsShuffled() = allPinsShuffled() |> Seq.map toPin

    /// <summary>
    /// Checks if a PersonalIdentityNumber is a test number
    /// </summary>
    /// <param name="pin">A PersonalIdentityNumber</param>
    static member IsTestNumber (pin:PersonalIdentityNumber) =
        (pin.Year, pin.Month, pin.Day, pin.BirthNumber, pin.Checksum)
        |> isTestNumberTuple

module CoordinationNumberTestDataInternal =
    open AllCoordNums
    let internal shuffledCoordNums() = allCoordNums |> Array.sortBy Random.random

    let raw12DigitStrings =
        allCoordNums
        |> Array.map (fun (year, month, day, birthNumber, checksum) -> sprintf "%04i%02i%02i%03i%i" year month day birthNumber checksum)

    let allCoordNumsByDateDesc() = seq { for coordNum in allCoordNums do yield CoordinationNumber coordNum }
    let allCoordNumsShuffled() = seq { for coordNum in shuffledCoordNums() do yield CoordinationNumber coordNum }
    let getRandom() =
        let index = Random.rng.Value(0, Array.length allCoordNums - 1)
        allCoordNums.[index]
        |> CoordinationNumber
    let getRandomWithCount count = allCoordNumsShuffled() |> Seq.take count

    let internal isTestNumberTuple (year, month, day, birthNumber, checksum) =
        allCoordNums |> Array.contains (year, month, day, birthNumber, checksum)

    let internal toTuple (coordNum: CoordinationNumber) =
        let numStr = coordNum.To12DigitString()
        let year = numStr.[0..3] |> int
        let month = numStr.[4..5] |> int
        let day = numStr.[6..7] |> int
        let individualNum = numStr.[8..10] |> int
        let checksum = numStr.[11..11] |> int
        (year, month, day, individualNum, checksum)

    let isTestNumber (coordNum: CoordinationNumber) =
        coordNum
        |> toTuple
        |> isTestNumberTuple

open CoordinationNumberTestDataInternal

/// A class that provides easy access to the official test numbers for Swedish Coordination Number (Personnummer)
/// from Skatteverket
type CoordinationNumberTestData() =
    static let toCoordNum (num: CoordinationNumber) =
        num
        |> toTuple
        |> CoordinationNumber

    /// All the testdata from Skatteverket presented as an array of 12 digit strings.
    static member Raw12DigitStrings = raw12DigitStrings
    /// A random test number
    static member GetRandom() = getRandom() |> toCoordNum
    /// <summary>
    /// Returns a sequence of length specified by count, of unique random test numbers. If it is not important that the
    /// sequence of numbers is unique it is more efficient to call getRandom() repeatedly
    /// </summary>
    /// <param name="count">The number of numbers to return</param>
    static member GetRandom(count) =
        getRandomWithCount(count)
        |> List.ofSeq
        |> Seq.map toCoordNum
    /// A sequence of all test numbers ordered by date descending
    static member AllCoordinationNumbersByDateDesc() = allCoordNumsByDateDesc() |> Seq.map toCoordNum
    /// A sequence of all test numbers in random order
    static member AllCoordinationNumbersShuffled() = allCoordNumsShuffled() |> Seq.map toCoordNum

    /// <summary>
    /// Checks if a CoordinationNumber is a test number
    /// </summary>
    /// <param name="num">A CoordinationNumber</param>
    static member IsTestNumber (num:CoordinationNumber) =
        num
        |> toTuple
        |> isTestNumberTuple


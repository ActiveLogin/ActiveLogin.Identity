module ActiveLogin.Identity.Swedish.FSharp.TestData
open ActiveLogin.Identity.Swedish.FSharp
open ActiveLogin.Identity.Swedish.TestData
open System
open System.Threading

let private rng =
    // this thread-safe implementation is required to handle running lots of invocations of getRandom in parallel
    let seedGenerator = Random()
    let localGenerator = new ThreadLocal<Random>(fun _ ->
        lock seedGenerator (fun _ ->
            let seed = seedGenerator.Next()
            Random(seed)))
    fun (min, max) -> localGenerator.Value.Next(min, max)

let private random _ = rng(Int32.MinValue, Int32.MaxValue)

/// A module that provides easy access to the official test numbers for Swedish Personal Identity Number (Personnummer)
/// from Skatteverket
module SwedishPersonalIdentityNumberTestData =
    open ActiveLogin.Identity.Swedish.TestData.AllPins

    let internal shuffledPins() = allPins |> Array.sortBy random

    /// All the testdata from Skatteverket presented as an array of 12 digit strings.
    let raw12DigitStrings =
        allPins
        |> Array.map (fun (year, month, day, birthNumber, checksum) -> sprintf "%04i%02i%02i%03i%i" year month day birthNumber checksum)

    /// A sequence of all test numbers ordered by date descending
    let allPinsByDateDesc() = seq { for pin in allPins do yield SwedishPersonalIdentityNumber.create pin }

    /// A sequence of all test numbers in random order
    let allPinsShuffled() = seq { for pin in shuffledPins() do yield SwedishPersonalIdentityNumber.create pin }

    /// A random test number
    let getRandom() =
        let index = rng(0, Array.length allPins - 1)
        allPins.[index]
        |> SwedishPersonalIdentityNumber.create

    /// <summary>
    /// Returns a sequence of length specified by count, of unique random test numbers. If it is not important that the
    /// sequence of numbers is unique it is more efficient to call getRandom() repeatedly
    /// </summary>
    /// <param name="count">The number of numbers to return</param>
    let getRandomWithCount count = allPinsShuffled() |> Seq.take count

    let internal isTestNumberTuple values =
        allPins |> Array.contains values

    /// <summary>
    /// Checks if a SwedishPersonalIdentityNumber is a test number
    /// </summary>
    /// <param name="pin">A SwedishPersonalIdentityNumber</param>
    let isTestNumber (pin: SwedishPersonalIdentityNumber) =
        (pin.Year, pin.Month, pin.Day, pin.BirthNumber, pin.Checksum)
        |> isTestNumberTuple

module SwedishPersonalIdentityNumber =
    /// <summary>
    /// Checks if a SwedishPersonalIdentityNumber is a test number
    /// </summary>
    /// <param name="pin">A SwedishPersonalIdentityNumber</param>
    let isTestNumber = SwedishPersonalIdentityNumberTestData.isTestNumber


/// A module that provides easy access to the official test numbers for Swedish Coordination Number (Samordningsnummer)
/// from Skatteverket
module SwedishCoordinationNumberTestData =
    open AllCoordNums
    let internal shuffledCoordNums() = allCoordNums |> Array.sortBy random

    /// All the testdata from Skatteverket presented as an array of 12 digit strings.
    let raw12DigitStrings =
        allCoordNums
        |> Array.map (fun (year, month, day, birthNumber, checksum) -> sprintf "%04i%02i%02i%03i%i" year month day birthNumber checksum)

    /// A sequence of all test numbers ordered by date descending
    let allCoordNumsByDateDesc() = seq { for coordNum in allCoordNums do yield SwedishCoordinationNumber.create coordNum }
    /// A sequence of all test numbers in random order
    let allCoordNumsShuffled() = seq { for coordNum in shuffledCoordNums() do yield SwedishCoordinationNumber.create coordNum }
    /// A random test number
    let getRandom() =
        let index = rng(0, Array.length allCoordNums - 1)
        allCoordNums.[index]
        |> SwedishCoordinationNumber.create
    /// <summary>
    /// Returns a sequence of length specified by count, of unique random test numbers. If it is not important that the
    /// sequence of numbers is unique it is more efficient to call getRandom() repeatedly
    /// </summary>
    /// <param name="count">The number of numbers to return</param>
    let getRandomWithCount count = allCoordNumsShuffled() |> Seq.take count

    let internal isTestNumberTuple (year, month, day, birthNumber, checksum) =
        allCoordNums |> Array.contains (year, month, day, birthNumber, checksum)

    /// <summary>
    /// Checks if a SwedishCoordinationNumber is a test number
    /// </summary>
    /// <param name="coordNum">A SwedishCoordinationNumber</param>
    let isTestNumber (coordNum: SwedishCoordinationNumber) =
        (coordNum.Year, coordNum.Month, coordNum.CoordinationDay, coordNum.BirthNumber, coordNum.Checksum)
        |> isTestNumberTuple

module SwedishCoordinationNumber =
    /// <summary>
    /// Checks if a SwedishCoordinationNumber is a test number
    /// </summary>
    /// <param name="coordNum">A SwedishCoordinationNumber</param>
    let isTestNumber = SwedishCoordinationNumberTestData.isTestNumber

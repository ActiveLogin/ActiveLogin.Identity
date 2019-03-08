namespace ActiveLogin.Identity.Swedish.FSharp.TestData
open ActiveLogin.Identity.Swedish.FSharp
open ActiveLogin.Identity.Swedish.TestData.AllPins
open System
open System.Threading


/// A module that provides easy access to the official test numbers for Swedish Personal Identity Number (Personnummer) 
/// from Skatteverket
module SwedishPersonalIdentityNumberTestData =
    let private rng = 
        // this thread-safe implementation is required to handle running lots of invocations of getRandom in parallel
        let seedGenerator = Random()
        let localGenerator = new ThreadLocal<Random>(fun _ ->
            lock seedGenerator (fun _ ->
                let seed = seedGenerator.Next()
                Random()))
        fun (min, max) -> localGenerator.Value.Next(min, max)
    let private random _ = rng(Int32.MinValue, Int32.MaxValue)
    let internal shuffledPins() = allPins |> Array.sortBy random

    /// All the testdata from Skatteverket presented as an array of 12 digit strings.
    let raw12DigitStrings = 
        allPins
        |> Array.map (fun (year, month, day, birthNumber, checksum) -> sprintf "%04i%02i%02i%03i%i" year month day birthNumber checksum)

    let internal create (year, month, day, birthNumber, checksum) =
        let values =
            { Year = year
              Month = month
              Day = day
              BirthNumber = birthNumber
              Checksum = checksum }
        match SwedishPersonalIdentityNumber.create values with
        | Ok p -> p
        | Error _ -> failwith "broken test data" 

    /// A seqence of all test numbers ordered by date descending
    let allPinsByDateDesc() = seq { for pin in allPins do yield create pin }
    /// A sequence of all test numbers in random order
    let allPinsShuffled() = seq { for pin in shuffledPins() do yield create pin }
    /// A random test number
    let getRandom() = 
        let index = rng(0, Array.length allPins - 1)
        allPins.[index]
        |> create
    /// <summary>
    /// Returns a sequence of length specified by count, of unique random test numbers. If it is not important that the 
    /// sequence of numbers is unique it is more efficient to call getRandom() repeatedly
    /// </summary>
    /// <param name="count">The number of numbers to return</param>
    let getRandomWithCount(count) = allPinsShuffled() |> Seq.take count

    let internal isTestNumberTuple (year, month, day, birthNumber, checksum) =
        allPins |> Array.contains (year, month, day, birthNumber, checksum)

    /// <summary>
    /// Checks if a SwedishPersonalIdentityNumber is a test number
    /// </summary>
    /// <param name="pin">A SwedishPersonalIdentityNumber</param>
    let isTestNumber pin =
        let asTuple 
            { SwedishPersonalIdentityNumber.Year = year
              Month = month
              Day = day
              BirthNumber = birthNumber
              Checksum = checksum } =
            (Year.value year, Month.value month, Day.value day, BirthNumber.value birthNumber, Checksum.value checksum)
        pin |> asTuple |> isTestNumberTuple

module SwedishPersonalIdentityNumber =
    /// <summary>
    /// Checks if a SwedishPersonalIdentityNumber is a test number
    /// </summary>
    /// <param name="pin">A SwedishPersonalIdentityNumber</param>
    let isTestNumber = SwedishPersonalIdentityNumberTestData.isTestNumber


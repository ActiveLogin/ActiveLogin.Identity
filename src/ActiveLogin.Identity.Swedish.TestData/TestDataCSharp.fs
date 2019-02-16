namespace ActiveLogin.Identity.Swedish.TestData
open ActiveLogin.Identity.Swedish.FSharp
open ActiveLogin.Identity.Swedish.FSharp.TestData.SwedishPersonalIdentityTestNumbers
open ActiveLogin.Identity.Swedish
open System.Collections.Generic

[<CompiledName("SwedishPersonalIdentityTestNumbers")>]
type SwedishPersonalIdentityTestNumbersCSharp() =
    static let toCSharpPin (pin: SwedishPersonalIdentityNumber) =
        let year = pin.Year |> Year.value
        let month = pin.Month |> Month.value
        let day = pin.Day |> Day.value
        let birthNumber = pin.BirthNumber |> BirthNumber.value
        let checksum = pin.Checksum |> Checksum.value
        SwedishPersonalIdentityNumberCSharp(year, month, day, birthNumber, checksum)

    static member Random12DigitString() = random12DigitString()
    static member Random12DigitStrings count  = random12DigitStrings count
    static member All12DigitStringsSorted = all12DigitStringsSorted
    static member All12DigitStringsShuffled() = all12DigitStringsShuffled()

    static member Random10DigitString() = random10DigitString()
    static member Random10DigitStrings count = random10DigitStrings count
    static member All10DigitStringsSorted = all10DigitStringsSorted
    static member All10DigitStringsShuffled() = all10DigitStringsShuffled()


    static member RandomPin() = randomPin() |> toCSharpPin
    static member RandomPins(count) = 
        shuffled12Digits() 
        |> Array.take count 
        |> Array.map (parse >> toCSharpPin) 
        :> IReadOnlyCollection<_>
    static member AllPinsSorted() = allPinsSorted() |> Seq.map toCSharpPin
    static member AllPinsShuffled() = allPinsShuffled() |> Seq.map toCSharpPin

    static member IsTestNumber (pin:SwedishPersonalIdentityNumberCSharp) = 
        pin.To12DigitString()
        |> isTestNumberString
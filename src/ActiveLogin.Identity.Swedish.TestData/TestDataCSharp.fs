namespace ActiveLogin.Identity.Swedish.TestData
open ActiveLogin.Identity.Swedish.FSharp
open ActiveLogin.Identity.Swedish.FSharp.TestData.SwedishPersonalIdentityNumberTestData
open ActiveLogin.Identity.Swedish
open System.Collections.Generic

[<CompiledName("SwedishPersonalIdentityNumberTestData")>]
type SwedishPersonalIdentityNumberTestDataCSharp() =
    static let toCSharpPin (pin: SwedishPersonalIdentityNumber) =
        let year = pin.Year |> Year.value
        let month = pin.Month |> Month.value
        let day = pin.Day |> Day.value
        let birthNumber = pin.BirthNumber |> BirthNumber.value
        let checksum = pin.Checksum |> Checksum.value
        SwedishPersonalIdentityNumberCSharp(year, month, day, birthNumber, checksum)

    static member GetRandom() = getRandom() |> toCSharpPin
    static member GetRandom(count) = getRandomWithCount(count) |> Seq.map toCSharpPin |> List.ofSeq :> IReadOnlyCollection<_>
    static member AllPinsSorted() = allPinsSorted() |> Seq.map toCSharpPin
    static member AllPinsShuffled() = allPinsShuffled() |> Seq.map toCSharpPin

    static member IsTestNumber (pin:SwedishPersonalIdentityNumberCSharp) = 
        (pin.Year, pin.Month, pin.Day, pin.BirthNumber, pin.Checksum)
        |> isTestNumberTuple

open System.Runtime.CompilerServices

[<Extension>]
type SwedishPersonalIdentityNumberCSharpTestDataExtensions() =
    [<Extension>]
    static member IsTestNumber(pin : SwedishPersonalIdentityNumberCSharp) = 
        SwedishPersonalIdentityNumberTestDataCSharp.IsTestNumber pin
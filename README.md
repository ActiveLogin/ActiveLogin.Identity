# ActiveLogin.Identity

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) [![Build status](https://dev.azure.com/activesolution/ActiveLogin/_apis/build/status/ActiveLogin.Identity)](https://dev.azure.com/activesolution/ActiveLogin/_build/latest?definitionId=154)

ActiveLogin.Identity provides parsing and validation of Swedish identities such as Personal Identity Number (svenskt personnummer). Built on NET Standard and packaged as NuGet-packages they are easy to install and use on multiple platforms.

## Features
- :id: .NET parser for Swedish Personal Identity Number (Svenskt personnummer)
- :penguin: Cross platform: Targets .NET Standard 2.0 and .NET Framework 4.6.1
- :heavy_check_mark: Strong named
- :lock: GDPR Compliant
- :calendar: Extracts metadata such as date of birth and gender
- :large_blue_diamond: Written in F# and C# and works great with VB.NET as well
- :white_check_mark: Well tested

## Continuous integration & Packages overview

| Project | Description | NuGet |
| ------- | ----------- | ----- |
| [ActiveLogin.Identity.Swedish](https://github.com/ActiveLogin/ActiveLogin.Identity/tree/master/src/ActiveLogin.Identity.Swedish) | .NET classes handling Personal Identity Number | [![NuGet](https://img.shields.io/nuget/v/ActiveLogin.Identity.Swedish.svg)](https://www.nuget.org/packages/ActiveLogin.Identity.Swedish/) |
| [ActiveLogin.Identity.Swedish.AspNetCore](https://github.com/ActiveLogin/ActiveLogin.Identity/tree/master/src/ActiveLogin.Identity.Swedish.AspNetCore) | Validation attributes for ASP.NET Core. | [![NuGet](https://img.shields.io/nuget/v/ActiveLogin.Identity.Swedish.AspNetCore.svg)](https://www.nuget.org/packages/ActiveLogin.Identity.Swedish.AspNetCore/) |

## Getting started

### 1. Install the NuGet package

ActiveLogin.Identity is distributed as [packages on NuGet](https://www.nuget.org/profiles/ActiveLogin), install using the tool of your choice, for example _dotnet cli_:

```powershell
dotnet add package ActiveLogin.Identity.Swedish
```

or

```powershell
dotnet add package ActiveLogin.Identity.Swedish.AspNetCore
```

### 2. Use the library in your C# project

`SwedishPersonalIdentityNumber` provides parsing methods such as `SwedishPersonalIdentityNumber.Parse()` and `SwedishPersonalIdentityNumber.TryParse()` that can be used like this:

```c#
var rawPersonalIdentityNumber = "990807-2391";
if (SwedishPersonalIdentityNumber.TryParse(rawPersonalIdentityNumber, out var personalIdentityNumber))
{
    Console.WriteLine("SwedishPersonalIdentityNumber");
    Console.WriteLine(" .ToString(): {0}", personalIdentityNumber.ToString());
    Console.WriteLine(" .To10DigitString(): {0}", personalIdentityNumber.To10DigitString());
    Console.WriteLine(" .To12DigitString(): {0}", personalIdentityNumber.To12DigitString());

    Console.WriteLine(" .GetDateOfBirthHint(): {0}", personalIdentityNumber.GetDateOfBirthHint().ToShortDateString());
    Console.WriteLine(" .GetAgeHint(): {0}", personalIdentityNumber.GetAgeHint().ToString());

    Console.WriteLine(" .GetGenderHint(): {0}", personalIdentityNumber.GetGenderHint().ToString());
    
    // IsTestNumber is an extension method from the package ActiveLogin.Identity.Swedish.TestData
    Console.WriteLine(" .IsTestNumber(): {0}", personalIdentityNumber.IsTestNumber().ToString());
}
else
{
    Console.Error.WriteLine("Unable to parse the input as a SwedishPersonalIdentityNumber.");
}
```

The code above would output (as of 2018-07-23):

```text
SwedishPersonalIdentityNumber
 .ToString(): 199908072391
 .To10DigitString(): 990807-2391
 .To12DigitString(): 199908072391
 .GetDateOfBirthHint(): 1999-08-07
 .GetAgeHint(): 18
 .GetGenderHint(): Male
 .IsTestNumber(): True
```

#### Hints

Some data, such as DateOfBirth, Age and Gender can't be guaranteed to reflect the truth due to the limited quantity of personal identity numbers per day.
Therefore they are exposed as extension methods in the C# api and are suffixed with `Hint` to reflect this. They are also placed in a separate namespace `ActiveLogin.Identity.Swedish.Extensions`. In the F# api these functions are available in the `ActiveLogin.Identity.Swedish.FSharp.SwedishPersonalIdentityNumber.Hints` module.

#### ASP.NET Core MVC

If used to validate input in an ASP.NET Core MVC project, the `SwedishPersonalIdentityNumberAttribute` can be used like this:

```c#
public class SampleDataModel
{
    [SwedishPersonalIdentityNumber]
    public string SwedishPersonalIdentityNumber { get; set; }
}
```

### 3. Use the library in your F# project

`open ActiveLogin.Swedish.Identity.FSharp`

The `SwedishPersonalIdentityNumber`-module provides functions for parsing, creating and converting a `SwedishPersonalIdentityNumber` to its 10- or 12-digit string representation.

```fsharp
"990807-2391" 
|> SwedishPersonalIdentityNumber.parse 
|> function 
| Ok pin -> 
    printfn "%A" pin
    pin |> SwedishPersonalIdentityNumber.to10DigitString |> printfn "to10DigitString: %s"
    pin |> SwedishPersonalIdentityNumber.to12DigitString |> printfn "to12DigitString: %s"
    pin |> SwedishPersonalIdentityNumber.Hints.getDateOfBirthHint |> (fun date -> date.ToShortDateString() |> printfn "getDateOfBirthHint: %s")
    pin |> SwedishPersonalIdentityNumber.Hints.getAgeHint |> printfn "getAgeHint: %i"
    pin |> SwedishPersonalIdentityNumber.Hints.getGenderHint |> printfn "getGenderHint: %A"
    
    // isTestNumber is an extension from the package ActiveLogin.Identity.Swedish.FSharp.TestData
    pin |> SwedishPersonalIdentityNumber.isTestNumber |> printfn "isTestNumber: %b"
| Error e -> printfn "Not a valid Swedish personal identity number. Error %A" e 
```

The code above would output (as of 2018-07-23):

```text
{Year = Year 1999;
 Month = Month 8;
 Day = Day 7;
 BirthNumber = BirthNumber 239;
 Checksum = Checksum 1;}
to10DigitString: 990807-2391
to12DigitString: 199908072391
getDateOfBirthHint: 1999-08-07
getAgeHint: 18
getGenderHint: Male
isTestNumber: true
```

### 4. Browse tests and samples

For more usecases, samples and inspiration; feel free to browse our unit tests and samples:

* [C# ConsoleSample](https://github.com/ActiveLogin/ActiveLogin.Identity/tree/master/samples/ConsoleSample.CSharp)
* [F# ConsoleSample](https://github.com/ActiveLogin/ActiveLogin.Identity/tree/master/samples/ConsoleSample.FSharp)
* [ActiveLogin.Identity.Swedish.Test](https://github.com/ActiveLogin/ActiveLogin.Identity/tree/master/test/ActiveLogin.Identity.Swedish.Test)
* [ActiveLogin.Identity.Swedish.AspNetCore.Test](https://github.com/ActiveLogin/ActiveLogin.Identity/tree/master/test/ActiveLogin.Identity.Swedish.AspNetCore.Test)

## FAQ

### What formats of a Swedish Personal Identity Number do you support parsing?

It will try cleaning away any invalid chars, while still preserving digits and + when that applies.
The "standard" ways of input would be in any of these formats:

* YYMMDD-BBBC
* YYMMDD+BBBC
* YYMMDDBBBC
* YYYYMMDDBBBC

But it also supports other variations such as:

* YYMMDD BBBC
* YYYYMMDD-BBBC
* YYYYMMDD BBBC
* YYYY MM DD BBB C
* YY-MM-DD-BBBC

And basically anything else that can be cleaned and parsed :)

#### Explanations

* **YY:** Year
* **MM:** Month
* **DD:** Day
* **BBB:** Birth number
* **C:** Checksum

### What definition of Swedish Personal Identity Number are the implementations based on?

The implementation is primarily based on the definition defined in Swedish Law:

* [Folkbokföringslagen (FOL 18 §)](https://www.riksdagen.se/sv/dokument-lagar/dokument/svensk-forfattningssamling/folkbokforingslag-1991481_sfs-1991-481#P18)

But when there have been ambiguities we have also read more info and samples from these links from Swedish authorities:
* [Skatteverket (English)](https://www.skatteverket.se/servicelankar/otherlanguages/inenglish/individualsandemployees/livinginsweden/personalidentitynumberandcoordinationnumber.4.2cf1b5cd163796a5c8b4295.html)
* [Skatteverket (Swedish)](https://www.skatteverket.se/privat/folkbokforing/personnummerochsamordningsnummer.4.3810a01c150939e893f18c29.html)
* [Statistiska Centralbyrån (Swedish)](https://www.scb.se/contentassets/8d9d985ca9c84c6e8d879cc89a8ae479/ov9999_2016a01_br_be96br1601.pdf)

Worth noticing is that the date part is not guaranteed to be the exact date you were born, but can be changed for another date within the same month.

### Why are you calling it "Swedish Personal Identity Number" and not Social Security Number?

The Swedish word "personnummer" is translated into ["personal identity number" by Skatteverket](https://www.skatteverket.se/servicelankar/otherlanguages/inenglish/individualsandemployees/livinginsweden/personalidentitynumberandcoordinationnumber.4.2cf1b5cd163796a5c8b4295.html) and that's the translation we decided on using as it's used in official documents.

Unforentunately the term "social security number" or SSN is often used even for a swedish personal identity number, even though that is misleading as a [SSN is something used in the United States](https://en.wikipedia.org/wiki/Social_Security_number) and should not be mixed up with a PIN.

### What data are you using for tests and samples?

To comply with GDPR and not no expose any real PINs, we are using the official test data for Swedish Personal Identity Numbers [provided by Skatteverket](https://skatteverket.entryscape.net/catalog/9/datasets/147).

If you need to use test numbers yourself, for example if you need to write tests using personal identity numbers, but want to avoid violating GDPR, we provide a nuget package with a simple api to get random test numbers or strings. You can also check if a personal identity number is a test number.

#### C#
```csharp
using ActiveLogin.Identity.Swedish.TestData; 

var aTestNumber = SwedishPersonalIdentityNumberTestData.GetRandom();
aTestNumber.IsTestNumber(); // => true
```

#### F#
```fsharp
open ActiveLogin.Identity.Swedish.TestData.FSharp

let aTestNumber = SwedishPersonalIdentityNumberTestData.getRandom()
aTestNumber |> SwedishPersonalIdentityNumber.isTestNumber // => true
```

### When should I use `...InSpecificYear(...)`?

Some forms of a Swedish Personal Identity Number depends of the age of the person it represents.
The "-" will be replaced with a "+" on January 1st the year a person turns 100 years old. Therefore these methods (`.To10DigitStringInSpecificYear(...)`, `.ParseInSpecificYear(...)`, `.TryParseInSpecificYear(...)`) exists to define at what year the the data should be represented or parsed.
Useful for parsing old data or printing data for the future.

## Active Login

_Integrating your systems with market leading authentication services._

Active Login is an Open Source project built on .NET Standard that makes it easy to integrate with leading Swedish authentication services like [BankID](https://www.bankid.com/).

It also provide examples of how to use it with the popular OpenID Connect & OAuth 2.0 Framework [IdentityServer](https://identityserver.io/) and provides a template for hosting the solution in Microsoft Azure.
In addition, Active Login also contain convenient modules that help you work with and handle validation of Swedish Personal Identity Number (svenskt personnummer).

### Contribute

We are very open to community contributions to Active Login. You'll need a basic understanding of Git and GitHub to get started. The easiest way to contribute is to open an issue and start a discussion. If you make code changes, submit a pull request with the changes and a description. Don’t forget to always provide tests that cover the code changes. 

### License & acknowledgements

Active Login is licensed under the very permissive [MIT license](https://opensource.org/licenses/MIT) for you to be able to use it in commercial or non-commercial applications without many restrictions.

Active Login is built on or uses the following great open source products:

* [.NET Standard](https://github.com/dotnet/standard)
* [ASP.NET Core](https://github.com/aspnet/Home)
* [XUnit](https://github.com/xunit/xunit)
* [IdentityServer](https://github.com/IdentityServer/)
* [Bootstrap](https://github.com/twbs/bootstrap)
* [Font Awesome](https://github.com/FortAwesome/Font-Awesome)

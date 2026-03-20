open Expecto

[<EntryPoint>]
let main argv =
    let allTests =
        testList "All Tests" [
             ActiveLogin.Identity.Swedish.FSharp.Test.PersonalIdentityNumber_Constructor.tests
             ActiveLogin.Identity.Swedish.FSharp.Test.CoordinationNumber_Constructor.tests
             ActiveLogin.Identity.Swedish.FSharp.Test.PersonalIdentityNumber_equality.tests
             ActiveLogin.Identity.Swedish.FSharp.Test.CoordinationNumber_equality.tests
             ActiveLogin.Identity.Swedish.FSharp.Test.PersonalIdentityNumber_hash.tests
             ActiveLogin.Identity.Swedish.FSharp.Test.CoordinationNumber_hash.tests
             ActiveLogin.Identity.Swedish.FSharp.Test.PersonalIdentityNumber_Parse.tests
             ActiveLogin.Identity.Swedish.FSharp.Test.PersonalIdentityNumber_ParseStrict.tests
             ActiveLogin.Identity.Swedish.FSharp.Test.CoordinationNumber_Parse.tests
             ActiveLogin.Identity.Swedish.FSharp.Test.CoordinationNumber_ParseStrict.tests
             ActiveLogin.Identity.Swedish.FSharp.Test.PersonalIdentityNumber_Hints.tests
             ActiveLogin.Identity.Swedish.FSharp.Test.CoordinationNumber_Hints.tests
        ]

    runTestsWithCLIArgs [] [||] allTests

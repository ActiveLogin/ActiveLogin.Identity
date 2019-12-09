namespace ActiveLogin.Identity.Common.FSharp

type ResultBuilder() =
    member __.Return x = Ok x
    member __.Bind(xResult, f) = Result.bind f xResult
    member __.ReturnFrom(x) = x

[<AutoOpen>]
module ResultBuilder =
    let result = ResultBuilder()

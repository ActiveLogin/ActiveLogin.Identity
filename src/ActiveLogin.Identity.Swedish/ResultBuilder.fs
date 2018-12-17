namespace ActiveLogin.Identity.Swedish.FSharp
type internal ResultBuilder() =
    member __.Return x = Ok x
    member __.Zero() = Ok ()
    member __.Bind(xResult,f) = Result.bind f xResult
    member __.ReturnFrom(x) = x

[<AutoOpen>]
module internal ResultBuilder =
    let result = ResultBuilder()
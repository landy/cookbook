// Learn more about F# at http://fsharp.org

open System.IO
open Cookbook.Server.GraphQL
open FSharp.Data.GraphQL
open Cookbook.Server.GraphQL.HttpHandlers

let writeFile path json =
    System.IO.File.WriteAllTextAsync(path, json)

[<EntryPoint>]
let main argv =
    let path =
        argv |> Array.tryItem 0
        |> Option.defaultWith (fun _ ->
            "../../cookbookSchema.json"
            |> Path.GetFullPath
        )
    let introspect =
        CookbookSchema.executor.AsyncExecute(Introspection.IntrospectionQuery)
        |> Async.RunSynchronously

    introspect
    |> GQLResponse.toJson
    |> writeFile path
    |> Async.AwaitTask
    |> Async.RunSynchronously

    printfn "Schema written to file '%s'" path
    0 // return an integer exit code

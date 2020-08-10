module Cookbook.Server.GraphQL.HttpHandlers

open System.IO
open System.Text
open FSharp.Control.Tasks.V2
open FSharp.Data.GraphQL
open Giraffe
open Microsoft.AspNetCore.Http
open FsToolkit.ErrorHandling

open Cookbook.Server.Users.Domain


[<RequireQualifiedAccess>]
module private GQLResponse =
    open FSharp.Data.GraphQL.Execution

    let toJson =
        function
        | Direct (data, _) ->
            Serialization.serialize data
        | Deferred (data, _, deferred) ->
            deferred |> Observable.add(fun d -> printfn "Deferred: %s" (Serialization.serialize d))
            Serialization.serialize data
        | Stream data ->
            data |> Observable.add(fun d -> printfn "Subscription data: %s" (Serialization.serialize d))
            "{}"


let private removeWhitespacesAndLineBreaks (str : string) = str.Trim().Replace("\r\n", " ")

let readStream (s : Stream) =
    task {
        use ms = new MemoryStream(4096)
        do! s.CopyToAsync(ms)
        return ms.ToArray()
    }

let tryGetQuery (data: Map<string,obj> option) =
    data
    |> Option.bind (Map.tryFind "query")
    |> Option.map (fun s ->
        match s with
        | :? string as x -> x
        | _ -> (failwith "Failure deserializing repsonse. Could not read query - it is not stringified in request.")
    )

let tryGetVariables (data: Map<string,obj> option) =
    data
    |> Option.bind (Map.tryFind "variables")
    |> Option.bind (function
        | null -> None
        | :? string as x -> Serialization.deserialize x
        | :? Map<string, obj> as x -> Some x
        | _ -> failwith "Failure deserializing response. Could not read variables - it is not a object in the request."
    )

let okWithStr str : HttpHandler = setStatusCode 200 >=> text str

let setCorsHeaders : HttpHandler =
    setHttpHeader "Access-Control-Allow-Origin" "*"
    >=> setHttpHeader "Access-Control-Allow-Headers" "content-type"

let setContentTypeAsJson : HttpHandler =
    setHttpHeader "Content-Type" "application/json"

let graphQL (next : HttpFunc) (ctx : HttpContext) = task {
    let! data =
        ctx.Request.Body
        |> readStream
        |> Task.map Encoding.UTF8.GetString
        |> Task.map Serialization.deserialize

    let queryOpt = data |> tryGetQuery
    let variablesOpt = data |> tryGetVariables

    match queryOpt, variablesOpt  with
    | Some query, Some variables ->
        let userDb = ctx.GetService<UsersStore>()
        let root : CookbookSchema.Root = { UserDb = userDb }

        let query = removeWhitespacesAndLineBreaks query
        let! result = CookbookSchema.executor.AsyncExecute(query, root, variables) |> Async.StartAsTask
        return! okWithStr (GQLResponse.toJson result) next ctx
    | Some query, None ->
        let userDb = ctx.GetService<UsersStore>()
        let root : CookbookSchema.Root = { UserDb = userDb }
        let query = removeWhitespacesAndLineBreaks query
        let! result = CookbookSchema.executor.AsyncExecute(query,data = root) |> Async.StartAsTask
        return! okWithStr (GQLResponse.toJson result) next ctx
    | None, _ ->
        let! result = CookbookSchema.executor.AsyncExecute(Introspection.IntrospectionQuery) |> Async.StartAsTask
        printfn "Result metadata: %A" result.Metadata
        return! okWithStr (GQLResponse.toJson result) next ctx
}

let graphQLHandler : HttpHandler =
    setCorsHeaders
    >=> graphQL
    >=> setContentTypeAsJson
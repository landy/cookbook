module Household.Api.Server.Remoting

open System
open Fable.Remoting.Server
open Microsoft.AspNetCore.Http
open Household.Api.Shared.Errors
open Microsoft.Extensions.Logging

[<RequireQualifiedAccess>]
module Remoting =
    let private statusCode = function
        | Exception _ -> 500
        | Validation _ -> 400

    let rec errorHandler (log:ILogger) (ex: Exception) (routeInfo: RouteInfo<HttpContext>) =
        log.LogError(ex, ex.Message)
        match ex with
        | ServerException err ->
            routeInfo.httpContext.Response.StatusCode <- err |> statusCode
            Propagate err
        | e when e.InnerException |> isNull |> not -> errorHandler log e.InnerException routeInfo
        | _ ->
            Propagate (ServerError.Exception(ex.Message))
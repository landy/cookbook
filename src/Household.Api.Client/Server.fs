module Cookbook.Client.Server

open Fable.Core.JS
open Fable.Core
open Fable.Remoting.Client
open Fable.SimpleJson

open Cookbook.Shared.Errors
open Cookbook.Shared

[<Emit("config.baseUrl")>]
let baseUrl : string = jsNative


type RemoteReadData<'a> =
    | Idle
    | InProgress
    | Finished of 'a

module RemoteReadData =
    let init = Idle
    let isInProgress = function
        | InProgress -> true
        | _ -> false
    let setInProgress = InProgress
    let setResponse r = Finished r

type RemoteData<'data,'response,'error> = {
    Data : 'data
    Response : 'response option
    InProgress : bool
    Errors : 'error list
}

module RemoteData =
    let noValidation _ = []
    let init data validationFn = { Data = data; Response = None; InProgress = false; Errors = data |> validationFn  }
    let setData value validationFn t = { t with Data = value; InProgress = false; Errors = value |> validationFn }
    let applyValidationErrors errors t = { t with Errors = errors @ t.Errors }
    let getData t = t.Data
    let isInProgress t = t.InProgress
    let setInProgress t = { t with InProgress = true }
    let setResponse r t = { t with InProgress = false; Response = Some r }
    let clearResponse t = { t with Response = None }
    let hasResponse t = t.Response.IsSome
    let isValid t = t.Errors.Length = 0
    let isNotValid t = t |> isValid |> not

let exnToError (e:exn) : ServerError =
    console.error e
    match e with
    | :? ProxyRequestException as ex ->
        try
            let serverError = Json.parseAs<{| error: ServerError |}>(ex.Response.ResponseBody)
            serverError.error
        with _ -> ServerError.Exception(e.Message)
    | _ -> ServerError.Exception(e.Message)
type ServerResult<'a> = Result<'a,ServerError>

module ServerResult =
    let getValidationErrors = function
        | Error (Validation errs) -> errs
        | _ -> []
    let isOk = function
        | Ok _ -> true
        | _ -> false

module Cmd =
    open Elmish

    module OfAsync =
        let eitherAsResult fn resultMsg =
            Cmd.OfAsync.either fn () (Result.Ok >> resultMsg) (exnToError >> Result.Error >> resultMsg)


    /// A proxy you can use to talk to server directly
let usersService : Users.UsersService =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Users.Route.builder
    |> Remoting.withBinarySerialization
    |> Remoting.withBaseUrl baseUrl
    |> Remoting.buildProxy<Users.UsersService>


let onRecipesService (fn: Recipes.RecipesService -> Async<'a>) =
    let builder =
        Remoting.createApi()
        |> Remoting.withRouteBuilder Recipes.Route.builder
        |> Remoting.withBinarySerialization
        |> Remoting.withBaseUrl baseUrl
        |> Remoting.buildProxy<Recipes.RecipesService>
    fn builder
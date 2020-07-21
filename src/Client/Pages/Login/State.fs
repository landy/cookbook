module Cookbook.Client.Pages.Login.State
open System
open Elmish
open FsToolkit.ErrorHandling

open Cookbook.Shared.Auth
open Cookbook.Shared.Errors
open Cookbook.Client.Router
open Cookbook.Client.Server

type LoginForm = {
    Username : string
    Password : string
}
type Model = {
    Form : LoginForm
    Errors : string list
    IsLoading : bool
}



type Msg =
    | UsernameChanged of string
    | PasswordChanged of string
    | Login
    | LoggedIn of Result<Response.Token, ApplicationError>

let stateInit () =
    {
        Form = { Username = String.Empty; Password = String.Empty }
        Errors = []
        IsLoading = false
    }
let init () =
     stateInit(), Cmd.none

let private handleLogin state :Cmd<Msg> =
    let fn (dispatch: Msg -> unit) : unit =
        async {
            let! res = authService.Login ({Username = state.Username; Password = state.Password}:Request.Login)
            dispatch (LoggedIn res)
        }
        |> Async.StartImmediate
    Cmd.ofSub fn

type LoginPageProps = {
    handleNewToken:(Response.Token option -> unit)
}

let update (props:LoginPageProps) msg state =
    match msg with
    | UsernameChanged username ->
        {state with Form = {state.Form with Username = username}}, Cmd.none
    | PasswordChanged password ->
        {state with Form = {state.Form with Password = password}}, Cmd.none
    | Login ->
        let state' = { state with Errors = []; IsLoading = true }
        state', handleLogin state.Form
    | LoggedIn res ->
        let state' = {state with IsLoading = false}
        match res with
        | Ok t ->

            stateInit(), Cmd.ofSub (fun _ -> t |> Some |> props.handleNewToken)
        | Error err ->
            {state' with Errors =  (err |> ApplicationError.explain) :: state'.Errors}, Cmd.none
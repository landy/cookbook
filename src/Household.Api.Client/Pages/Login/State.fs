module Household.Api.Client.Pages.Login.State
open System
open Household.Api.Shared.Users.Response
open Elmish
open FsToolkit.ErrorHandling

open Household.Api.Shared.Users
open Household.Api.Shared.Errors
open Household.Api.Client.Router
open Household.Api.Client.Server
open Household.Api.Client.Pages.Login.Domain


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
            let! res = usersService.Login ({Username = state.Username; Password = state.Password}:Request.Login)
            dispatch (LoggedIn res)
        }
        |> Async.StartImmediate
    Cmd.ofSub fn

type LoginPageProps = {
    handleNewToken:(Response.UserSession option -> unit)
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
    | LoggedIn userSession ->
        stateInit(), Cmd.ofSub (fun _ -> userSession |> Some |> props.handleNewToken)
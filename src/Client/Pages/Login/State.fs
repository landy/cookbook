module Cookbook.Client.Pages.Login.State
open System
open Cookbook.Shared.Auth
open Elmish

open Cookbook.Client.Server


type Model =
    {
        Username : string
        Password : string
    }

type Msg =
    | UsernameChanged of string
    | PasswordChanged of string
    | FormSubmitted




let init () =
    { Username = String.Empty; Password = String.Empty },Cmd.none

let update msg state =
    match msg with
    | UsernameChanged username ->
        {state with Username = username}, Cmd.none
    | PasswordChanged password ->
        {state with Password = password}, Cmd.none
    | FormSubmitted ->
        async {
            let! retval = authService.Login ({Username = state.Username; Password = state.Password}:Request.Login)
            printfn "returned: %s" retval
            return ()
        }
        |> Async.StartImmediate
//        |> printfn "result: %s"
        state,Cmd.none
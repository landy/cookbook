module Cookbook.Client.Pages.UsersAdd.State

open Elmish
open FsToolkit.ErrorHandling

open Cookbook.Shared.Errors
open Cookbook.Shared.Users
open Cookbook.Client.Server
open Domain


let init =
    {
        FormData = {
            Username = ""
            Name = ""
            Password = ""
            ConfirmPassword = ""
        }
        IsSaving = false
    }, Cmd.none

let update (msg:Msg) (state:Model) =
    match msg with
    | SaveUser Started ->
        let user : Request.AddUser =
            {
                Username = state.FormData.Username
                Name = state.FormData.Name
                Password = state.FormData.Password
            }

        let op = usersService.SaveUser user |> Async.map (Finished >> SaveUser)
        {state with IsSaving = true}, Cmd.OfAsync.result op
    | SaveUser (Finished res) ->
        let state' = {state with IsSaving = false}
        match res with
        | Ok _ -> state', Cmd.none
        | Error err ->
            System.Console.WriteLine(err |> ApplicationError.explain)
            state', Cmd.none

    | FormChanged update ->
        {state with FormData = state.FormData |> update}, Cmd.none
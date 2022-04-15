module Cookbook.Client.Pages.UsersList.State

open Elmish
open FsToolkit.ErrorHandling

open Cookbook.Client.Server
open Domain

let init () =
    {
        Users = []
        IsLoading = false
    }, Cmd.ofMsg LoadUsers

let update  msg state =
    match msg with
    | LoadUsers ->
        let loadUsers = async {
            let! users = usersService.GetUsers ()

            return
                users
                |> List.map (fun u ->
                    {Username = u.Username; Name = u.Name}
                )
                |> UsersLoaded
        }


        {state with IsLoading = true}, Cmd.OfAsync.result loadUsers
    | UsersLoaded users ->
        {state with Users = users; IsLoading = false}, Cmd.none
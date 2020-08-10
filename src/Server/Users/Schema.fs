module Cookbook.Server.Users.Schema

open FsToolkit.ErrorHandling
open FSharp.Data.GraphQL.Types

open Cookbook.Shared.Users
open Domain

let private userViewToRow (view:Views.CookbookUser) : Response.UserRow =
    {Username = view.Username; Name = view.Name}

let getUsers (userDb:UsersStore) =
    userDb.getUsers ()
    |> Task.map (List.map userViewToRow)
    |> Async.AwaitTask

open Cookbook.Shared.Users.Response
let UserType =
    Define.Object<UserRow>(
        name = "User",
        fields = [
            Define.Field("username", String, fun _ u -> u.Username)
            Define.Field("name", String, fun _ u -> u.Name)
        ]
    )
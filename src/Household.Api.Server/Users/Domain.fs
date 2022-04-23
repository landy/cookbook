module Cookbook.Server.Users.Domain

open System
open System.Threading.Tasks

open Cookbook.Shared.Errors
open Cookbook.Libraries
open FsToolkit.ErrorHandling



[<RequireQualifiedAccess>]
module Views =
    type CookbookUser = {
        Username : string
        Name : string
    }

[<RequireQualifiedAccess>]
module CmdArgs =

    type AddNewUser = {
        Username : string
        Password : string
        Name : string
    }

module EventArgs =

    type UserAdded = {
        Username : string
        PasswordHash : string
        Name : string
    }

type UsersStore =
    abstract tryFindUser : string -> Task<Views.CookbookUser option>
    abstract setRefreshToken : string -> string -> DateTimeOffset -> Task<unit>
    abstract getUsers : unit -> Task<Views.CookbookUser list>
    abstract addUser : EventArgs.UserAdded -> Task<unit>


type Command =
    | AddNewUser of CmdArgs.AddNewUser

type Event =
    | UserAdded of EventArgs.UserAdded


open EventArgs
let execute cmd =
    match cmd with
    | AddNewUser args ->
        {
            Username = args.Username
            PasswordHash = Password.createHash args.Password
            Name = args.Name
        }
        |> UserAdded

let handle (usersDb : UsersStore) evnt =
    match evnt with
    | UserAdded args ->
        usersDb.addUser args
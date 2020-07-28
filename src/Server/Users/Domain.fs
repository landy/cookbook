module Cookbook.Server.Users.Domain

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

[<RequireQualifiedAccess>]
module EventArgs =

    type UserAdded = {
        Username : string
        Name : string
    }

type Command =
    | AddNewUser of CmdArgs.AddNewUser

type Event =
    | UserAdded of EventArgs.UserAdded
module Cookbook.Shared.Users

open System

open Cookbook.Shared.Errors

[<RequireQualifiedAccess>]
module Request =

    type Login = {
        Username : string
        Password : string
    }

    type AddUser = {
        Username : string
        Name : string
        Password : string
    }

module Response =

    type Token = {
        Token : string
        ExpiresUtc : DateTimeOffset
    }
    type UserSession = {
        Username : string
        Name : string
        Token : Token
        RefreshToken : Token
    }

    type UserRow = {
        Username : string
        Name : string
    }

[<RequireQualifiedAccess>]
module Route =
    let builder _ m =
        sprintf "/api/users/%s" m

type UsersService = {
    Login : Request.Login -> Async<Result<Response.UserSession, ApplicationError>>
    GetUsers : unit -> Async<Result<Response.UserRow list, ApplicationError>>
    SaveUser : Request.AddUser -> Async<Result<unit, ApplicationError>>
}
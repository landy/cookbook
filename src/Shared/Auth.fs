module Cookbook.Shared.Auth

open System

open Cookbook.Shared.Errors

[<RequireQualifiedAccess>]
module Request =

    type Login = {
        Username : string
        Password : string
    }

module Response =

    type Token = {
        Token : string
        ExpiresUtc : DateTimeOffset
    }
    type LoggedInUser = {
        Username : string
        Name : string
        Token : Token
        RefreshToken : Token
    }

[<RequireQualifiedAccess>]
module Route =
    let builder _ m =
        sprintf "/api/auth/%s" m

type AuthService = {
    Login : Request.Login -> Async<Result<Response.LoggedInUser, ApplicationError>>
}
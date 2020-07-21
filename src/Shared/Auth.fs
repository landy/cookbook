module Cookbook.Shared.Auth

open System

open Cookbook.Shared.Errors

[<RequireQualifiedAccess>]
module Request =

    type Login = {
        Username : string
        Password : string
    }

[<RequireQualifiedAccess>]
module Response =

    type Token = {
        Token : string
        ExpiresOnUtc : DateTime
    }

[<RequireQualifiedAccess>]
module Route =
    let builder _ m =
        sprintf "/api/auth/%s" m

type AuthService = {
    Login : Request.Login -> Async<Result<Response.Token, ApplicationError>>
}
module Cookbook.Shared.Auth

[<RequireQualifiedAccess>]
module Request =

    type Login = {
        Username : string
        Password : string
    }

[<RequireQualifiedAccess>]
module Route =
    let builder _ m =
        sprintf "/api/auth/%s" m

type AuthService = {
    Login : Request.Login -> Async<string>
}
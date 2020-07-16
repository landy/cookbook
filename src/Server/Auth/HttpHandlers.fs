module Cookbook.Server.Auth.HttpHandlers

open System
open System.Security.Claims
open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open FSharp.Control.Tasks.V2
open Giraffe

open Cookbook.Shared.Auth

open Cookbook.Server.Auth.Domain

let private toClaims (user:User) =
    [
        Claim(ClaimTypes.Sid, user.Username)
        Claim(ClaimTypes.Name, user.Name)
    ]

[<Literal>]
let private Secret = "5CEFF3A9-949B-483C-B8FC-96F98D557102"

let private login (r:Request.Login) =
    task {
        return
            { Username = r.Username; Name = "Landy" }
            |> toClaims
            |> Jwt.createToken "testAudience" "cookbook.net" Secret (TimeSpan.FromDays(14.))
            |> fun t -> t.Token

    }

let private authService = {
    Login = login >> Async.AwaitTask
}

let authServiceHandler : HttpHandler =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue authService
    |> Remoting.buildHttpHandler

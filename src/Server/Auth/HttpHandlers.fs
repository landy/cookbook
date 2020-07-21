module Cookbook.Server.Auth.HttpHandlers

open System
open Cookbook.Shared.Errors
open Microsoft.AspNetCore.Http
open System.Security.Claims
open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open FSharp.Control.Tasks.V2
open Giraffe
open FsToolkit.ErrorHandling

open Cookbook.Shared.Auth
open Cookbook.Server.Auth.Database
open Cookbook.Server.Auth.Domain

let private toClaims (user:CookbookUser) =
    [
        Claim(ClaimTypes.Sid, user.Username)
        Claim(ClaimTypes.Name, user.Name)
    ]

[<Literal>]
let private Secret = "5CEFF3A9-949B-483C-B8FC-96F98D557102"

let private login (usersDb:UserStore) (r:Request.Login) =
    task {
        let! user = usersDb.tryFindUser r.Username
        return
            user
            |> Option.map (fun u ->
                { Username = u.Username; Name = u.Name }
                |> toClaims
                |> Jwt.createToken "testAudience" "cookbook.net" Secret (TimeSpan.FromDays(14.))
            )
            |> Result.requireSome (AuthenticationError.InvalidUsernameOrPassword |> ApplicationError.AuthenticationError)
    }

let private authService usersDb = {
    Login = login usersDb >> Async.AwaitTask
}

let private createAuthServiceFromContext (httpContext: HttpContext) =
    let usersDb = httpContext.GetService<UserStore>()
    authService usersDb

let authServiceHandler : HttpHandler =
    Remoting.createApi()
    |> Remoting.withErrorHandler (fun ex _  -> Propagate ex)
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromContext createAuthServiceFromContext
    |> Remoting.buildHttpHandler

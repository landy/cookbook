module Cookbook.Server.Users.HttpHandlers

open System
open System.Threading.Tasks
open FsToolkit.ErrorHandling.Operator.TaskResult
open Microsoft.AspNetCore.Http
open System.Security.Claims
open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open FSharp.Control.Tasks.V2
open Giraffe
open FsToolkit.ErrorHandling

open Cookbook.Libraries
open Cookbook.Shared.Errors
open Cookbook.Shared.Users
open Cookbook.Server.Users.Database
open Cookbook.Server.Users.Domain
open Cookbook.Shared.Users.Response

let private toClaims (user:Views.CookbookUser) =
    [
        Claim(ClaimTypes.Sid, user.Username)
        Claim(ClaimTypes.Name, user.Name)
    ]

[<Literal>]
let private Secret = "5CEFF3A9-949B-483C-B8FC-96F98D557102"

let private login (usersDb:UsersStore) (r:Request.Login) =
    task {
        let! user = usersDb.tryFindUser r.Username
        return!
            user
            |> Option.map (fun u ->

                let token, expiresOn =
                    ({ Username = u.Username; Name = u.Name } : Views.CookbookUser)
                    |> toClaims
                    |> Jwt.createToken "testAudience" "cookbook.net" Secret (TimeSpan.FromHours(1.))
                let (refreshToken,refreshExpiresOn) =
                    Jwt.createRefreshToken Jwt.DefaultRefreshKeyLength (TimeSpan.FromDays(14.))
                {
                    Username = u.Username
                    Name = u.Name
                    Token = {Token = token; ExpiresUtc = expiresOn}
                    RefreshToken = {Token = refreshToken; ExpiresUtc = refreshExpiresOn}
                }
            )
            |> Result.requireSome (AuthenticationError.InvalidUsernameOrPassword |> ApplicationError.AuthenticationError)
            |> Result.map (fun (t:LoggedInUser) ->
                usersDb.setRefreshToken t.Username t.RefreshToken.Token t.RefreshToken.ExpiresUtc
                |> Task.map (fun _ -> t)
            )
            |> Result.sequenceTask
    }

let private getUsers (usersDb:UsersStore) () =
    usersDb.getUsers ()
    |> Task.map (
        List.map (fun u ->
            {Username = u.Username; Name = u.Name}
        )
        >> Ok
    )

let private saveUser (usersDb : UsersStore) (user:Request.AddUser) =
    usersDb.tryFindUser user.Username
    |> TaskResult.requireNone (UserError.UserAlreadyExists user.Username)
    |> TaskResult.mapError ApplicationError.UserError
    |> TaskResult.bind (fun _ ->
        ({
            Username = user.Username
            Name = user.Name
            Password = user.Password
        } : CmdArgs.AddNewUser)
        |> AddNewUser
        |> execute
        |> handle usersDb
        |> TaskResult.mapError ApplicationError.DatabaseError
    )

    //Task.FromResult (UserError.UserAlreadyExists user.Username |> ApplicationError.UserError |> Error)

let private usersService usersDb = {
    Login = login usersDb >> Async.AwaitTask
    GetUsers = getUsers usersDb >> Async.AwaitTask
    SaveUser = saveUser usersDb >> Async.AwaitTask
}

let private createAuthServiceFromContext (httpContext: HttpContext) =
    let usersDb = httpContext.GetService<UsersStore>()
    usersService usersDb

let authServiceHandler : HttpHandler =
    Remoting.createApi()
    |> Remoting.withErrorHandler (fun ex _  -> Propagate ex)
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromContext createAuthServiceFromContext
    |> Remoting.buildHttpHandler

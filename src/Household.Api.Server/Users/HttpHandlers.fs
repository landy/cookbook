module Household.Api.Server.Users.HttpHandlers

open System
open System.Threading.Tasks
open FsToolkit.ErrorHandling.Operator.TaskResult
open Microsoft.AspNetCore.Http
open System.Security.Claims
open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Giraffe
open FsToolkit.ErrorHandling

open Household.Libraries
open Household.Api.Shared.Errors
open Household.Api.Shared.Users
open Household.Api.Server.Users.Database
open Household.Api.Server.Users.Domain
open Household.Api.Shared.Users.Response

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
                printfn $"user %s{u.Name}"
                let token, expiresOn =
                    ({ Username = u.Username; Name = u.Name } : Views.CookbookUser)
                    |> toClaims
                    |> Jwt.createToken "testAudience" "cookbook.net" Secret (TimeSpan.FromHours(1.))
                let refreshToken,refreshExpiresOn =
                    Jwt.createRefreshToken Jwt.DefaultRefreshKeyLength (TimeSpan.FromDays(14.))
                {
                    Username = u.Username
                    Name = u.Name
                    Token = {Token = token; ExpiresUtc = expiresOn}
                    RefreshToken = {Token = refreshToken; ExpiresUtc = refreshExpiresOn}
                }
            )
            |> Option.defaultWith (fun _ ->"Invalid username or password" |> failwith)
            |> (fun (t:UserSession) ->
                usersDb.setRefreshToken t.Username t.RefreshToken.Token t.RefreshToken.ExpiresUtc
                |> Task.map (fun _ -> t)
            )
    }

let private getUsers (usersDb:UsersStore) () =
    usersDb.getUsers ()
    |> Task.map (
        List.map (fun u ->
            {Username = u.Username; Name = u.Name}
        )
    )

let private createSaveUser (user:Request.AddUser) =
    ({
        Username = user.Username
        Name = user.Name
        Password = user.Password
    } : CmdArgs.AddNewUser)
    |> AddNewUser


let private usersService usersDb =
    let pipeline = CommandHandlers.pipeline usersDb
    {
        Login = login usersDb >> Async.AwaitTask
        GetUsers = getUsers usersDb >> Async.AwaitTask
        SaveUser = createSaveUser >> pipeline >> Async.AwaitTask
    }

let private createAuthServiceFromContext (httpContext: HttpContext) =
    let usersDb = httpContext.GetService<UsersStore>()
    usersService usersDb

let authServiceHandler : HttpHandler =
    Remoting.createApi()
    |> Remoting.withErrorHandler (fun ex _  -> Propagate ex)
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.withBinarySerialization
    |> Remoting.fromContext createAuthServiceFromContext
    |> Remoting.buildHttpHandler
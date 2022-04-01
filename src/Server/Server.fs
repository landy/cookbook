open System
open System.IO
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Azure.Cosmos
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Giraffe

open Cookbook.Server.Users.Domain
open Cookbook.Server.Users.Database
open Cookbook.Libraries.CosmosDb
open Cookbook.Server
open Cookbook.Server.Configuration


let tryGetEnv = System.Environment.GetEnvironmentVariable >> function null | "" -> None | x -> Some x

let contentRoot = tryGetEnv "public_path" |> Option.defaultValue "." |> Path.GetFullPath
let wwwRoot = tryGetEnv "public_path" |> Option.defaultValue "./public" |> Path.GetFullPath
let port =
    "SERVER_PORT"
    |> tryGetEnv |> Option.map uint16 |> Option.defaultValue 5000us


let webApp =
    choose [
        Users.HttpHandlers.authServiceHandler
        htmlFile <| Path.Combine(wwwRoot, "index.html")
    ]


type Startup (cfg:IConfiguration) =

    member _.ConfigureServices (sc:IServiceCollection) =
        sc.AddApplicationInsightsTelemetry() |> ignore

        let dbServer = cfg.["cosmosDb:connectionString"]
        let dbKey = cfg.["cosmosDb:key"]

        let dbName = cfg.["cosmosDb:databaseName"]
        let usersContainer = cfg.["cosmosDb:containers:users"]
        let refreshTokensContainer = cfg.["cosmosDb:containers:refreshTokens"]

        let dbConfig = {
            DatabaseName = dbName
            UsersContainerName = usersContainer
            RefreshTokensContainerName = refreshTokensContainer

        }

        let client = createCosmosClient dbServer dbKey
        sc.AddSingleton<CosmosClient>(client) |> ignore
        sc.AddSingleton<DatabaseConfiguration>(dbConfig) |> ignore
        sc.AddSingleton<UsersStore, CosmosDbUserStore>() |> ignore
        sc.AddGiraffe() |> ignore
        tryGetEnv "APPINSIGHTS_INSTRUMENTATIONKEY" |> Option.iter (sc.AddApplicationInsightsTelemetry >> ignore)

    member _.Configure(app:IApplicationBuilder, env:IWebHostEnvironment) =


        app.UseDefaultFiles()
            .UseStaticFiles()
            .UseGiraffe webApp


WebHost
    .CreateDefaultBuilder()
    .UseWebRoot(wwwRoot)
    .UseContentRoot(contentRoot)
    .UseStartup<Startup>()
    .Build()
    .Run()
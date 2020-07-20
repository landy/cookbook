open System
open System.IO
open Cookbook.Server.Auth.Database
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Azure.Cosmos
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Microsoft.WindowsAzure.Storage

open Cookbook.Libraries.CosmosDb
open Cookbook.Server
open Cookbook.Server.Configuration


let tryGetEnv = System.Environment.GetEnvironmentVariable >> function null | "" -> None | x -> Some x

let contentRoot = tryGetEnv "public_path" |> Option.defaultValue "." |> Path.GetFullPath
let wwwRoot = tryGetEnv "public_path" |> Option.defaultValue "./public" |> Path.GetFullPath
let storageAccount = tryGetEnv "STORAGE_CONNECTIONSTRING" |> Option.defaultValue "UseDevelopmentStorage=true" |> CloudStorageAccount.Parse
let port =
    "SERVER_PORT"
    |> tryGetEnv |> Option.map uint16 |> Option.defaultValue 8085us



let webApp =
    choose [
        Auth.HttpHandlers.authServiceHandler
    ]


type Startup (cfg:IConfiguration) =

    member _.ConfigureServices (sc:IServiceCollection) =
        let dbServer = cfg.["cosmosDbConnection"]
        let dbKey = cfg.["cosmosDbKey"]

        let dbName = cfg.["cosmosDb:databaseName"]
        let usersContainer = cfg.["cosmosDb:containers:users"]

        let dbConfig = {DatabaseName = dbName; UsersContainerName = usersContainer}

        let client = createCosmosClient dbServer dbKey
        sc.AddSingleton<CosmosClient>(client) |> ignore
        sc.AddSingleton<DatabaseConfiguration>(dbConfig) |> ignore
        sc.AddSingleton<UserStore, CosmosDbUserStore>() |> ignore
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
    .UseUrls("http://0.0.0.0:" + port.ToString() + "/")
    .Build()
    .Run()

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

open Cookbook.Server
open Cookbook.Server.Configuration


let tryGetEnv = System.Environment.GetEnvironmentVariable >> function null | "" -> None | x -> Some x

let publicPath = tryGetEnv "public_path" |> Option.defaultValue "../Client/public" |> Path.GetFullPath
let storageAccount = tryGetEnv "STORAGE_CONNECTIONSTRING" |> Option.defaultValue "UseDevelopmentStorage=true" |> CloudStorageAccount.Parse
let port =
    "SERVER_PORT"
    |> tryGetEnv |> Option.map uint16 |> Option.defaultValue 8085us



let webApp =
    choose [
        Auth.HttpHandlers.authServiceHandler
    ]

let private createCosmosClient endpoint authKey =
    let opts = CosmosClientOptions()
    opts.ConnectionMode <- ConnectionMode.Direct
    new CosmosClient(endpoint, authKey, opts)

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
        printfn "env: %s" env.EnvironmentName
        app.UseDefaultFiles()
            .UseStaticFiles()
            .UseGiraffe webApp

WebHost
    .CreateDefaultBuilder()
    //.UseWebRoot(publicPath)
    //.UseContentRoot(publicPath)
    .UseStartup<Startup>()
    .UseUrls("http://0.0.0.0:" + port.ToString() + "/")
    .Build()
    .Run()

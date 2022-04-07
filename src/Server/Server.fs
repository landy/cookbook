open System
open System.IO
open Cookbook.Server.Recipes.Database
open Cookbook.Server.Recipes.Domain
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
        Recipes.HttpHandlers.recipesHandler
        htmlFile <| Path.Combine(wwwRoot, "index.html")
    ]


type Startup (cfg:IConfiguration) =

    member _.ConfigureServices (sc:IServiceCollection) =
        sc.AddApplicationInsightsTelemetry() |> ignore

        let dbServer = cfg.["cosmosDb:connectionString"]
        let dbKey = cfg.["cosmosDb:key"]

        let dbName = cfg.["cosmosDb:databaseName"]
        let refreshTokensContainer = cfg.["cosmosDb:containers:refreshTokens"]
        let usersContainer = cfg.["cosmosDb:containers:users"]
        let recipesContainer = cfg.["cosmosDb:containers:recipes"]

        let dbConfig = {
            DatabaseName = dbName
            RefreshTokensContainerName = refreshTokensContainer
            UsersContainerName = usersContainer
            RecipesContainerName = recipesContainer
        }

        let client = createCosmosClient dbServer dbKey
        sc.AddSingleton<CosmosClient>(client) |> ignore
        sc.AddSingleton<DatabaseConfiguration>(dbConfig) |> ignore
        sc.AddSingleton<UsersStore, CosmosDbUserStore>() |> ignore
        sc.AddSingleton<RecipesStore, CosmosDbRecipeStore>() |> ignore
        sc.AddGiraffe() |> ignore
        tryGetEnv "appinsightsinstrumentationkey" |> Option.iter (sc.AddApplicationInsightsTelemetry >> ignore)

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
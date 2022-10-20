open System.IO
open Household.Api.Server.Recipes.Database
open Household.Api.Server.Recipes.Domain
open Microsoft.AspNetCore.Builder
open Microsoft.Azure.Cosmos
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Serilog
open Serilog.Events


open Household.Api.Server.Users.Domain
open Household.Api.Server.Users.Database
open Household.Libraries.CosmosDb
open Household.Api.Server
open Household.Api.Server.Configuration


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
        // htmlFile <| Path.Combine(wwwRoot, "index.html")
    ]

let cfgServices (cfg:IConfiguration) (sc:IServiceCollection) =
    sc.AddApplicationInsightsTelemetry() |> ignore

    let dbServer = cfg["cosmosDb:endpoint"]
    let dbKey = cfg["cosmosDb:key"]

    let dbName = cfg["cosmosDb:databaseName"]
    let refreshTokensContainer = cfg["cosmosDb:containers:refreshTokens"]
    let usersContainer = cfg["cosmosDb:containers:users"]
    let recipesContainer = cfg["cosmosDb:containers:recipes"]

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

let configure (app:IApplicationBuilder) =
    app.UseDefaultFiles()
        .UseStaticFiles()
        .UseGiraffe webApp

Log.Logger <-
    LoggerConfiguration()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .CreateLogger()

try
    try

        let builderOptions = WebApplicationOptions(WebRootPath = wwwRoot, ContentRootPath = contentRoot)
        let builder = WebApplication.CreateBuilder(builderOptions)

        let appCfgConnString = builder.Configuration.GetConnectionString("appCfg")
        builder.Configuration.AddAzureAppConfiguration(appCfgConnString) |> ignore

        cfgServices builder.Configuration builder.Services

        let app = builder.Build()
        configure app


        app.Run()
    with ex ->
        Log.Fatal(ex, "Host terminated unexpectedly")
finally
    Log.CloseAndFlush()
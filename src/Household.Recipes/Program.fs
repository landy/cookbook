open Microsoft.AspNetCore.Builder
open Giraffe
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Serilog
open Serilog.Events


let webApp = choose [
    route "/test" >=> json "hello world"
    route "/liveness" >=> Successful.ok (text "Ok")
    route "/" >=> json "recipes api"
]

let cfgServices (sc:IServiceCollection) =
    sc.AddGiraffe()

let configure (app:IApplicationBuilder) =
    app.UseGiraffe webApp

Log.Logger <-
    LoggerConfiguration()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .CreateLogger()

try
    try
        Log.Information("Starting application")

        let builder = WebApplication.CreateBuilder()
        builder.Host.UseSerilog() |> ignore

        let appCfgConnString = builder.Configuration.GetConnectionString("appCfg")
        builder.Configuration.AddAzureAppConfiguration(appCfgConnString) |> ignore

        cfgServices builder.Services |> ignore

        let app = builder.Build()

        configure app
        //app.MapGet("/test", Func<string>(fun () -> JSon"Hello World!")) |> ignore

        app.Run()
    with ex ->
        Log.Fatal(ex, "Host terminated unexpectedly")
finally
    Log.CloseAndFlush()
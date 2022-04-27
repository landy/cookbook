open Microsoft.AspNetCore.Builder
open Giraffe
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection

let webApp = choose [
    route "/test" >=> json "hello world"
    route "/liveness" >=> Successful.ok (text "Ok")
    route "/" >=> json "recipes api"
]

let cfgServices (sc:IServiceCollection) =
    sc.AddGiraffe()


let configure (app:IApplicationBuilder) =
    app.UseGiraffe webApp

let builder = WebApplication.CreateBuilder()

let appCfgConnString = builder.Configuration.GetConnectionString("appCfg")
builder.Configuration.AddAzureAppConfiguration("") |> ignore

cfgServices builder.Services |> ignore

let app = builder.Build()

configure app
//app.MapGet("/test", Func<string>(fun () -> JSon"Hello World!")) |> ignore

app.Run()
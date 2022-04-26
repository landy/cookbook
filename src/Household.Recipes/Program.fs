open System
open Microsoft.AspNetCore.Builder
open Giraffe
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection

let webApp = choose [
    route "/test" >=> json "hello world"
    route "/" >=> json "recipes api"
]

let cfgServices (sc:IServiceCollection) =
    sc.AddGiraffe()


let configure (app:IApplicationBuilder) =
    app.UseGiraffe webApp

let builder = WebApplication.CreateBuilder()
cfgServices builder.Services |> ignore

let app = builder.Build()

configure app
//app.MapGet("/test", Func<string>(fun () -> JSon"Hello World!")) |> ignore

app.Run()
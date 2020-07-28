module Cookbook.Client.Server

open Fable.Core
open Fable.Remoting.Client

open Cookbook.Shared

[<Emit("config.baseUrl")>]
let baseUrl : string = jsNative

    /// A proxy you can use to talk to server directly
let usersService : Users.UsersService =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Users.Route.builder
    |> Remoting.withBaseUrl baseUrl
    |> Remoting.buildProxy<Users.UsersService>

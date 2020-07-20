module Cookbook.Client.Server

open Fable.Core
open Fable.Remoting.Client

open Cookbook.Shared.Auth

[<Emit("config.baseUrl")>]
let baseUrl : string = jsNative

    /// A proxy you can use to talk to server directly
let authService : AuthService =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.withBaseUrl baseUrl
    |> Remoting.buildProxy<AuthService>
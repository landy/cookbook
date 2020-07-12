module Cookbook.Client.Router

open Feliz.Router
open Browser.Types
open Fable.Core.JsInterop

type Page =
    | Main
    | Login

module private Paths =
    let [<Literal>] Login = "login"

let private basicMapping =
    [
        [ Paths.Login ], Login

    ]

module Page =
    let parseFromUrlSegments = function
        | path ->
            basicMapping
            |> List.tryFind (fun (p,_) -> p = path)
            |> Option.map snd
            |> Option.defaultValue (Main)
    let toUrlSegments = function
        | page ->
            basicMapping
            |> List.tryFind (fun (_,p) -> p = page)
            |> Option.map fst
            |> Option.defaultValue []

module Router =
    let goToUrl (e:MouseEvent) =
        e.preventDefault()
        let href : string = !!e.currentTarget?attributes?href?value
        Router.navigatePath href |> List.map (fun f -> f ignore) |> ignore

    let navigatePage (p:Page) = p |> Page.toUrlSegments |> Array.ofList |> Router.navigatePath
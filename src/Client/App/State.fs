module Cookbook.Client.App.State

open Elmish

open Feliz.Router
open Cookbook.Client.Router

open Domain


let init () : Model * Cmd<Msg> =
    let initialModel = { CurrentPage = Main; Token = None}
    let page = Router.currentPath () |> Page.parseFromUrlSegments
    printfn "page: %A" page
    initialModel, (page |> UrlChanged |> Cmd.ofMsg)


let update (msg : Msg) (state : Model) : Model * Cmd<Msg> =
    match msg with
    | UrlChanged page -> {state with CurrentPage = page },Cmd.none
    | TokenChanged token ->
        { state with Token = token }, Router.navigatePage Main
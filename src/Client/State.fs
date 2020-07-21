module Cookbook.Client.State

open Elmish

open Feliz.Router
open Cookbook.Client.Router
open Cookbook.Shared.Auth

type Msg =
    | UrlChanged of Page
    | TokenChanged of Response.Token option


type Model = {
    CurrentPage: Page
    Token : Response.Token option
}

let init () : Model * Cmd<Msg> =
    let initialModel = { CurrentPage = Main; Token = None }
    let page = Router.currentPath () |> Page.parseFromUrlSegments
    printfn "page: %A" page
    initialModel, (page |> UrlChanged |> Cmd.ofMsg)


let update (msg : Msg) (state : Model) : Model * Cmd<Msg> =
    match msg with
    | UrlChanged page -> {state with CurrentPage = page },Cmd.none
    | TokenChanged token ->
        {state with Token = token}, Router.navigatePage Main
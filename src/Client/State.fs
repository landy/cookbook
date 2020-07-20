module Cookbook.Client.State

open Elmish

open Feliz.Router
open Cookbook.Client.Router


type Msg =
    | UrlChanged of Page

type Model = { CurrentPage: Page }

let init () : Model * Cmd<Msg> =
    let initialModel = { CurrentPage = Main }
    let page = Router.currentPath () |> Page.parseFromUrlSegments
    printfn "page: %A" page
    initialModel, (page |> UrlChanged |> Cmd.ofMsg)

let update (msg : Msg) (state : Model) : Model * Cmd<Msg> =
   match msg with
   | UrlChanged page -> {state with CurrentPage = page },Cmd.none
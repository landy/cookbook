module Cookbook.Client.State

open Elmish

open Cookbook.Client.Router


type Msg =
    | UrlChanged of Page

type Model = { CurrentPage: Page }

let init () : Model * Cmd<Msg> =
    let initialModel = { CurrentPage = Main }

    initialModel, Cmd.none

let update (msg : Msg) (state : Model) : Model * Cmd<Msg> =
   match msg with
   | UrlChanged page -> {state with CurrentPage = page },Cmd.none
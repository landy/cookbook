module Cookbook.Client.State

open Elmish

type Page =
    | Main
    | Login

type Msg =
    | PageChanged of Page

type Model = { CurrentPage: Page }

let init () : Model * Cmd<Msg> =
    let initialModel = { CurrentPage = Main }

    initialModel, Cmd.none

let update (msg : Msg) (state : Model) : Model * Cmd<Msg> =
   match msg with
   | PageChanged page -> {state with CurrentPage = page },Cmd.none
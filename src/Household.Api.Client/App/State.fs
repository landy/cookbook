module Household.Api.Client.App.State

open Elmish
open Feliz.Router
open Fable.Core
open Household.Api.Client.App.Domain
open Household.Api.Client.App.Router

let init () =
    let loadCurrentPage = Router.currentPath >> Page.parseFromUrlSegments
    {
        CurrentPage = None
    }, Cmd.OfFunc.perform loadCurrentPage () PageChanged


let update (msg:Msg) (state:Model) =
    JS.console.log("state update")
    match msg with
    | PageChanged page ->
        { state with CurrentPage = Some page }, Cmd.none
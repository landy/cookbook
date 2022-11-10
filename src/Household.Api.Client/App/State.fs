module Household.Api.Client.App.State

open Elmish
open Feliz.Router

open Household.Api.Client.ElmishHelpers
open Household.Api.Client.App.Domain
open Household.Api.Client.App.Router

let init () =
    let loadCurrentPage = Router.currentPath >> Page.parseFromUrlSegments
    {
        CurrentPage = None
    }, Cmd.OfFunc.perform loadCurrentPage () PageChanged


let update (msg:Msg) (state:Model) =
    match msg with
    | PageChanged page ->
        { state with CurrentPage = Some page }
        |> Cmd.withoutCmd
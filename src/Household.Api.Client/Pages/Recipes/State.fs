module Household.Api.Client.Pages.Recipes.State

open Fable.Core
open Household.Api.Client.Server
open Domain
open Elmish

let init () =
    {
        Recipes = RemoteReadData.init
    }, Cmd.ofMsg LoadRecipes


let update (msg:Msg) (state: Model) =
    match msg with
    | LoadRecipes ->
        {state with Recipes = RemoteReadData.setInProgress},Cmd.OfAsync.eitherAsResult (fun _ -> onRecipesService(fun s -> s.GetRecipesList())) RecipesLoaded
    | RecipesLoaded res ->
        match res with
        | Ok recipes ->
            {state with Recipes = RemoteReadData.setResponse recipes},Cmd.none
        | Error err ->
            {state with Recipes = RemoteReadData.init},Cmd.none
    | TestApi ->
        state, Cmd.OfAsync.eitherAsResult (fun _ -> onRecipesService(fun s -> s.TestDapr())) ApiTested
    | ApiTested stringResult ->
        match stringResult with
        | Ok str ->
            JS.console.log(str)
            state,Cmd.none
        | Error serverError ->
            JS.console.log(serverError)
            state,Cmd.none
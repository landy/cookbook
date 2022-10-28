module Household.Api.Client.Pages.RecipesList.State

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
            JS.console.log(recipes)
            {state with Recipes = RemoteReadData.setResponse recipes},Cmd.none
        | Error err ->
            {state with Recipes = RemoteReadData.init},Cmd.none

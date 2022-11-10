module Household.Api.Client.Pages.RecipesList.State

open Elmish

open Household.Api.Client.ElmishHelpers
open Household.Api.Client.Server
open Household.Api.Client.Pages.RecipesList.Domain

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
            {state with Recipes = RemoteReadData.setResponse recipes}
            |> Cmd.withoutCmd
        | Error err ->
            {state with Recipes = RemoteReadData.init},Cmd.none

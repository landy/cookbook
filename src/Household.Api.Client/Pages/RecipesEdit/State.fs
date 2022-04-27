module Household.Api.Client.Pages.RecipeEdit.State

open Household.Api.Client.Router
open Elmish
open Domain

open Household.Api.Client.Server
open Household.Api.Shared.Recipes.Contracts
open Fable.Core

let init recipeId () =
    let cmd =
        match recipeId with
        | Some recId ->
            Cmd.ofMsg (LoadRecipe recId)
        | None -> Cmd.none
    {
        Recipe = RemoteReadData.init
        FormData = RemoteData.init EditRecipe.init EditRecipe.validate
    }, cmd


let update (msg:Msg) (state: Model) =
    match msg with
    | LoadRecipe recipeId ->
        let cmd = Cmd.OfAsync.eitherAsResult (fun _ -> onRecipesService(fun s -> s.GetRecipe recipeId)) RecipeLoaded
        {state with Recipe = RemoteReadData.setInProgress},cmd
    | RecipeLoaded res ->
        match res with
        | Ok recipe ->
            let state' =
                { state with
                    Recipe = RemoteReadData.setResponse recipe
                    FormData = state.FormData |> RemoteData.setData recipe EditRecipe.validate
                }
            JS.console.log(state')
            state', Cmd.none
    | FormChanged form -> { state with FormData = state.FormData |> RemoteData.setData form EditRecipe.validate }, Cmd.none
    | SaveRecipe ->
        let cmd = Cmd.OfAsync.eitherAsResult (fun _ -> onRecipesService(fun s -> s.SaveRecipe state.FormData.Data)) RecipeSaved
        {state with FormData = state.FormData |> RemoteData.setInProgress },cmd
    | RecipeSaved res ->
        match res with
        | Ok recipe ->
            let state' =
                { state with
                    FormData = state.FormData |> RemoteData.setResponse ()
                }
            state', Router.navigatePageCmd (Page.RecipesEdit (Some recipe))
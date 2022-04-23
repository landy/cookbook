module Cookbook.Client.Pages.RecipeEdit.Domain

open System

open Cookbook.Client.Server
open Cookbook.Shared.Recipes.Contracts
open Cookbook.Shared.Validation

type Model = {
    Recipe: RemoteReadData<EditRecipe>
    FormData : RemoteData<EditRecipe, unit, ValidationError>
}

type Msg =
    | LoadRecipe of recipeId:Guid
    | RecipeLoaded of ServerResult<EditRecipe>
    | FormChanged of EditRecipe
    | SaveRecipe
    | RecipeSaved of ServerResult<Guid>
module Household.Api.Client.Pages.RecipeEdit.Domain

open System

open Household.Api.Client.Server
open Household.Api.Shared.Recipes.Contracts
open Household.Api.Shared.Validation

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
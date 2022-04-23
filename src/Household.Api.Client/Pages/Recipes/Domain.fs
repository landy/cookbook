module Household.Api.Client.Pages.Recipes.Domain

open Household.Api.Client.Server
open Household.Api.Shared.Recipes.Contracts

type Msg =
    | LoadRecipes
    | RecipesLoaded of ServerResult<RecipeListItem list>


type Model = {
    Recipes: RemoteReadData<RecipeListItem list>
}
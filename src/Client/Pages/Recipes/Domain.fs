module Cookbook.Client.Pages.Recipes.Domain

open Cookbook.Client.Server
open Cookbook.Shared.Recipes.Contracts

type Msg =
    | LoadRecipes
    | RecipesLoaded of ServerResult<RecipeListItem list>


type Model = {
    Recipes: RemoteReadData<RecipeListItem list>
}
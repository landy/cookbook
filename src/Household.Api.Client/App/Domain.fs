module Household.Api.Client.App.Domain

open System

type Page =
    | Main
    | RecipesList
    | RecipesEdit of recipeId:Guid option

type Msg =
    | PageChanged of Page


type Model = {
    CurrentPage: Page option
}
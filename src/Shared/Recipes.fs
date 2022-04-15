module Cookbook.Shared.Recipes

open System
open Aether
open Cookbook.Shared.Errors
open Cookbook.Shared.Validation

[<RequireQualifiedAccess>]
module Route =
    let builder _ m =
        sprintf "/api/recipes/%s" m

module Contracts =
    type EditRecipe = {
        Id: Guid
        Name: string
        Description: string
    }

    module EditRecipe =
        let init =
            {
                Id = Guid.NewGuid()
                Name = ""
                Description = ""
            }

        let name = NamedLens.create "Název" (fun x -> x.Name) (fun x v -> { v with Name = x })

        let validate =
            rules [
                check name Validator.isNotEmpty
            ]

    type RecipeListItem = {
        Id: Guid
        Name: string
    }


type RecipesService = {
    SaveRecipe: Contracts.EditRecipe -> Async<unit>
    GetRecipe: Guid -> Async<Contracts.EditRecipe>
    GetRecipesList: unit -> Async<Contracts.RecipeListItem list>
}
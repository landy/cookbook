module Cookbook.Shared.Recipes

open System
open Aether
open Cookbook.Shared.Validation

[<RequireQualifiedAccess>]
module Route =
    let builder _ m =
        sprintf "/api/recipes/%s" m

module Contracts =
    type EditRecipe = {
        Id: Guid option
        Name: string
        Description: string
    }

    module EditRecipe =
        let init =
            {
                Id = None
                Name = ""
                Description = ""
            }

        let name = NamedLens.create "Název" (fun x -> x.Name) (fun x v -> { v with Name = x })
        let description = NamedLens.create "Postup" (fun x -> x.Description) (fun x v -> { v with Description = x })

        let validate =
            rules [
                check name Validator.isNotEmpty
            ]

    type RecipeListItem = {
        Id: Guid
        Name: string
    }


open Contracts
type RecipesService = {
    SaveRecipe: Contracts.EditRecipe -> Async<Guid>
    GetRecipe: Guid -> Async<EditRecipe>
    GetRecipesList: unit -> Async<RecipeListItem list>
}
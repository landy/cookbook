module Cookbook.Shared.Recipes

open System
open Cookbook.Shared.Errors

[<RequireQualifiedAccess>]
module Route =
    let builder _ m =
        sprintf "/api/recipes/%s" m

[<RequireQualifiedAccess>]
module Request =
    type SaveRecipe = {
        Id: Guid
        Name: string
        Description: string
    }


type RecipesService = {
    SaveRecipe: Request.SaveRecipe -> Async<Result<unit, ApplicationError>>
}
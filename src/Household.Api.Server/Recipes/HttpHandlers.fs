module Cookbook.Server.Recipes.HttpHandlers

open System
open Microsoft.AspNetCore.Http
open Giraffe
open Giraffe.GoodRead
open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open FsToolkit.ErrorHandling

open Cookbook.Server.Remoting
open Cookbook.Shared.Recipes
open Cookbook.Shared.Recipes.Contracts
open Cookbook.Server.Recipes.Domain
open Microsoft.Extensions.Logging

let private saveRecipeHandler pipeline (rq:Contracts.EditRecipe) = task {
    let recipeId = rq.Id |> Option.defaultWith (fun _ -> Guid.NewGuid())
    return!
        ({
            Id = recipeId
            Name = rq.Name
            Description = rq.Description
        } : CmdArgs.SaveRecipe)
        |> SaveRecipe
        |> pipeline
        |> Task.map (fun _ -> recipeId)
}


let private createLoadRecipesList (recipesDb: RecipesStore) ()=
    recipesDb.getRecipesList()
    |> Task.map (fun rx ->
        rx
        |> List.map (fun r ->
            ({
                Id = r.Id
                Name = r.Name
            }: RecipeListItem)
        )
    )

let private createGetRecipe (recipesDb: RecipesStore) recipeId =
    recipesDb.tryGetRecipe recipeId
    |> Task.map (fun recipeOpt ->
        recipeOpt
        |> Option.map (fun r ->
            ({
                Id = r.Id |> Some
                Name = r.Name
                Description = r.Description
            }: Contracts.EditRecipe)
        )
        |> Option.defaultWith (fun _ -> "Recept nenalezen" |> failwith)
    )

let private mapRecipeSaved (RecipeSaved recipe) =
    {
        Id = recipe.Id |> Some
        Name = recipe.Name
        Description = recipe.Description
    } : Contracts.EditRecipe

let private recipesService recipesDb (httpContext: HttpContext) =
    let pipeline = CommandHandlers.pipeline recipesDb
    {
        SaveRecipe = saveRecipeHandler pipeline >> Async.AwaitTask
        GetRecipesList = createLoadRecipesList recipesDb >> Async.AwaitTask
        GetRecipe = createGetRecipe recipesDb >> Async.AwaitTask
    }


let recipesHandler : HttpHandler =
    Require.services<ILogger<_>, RecipesStore> (fun logger recipesStore ->
        Remoting.createApi ()
        |> Remoting.withErrorHandler (Remoting.errorHandler logger)
        |> Remoting.withRouteBuilder Route.builder
        |> Remoting.withBinarySerialization
        |> Remoting.fromContext (recipesService recipesStore)
        |> Remoting.buildHttpHandler
    )
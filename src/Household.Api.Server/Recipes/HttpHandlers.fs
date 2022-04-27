module Household.Api.Server.Recipes.HttpHandlers

open System
open System.Net.Http
open Dapr.Client
open Household.Api.Server.Recipes.RecipesServices
open Microsoft.AspNetCore.Http
open Giraffe
open Giraffe.GoodRead
open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open FsToolkit.ErrorHandling

open Household.Api.Server.Remoting
open Household.Api.Shared.Recipes
open Household.Api.Shared.Recipes.Contracts
open Household.Api.Server.Recipes.Domain
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

let private testDaprHandler (log:ILogger) (daprClient:RecipesDaprService) () = task {
    return! daprClient.Test()
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

let private recipesService (log:ILogger) recipesDb daprClient (httpContext: HttpContext) =
    let pipeline = CommandHandlers.pipeline recipesDb
    {
        SaveRecipe = saveRecipeHandler pipeline >> Async.AwaitTask
        GetRecipesList = createLoadRecipesList recipesDb >> Async.AwaitTask
        GetRecipe = createGetRecipe recipesDb >> Async.AwaitTask
        TestDapr = testDaprHandler log daprClient >> Async.AwaitTask
    }


let recipesHandler : HttpHandler =
    Require.services<ILogger<_>, RecipesStore, RecipesDaprService> (fun logger recipesStore daprClient ->
        Remoting.createApi ()
        |> Remoting.withErrorHandler (Remoting.errorHandler logger)
        |> Remoting.withRouteBuilder Route.builder
        |> Remoting.withBinarySerialization
        |> Remoting.fromContext (recipesService logger recipesStore daprClient)
        |> Remoting.buildHttpHandler
    )
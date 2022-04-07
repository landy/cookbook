module Cookbook.Server.Recipes.HttpHandlers

open Microsoft.AspNetCore.Http
open Giraffe
open Fable.Remoting.Server
open Fable.Remoting.Giraffe

open Cookbook.Shared.Recipes
open Cookbook.Server.Recipes.Domain

let private createSaveRecipe (rq:Request.SaveRecipe) =
    ({
        Id = rq.Id
        Name = rq.Name
        Description = rq.Description
    } : CmdArgs.SaveRecipe)
    |> SaveRecipe

let private recipesService recipesDb =
    let pipeline = CommandHandlers.pipeline recipesDb
    {
        SaveRecipe = createSaveRecipe >> pipeline >> Async.AwaitTask
    }

let private createRecipesServiceFromContext (httpContext: HttpContext) =
    let recipesStore = httpContext.GetService<RecipesStore>()
    recipesService recipesStore

let recipesHandler : HttpHandler =
    Remoting.createApi ()
    |> Remoting.withErrorHandler (fun ex _  -> Propagate ex)
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromContext createRecipesServiceFromContext
    |> Remoting.buildHttpHandler
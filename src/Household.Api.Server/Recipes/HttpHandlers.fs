module Household.Api.Server.Recipes.HttpHandlers

open System
open System.Text
open System.Linq
open System.Text.Json
open Microsoft.AspNetCore.Http
open System.Security.Claims
open Microsoft.AspNetCore.Http
open Giraffe
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

type  ClientPrincipal() =
    member val IdentityProvider = "" with get, set
    member val UserId  = "" with get, set
    member val UserDetails = "" with get, set
    member val UserRoles : seq<string> = List.empty with get, set


let private recipesService  (httpContext: HttpContext) =
    // (log:ILogger)
    let recipesDb = httpContext.GetService<RecipesStore>()
    let logFactory = httpContext.GetService<ILoggerFactory>()
    let log = logFactory.CreateLogger("requestLogger")
    let headerExists, headerValue = httpContext.Request.Headers.TryGetValue("x-ms-client-principal")
    let principal =
        if headerExists then
            headerValue[0]
            |> Convert.FromBase64String
            |> Encoding.UTF8.GetString
            |> fun v -> JsonSerializer.Deserialize<ClientPrincipal>(v, JsonSerializerOptions(PropertyNameCaseInsensitive = true))
            |> fun p ->
                p.UserRoles <- p.UserRoles.Except([| "anonymous" |], StringComparer.CurrentCultureIgnoreCase)
                p
            |> Some
        else
            None
        |> Option.map (fun p ->
            let identity = ClaimsIdentity(p.IdentityProvider)
            identity.AddClaim(Claim(ClaimTypes.NameIdentifier, p.UserId))
            identity.AddClaim(Claim(ClaimTypes.Name, p.UserDetails))
            identity.AddClaims(p.UserRoles.Select(fun r -> Claim(ClaimTypes.Role, r)))
            ClaimsPrincipal(identity)
        )
        |> Option.defaultWith (fun _ ->
            ClaimsPrincipal()
        )

    let pipeline = CommandHandlers.pipeline recipesDb
    {
        SaveRecipe = saveRecipeHandler pipeline >> Async.AwaitTask
        GetRecipesList = createLoadRecipesList recipesDb >> Async.AwaitTask
        GetRecipe = createGetRecipe recipesDb >> Async.AwaitTask
    }

let recipesHandler : HttpHandler =
    Remoting.createApi ()
    // |> Remoting.withErrorHandler (Remoting.errorHandler logger)
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.withBinarySerialization
    |> Remoting.fromContext recipesService
    |> Remoting.buildHttpHandler
module Household.Api.Server.Recipes.RecipesServices

open System.Net.Http
open System.Net.Http.Json
open Microsoft.Extensions.Logging

type RecipesDaprService (log:ILogger, httpClient:HttpClient) =
    member self.Test() = task {
        log.LogInformation("making call to recipes service")
        let! res = httpClient.GetFromJsonAsync("/test")
        return res
    }
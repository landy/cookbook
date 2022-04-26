module Household.Api.Server.Recipes.RecipesServices

open System.Net.Http
open System.Net.Http.Json
open Microsoft.Extensions.Logging

type RecipesDaprService (httpClient:HttpClient) =
    member self.Test() = task {
//        logger.LogInformation("making call to recipes service")
        let! res = httpClient.GetFromJsonAsync("/test")
        return res
    }
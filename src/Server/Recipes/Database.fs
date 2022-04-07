module Cookbook.Server.Recipes.Database

open System
open Cookbook.Server.Recipes.Domain
open Cookbook.Shared.Errors
open Microsoft.Azure.Cosmos

open Cookbook.Libraries.CosmosDb
open Cookbook.Server.Configuration

module Schema =
    [<Literal>]
    let PartitionKey = "/partitionKey"
    [<Literal>]
    let PartitionKeyValue = "cookbook"

    type RecipeDocument = {
        Id: Guid
        PartitionKey: string
        Name: string
        Description: string
        LastUpdated: DateTimeOffset
    }

type CosmosDbRecipeStore (config: DatabaseConfiguration, client: CosmosClient) =
    let getRecipeContainer () =
        ContainerProperties(config.RecipesContainerName, Schema.PartitionKey)
        |> getContainer client config.DatabaseName

    interface RecipesStore with
        member _.saveRecipe recipe =
            task {
                try
                    let! container = getRecipeContainer ()

                    let row : Schema.RecipeDocument = {
                        Id = recipe.Id
                        PartitionKey = Schema.PartitionKeyValue
                        Name = recipe.Name
                        Description = recipe.Description
                        LastUpdated = DateTimeOffset.UtcNow
                    }
                    let! _ = upsertItem<Schema.RecipeDocument> container Schema.PartitionKeyValue row
                    return () |> Ok
                with
                    | ex -> return Error (DatabaseError.Exception ex)
            }
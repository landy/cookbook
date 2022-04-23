module Household.Api.Server.Recipes.Database

open System
open System.Threading.Tasks
open FsToolkit.ErrorHandling
open Microsoft.Azure.Cosmos

open Household.Api.Server.Recipes.Domain
open Household.Api.Shared.Errors
open Household.Libraries.CosmosDb
open Household.Api.Server.Configuration

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
    let getRecipeContainer () : Task<Container> =
        ContainerProperties(config.RecipesContainerName, Schema.PartitionKey)
        |> getContainer client config.DatabaseName

    interface RecipesStore with
        member _.saveRecipe recipe =
            task {
                let! container = getRecipeContainer ()

                let! _ = Task.Delay (TimeSpan.FromSeconds(2))

                let row : Schema.RecipeDocument = {
                    Id = recipe.Id
                    PartitionKey = Schema.PartitionKeyValue
                    Name = recipe.Name
                    Description = recipe.Description
                    LastUpdated = DateTimeOffset.UtcNow
                }
                let! _ = upsertItem<Schema.RecipeDocument> container Schema.PartitionKeyValue row
                return ()
            }



        member _.getRecipesList () =
            task {
                let! container = getRecipeContainer()
                let query = QueryDefinition "SELECT * FROM c"

                return!
                    getItems<Schema.RecipeDocument> container Schema.PartitionKeyValue query
                    |> Task.map (fun rows ->
                        rows
                        |> List.map (fun row ->
                            ({
                                Id = row.Id
                                Name = row.Name
                                Description = row.Description
                            }:Views.Recipe)
                        )
                    )
            }

        member _.tryGetRecipe recipeId =
            task {
                let! container = getRecipeContainer()

                return!
                    tryGetItem<Schema.RecipeDocument> container (recipeId.ToString()) Schema.PartitionKeyValue
                    |> TaskOption.map (fun row ->
                        ({
                            Id = row.Id
                            Name = row.Name
                            Description = row.Description
                        }:Views.Recipe)
                    )
            }
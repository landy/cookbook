module Cookbook.Server.Auth.Database

open System.Net
open System.Threading.Tasks
open FSharp.Control.Tasks.V2
open Microsoft.Azure.Cosmos

open Cookbook.Libraries.CosmosDb
open Cookbook.Server.Configuration
open Cookbook.Server.Auth.Domain


type UserStore =
    abstract tryFindUser : string -> Task<CookbookUser option>


module Schema =
    [<Literal>]
    let PartitionKey = "/partitionKey"
    [<Literal>]
    let PartitionKeyValue = "cookbook"

    type UserDocument = {
        Id : string
        PartitionKey : string
        Name : string
        PasswordHash :string
    }


type CosmosDbUserStore (config: DatabaseConfiguration, client:CosmosClient) =

    let getContainer () =
        ContainerProperties(config.UsersContainerName,Schema.PartitionKey)
        |> getContainer client config.DatabaseName


    interface UserStore with
        member _.tryFindUser username =
            task {
                let! container = getContainer()
                let! r = tryGetItem<Schema.UserDocument> container username Schema.PartitionKeyValue
                return r |> Option.map (fun row ->{ Username = row.Id; Name = row.Name })
            }
module Cookbook.Server.Auth.Database

open System.Net
open System.Threading.Tasks
open FSharp.Control.Tasks.V2
open Microsoft.Azure.Cosmos

open Cookbook.Server.Configuration
open Cookbook.Server.Auth.Domain


type UserStore =
    abstract tryFindUser : string -> Task<CookbookUser option>


module private Schema =
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


let tryGetItem<'a> (container:Container) primaryKey (partitionKey:string) =
    task {
        try
            let! item = container.ReadItemAsync<'a>(primaryKey, PartitionKey(partitionKey))
            return item.Resource |> Some
        with
        | :? CosmosException as ex when ex.StatusCode = HttpStatusCode.NotFound ->
            return None
    }


type CosmosDbUserStore (config: DatabaseConfiguration, client:CosmosClient) =

    let getContainer () =
        task {
            let db = client.GetDatabase(config.DatabaseName)
            let props = ContainerProperties(config.UsersContainerName,Schema.PartitionKey)
            let! r = db.CreateContainerIfNotExistsAsync(props)
            return r.Container
        }




    interface UserStore with
        member _.tryFindUser username =
            task {
                let! container = getContainer()
                let! r = tryGetItem<Schema.UserDocument> container username Schema.PartitionKeyValue
                return r |> Option.map (fun row ->{ Username = row.Id; Name = row.Name })
            }
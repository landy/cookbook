module Cookbook.Libraries.CosmosDb

open System
open System.Net
open FsToolkit.ErrorHandling
open Microsoft.Azure.Cosmos
open FSharp.Control.Tasks.V2


let createCosmosClient endpoint authKey =
    let serializerOptions = CosmosSerializationOptions()
    serializerOptions.Indented <- true
    serializerOptions.PropertyNamingPolicy <- CosmosPropertyNamingPolicy.CamelCase

    let opts = CosmosClientOptions()
    opts.ConnectionMode <- ConnectionMode.Direct
    opts.SerializerOptions <- serializerOptions

    new CosmosClient(endpoint, authKey, opts)

let getContainer (client:CosmosClient) dbName containerProps =
    task {
        let db = client.GetDatabase(dbName)
        let! r = db.CreateContainerIfNotExistsAsync(containerProps)
        return r.Container
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

let upsertItem<'a> (container:Container) (partitionKey: string) item =
    container.UpsertItemAsync(item, (PartitionKey(partitionKey) |> Nullable ))
    |> Task.map ignore
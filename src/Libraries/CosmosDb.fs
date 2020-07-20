module Cookbook.Libraries.CosmosDb

open System.Net
open Microsoft.Azure.Cosmos
open FSharp.Control.Tasks.V2


let createCosmosClient endpoint authKey =
    let opts = CosmosClientOptions()
    opts.ConnectionMode <- ConnectionMode.Direct
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
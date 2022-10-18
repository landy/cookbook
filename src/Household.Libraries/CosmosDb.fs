module Household.Libraries.CosmosDb

open System
open System.Net
open Microsoft.Azure.Cosmos.Fluent
open FsToolkit.ErrorHandling
open Microsoft.Azure.Cosmos


let createCosmosClient endpoint authKey =
    let opts = CosmosSerializationOptions()
    opts.PropertyNamingPolicy <- CosmosPropertyNamingPolicy.CamelCase

    CosmosClientBuilder(accountEndpoint = endpoint, authKeyOrResourceToken = authKey)
        .WithConnectionModeDirect()
        .WithSerializerOptions(opts)
        .Build()

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


let getItems<'a> (container:Container) (partitionKey: string) (query:QueryDefinition) =
    let opts = QueryRequestOptions()
    opts.PartitionKey <- partitionKey |> PartitionKey |> Nullable

    let iterator  = container.GetItemQueryIterator<'a>(query, requestOptions = opts)
    let items = ResizeArray<'a>()
    task {
        while iterator.HasMoreResults do
            let! values = iterator.ReadNextAsync()
            values.Resource |> Seq.toList |> items.AddRange
        return items |> Seq.toList
    }
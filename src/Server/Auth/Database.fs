module Cookbook.Server.Auth.Database

open System
open System.Threading.Tasks
open FSharp.Control.Tasks.V2
open Microsoft.Azure.Cosmos

open Cookbook.Libraries.CosmosDb
open Cookbook.Server.Configuration
open Cookbook.Server.Auth.Domain


type UserStore =
    abstract tryFindUser : string -> Task<CookbookUser option>
    abstract setRefreshToken : string -> string -> DateTimeOffset -> Task<unit>


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

    type RefreshToken = {
        Id : string
        PartitionKey : string
        Token : string
        ExpiresOn : DateTimeOffset
        Ttl : int
    }


type CosmosDbUserStore (config: DatabaseConfiguration, client:CosmosClient) =

    let getUsersContainer () =
        ContainerProperties(config.UsersContainerName,Schema.PartitionKey)
        |> getContainer client config.DatabaseName

    let getRefreshTokensContainer () =
        ContainerProperties(config.RefreshTokensContainerName,Schema.PartitionKey)
        |> fun p ->
            p.DefaultTimeToLive <- (14 * 24 * 60 * 60) |> Nullable
            p
        |> getContainer client config.DatabaseName


    interface UserStore with
        member _.tryFindUser username =
            task {
                let! container = getUsersContainer()
                let! r = tryGetItem<Schema.UserDocument> container username Schema.PartitionKeyValue
                return r |> Option.map (fun row ->{ Username = row.Id; Name = row.Name })
            }

        member _.setRefreshToken username token expires =
            task {
                let row : Schema.RefreshToken = {
                    Id = username
                    PartitionKey = Schema.PartitionKeyValue
                    Token = token
                    ExpiresOn = expires
                    Ttl = expires.Subtract(DateTimeOffset.UtcNow).Seconds
                }
                let! container = getRefreshTokensContainer ()
                let! _ = upsertItem<Schema.UserDocument> container Schema.PartitionKeyValue row
                return ()
            }
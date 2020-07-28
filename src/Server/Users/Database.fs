module Cookbook.Server.Users.Database

open System
open System.Threading.Tasks
open Microsoft.Azure.Cosmos
open Cookbook.Server.Users.Domain
open FsToolkit.ErrorHandling
open FSharp.Control.Tasks.V2

open Cookbook.Libraries.CosmosDb
open Cookbook.Server.Configuration
open Cookbook.Server.Users.Domain


type UsersStore =
    abstract tryFindUser : string -> Task<Views.CookbookUser option>
    abstract setRefreshToken : string -> string -> DateTimeOffset -> Task<unit>
    abstract getUsers : unit -> Task<Views.CookbookUser list>


module Schema =
    [<Literal>]
    let PartitionKey = "/partitionKey"
    [<Literal>]
    let PartitionKeyValue = "cookbook"

    [<CLIMutable>]
    type UserDocument = {
        Id : string
        PartitionKey : string
        Name : string
        PasswordHash :string
    }

    [<CLIMutable>]
    type RefreshTokenDocument = {
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


    interface UsersStore with
        member _.tryFindUser username =
            task {
                let! container = getUsersContainer()
                let! r = tryGetItem<Schema.UserDocument> container username Schema.PartitionKeyValue
                return r |> Option.map (fun row ->{ Username = row.Id; Name = row.Name })
            }

        member _.setRefreshToken username token expires =
            task {
                let row : Schema.RefreshTokenDocument = {
                    Id = username
                    PartitionKey = Schema.PartitionKeyValue
                    Token = token
                    ExpiresOn = expires
                    Ttl = expires.Subtract(DateTimeOffset.UtcNow).TotalSeconds |> int
                }
                let! container = getRefreshTokensContainer ()
                let! _ = upsertItem<Schema.RefreshTokenDocument> container Schema.PartitionKeyValue row
                return ()
            }

        member _.getUsers () =
            task {
                let! container = getUsersContainer ()
                let query = QueryDefinition("SELECT * FROM c")

                return!
                    getItems<Schema.UserDocument> container Schema.PartitionKeyValue query
                    |> Task.map (fun rows ->
                        rows
                        |> List.map (fun row ->
                            ({ Username = row.Id; Name = row.Name } : Views.CookbookUser)
                        )
                    )
            }
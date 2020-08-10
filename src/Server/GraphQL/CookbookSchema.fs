module Cookbook.Server.GraphQL.CookbookSchema

open Cookbook.Shared.Users.Response
open FSharp.Data.GraphQL
open FSharp.Data.GraphQL.Server.Middleware
open FSharp.Data.GraphQL.Types

open Cookbook.Server.Users.Domain
open Cookbook.Server.Users.Schema


type Root = {
    UserDb : UsersStore
}

let QueryRoot =
    Define.Object<Root>(
        name = "Query",
        fields = [
            Define.AsyncField("users", ListOf UserType, fun _ r -> getUsers r.UserDb)
        ]
    )

let schema = Schema(query = QueryRoot)

let executor = Executor(schema)
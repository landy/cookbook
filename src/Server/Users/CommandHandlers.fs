module Cookbook.Server.Users.CommandHandlers

open System.Threading.Tasks
open FsToolkit.ErrorHandling
open FSharp.Control.Tasks

open Cookbook.Shared.Errors
open Cookbook.Server.Users.Domain

let validate (usersDb :UsersStore) cmd : Task<Result<Command, UserError>> =
    match cmd with
    | AddNewUser args ->
        task {
            let! existing = usersDb.tryFindUser args.Username
            return
                existing
                |> Option.map (fun _ -> UserAlreadyExists args.Username)
        }
    |> Task.map (function
        | None ->
            cmd |> Ok
        | Some err ->
            err |> Error)


let pipeline (usersDb: UsersStore) =
    validate usersDb
    >> TaskResult.mapError ApplicationError.UserError
    >> TaskResult.map execute
    >> TaskResult.bind (handle usersDb)

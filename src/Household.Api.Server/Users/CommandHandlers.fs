module Household.Api.Server.Users.CommandHandlers

open System.Threading.Tasks
open FsToolkit.ErrorHandling
open FSharp.Control.Tasks

open Household.Api.Shared.Errors
open Household.Api.Server.Users.Domain


//TODO: fix validation
let validate (usersDb :UsersStore) cmd : Task<Command> =
    match cmd with
    | AddNewUser args ->
        task {
            let! existing = usersDb.tryFindUser args.Username
            return None
//            return
//                existing
//                |> Option.map (fun _ -> UserAlreadyExists args.Username)
        }
    |> Task.map (function
        | None ->
            cmd
        | Some err ->
            cmd)


let pipeline (usersDb: UsersStore) =
    validate usersDb
    >> Task.map execute
    >> Task.bind (handle usersDb)
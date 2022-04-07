module Cookbook.Server.Recipes.CommandHandlers

open System.Threading.Tasks
open FsToolkit.ErrorHandling

open Cookbook.Server.Recipes.Domain
open Cookbook.Shared.Errors

let validate (recipesDb :RecipesStore) cmd : Task<Result<Command, UserError>> =
    match cmd with
    | SaveRecipe args ->
        task {
            return None
        }
    |> Task.map (function
        | None ->
            cmd |> Ok
        | Some err ->
            err |> Error)


let pipeline (recipesDb: RecipesStore) =
    validate recipesDb
    >> TaskResult.mapError ApplicationError.UserError
    >> TaskResult.map execute
    >> TaskResult.bind (handle recipesDb)
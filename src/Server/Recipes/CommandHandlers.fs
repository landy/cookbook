module Cookbook.Server.Recipes.CommandHandlers

open System.Threading.Tasks
open FsToolkit.ErrorHandling

open Cookbook.Server.Recipes.Domain

let validate (recipesDb :RecipesStore) cmd : Task<Command> =
    match cmd with
    | SaveRecipe args ->
        task {
            return None
        }
    |> Task.map (function
        | None ->
            cmd
        | Some err ->
            err |> failwith)


let pipeline (recipesDb: RecipesStore) =
    validate recipesDb
    >> Task.map execute
    >> Task.bind (handle recipesDb)
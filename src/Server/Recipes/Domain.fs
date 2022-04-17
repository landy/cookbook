module Cookbook.Server.Recipes.Domain

open System
open System.Threading.Tasks
open Cookbook.Shared.Errors
open FsToolkit.ErrorHandling

[<RequireQualifiedAccess>]
module Views =
    type Recipe = {
        Id: Guid
        Name : string
        Description : string
    }

[<RequireQualifiedAccess>]
module CmdArgs =
    type SaveRecipe = {
        Id : Guid option
        Name : string
        Description :string
    }

module EventArgs =
    type RecipeSaved = {
        Id : Guid
        Name : string
        Description :string
    }

type RecipesStore =
//    abstract tryFindRecipe: Guid -> Task<Result<Views.Recipe,DatabaseError>>
    abstract saveRecipe: EventArgs.RecipeSaved -> Task<unit>
    abstract getRecipesList: unit -> Task<Views.Recipe list>
    abstract tryGetRecipe : Guid -> Task<Views.Recipe option>

type Command =
    | SaveRecipe of CmdArgs.SaveRecipe

type Event =
    | RecipeSaved of EventArgs.RecipeSaved


open EventArgs
let execute cmd =
    match cmd with
    | SaveRecipe args ->
        {
            Id = args.Id |> Option.defaultWith (fun _ -> Guid.NewGuid())
            Name = args.Name
            Description = args.Description
        }
        |> RecipeSaved

let handle (recipesDb : RecipesStore) evnt =
    match evnt with
    | RecipeSaved args ->
        recipesDb.saveRecipe args

    |> Task.map (fun _ -> evnt)
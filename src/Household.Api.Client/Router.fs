module Cookbook.Client.Router

open System
open Elmish
open Feliz.Router
open Browser.Types
open Fable.Core.JsInterop

type Page =
    | Main
    | Login
    | UsersList
    | UsersAdd
    | UsersEdit
    | RecipesList
    | RecipesEdit of recipeId:Guid option


module private Paths =
    let [<Literal>] Login = "login"
    let [<Literal>] UsersPath = "users"
    let [<Literal>] RecipesList = "recipes"
    let [<Literal>] Recipe = "recipe"

    module Users =
        let [<Literal>] Add = "add"
        let [<Literal>] Edit = "edit"

module Page =
    let parseFromUrlSegments = function
        | [ Paths.Login ] -> Login
        | [ Paths.UsersPath; Paths.Users.Add ] -> UsersAdd
        | [ Paths.UsersPath; Paths.Users.Edit ] -> UsersEdit
        | [ Paths.UsersPath ] -> UsersList
        | [ Paths.RecipesList ] -> RecipesList
        | [ Paths.Recipe; Route.Guid recipeId ] -> RecipesEdit (Some recipeId)
        | [ Paths.Recipe ] -> RecipesEdit None
        | _ -> Main

    let toUrlSegments = function
        | Page.Login -> [ Paths.Login ]
        | Page.UsersList -> [ Paths.UsersPath ]
        | Page.UsersAdd -> [ Paths.UsersPath; Paths.Users.Add ]
        | Page.UsersEdit -> [ Paths.UsersPath; Paths.Users.Edit ]
        | Page.RecipesList -> [ Paths.RecipesList ]
        | Page.RecipesEdit (Some recipeId) -> [ Paths.Recipe; recipeId.ToString() ]
        | Page.RecipesEdit None -> [ Paths.Recipe ]
        | Page.Main -> []

module Router =
    let goToUrl (e:MouseEvent) =
        e.preventDefault()
        let href : string = !!e.currentTarget?attributes?href?value
        Router.navigatePath href

    let navigatePageCmd<'a> (p:Page) : Cmd<'a> = p |> Page.toUrlSegments |> Array.ofList |> Cmd.navigatePath

    let navigatePage (p:Page) =
        p |> Page.toUrlSegments |> Array.ofList |> Router.navigatePath
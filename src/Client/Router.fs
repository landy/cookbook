module Cookbook.Client.Router

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
    | RecipesAdd


module private Paths =
    let [<Literal>] Login = "login"
    let [<Literal>] UsersPath = "users"
    let [<Literal>] RecipesList = "recipes"

    module Users =
        let [<Literal>] Add = "add"
        let [<Literal>] Edit = "edit"

    module Recipes =
        let [<Literal>] Add = "add"

let private basicMapping =
    [
        [ Paths.Login ], Login
        [ Paths.UsersPath ], UsersList
        [ Paths.UsersPath; Paths.Users.Add ], UsersAdd
        [ Paths.UsersPath; Paths.Users.Edit ], UsersEdit
        [ Paths.RecipesList ], RecipesList
        [ Paths.RecipesList; Paths.Recipes.Add ], RecipesAdd
    ]

module Page =
    let parseFromUrlSegments = function
        | path ->
            basicMapping
            |> List.tryFind (fun (p,_) -> p = path)
            |> Option.map snd
            |> Option.defaultValue (Main)

    let toUrlSegments = function
        | page ->
            basicMapping
            |> List.tryFind (fun (_,p) -> p = page)
            |> Option.map fst
            |> Option.defaultValue []

module Router =
    let goToUrl (e:MouseEvent) =
        e.preventDefault()
        let href : string = !!e.currentTarget?attributes?href?value
        Router.navigatePath href

    let navigatePageCmd<'a> (p:Page) : Cmd<'a> = p |> Page.toUrlSegments |> Array.ofList |> Cmd.navigatePath

    let navigatePage (p:Page) =
        p |> Page.toUrlSegments |> Array.ofList |> Router.navigatePath
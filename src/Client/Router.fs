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

module private Paths =
    let [<Literal>] Login = "login"
    let [<Literal>] UsersPath = "users"

    module Users =
        let [<Literal>] Add = "add"
        let [<Literal>] Edit = "edit"

let private basicMapping =
    [
        [ Paths.Login ], Login
        [ Paths.UsersPath ], UsersList
        [ Paths.UsersPath; Paths.Users.Add ], UsersAdd
        [ Paths.UsersPath; Paths.Users.Edit ], UsersEdit
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

    let navigatePage<'a> (p:Page) : Cmd<'a> = p |> Page.toUrlSegments |> Array.ofList |> Cmd.navigatePath
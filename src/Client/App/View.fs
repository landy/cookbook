module Cookbook.Client.App.View

open Cookbook.Client.Auth
open Cookbook.Client.Auth.Context
open Cookbook.Client.Auth.Domain
open Cookbook.Shared.Users.Response
open Feliz
open Feliz.Bulma
open Feliz.Router

open Cookbook.Client.Router
open Cookbook.Client.Components.NavBar


[<ReactComponent>]
let MainContent page =
    Bulma.section[
        spacing.pt3
        prop.children [
            Bulma.container [
                prop.children [
                    match page with
                    | RecipesList ->
                        Cookbook.Client.Pages.Recipes.Components.RecipesList ()
                    | _ -> Html.div "main"
                ]
            ]
        ]

    ]

[<ReactComponent>]
let private LoginContent () =

    Bulma.hero [
        color.isPrimary
        hero.isFullHeight
        prop.children [
            Bulma.heroBody [
                Bulma.container [
                    text.hasTextCentered
                    prop.children [
                        Bulma.column [
                            column.is4
                            column.isOffset4
                            prop.children [
                                Cookbook.Client.Pages.Login.View.LoginForm ()
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]

[<ReactComponent>]
let MainView page =
    React.fragment [
        TopNavBar page
        MainContent page
    ]

[<ReactComponent>]
let TemplateSelector (page:Page) =
    match page with
    | Login ->
        LoginContent ()
    | _ ->
        MainView page


[<ReactComponent>]
let MainApplication () =
    let currentPage = Router.currentPath () |> Page.parseFromUrlSegments
    let page,setPage = React.useState(currentPage)

    let (user: UserSession option),setUser = React.useState(None)

    React.useEffectOnce(fun () ->
        AuthStorage.tryGetSession()
        |> setUser
    )

    let ctx = {
        CurrentUser = user
        SetUser = (fun u ->
            AuthStorage.save u
            u |> Some |> setUser)
        Logout = (fun () ->
            setUser None
            Router.navigatePage Login
        )
    }

    React.router [
        router.pathMode
        router.onUrlChanged (Page.parseFromUrlSegments >> setPage)
        router.children [
            (AuthContext ctx (TemplateSelector page))
        ]
    ]
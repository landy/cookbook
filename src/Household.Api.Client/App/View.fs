module Household.Api.Client.App.View

open Feliz
open Feliz.DaisyUI
open Feliz.Router

open Household.Api.Client.Components.Html
open Household.Api.Client.Auth
open Household.Api.Client.Auth.Context
open Household.Api.Client.Auth.Domain
open Household.Api.Shared.Users.Response
open Household.Api.Client.Router
open Household.Api.Client.Components.NavBar


[<ReactComponent>]
let MainContent page =
    Html.divClassed "container mx-auto" [
        match page with
        | RecipesList ->
            Household.Api.Client.Pages.Recipes.Components.RecipesPage ()
        | RecipesEdit recipeId ->
            Household.Api.Client.Pages.RecipeEdit.Components.RecipeForm recipeId
        | _ -> Html.div "main"
    ]

[<ReactComponent>]
let private LoginContent () =

    Html.divClassed "" [
        Household.Api.Client.Pages.Login.View.LoginForm ()
    ]

[<ReactComponent>]
let MainView page =
    Html.divClassed "flex flex-col h-screen" [
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

    React.strictMode [
        React.router [
            router.pathMode
            router.onUrlChanged (Page.parseFromUrlSegments >> setPage)
            router.children [
                (AuthContext ctx (TemplateSelector page))
            ]
        ]
    ]
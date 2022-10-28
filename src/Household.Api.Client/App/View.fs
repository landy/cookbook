module Household.Api.Client.App.View

open System
open Fable.Core
open Feliz
open Feliz.UseElmish
open Feliz.Router

open Household.Api.Client.App.Domain
open Household.Api.Client.App.Router
open Household.Api.Client.Components.Html
open Household.Api.Client.Components.NavBar
open Household.Api.Client.Pages.Main.View
open Household.Api.Client.Pages.RecipesList.View


let MainContent (page:Page) =
    Html.divClassed "container mx-auto px-4" [
        match page with
        | Main ->
            MainPage()
        | RecipesList ->
            RecipesListPage()
    ]

let MainView (page:Page) =
    Html.div [
        prop.className "flex flex-col h-screen"
        prop.children [
            MainNavBar page
            MainContent page
        ]
    ]


[<ReactComponent>]
let MainApplication() =
    let state,dispatch = React.useElmish(State.init, State.update)

    React.strictMode [
        React.router [
            router.pathMode
            router.onUrlChanged (Page.parseFromUrlSegments >> PageChanged >> dispatch)
            router.children [
                match state.CurrentPage with
                | Some page ->
                    MainView page
                | None -> Html.div "No page loaded"
            ]
        ]
    ]
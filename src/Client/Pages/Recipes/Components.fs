module Cookbook.Client.Pages.Recipes.Components

open Fable.Core
open Feliz
open Feliz.Bulma
open Feliz.UseElmish

open Cookbook.Client.Router
open Cookbook.Client.Server
open Cookbook.Shared.Recipes
open Cookbook.Client.Components.Html
open Cookbook.Client
open Cookbook.Client.Pages.Recipes.Domain


let styles = Stylesheet.load "./recipeslist.module.scss"

[<ReactComponent>]
let RecipeCard (recipe: Contracts.RecipeListItem) =
    Html.div [
        prop.className styles.["recipe-card"]
        prop.children [
            Html.a [
                Html.img [
                    prop.style [
                        style.width (length.perc 100)
                        style.height length.auto
                    ]
                    prop.src "https://picsum.photos/id/292/600/400/?random"
                ]
            ]
            Html.a [
                yield! prop.routed (Page.RecipesEdit (Some recipe.Id))
                prop.text recipe.Name
            ]
        ]
    ]

[<ReactComponent>]
let Recipes (state: Contracts.RecipeListItem list) =
    let recipes =
        state
        |> List.map RecipeCard

    Html.div [
        prop.className styles.["recipes-list"]
        prop.children recipes
    ]

[<ReactComponent>]
let RecipesPage () =
    let state,dispatch = React.useElmish(State.init, State.update, [|  |])
    Bulma.columns [
        Bulma.column [
            column.is2
            prop.children [
                Bulma.button.a [
                    color.isPrimary
                    button.isFullWidth
                    yield! prop.routed (Page.RecipesEdit None)
                    prop.style [
                        style.marginBottom (length.rem 1 )
                        style.marginTop (length.rem 0.5 )
                    ]
                    prop.children [
                        Bulma.icon [
                            Html.i [
                                prop.classes ["fas"; "fa-plus"]
                            ]
                        ]
                        Html.span "Přidat"
                    ]
                ]
                Html.div "filtery header"
                Html.div [
                    prop.text "filtery"
                ]
            ]
        ]
        Bulma.column [
            column.is10
            prop.children [
                match state.Recipes with
                | Idle | InProgress -> Html.none
                | Finished recipes -> Recipes recipes
            ]
        ]
    ]
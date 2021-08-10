module Cookbook.Client.Pages.Recipes.Components

open Cookbook.Client.Router
open Feliz
open Feliz.Bulma

open Cookbook.Client.Components.Html
open Cookbook.Client

let styles = Stylesheet.load "./recipeslist.module.scss"

[<ReactComponent>]
let RecipeCard () =
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
                prop.href "#"
                prop.text "Guláš"
            ]
        ]
    ]

[<ReactComponent>]
let Recipes () =
    Html.div [
        prop.className styles.["recipes-list"]
        prop.children [
            RecipeCard ()
            RecipeCard ()
            RecipeCard ()
            RecipeCard ()
            RecipeCard ()
        ]
    ]

[<ReactComponent>]
let RecipesList () =
    Bulma.columns [
        Bulma.column [
            column.is2
            prop.children [
                Bulma.button.a [
                    color.isPrimary
                    button.isFullWidth
                    yield! prop.routed Page.RecipesAdd
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
                Recipes ()
            ]
        ]
    ]
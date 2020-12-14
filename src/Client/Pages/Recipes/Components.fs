module Cookbook.Client.Pages.Recipes.Components

open Feliz
open Feliz.Bulma


[<ReactComponent>]
let RecipesList () =
    Bulma.columns [
        Bulma.column [
            column.is3
            prop.children [
                Html.div "filtery header"
                Html.div [
                    prop.text "filtery"
                ]
            ]
        ]
        Bulma.column [
            column.is9
            prop.children [
                Html.div "recipe list"
            ]
        ]
    ]
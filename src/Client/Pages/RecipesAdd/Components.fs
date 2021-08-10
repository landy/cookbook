module Cookbook.Client.Pages.RecipesAdd.Components

open Feliz
open Feliz.Bulma

open Cookbook.Client


[<ReactComponent>]
let RecipeForm () =
    let styles = Stylesheet.load "./recipesadd.module.scss"
    Html.div [
        prop.className styles.["recipe-add-page"]
        prop.children [
            Html.div [
                prop.className styles.["recipe-form"]
                prop.children [
                    Html.form [
                        prop.children [
                            Bulma.field.div [
                                field.isHorizontal
                                prop.children [
                                    Bulma.fieldLabel [
                                        Bulma.label "Název"
                                    ]
                                    Bulma.fieldBody [
                                        Bulma.field.div [
                                            Bulma.control.div [
                                                Bulma.input.text [
                                                    prop.placeholder "Název"
                                                ]
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]

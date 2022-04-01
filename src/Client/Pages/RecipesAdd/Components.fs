module Cookbook.Client.Pages.RecipesAdd.Components

open Fable.Core
open Feliz
open Feliz.Bulma
open Fable.MarkdownToJsx

open Cookbook.Client

[<ReactComponent>]
let MarkdownDiv str =
    Markdown.render str

[<ReactComponent>]
let RecipeForm () =
    let styles = Stylesheet.load "./recipesadd.module.scss"
    let recipe,setRecipe = React.useState("")
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
                            Bulma.textarea [
                                prop.value recipe
                                prop.onChange setRecipe

                            ]
                            MarkdownDiv recipe
                        ]
                    ]
                ]
            ]

        ]
    ]
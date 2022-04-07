module Cookbook.Client.Pages.RecipesAdd.Components

open System
open Fable.Core
open Feliz
open Feliz.UseDeferred
open Feliz.Bulma
open Fable.MarkdownToJsx

open Cookbook.Client
open Cookbook.Shared.Recipes

[<ReactComponent>]
let MarkdownDiv str =
    Markdown.render str

[<ReactComponent>]
let RecipeForm () =
    let styles = Stylesheet.load "./recipesadd.module.scss"

    let recipeName,setRecipeName = React.useState("")
    let recipeDescription,setRecipeDescription = React.useState("")
    let formSaveState,setFormSaveState = React.useState(Deferred.HasNotStartedYet)

    let saveFormAction = async {
        let data : Request.SaveRecipe = {
            Id = Guid.NewGuid()
            Name = recipeName
            Description = recipeDescription
        }
        let! res = Server.onRecipesService (fun s -> s.SaveRecipe data)
        return res
    }
    let saveForm = React.useDeferredCallback((fun () -> saveFormAction), setFormSaveState)



    Html.div [
        prop.className styles.["recipe-add-page"]
        prop.children [
            Html.div [
                prop.className styles.["recipe-form"]
                prop.children [
                    Html.form [
                        prop.onSubmit (fun evnt ->
                            evnt.preventDefault()
                            JS.console.log("save")
                            saveForm()
                        )
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
                                                    prop.onChange setRecipeName
                                                    prop.value recipeName
                                                ]
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                            Bulma.textarea [
                                prop.placeholder "Popis"
                                prop.value recipeDescription
                                prop.onChange setRecipeDescription

                            ]
                            Bulma.button.submit [
                                prop.value "Přidat recept"
                            ]
                        ]
                    ]
                ]
            ]
            Html.div [
                prop.className styles["recipe-description-formatted"]
                prop.children [
                    MarkdownDiv recipeDescription
                ]
            ]
        ]
    ]
module Cookbook.Client.Pages.RecipeEdit.Components

open System
open Cookbook.Shared.Recipes.Contracts
open Fable.Core
open Feliz
open Feliz.UseDeferred
open Feliz.UseElmish
open Feliz.Bulma
open Fable.MarkdownToJsx
open Fable.FormValidation

open Cookbook.Client
open Cookbook.Shared.Recipes
open Cookbook.Client.Components.Forms
open Cookbook.Client.Pages.RecipeEdit.Domain

[<ReactComponent>]
let MarkdownDiv str =
    Markdown.render str

[<ReactComponent>]
let RecipeForm (recipeId: Guid option) =
    let styles = Stylesheet.load "./recipesadd.module.scss"

    let state,dispatch = React.useElmish(State.init recipeId, State.update, [|  |])

    let recipeName,setRecipeName = React.useState(state.FormData.Data.Name)
    let recipeDescription,setRecipeDescription = React.useState("")
    let formSaveState,setFormSaveState = React.useState(Deferred.HasNotStartedYet)

    let rulesFor, validate, resetValidation, errors = useValidation()

    let saveFormAction = async {
        let data : Contracts.EditRecipe = {
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
                            let valid = validate()
                            if valid then
                                JS.console.log("save")
                                saveForm()
                            else
                                JS.console.log("invalid")
                        )
                        prop.children [
                            ValidatedTextInput state.FormData (FormChanged >> dispatch) EditRecipe.name []
                            Bulma.textarea [
                                prop.placeholder "Popis"
                                prop.value recipeDescription
                                prop.onChange setRecipeDescription

                            ]
                            Bulma.button.submit [
                                prop.value "Přidat recept"
                            ]
                            Html.div [
                                prop.children [
                                    errorSummary errors
                                ]
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
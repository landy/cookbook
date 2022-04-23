module Household.Api.Client.Pages.RecipeEdit.Components

open System
open Household.Api.Client.Server
open Household.Api.Shared.Recipes.Contracts
open Fable.Core
open Feliz
open Feliz.UseElmish
open Feliz.DaisyUI
open Fable.MarkdownToJsx

open Household.Api.Client
open Household.Api.Client.Components.Html
open Household.Api.Client.Components.Forms
open Household.Api.Client.Pages.RecipeEdit.Domain

[<ReactComponent>]
let RecipePreview (recipe: EditRecipe) =
    Html.divClassed "prose prose-sm" [
        Html.h1 recipe.Name
        Markdown.render recipe.Description
    ]


[<ReactComponent>]
let RecipeForm (recipeId: Guid option) =

    let state,dispatch = React.useElmish(State.init recipeId, State.update, [|  |])

    Html.divClassed "flex flex-col lg:flex-row gap-8" [
        Html.divClassed "lg:w-1/2" [
            Html.form [
                prop.onSubmit (fun evnt ->
                    evnt.preventDefault()
                    SaveRecipe |> dispatch
                )
                prop.children [
                    ValidatedTextInput state.FormData (FormChanged >> dispatch) EditRecipe.name []
                    ValidatedTextArea state.FormData (FormChanged >> dispatch) EditRecipe.description
                    Daisy.formControl [
                        Daisy.button.submit [
                            button.primary
                            prop.className "mt-3"
                            state.FormData.Data.Id |> Option.map (fun _ -> prop.value "Uložit") |> Option.defaultValue (prop.value "Vytvořit")
                            prop.disabled (state.FormData |> RemoteData.isNotValid)
//                            if state.FormData.InProgress then button.loading
//                            button.loading
                        ]
                    ]
                ]
            ]
        ]
        Html.divClassed "lg:w-1/2 mt-10" [
            RecipePreview state.FormData.Data
        ]
    ]
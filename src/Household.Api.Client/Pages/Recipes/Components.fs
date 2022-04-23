module Household.Api.Client.Pages.Recipes.Components

open Feliz
open Feliz.DaisyUI
open Feliz.UseElmish

open Household.Api.Shared.Recipes
open Household.Api.Client.Router
open Household.Api.Client.Server
open Household.Api.Client.Components.Html
open Household.Api.Client
open Household.Api.Client.Pages.Recipes.Domain


let styles = Stylesheet.load "./recipeslist.module.scss"

[<ReactComponent>]
let RecipeCard (recipe: Contracts.RecipeListItem) =
    Html.a [
        yield! prop.routed (Page.RecipesEdit (Some recipe.Id))
        link.hover
        prop.children [
            Daisy.card [
                prop.className "shadow-lg"
                card.compact
                prop.children [
                    Html.figure [
                        Html.img [ prop.src "https://picsum.photos/id/292/600/400/?random" ]
                    ]
                    Daisy.cardBody [ Daisy.cardTitle recipe.Name ]
                ]
            ]
        ]
    ]

[<ReactComponent>]
let Recipes (state: Contracts.RecipeListItem list) =
    state
    |> List.map RecipeCard
    |> Html.divClassed "grid lg:grid-cols-3 gap-4"

[<ReactComponent>]
let RecipesPage () =
    let state,dispatch = React.useElmish(State.init, State.update, [|  |])
    Html.divClassed "flex flex-col lg:flex-row lg:gap-2" [
        Html.divClassed "lg:w-2/12" [
            Daisy.button.a [
                button.primary
                button.block
                prop.className "mb-4 mt-2 gap-2"
                yield! prop.routed (Page.RecipesEdit None)
                prop.children [
                    Html.i [
                        prop.classes ["fas"; "fa-plus"]
                    ]
                    Html.span "Přidat"
                ]
            ]
            Html.div "filtery header"
            Html.div [
                prop.text "filtery"
            ]
        ]
        Html.divClassed "lg:w-10/12" [
            match state.Recipes with
            | Idle | InProgress -> Html.none
            | Finished recipes -> Recipes recipes
        ]
    ]
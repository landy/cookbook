module Household.Api.Client.Components.NavBar

open Feliz
open Feliz.DaisyUI

open Household.Api.Client.App.Domain

let AppLogoLink () =
    Html.divClassed "text-2xl font-bold mr-8" [
        Daisy.link [
            link.hover
            yield! prop.routed Page.Main
            prop.text "HHM"
        ]
    ]

let MenuItem isSelected (page: Page) (label: string) =
    Html.li [
        prop.className
            (if isSelected page then "bordered"
            else "mb-1")
        prop.children [
            Html.a [
                yield! prop.routed page
                prop.text label
            ]
        ]
    ]

let Navigation page =
    let pagesEqual currPage  =
        currPage = page
    Daisy.menu [
        menu.horizontal
        prop.children [
            MenuItem pagesEqual Page.RecipesList "Recepty"
        ]
    ]

let MainNavBar page =
    Daisy.navbar [
        prop.className "mb-4 px-4 md:px-4 lg:px-16 shadow-lg"
        prop.children [
            Daisy.navbarStart [
                AppLogoLink ()
                Daisy.menu [
                    menu.horizontal
                    prop.children (Navigation page)
                ]
            ]

        ]
    ]
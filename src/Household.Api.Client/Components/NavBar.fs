module Cookbook.Client.Components.NavBar

open Feliz
open Feliz.DaisyUI

open Cookbook.Client.Router
open Cookbook.Client.Auth.Context
open Cookbook.Client.Components.Html


let MenuLink isSelected (page: Page) (label: string) =
    Daisy.button.a [
        if isSelected page then button.active
        yield! prop.routed page
        prop.text label
    ]

let MenuItem isSelected (page: Page) (label: string) =
    Html.li [
        if isSelected page then prop.className "bordered"
        prop.children [
            Html.a [
                yield! prop.routed page
                prop.text label
            ]
        ]
    ]

let UserInfo isSelected =
    let authCtx = React.useContext(authContext)
    match authCtx.CurrentUser with
    | Some user ->
        Html.divClassed "flex flex-0 justify-between items-center gap-x-4" [
            Html.div user.Name
            Daisy.button.button [
                yield! [button.error; button.outline; button.sm]
                prop.onClick (fun e ->
                    e.preventDefault()
                    authCtx.Logout()
                )
                prop.children [
                    Html.i [ prop.classes ["fas"; "fa-power-off"; "mr-2"] ]
                    Html.span "Odhlásit"
                ]
            ]
        ]
    | None ->
        MenuLink isSelected Page.Login "Login"

let pagesEqual currPage page =
        currPage = page

let NavbarNavigation isSelected =
    Daisy.menu [
        menu.horizontal
        prop.children [
            MenuItem isSelected Page.RecipesList "Recepty"
        ]
    ]

[<ReactComponent>]
let TopNavBar (page:Page) =
    let isSelected = pagesEqual page

    Daisy.navbar [
        prop.className "mb-4 px-4 md:px-4 lg:px-16 shadow-lg"
        prop.children [
            Daisy.navbarStart [
                Html.divClassed "text-2xl font-bold mr-8" [
                    Daisy.link [
                        link.hover
                        yield! prop.routed Page.Main
                        prop.text "HHM"
                    ]
                ]
                NavbarNavigation isSelected
            ]
            Daisy.navbarEnd [
                UserInfo isSelected
            ]
        ]
    ]
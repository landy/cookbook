module Cookbook.Client.Components.NavBar

open Cookbook.Client
open Feliz
open Feliz.Bulma
open Feliz.Router

open Cookbook.Client.Router
open Cookbook.Client.Auth.Context
open Cookbook.Client.Components.Html

let navbarStyles = Stylesheet.load "./navbar.module.scss"


[<ReactComponent>]
let NavbarMenuItem page (label:string) closeBurgerMenu (children: ReactElement list) =
    let url = page |> Page.toUrlSegments |> Router.formatPath
    let currentUrl = Router.currentPath () |> Router.formatPath

    Bulma.navbarItem.div [
        prop.key currentUrl
        if children |> List.isEmpty |> not then
            yield!
                [
                    navbarItem.hasDropdown
                    navbarItem.isHoverable
                ]

        prop.children [
            Bulma.navbarLink.a [
                prop.href url
                navbarLink.isArrowless
                prop.text label
                prop.onClick (fun e ->
                    closeBurgerMenu ()
                    Router.goToUrl e
                )
            ]
            if children |> List.isEmpty |> not then
                Bulma.navbarDropdown.div [
                    navbarDropdown.isBoxed
                    prop.children children
                ]
        ]
    ]

[<ReactComponent>]
let NavbarLink page (label: string) closeBurgerMenu =
    let url = page |> Page.toUrlSegments |> Router.formatPath
    Bulma.navbarItem.a [
        prop.href url
        prop.text label
        prop.onClick (fun e ->
            closeBurgerMenu ()
            Router.goToUrl e
        )
    ]

let UserInfo closeMenu =
    let authCtx = React.useContext(authContext)
    match authCtx.CurrentUser with
    | Some user ->
        Bulma.navbarItem.div [
            prop.children [
                Html.div [
                    prop.className navbarStyles.["user-info"]
                    prop.children [
                        Html.div [
                            prop.className navbarStyles.["user-name"]
                            prop.text user.Name
                        ]
                        Html.div [
                            prop.children [
                                Bulma.button.button [
                                    button.isInverted
                                    prop.onClick (fun e ->
                                        e.preventDefault()
                                        closeMenu ()
                                        authCtx.Logout()

                                    )
                                    prop.children [
                                        Bulma.icon [
                                            Html.i [
                                                prop.classes ["fas"; "fa-power-off"]
                                            ]
                                        ]
                                        Html.span "Odhlásit"
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]

    | None ->
        NavbarLink Page.Login "Login" closeMenu

[<ReactComponent>]
let TopNavBar (page:Page) =
    let isActive,setIsActive = React.useState(false)
    let url = page |> Page.toUrlSegments |> Router.formatPath
    let closeMenu = (fun  _ -> false |> setIsActive)
    Bulma.navbar [
        navbar.isTransparent
        navbar.hasShadow
        prop.children [
            Bulma.container[
                Bulma.navbarBrand.div [
                    Bulma.navbarItem.a [
                        size.isSize3
                        yield! prop.routed Page.Main
                        prop.text "HHM"
                    ]
                    Bulma.navbarBurger [
                        if isActive then navbarBurger.isActive
                        prop.onClick (fun e -> e.preventDefault(); isActive |> not |> setIsActive)
                        prop.children [
                            Html.span [ prop.ariaHidden true ]
                            Html.span [ prop.ariaHidden true ]
                            Html.span [ prop.ariaHidden true ]
                        ]
                    ]
                ]
                Bulma.navbarMenu [
                    if isActive then navbarMenu.isActive
                    prop.key url
                    prop.children [
                        Bulma.navbarStart.div [
                            NavbarMenuItem Page.RecipesList "Recepty" closeMenu []
                        ]
                        Bulma.navbarEnd.div [
                            Bulma.dropdownDivider []
                            UserInfo closeMenu
                        ]
                    ]
                ]
            ]
        ]
    ]
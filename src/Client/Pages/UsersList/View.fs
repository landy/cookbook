module Cookbook.Client.Pages.UsersList.View

open Feliz
open Feliz.UseElmish
open Feliz.MaterialUI
open Fable.MaterialUI.Icons

open Domain
open Cookbook.Client.Router
open Cookbook.Client.Components.FadedLoader
open Cookbook.Client.Components.Html


let useStyles = Styles.makeStyles (fun styles theme ->
    {|
        root = styles.create [
            style.display.flex
            style.flexDirection.column
        ]
        contextButtons = styles.create [
            style.margin ((theme.spacing 2), 0, (theme.spacing 1), 0)
            style.display.flex
            style.justifyContent.flexEnd
        ]
        loader = styles.create [
            style.display.flex
            style.justifyContent.center
            style.alignItems.center
        ]
    |}
)

let render = React.functionComponent (fun () ->
    let state, dispatch = React.useElmish(State.init, State.update, [||])

    let s = useStyles ()
    Mui.container [
        prop.className s.root
        prop.children [
            Html.div [
                prop.className s.contextButtons
                prop.children [
                    Mui.button [
                        button.variant.contained
                        button.color.primary
                        prop.text "Add"
                        button.startIcon (addIcon [])
                        yield! prop.routed UsersAdd
                    ]
                ]
            ]
            if state.IsLoading |> not || state.Users |> List.isEmpty |> not then
                Mui.paper [
                    Mui.table [
                        Mui.tableHead [
                            Mui.tableRow [
                                Mui.tableCell "Username"
                                Mui.tableCell "Name"
                                Mui.tableCell ""
                            ]
                        ]
                        Mui.tableBody [
                            prop.children
                                (state.Users
                                |> List.map (fun user ->
                                    Mui.tableRow [
                                        Mui.tableCell user.Username
                                        Mui.tableCell user.Name
                                        Mui.tableCell ""
                                    ]
                                ))
                        ]
                    ]
                ]
            else
                FadedLoader state.IsLoading 800. [
                    prop.className s.loader
                ]
        ]
    ]
)
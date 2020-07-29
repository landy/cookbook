module Cookbook.Client.Pages.UsersList.View

open Feliz
open Feliz.UseElmish
open Feliz.MaterialUI
open Fable.MaterialUI.Icons

open Domain


let useStyles = Styles.makeStyles (fun styles theme ->
    {|
        contextButtons = styles.create [
            style.display.flex
            style.justifyContent.flexEnd
        ]
        usersTable = styles.create [
            style.marginTop (theme.spacing 1)
        ]
        loaderContainer = styles.create [
            style.display.flex
            style.justifyContent.center
            style.alignItems.center
            style.flexDirection.column
            style.flexGrow 1
            style.flexShrink 1
            style.flexBasis (length.percent 0)
        ]
    |}
)

let render = React.functionComponent (fun () ->
    let state, dispatch = React.useElmish(State.init, State.update, [||])

    let s = useStyles ()
    Mui.container [
        Html.div [
            prop.className s.contextButtons
            prop.children [
                Mui.button [
                    button.variant.contained
                    button.color.primary
                    prop.text "Add"
                    button.startIcon (addIcon [])
                ]
            ]
        ]
        if true = false then
            Mui.paper [
                prop.className s.usersTable
                prop.children [
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
            ]
        else
            Html.div [
                prop.className s.loaderContainer
                prop.children [
                    Mui.circularProgress [ ]
                ]
            ]
    ]
)
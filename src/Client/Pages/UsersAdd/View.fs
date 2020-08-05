module Cookbook.Client.Pages.UsersAdd.View

open Feliz
open Feliz.MaterialUI
open Feliz.UseElmish

open Domain

let useStyles = Styles.makeStyles(fun styles theme ->
    {|
        root = styles.create [
            style.margin (theme.spacing 2)
        ]
        paper = styles.create [
            style.padding (theme.spacing 4)
        ]
        form = styles.create [
            style.display.flex
            style.flexDirection.column

        ]
        formGroup = styles.create [
            style.marginBottom (theme.spacing 2)
        ]
        textField = styles.create [
            style.marginRight (theme.spacing 1)
            style.width (length.ch 25)
        ]
        wrapper = styles.create [
            style.marginTop  (theme.spacing(1))
            style.position.relative
        ]
        submitRow = styles.create [
            style.display.flex
            style.alignSelf.flexStart
        ]
        submitProgress = styles.create [
            style.position.absolute
            style.top (length.percent 50)
            style.left (length.percent 50)
            style.marginTop -12
            style.marginLeft -12
            style.color theme.palette.primary.light
        ]
    |}
)

let render = React.functionComponent(fun () ->
    let state,dispatch = React.useElmish(State.init, State.update, [||])

    let s = useStyles()

    Html.div [
        prop.className s.root
        prop.children [
            Mui.paper [
                prop.className s.paper
                prop.children [
                    Mui.typography [
                        typography.variant.h6
                        typography.color.inherit'
                        typography.children [
                            Html.div "Add user"
                        ]
                    ]
                    Html.form [
                        prop.autoComplete "off"
                        prop.className s.form
                        prop.onSubmit (fun e ->
                            e.preventDefault()
                            Started |> SaveUser |> dispatch
                        )
                        prop.children [
                            Mui.formGroup [
                                prop.className s.formGroup
                                prop.children [
                                    Mui.textField [
                                        prop.className s.textField
                                        textField.value state.FormData.Username
                                        textField.label "Username"
                                        textField.margin.dense
                                        textField.variant.outlined
                                        textField.name (nameof state.FormData.Username)
                                        textField.onChange (fun v -> FormChanged (fun f -> {f with Username = v}) |> dispatch)
                                    ]
                                    Mui.textField [
                                        prop.className s.textField
                                        textField.value state.FormData.Name
                                        textField.label "Name"
                                        textField.margin.dense
                                        textField.name (nameof state.FormData.Name)
                                        textField.variant.outlined
                                        textField.onChange (fun v -> FormChanged (fun f -> {f with Name = v}) |> dispatch)
                                    ]
                                    Mui.textField [
                                        prop.className s.textField
                                        textField.label "Password"
                                        textField.margin.dense
                                        textField.variant.outlined
                                        textField.value state.FormData.Password
                                        textField.name (nameof state.FormData.Password)
                                        prop.type'.password
                                        textField.onChange (fun v -> FormChanged (fun f -> {f with Password = v}) |> dispatch)
                                    ]
                                    Mui.textField [
                                        prop.className s.textField
                                        textField.label "Confirm password"
                                        textField.margin.dense
                                        textField.variant.outlined
                                        textField.value state.FormData.ConfirmPassword
                                        textField.name (nameof state.FormData.ConfirmPassword)
                                        prop.type'.password
                                        textField.onChange (fun v -> FormChanged (fun f -> {f with ConfirmPassword = v}) |> dispatch)
                                    ]
                                ]
                            ]
                            Html.div [
                                prop.className s.submitRow
                                prop.children [
                                    Html.div [
                                        prop.className s.wrapper
                                        prop.children [
                                            Mui.button [
                                                prop.type'.submit
                                                prop.text "Save"
                                                button.fullWidth true
                                                button.disabled state.IsSaving
                                                button.variant.contained
                                                button.color.primary
                                            ]
                                            if state.IsSaving then
                                                Mui.circularProgress [
                                                    circularProgress.classes.root s.submitProgress
                                                    circularProgress.size 24
                                                    circularProgress.variant.indeterminate
                                                ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]
)
module Cookbook.Client.Pages.UsersAdd.View

open Feliz
open Feliz.Bulma
open Feliz.UseElmish
open Aether

open Domain

[<ReactComponent>]
let Render () =
    let state,dispatch = React.useElmish(State.init, State.update, [||])


    Html.div [
        prop.children [
            Bulma.box [
                prop.children [
                    Html.form [
                        prop.autoComplete "off"
                        prop.onSubmit (fun e ->
                            e.preventDefault()
                            Started |> SaveUser |> dispatch
                        )
                        prop.children [
                            Bulma.field.div [
                                field.isGrouped
                                prop.children [
                                    Bulma.control.div [
                                        Bulma.label "Username"
                                        Bulma.input.text [
                                            prop.value state.FormData.Username
                                            prop.name (nameof state.FormData.Username)
                                            prop.onChange (Optic.set Optics.username >> FormChanged >> dispatch)
                                        ]
                                    ]
                                    Bulma.control.div [
                                        Bulma.label "Name"
                                        Bulma.input.text [
                                            prop.value state.FormData.Username
                                            prop.name (nameof state.FormData.Name)
                                            prop.onChange (Optic.set Optics.name >> FormChanged >> dispatch)
                                        ]
                                    ]
//                                    Mui.textField [
//                                        prop.className s.textField
//                                        textField.label "Password"
//                                        textField.margin.dense
//                                        textField.variant.outlined
//                                        textField.value state.FormData.Password
//                                        textField.name (nameof state.FormData.Password)
//                                        prop.type'.password
//                                        textField.onChange (Optic.set Optics.password >> FormChanged >> dispatch)
//                                    ]
//                                    Mui.textField [
//                                        prop.className s.textField
//                                        textField.label "Confirm password"
//                                        textField.margin.dense
//                                        textField.variant.outlined
//                                        textField.value state.FormData.ConfirmPassword
//                                        textField.name (nameof state.FormData.ConfirmPassword)
//                                        prop.type'.password
//                                        textField.onChange (Optic.set Optics.confirmPassword >> FormChanged >> dispatch)
//                                    ]
                                ]
                            ]
                            Html.div [
                                prop.children [
                                    Html.div [
                                        prop.children [
                                            Bulma.button.submit [
                                                prop.value "Save"
                                                prop.disabled state.IsSaving
                                                color.isPrimary
                                                if state.IsSaving then
                                                    button.isLoading
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

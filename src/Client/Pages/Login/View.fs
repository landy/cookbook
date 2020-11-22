module Cookbook.Client.Pages.Login.View

open Cookbook.Client
open Cookbook.Shared.Users
open Fable.Core.JsInterop
open Feliz
open Feliz.Bulma
open Feliz.UseElmish
open Domain

let private stylesheet = Stylesheet.load "./styles.module.scss"

[<ReactComponent>]
let render props =
    let state, dispatch = React.useElmish (State.init, State.update props, [|  |])
    printfn "class: %s" stylesheet.["test"]

    Bulma.container [
        prop.children [
            Bulma.box [
                prop.classes [stylesheet.["test"]]
                prop.children [
                    Html.div [
                        prop.children
                            (state.Errors
                            |> List.map (fun err ->
                                Bulma.message [
                                    color.isDanger
                                    prop.children [
                                        Bulma.messageBody err
                                    ]
                                ]
                            ))
                    ]
                    Html.form [
                        prop.onSubmit (fun e ->
                            e.preventDefault()
                            Login |> dispatch
                        )
                        prop.children [
                            Bulma.field.div [
                                Bulma.label "Username"
                                Bulma.input.text [
                                    prop.onChange (UsernameChanged >> dispatch)
                                    prop.value state.Form.Username
                                    prop.autoFocus true
                                    prop.name "username"
                                    prop.required true
                                ]
                            ]
                            Bulma.field.div [
                                Bulma.label "Password"
                                Bulma.input.password [
                                    prop.onChange (PasswordChanged >> dispatch)
                                    prop.name "password"
                                    prop.required true
                                    prop.value state.Form.Password
                                ]
                            ]
                            Bulma.field.div [
                                prop.children [
                                    Bulma.button.submit [
                                        if state.IsLoading then button.isLoading
                                        prop.value "Login"
                                    ]
                                ]
                            ]
                        ]
                    ]

                ]
            ]
        ]
    ]

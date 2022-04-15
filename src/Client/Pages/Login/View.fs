module Cookbook.Client.Pages.Login.View

open FsToolkit.ErrorHandling

open Cookbook.Client.Router
open Cookbook.Client.Server
open Cookbook.Client.Auth.Context
open Cookbook.Shared.Users
open Cookbook.Shared.Errors
open Cookbook.Shared.Users.Response
open Fable.Core.JsInterop
open Feliz
open Feliz.UseDeferred
open Feliz.Bulma
open Domain

//let private stylesheet = Stylesheet.load "./styles.module.scss"

[<ReactComponent>]
let LoginForm () =
    let auth = React.useContext(authContext)
    let username, setUsername = React.useState("")
    let password, setPassword = React.useState("")
    let (errors: string list), setErrors = React.useState([])

    let handleLogin = async {
        let! loginResult =
            ({Username = username; Password = password}: Request.Login)
            |> usersService.Login
        auth.SetUser loginResult

        Router.navigatePage Page.Main
        return loginResult
    }

    let loginState, setLoginState = React.useState(Deferred.HasNotStartedYet)
    let startLogin = React.useDeferredCallback((fun () -> handleLogin), setLoginState)


    Bulma.container [
        prop.children [
            Bulma.box [
                prop.children [
                    Html.div [
                        prop.children
                            (errors
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
                            startLogin()
                        )
                        prop.children [
                            Bulma.field.div [
                                prop.children [
                                    Bulma.label [
                                        text.hasTextLeft
                                        prop.text "Uživatelské jméno"
                                    ]
                                    Bulma.control.div [
                                        Bulma.input.text [
                                            prop.onChange (setUsername)
                                            prop.value username
                                            prop.autoFocus true
                                            prop.name "username"
                                            prop.required true
                                        ]
                                    ]
                                ]
                            ]
                            Bulma.field.div [
                                Bulma.label [
                                    text.hasTextLeft
                                    prop.text "Heslo"
                                ]
                                Bulma.input.password [
                                    prop.onChange setPassword
                                    prop.name "password"
                                    prop.required true
                                    prop.value password
                                ]
                            ]
                            Bulma.field.div [
                                prop.children [
                                    Bulma.button.submit [
                                        color.isSuccess
                                        spacing.px6
                                        size.isSize5

                                        if Deferred.inProgress loginState then button.isLoading
                                        prop.value "Přihlásit"
                                    ]
                                ]
                            ]
                        ]
                    ]

                ]
            ]
        ]
    ]
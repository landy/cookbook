module Household.Api.Client.Pages.Login.View

open Fable.Core
open Feliz.DaisyUI

open Household.Api.Client.Components.Html
open Household.Api.Client.Router
open Household.Api.Client.Server
open Household.Api.Client.Auth.Context
open Household.Api.Shared.Users
open Feliz
open Feliz.UseDeferred

//let private stylesheet = Stylesheet.load "./styles.module.scss"

[<ReactComponent>]
let LoginForm () =
    let auth = React.useContext(authContext)
    let username, setUsername = React.useState("")
    let password, setPassword = React.useState("")

    let handleLogin = async {
        let! loginResult =
            ({Username = username; Password = password}: Request.Login)
            |> usersService.Login
        JS.console.log(loginResult)
        auth.SetUser loginResult

        Router.navigatePage Page.Main
        return loginResult
    }

    let loginState, setLoginState = React.useState(Deferred.HasNotStartedYet)
    let startLogin = React.useDeferredCallback((fun () -> handleLogin), setLoginState)


    Html.divClassed "flex items-center justify-center bg-cyan-200 min-h-screen" [
        Html.divClassed "shadow-xl px-8 py-6 w-1/4 bg-white rounded-lg" [
            Html.div [
                prop.className "text-2xl font-bold mb-4 text-center"
                prop.text "Přihlášení"
            ]
            Html.form [
                prop.onSubmit (fun e ->
                    e.preventDefault()
                    startLogin()
                )
                prop.className "space-y-4"
                prop.children [
                    Daisy.formControl [
                        Daisy.label [ Daisy.labelText "Uživatelské jméno" ]
                        Daisy.input [
                            prop.className "w-full px-4 py-2 mt-2 border rounded-md"
                            prop.onChange setUsername
                            input.bordered
                            prop.value username
                            prop.autoFocus true
                            prop.name "username"
                            prop.required true
                        ]
                    ]
                    Daisy.formControl [
                        Daisy.label [ Daisy.labelText "Heslo" ]
                        Daisy.input [
                            prop.onChange setPassword
                            input.bordered
                            prop.type'.password
                            prop.value password
                            prop.name "password"
                            prop.required true
                        ]
                    ]
                    Daisy.formControl [
                        Daisy.button.submit [
                            button.primary
                            prop.value "Přihlásit"
                        ]
                    ]
                ]
            ]
        ]
    ]
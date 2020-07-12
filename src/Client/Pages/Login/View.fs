module Cookbook.Client.Pages.Login.View

open Feliz
open Feliz.MaterialUI
open Feliz.UseElmish

open Cookbook.Client.Pages.Login.State

let useStyles = Styles.makeStyles(fun styles theme ->
  {|
    paper = styles.create [
      style.marginTop (theme.spacing 8)
      style.display.flex
      style.flexDirection.column
      style.alignItems.center
    ]
    progressBar = styles.create [
      style.margin (theme.spacing 1)
    ]
    avatar = styles.create [
      style.margin (theme.spacing 1)
      style.backgroundColor theme.palette.primary.main
    ]
    form = styles.create [
      style.width (length.perc 100)  // Allegedly fixes an IE 11 issue
      style.marginTop (theme.spacing 1)
    ]
    submit = styles.create [
      style.margin (theme.spacing(3, 0, 2))
    ]
  |}
)

let render = React.functionComponent(fun () ->
    let classes = useStyles()
    let state, dispatch = React.useElmish (State.init, State.update, [|  |])

    Mui.container [
        container.component' "main"
        container.maxWidth.xs
        container.children [
            Html.div [
                prop.className classes.paper
                prop.children [
                    Mui.typography [
                        typography.component' "h1"
                        typography.variant.h5
                        typography.children "Sign in"
                    ]
                    Html.form [
                        prop.onSubmit (fun e ->
                            e.preventDefault()
                            FormSubmitted |> dispatch
                        )
                        prop.className classes.form
                        prop.type' ""
                        prop.children [
                            Mui.textField [
                                textField.label "Username"
                                textField.onChange (UsernameChanged >> dispatch)
                                textField.variant.outlined
                                textField.margin.normal
                                textField.required true
                                textField.fullWidth true
                                textField.id "username"
                                textField.name "username"
                                textField.autoComplete "username"
                                textField.autoFocus true
                                textField.value state.Username
                            ]
                            Mui.textField [
                                textField.type' "password"
                                textField.label "Password"
                                textField.onChange (PasswordChanged >> dispatch)
                                textField.variant.outlined
                                textField.margin.normal
                                textField.required true
                                textField.fullWidth true
                                textField.id "password"
                                textField.name "password"
                                textField.autoComplete "current-password"
                                textField.value state.Password
                            ]
                            Mui.button [
                                button.type'.submit
                                button.fullWidth true
                                button.variant.contained
                                button.color.primary
                                button.classes.root classes.submit
                                button.children "Sign In"
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]


)
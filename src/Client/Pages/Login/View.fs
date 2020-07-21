module Cookbook.Client.Pages.Login.View

open Cookbook.Shared.Auth
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
    errors = styles.create [
        style.width (length.perc 100)
    ]
    buttonProgress = styles.create [
        style.position.absolute
        style.top (length.perc 50)
        style.right (length.perc 5)
        style.marginTop -12
        style.marginRight -12
        style.color theme.palette.primary.light
    ]
    wrapper = styles.create [
        style.margin  (theme.spacing(1))
        style.position.relative
    ]
  |}
)



let render = React.functionComponent("LoginPage", fun (props:LoginPageProps) ->
    let classes = useStyles()
    let state, dispatch = React.useElmish (State.init, State.update props, [|  |])

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
                    Html.div [
                        prop.className classes.errors
                        prop.children
                            (state.Errors
                            |> List.map (fun err ->
                                Mui.alert [
                                    alert.severity.error
                                    alert.elevation 1
                                    alert.children err
                                ]
                            ))
                    ]
                    Html.form [
                        prop.onSubmit (fun e ->
                            e.preventDefault()
                            Login |> dispatch
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
                                textField.value state.Form.Username
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
                                textField.value state.Form.Password
                            ]
                            Html.div [
                                prop.className classes.wrapper
                                prop.children [
                                    yield Mui.button [
                                        button.type'.submit
                                        button.fullWidth true
                                        button.disabled (state.IsLoading)
                                        button.variant.contained
                                        button.color.primary
                                        button.children "Sign In"
                                    ]
                                    if state.IsLoading then
                                        yield Mui.circularProgress [
                                            circularProgress.classes.root classes.buttonProgress
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
)
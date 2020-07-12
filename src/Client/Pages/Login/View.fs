module Cookbook.Client.Pages.Login.View

open Feliz
open Feliz.MaterialUI
open Feliz.UseElmish

let render = React.functionComponent(fun () ->
    Html.form [
        Mui.textField [
            textField.label "Username"
        ]
        Mui.textField [
            textField.type' "password"
            textField.label "Password"
        ]
    ]
)
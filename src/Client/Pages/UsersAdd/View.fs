module Cookbook.Client.Pages.UsersAdd.View

open Feliz
open Feliz.MaterialUI
open Feliz.UseElmish

let render = React.functionComponent(fun () ->
    let state,dispatch = React.useElmish(State.init, State.update, [||])

    Mui.container [
        prop.children [
            Html.div "users add"
        ]
    ]
)
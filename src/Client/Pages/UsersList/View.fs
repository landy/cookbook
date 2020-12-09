module Cookbook.Client.Pages.UsersList.View

open Feliz
open Feliz.Bulma
open Feliz.UseElmish

open Domain
open Cookbook.Client.Router
open Cookbook.Client.Components.Html


let render = React.functionComponent (fun () ->
    let state, dispatch = React.useElmish(State.init, State.update, [||])
    Html.div "tesdd"
//    Bulma.container [
//        prop.children [
//            Html.div [
//                prop.children [
//                    Bulma.button.button [
//                        color.isPrimary
//                        prop.text "Add"
//                        yield! prop.routed UsersAdd
//                    ]
//                ]
//            ]
//            if state.IsLoading |> not || state.Users |> List.isEmpty |> not then
//                Bulma.box [
//                    Bulma.table [
//                        Html.thead [
//                            Html.tr [
//                                Html.th "Usernameeee"
//                                Html.th "Name"
//                                Html.th ""
//                            ]
//                        ]
//                        Html.tbody [
//                            prop.children
//                                (state.Users
//                                |> List.map (fun user ->
//                                    Html.tr [
//                                        Html.th user.Username
//                                        Html.th user.Name
//                                        Html.th ""
//                                    ]
//                                ))
//                        ]
//                    ]
//                ]
//            else
//                Html.div "loading"
//        ]
//    ]
)
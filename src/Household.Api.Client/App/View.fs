module Household.Api.Client.App.View

open Feliz
open Feliz.Router



[<ReactComponent>]
let MainApplication() =
    React.strictMode [
        React.router [
            router.pathMode
            router.children [
                Html.div "main"
            ]
        ]
    ]
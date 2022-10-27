module Household.Api.Client.Layouts.Main.View

open Feliz

[<ReactComponent>]
let MainLayout () =
    Html.div [
        prop.className "w-[300px]"
        prop.text "dd"
    ]
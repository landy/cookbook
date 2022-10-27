module App.Components

open Feliz

[<ReactComponent>]
let MyLogin () =
    Html.div [
        prop.className "bg-white w-[300px] h-[200px] dark:bg-slate-800"
        prop.text "testaaass"
    ]
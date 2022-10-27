module App.FelizApp

open Feliz

open Feliz.DaisyUI

Fable.Core.JsInterop.importAll "./index.css"

[<ReactComponent>]
let Foo() =
    Html.div [
        prop.className "w-[500px] h-[500px] bg-black text-white"
        prop.text "fsdfsd"
    ]
    // Html.div "testsss aaafasdfasd "

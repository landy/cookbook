module App.FelizApp

open Feliz

open Feliz.DaisyUI
open App.Components

Fable.Core.JsInterop.importAll "./index.css"

[<ReactComponent>]
let Foo() =
    MyLogin()
    // Html.div "testsss aaafasdfasd "

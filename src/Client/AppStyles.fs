module Cookbook.Client.AppStyles

open Fable.Core.JS
open Fable.Core.JsInterop

let initStyles () =
    console.log("init styles")
    importSideEffects "./styles/style.scss"
    ()

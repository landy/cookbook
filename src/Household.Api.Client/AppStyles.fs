module Cookbook.Client.AppStyles

open Fable.Core.JS
open Fable.Core.JsInterop

let initStyles () =
    console.log("init styles")
    importSideEffects "./styles/style.css"
//    importSideEffects "bulma"
    importSideEffects "@fortawesome/fontawesome-free/css/all.min.css"
    ()
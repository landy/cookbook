module Cookbook.Client.AppStyles

open Fable.Core.JsInterop

let initStyles () =
    importSideEffects "./style.scss"
    ()

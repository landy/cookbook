module Cookbook.Client.AppStyles

open Fable.Core.JsInterop


let init () =
    importSideEffects "./style.scss"
    ()

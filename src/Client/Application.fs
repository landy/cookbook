module Cookbook.Client.Application


open Fable.Core.JsInterop
open Feliz
open Browser.Dom

let elem = Html.div "test"
ReactDOM.render(elem, document.getElementById("elmish-app"))


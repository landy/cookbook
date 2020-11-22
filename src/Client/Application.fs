module Cookbook.Client.Application


open Fable.Core.JsInterop
let props : Pages.Login.State.LoginPageProps = { handleNewToken = ignore }
let foo = Pages.Login.View.render props
importSideEffects "./style.scss"
open Feliz
open Browser.Dom


let elem = Html.div "test"
ReactDOM.render(foo , document.getElementById("elmish-app"))

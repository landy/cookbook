module Household.Api.Client.App

open Feliz
open Browser.Dom

open Household.Api.Client.App.View
Fable.Core.JsInterop.importAll "./index.css"

// Entry point must be in a separate file
// for Vite Hot Reload to work


ReactDOM.render(MainApplication() , document.getElementById("app-container"))
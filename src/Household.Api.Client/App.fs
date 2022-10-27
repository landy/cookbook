module Household.Api.Client.App

open Feliz
open Browser.Dom

open Household.Api.Client.Layouts.Main.View

// Entry point must be in a separate file
// for Vite Hot Reload to work


ReactDOM.render(MainLayout() , document.getElementById("app-container"))
module Household.Api.Client.Application

open Household.Api.Client.AppStyles
initStyles()

open Feliz
open Browser.Dom

open Household.Api.Client.App.View


ReactDOM.render(MainApplication() , document.getElementById("elmish-app"))
module Cookbook.Client.Application

open Cookbook.Client.AppStyles
initStyles()

open Feliz
open Browser.Dom

open Cookbook.Client.App.View

open Cookbook.Client



ReactDOM.render(MainApplication() , document.getElementById("elmish-app"))
module Cookbook.Client.Application

open Cookbook.Client.AppStyles
initStyles()

open Feliz
open Browser.Dom

open Cookbook.Client.App.View



ReactDOM.render(MainApplication() , document.getElementById("elmish-app"))
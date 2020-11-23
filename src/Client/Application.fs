module Cookbook.Client.Application

open Cookbook.Client.AppStyles
init()

open Feliz
open Browser.Dom


let props : Pages.Login.State.LoginPageProps = { handleNewToken = ignore }
ReactDOM.render(Pages.Login.View.render props , document.getElementById("elmish-app"))

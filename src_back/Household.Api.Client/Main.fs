module Main

open App
open Feliz
open Browser.Dom

// Entry point must be in a separate file
// for Vite Hot Reload to work

ReactDOM.render(FelizApp.Foo() , document.getElementById("app-container"))
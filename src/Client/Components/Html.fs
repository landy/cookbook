[<AutoOpen>]
module Cookbook.Client.Components.Html

open Feliz
open Feliz.Router
open Fable.Core

open Cookbook.Client.Router


[<Erase>]
type prop =
    static member inline routed (page : Page) = [
        prop.href (page |> Page.toUrlSegments |> Router.formatPath)
        prop.onClick (Router.goToUrl)
    ]

[<Erase>]
type Html =
    static member inline aRouted (text: string) (page: Page) =
        Html.a [
            yield! prop.routed page
            prop.text text
        ]
module Cookbook.Client.App.Domain

open Cookbook.Client.Router
open Cookbook.Shared.Users

type Msg =
    | UrlChanged of Page
    | TokenChanged of Response.LoggedInUser option


type Model = {
    CurrentPage: Page
    Token : Response.LoggedInUser option
}

module Styles =

    type RootViewStyles = {
        root : string
        content : string
    }




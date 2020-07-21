module Cookbook.Client.App.Domain

open Cookbook.Client.Router
open Cookbook.Shared.Auth

type Msg =
    | UrlChanged of Page
    | TokenChanged of Response.Token option


type Model = {
    CurrentPage: Page
    Token : Response.Token option
}

module Styles =

    type RootViewStyles = {
        root : string
        appBar : string
        drawer : string
        drawerPaper : string
        content : string
        toolbar : string
    }




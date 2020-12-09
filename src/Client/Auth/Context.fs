module Cookbook.Client.Auth.Context

open Feliz
open Cookbook.Client.Auth.Domain


let authContext = React.createContext(name = "AuthContext", defaultValue = {CurrentUser = None; SetUser = ignore; Logout = ignore})

[<ReactComponent>]
let AuthContext ctx (children: ReactElement) =

    React.contextProvider(authContext, ctx, React.fragment[
        children
    ])
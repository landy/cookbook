module Cookbook.Client.Pages.Login.Domain

open Cookbook.Shared.Users
open Cookbook.Shared.Errors


type LoginForm = {
    Username : string
    Password : string
}
type Model = {
    Form : LoginForm
    Errors : string list
    IsLoading : bool
}

type Msg =
    | UsernameChanged of string
    | PasswordChanged of string
    | Login
    | LoggedIn of Result<Response.LoggedInUser, ApplicationError>
module Household.Api.Client.Pages.Login.Domain

open Household.Api.Shared.Users
open Household.Api.Shared.Errors


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
    | LoggedIn of Response.UserSession
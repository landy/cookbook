module Household.Api.Client.Auth.Domain

open Household.Api.Shared.Users.Response

type AuthContext = {
    CurrentUser : UserSession option
    SetUser : UserSession -> unit
    Logout : unit -> unit
}
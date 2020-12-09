module Cookbook.Client.Auth.Domain

open Cookbook.Shared.Users.Response

type AuthContext = {
    CurrentUser : UserSession option
    SetUser : UserSession -> unit
    Logout : unit -> unit
}
module Cookbook.Shared.Errors


type AuthenticationError =
    | InvalidUsernameOrPassword

[<RequireQualifiedAccess>]
module AuthenticationError =
    let explain = function
        | InvalidUsernameOrPassword -> "Invalid username or password"



type ApplicationError =
    | AuthenticationError of AuthenticationError

[<RequireQualifiedAccess>]
module ApplicationError =
    let explain = function
        | AuthenticationError err -> AuthenticationError.explain err
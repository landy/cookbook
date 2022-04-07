module Cookbook.Shared.Errors

type DatabaseError =
    | Unspecified
    | Exception of exn

[<RequireQualifiedAccess>]
module DatabaseError =
    let explain = function
        | Unspecified -> "Unspecified Database error occured"

type UserError =
    | UserAlreadyExists of username : string

type RecipeError =
    | Foo

[<RequireQualifiedAccess>]
module UserError =
    let explain = function
        | UserAlreadyExists username -> sprintf "User with username '%s' already exists" username

type AuthenticationError =
    | InvalidUsernameOrPassword

[<RequireQualifiedAccess>]
module AuthenticationError =
    let explain = function
        | InvalidUsernameOrPassword -> "Invalid username or password"



type ApplicationError =
    | AuthenticationError of AuthenticationError
    | UserError of UserError
    | RecipeError of RecipeError
    | DatabaseError of DatabaseError

[<RequireQualifiedAccess>]
module ApplicationError =
    let explain = function
        | AuthenticationError err -> AuthenticationError.explain err
        | UserError err -> UserError.explain err
        | DatabaseError err -> DatabaseError.explain err
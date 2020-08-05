module Cookbook.Client.Pages.UsersAdd.Domain

open Cookbook.Shared.Errors

type AsyncOperationStatus<'t> =
    | Started
    | Finished of 't

type Deferred<'t> =
    | NotStarted
    | InProgress
    | Resolved of 't

type FormData = {
    Username : string
    Name : string
    Password : string
    ConfirmPassword : string
}
type Model = {
    FormData : FormData
    IsSaving : bool
}


type Msg =
    | SaveUser of AsyncOperationStatus<Result<unit,ApplicationError>>
    | FormChanged of (FormData -> FormData)
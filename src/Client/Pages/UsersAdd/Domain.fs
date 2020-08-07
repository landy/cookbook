module Cookbook.Client.Pages.UsersAdd.Domain

open Aether

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

module Optics =
    let formData : Lens<Model, FormData> =
        (fun m -> m.FormData),
        (fun fd m -> {m with FormData = fd})


    let username : Lens<FormData, string> =
        (fun fd -> fd.Username),
        (fun u fd -> {fd with Username = u})

    let name : Lens<FormData, string> =
        (fun fd -> fd.Name),
        (fun u fd -> {fd with Name = u})

    let password : Lens<FormData, string> =
        (fun fd -> fd.Password),
        (fun u fd -> {fd with Password = u})

    let confirmPassword : Lens<FormData, string> =
        (fun fd -> fd.ConfirmPassword),
        (fun u fd -> {fd with ConfirmPassword = u})


type Msg =
    | SaveUser of AsyncOperationStatus<Result<unit,ApplicationError>>
    | FormChanged of (FormData -> FormData)
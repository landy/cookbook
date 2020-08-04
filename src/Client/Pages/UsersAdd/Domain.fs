module Cookbook.Client.Pages.UsersAdd.Domain

type Password = {
    Password : string
    ConfirmPassword : string
}

type FormData = {
    Username : string
    Name : string
    Password : Password option
}
type Model = {
    FormData : FormData
    IsSaving : bool
}

type Msg =
    | SaveUser of FormData
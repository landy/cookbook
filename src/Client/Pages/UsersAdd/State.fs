module Cookbook.Client.Pages.UsersAdd.State

open Elmish

open Domain


let init =
    {
        FormData = {
            Username = ""
            Name = ""
            Password =  None
        }
        IsSaving = false
    }, Cmd.none

let update (msg:Msg) (state:Model) =
    state, Cmd.none
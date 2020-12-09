module Cookbook.Client.Auth.AuthStorage

open System
open Cookbook.Shared.Users.Response
open Fable.Import
open FsToolkit.ErrorHandling
open Thoth.Json

[<Literal>]
let currentUserKey = "cookbook.currentUser"

let tryGetSession () =
    Browser.WebStorage.localStorage.getItem currentUserKey
    |> Result.requireNotNull "None"
    |> Result.bind Decode.Auto.fromString<UserSession>
    |> Result.fold Some (fun _ -> None)


let save (user: UserSession) =

    Browser.WebStorage.localStorage.setItem (currentUserKey,(Encode.Auto.toString(0, user)))

let delete () =
    Browser.WebStorage.localStorage.removeItem currentUserKey
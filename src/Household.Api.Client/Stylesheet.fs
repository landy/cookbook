[<RequireQualifiedAccess>]
module Cookbook.Client.Stylesheet

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop

type IStylesheet =
    [<Emit "$0[$1]">]
    abstract Item : className:string -> string

/// Loads a CSS module and makes the classes within available
let inline load (path: string) =
    importSideEffects path
    importDefault<IStylesheet> path
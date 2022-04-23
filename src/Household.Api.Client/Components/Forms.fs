module Cookbook.Client.Components.Forms

open Feliz
open Feliz.DaisyUI

open Aether
open Cookbook.Client.Server
open Cookbook.Shared.Validation


[<ReactComponent>]
let ValidatedTextInput (form:RemoteData<'a,_,ValidationError>) (onDataChanged:'a -> unit) (n:NamedLens<'a,string>) props =
    let value = form.Data |> Optic.get n.Lens
    let err = form.Errors |> ValidationError.get n
    Daisy.formControl [
        Daisy.label [ Daisy.labelText n.Name ]
        Daisy.input [
            input.bordered
            if err.IsSome then input.error
            prop.valueOrDefault value
            prop.onTextChange (fun t -> form.Data |> Optic.set n.Lens t |> onDataChanged)
            prop.placeholder n.Name
            yield! props
        ]
        match err with
        | Some e -> Daisy.label [ Daisy.labelTextAlt [ prop.text (ValidationErrorType.explain e); color.textError ] ]
        | None -> Html.none
    ]

[<ReactComponent>]
let ValidatedTextArea (form:RemoteData<'a,_,ValidationError>) (onDataChanged:'a -> unit) (n:NamedLens<'a,string>) =
    let value = form.Data |> Optic.get n.Lens
    let err = form.Errors |> ValidationError.get n
    Daisy.formControl [
        Daisy.label [ Daisy.labelText n.Name ]
        Daisy.textarea [
            input.bordered
            if err.IsSome then input.error
            prop.valueOrDefault value
            prop.onTextChange (fun t -> form.Data |> Optic.set n.Lens t |> onDataChanged)
            prop.placeholder n.Name
            prop.rows 4
        ]
        match err with
        | Some e -> Daisy.label [ Daisy.labelTextAlt [ prop.text (ValidationErrorType.explain e); color.textError ] ]
        | None -> Html.none
    ]
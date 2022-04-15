module Cookbook.Client.Components.Forms

open Feliz
open Feliz.Bulma

open Aether
open Cookbook.Client.Server
open Cookbook.Shared.Validation


[<ReactComponent>]
let ValidatedTextInput (form:RemoteData<'a,_,ValidationError>) (onDataChanged:'a -> unit) (n:NamedLens<'a,string>) props =
    let value = form.Data |> Optic.get n.Lens
    let err = form.Errors |> ValidationError.get n
    Bulma.field.div [
        field.isHorizontal
        prop.children [
            Bulma.fieldLabel [
                Bulma.label n.Name
            ]
            Bulma.fieldBody [
                Bulma.field.div [
                    Bulma.control.div [
                        Bulma.input.text [
                            prop.placeholder n.Name
                            prop.onTextChange (fun t -> form.Data |> Optic.set n.Lens t |> onDataChanged)
                            prop.valueOrDefault value
                        ]
                    ]
                ]
            ]
        ]
    ]
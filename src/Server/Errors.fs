module Cookbook.Server.Errors

open System.Threading.Tasks
open Cookbook.Shared.Validation
open Cookbook.Shared.Errors

module ServerError =
    let validateAsync (validationFn:'a -> Task<ValidationError list>) (value:'a) =
        task {
            match! value |> validationFn with
            | [] -> return value
            | errs -> return errs |> Validation |> ServerError.failwith
        }
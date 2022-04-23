module Household.Api.Server.Errors

open System.Threading.Tasks
open Household.Api.Shared.Validation
open Household.Api.Shared.Errors

module ServerError =
    let validateAsync (validationFn:'a -> Task<ValidationError list>) (value:'a) =
        task {
            match! value |> validationFn with
            | [] -> return value
            | errs -> return errs |> Validation |> ServerError.failwith
        }
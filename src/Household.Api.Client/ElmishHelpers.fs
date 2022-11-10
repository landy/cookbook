module Household.Api.Client.ElmishHelpers

module Cmd =
    open Elmish
    let withoutCmd state =
        state,Cmd.none
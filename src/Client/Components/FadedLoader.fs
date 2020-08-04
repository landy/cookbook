module Cookbook.Client.Components.FadedLoader


open System
open Feliz
open Feliz.MaterialUI

let FadedLoader isLoading delay childProps =
    Mui.fade [
        fade.in' isLoading
        prop.style[
            if isLoading then
                style.transitionDelay (TimeSpan.FromMilliseconds delay)
            else style.transitionDelay (TimeSpan.FromMilliseconds 0.)
        ]

        prop.children
            (Html.div [
                yield! childProps
                prop.children [
                    Mui.circularProgress []
                ]
            ])

    ]
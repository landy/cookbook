
open Farmer.CoreTypes

#r "paket: groupref build //"
#load "./.fake/build.fsx/intellisense.fsx"

open Farmer
open Farmer.Builders

let makeNameEnvSpecific (env:string) name =
    name + "-" + env

let deployment env : Deployment =

    let envSpecific = makeNameEnvSpecific env

    // appinsights
    let insights = appInsights {
        name ("cookbook-ai" |> envSpecific)
    }

    let webApp = webApp {
        name ("cookbook-web" |> envSpecific)
        link_to_app_insights insights.Name
        setting "public_path" "./public"
    }

    arm {
        location Location.WestEurope
        add_resource insights
        add_resource webApp
        output "WebAppName" webApp.Name
        output "WebAppPassword" webApp.PublishingPassword
    }


let toTemplate deployment =
    deployment.Template
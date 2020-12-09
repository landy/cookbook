
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

    let db = cosmosDb {
        name ("cookbook-db" |> envSpecific)
        account_name ("cookbook-db-account" |> envSpecific)
    }

    let webApp = webApp {
        name ("cookbook-web" |> envSpecific)
        link_to_app_insights insights.Name
        docker_image "landys/cookbook:latest" ""
        sku WebApp.Sku.B1
        setting "public_path" "./public"
        setting "cosmosDbConnection" db.Endpoint
        setting "cosmosDbKey" db.PrimaryKey
        setting "cosmosDb:databaseName" db.DbName
        setting "cosmosDb:containers:users" "Users"
        setting "cosmosDb:containers:refreshTokens" "RefreshTokens"
    }

    arm {
        location Location.WestEurope
        add_resource insights
        add_resource webApp
        add_resource db
    }


let toTemplate deployment =
    deployment.Template
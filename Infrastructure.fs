module Infrastructure


open Farmer
open Farmer.Builders
open Farmer.ContainerApp

let makeNameEnvSpecific (env:string) name =
    name + "-" + env

let deployment env =

    let envSpecific = makeNameEnvSpecific env

    let logAnalytics = logAnalytics {
        name ("cookbook-log-analytics" |> envSpecific)
        retention_period 30<Days>
        enable_ingestion
        enable_query
    }

    let insights = appInsights {
        name ("cookbook-ai" |> envSpecific)
        log_analytics_workspace logAnalytics
    }

    let db = cosmosDb {
        name ("cookbook-db" |> envSpecific)
        account_name ("cookbook-db-account" |> envSpecific)
        free_tier
    }

    let webContainer = container {
        name ("cookbook-web" |> envSpecific)
        public_docker_image "landys/cookbook" "latest"
    }

    let containerEnv = containerEnvironment {
        name ("cookbook-env" |> envSpecific)
        log_analytics_instance logAnalytics
        add_containers [
            containerApp {
                name ("cookbook-web" |> envSpecific)
                add_containers [
                    webContainer
                ]
                add_env_variable "cosmosDb__Connection" db.Endpoint.Value
                add_env_variable "cosmosDb__Key" db.PrimaryKey.Value
                add_env_variable "cosmosDb__databaseName" db.DbName.Value
                add_env_variable "cosmosDb__containers__users" "Users"
                add_env_variable "cosmosDb__containers__refreshTokens" "RefreshTokens"
                add_env_variable "ApplicationInsights__InstrumentationKey" insights.InstrumentationKey.Value
//                add_env_variable "SERVER_PORT" "8085"
                ingress_target_port 80us
                ingress_transport Auto
                dapr_app_id ("cookbook-web" |> envSpecific)
            }
        ]
    }


//    let webApp = webApp {
//        name ("cookbook-web" |> envSpecific)
//        link_to_app_insights insights.Name
//        docker_image "landys/cookbook:latest" ""
//        docker_ci
//        sku WebApp.Sku.B1
//
//        setting "public_path" "./public"
//        setting "cosmosDbConnection" db.Endpoint
//        setting "cosmosDbKey" db.PrimaryKey
//        setting "cosmosDb__databaseName" db.DbName
//        setting "cosmosDb__containers__users" "Users"
//        setting "cosmosDb__containers__refreshTokens" "RefreshTokens"
//        setting "SERVER_PORT" "8085"
//        setting "ApplicationInsights__InstrumentationKey" insights.InstrumentationKey
//    }

    arm {
        location Location.WestEurope
        add_resources [
            logAnalytics
            insights
            db
            containerEnv
        ]
    }


let toTemplate deployment =
    deployment.Template
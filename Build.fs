open Fake.Core
open Fake.IO
open Farmer
open Farmer.Builders

open Helpers

initializeContext()

let sharedPath = Path.getFullName "src/Household.Api.Shared"
let serverApiPath = Path.getFullName "src/Household.Api.Server"
let recipesPath = Path.getFullName "src/Household.Recipes"
let clientPath = Path.getFullName "src/Household.Api.Client"
let deployApiPath = Path.getFullName "deploy"
let deployRecipesPath = Path.getFullName "deployRecipes"
let sharedTestsPath = Path.getFullName "tests/Shared"
let serverTestsPath = Path.getFullName "tests/Server"
let clientTestsPath = Path.getFullName "tests/Client"

Target.create "CleanApi" (fun _ ->
    Shell.cleanDir deployApiPath
    run Tools.dotnet "fable clean --yes" clientPath // Delete *.fs.js files created by Fable
)

Target.create "CleanRecipes" (fun _ ->
    Shell.cleanDir deployRecipesPath
)

Target.create "InstallClient" (fun _ -> run Tools.npm "install" ".")

Target.create "BundleApi" (fun _ ->
    [ "server", Tools.dotnet $"publish -c Release -o \"{deployApiPath}\"" serverApiPath
      "client", Tools.npm "run build" __SOURCE_DIRECTORY__ ]
    |> runParallel
)

Target.create "BundleRecipesApi" (fun _ ->
    [ "server", Tools.dotnet $"publish -c Release -o \"{deployRecipesPath}\"" recipesPath ]
    |> runParallel
)

Target.create "Azure" (fun _ ->
    let environment = Environment.environVarOrDefault "environment" "dev"
    let resourceGroupName = "cookbook-" + environment

    Infrastructure.deployment environment
    |> Deploy.execute resourceGroupName Deploy.NoParameters
    |> ignore
)

Target.create "farmer" (fun _ ->

    Infrastructure.deployment "dev"
    |> Writer.quickWrite "my-template"
)

Target.create "Run" (fun _ ->
    run Tools.dotnet "build" sharedPath
    [ "server", Tools.dotnet "watch run" serverApiPath
      "client", Tools.npm "start" __SOURCE_DIRECTORY__ ]
    |> runParallel
)

Target.create "RunTests" (fun _ ->
    run Tools.dotnet "build" sharedTestsPath
    [ "server", Tools.dotnet "watch run" serverTestsPath
      "client", Tools.dotnet "fable watch --run webpack-dev-server --config ../../webpack.tests.config.js" clientTestsPath ]
    |> runParallel
)

Target.create "Format" (fun _ ->
    run Tools.dotnet "fantomas . -r" "src"
)

open Fake.Core.TargetOperators

let dependencies = [
    "CleanApi"
        ==> "InstallClient"
        ==> "BundleApi"

    "CleanRecipes"
        ==> "BundleRecipesApi"

    "Azure"

    "CleanApi"
        ==> "InstallClient"
        ==> "Run"

    "InstallClient"
        ==> "RunTests"
]

[<EntryPoint>]
let main args = runOrDefault args
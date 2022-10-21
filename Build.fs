open Fake.Core
open Fake.IO

open Helpers

initializeContext()

let sharedPath = Path.getFullName "src/Household.Api.Shared"
let serverPath = Path.getFullName "src/Household.Api.Server"
let clientPath = Path.getFullName "src/Household.Api.Client"
let deployApiPath = Path.getFullName "deploy-api"
let deployFrontendPath = Path.getFullName "deploy-fe"
let sharedTestsPath = Path.getFullName "tests/Shared"
let serverTestsPath = Path.getFullName "tests/Server"
let clientTestsPath = Path.getFullName "tests/Client"

Target.create "Clean" (fun _ ->
    Shell.cleanDir deployApiPath
    Shell.cleanDir deployFrontendPath
    run Tools.dotnet "fable clean --yes" clientPath // Delete *.fs.js files created by Fable
)

Target.create "InstallClient" (fun _ -> run Tools.npm "install" ".")

Target.create "BundleApi" (fun _ ->
    [ "server", Tools.dotnet $"publish -c Release -o \"{deployApiPath}\"" serverPath ]
    |> runParallel
)

Target.create "BundleFrontend" (fun _ ->
    [ "client", Tools.npm "run build" __SOURCE_DIRECTORY__ ]
    |> runParallel
)

Target.create "Run" (fun _ ->
    run Tools.dotnet "build" sharedPath
    [ "server", Tools.dotnet "watch run" serverPath
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
    "Clean"
        ==> "BundleApi"

    "Clean"
        ==> "InstallClient"
        ==> "BundleFrontend"

    "Azure"

    "Clean"
        ==> "InstallClient"
        ==> "Run"

    "InstallClient"
        ==> "RunTests"
]

[<EntryPoint>]
let main args = runOrDefault args
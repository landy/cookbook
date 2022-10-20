open Fake.Core
open Fake.IO

open Helpers

initializeContext()

let sharedPath = Path.getFullName "src/Household.Api.Shared"
let serverPath = Path.getFullName "src/Household.Api.Server"
let clientPath = Path.getFullName "src/Household.Api.Client"
let deployPath = Path.getFullName "deploy"
let sharedTestsPath = Path.getFullName "tests/Shared"
let serverTestsPath = Path.getFullName "tests/Server"
let clientTestsPath = Path.getFullName "tests/Client"

Target.create "Clean" (fun _ ->
    Shell.cleanDir deployPath
    run Tools.dotnet "fable clean --yes" clientPath // Delete *.fs.js files created by Fable
)

Target.create "InstallClient" (fun _ -> run Tools.npm "install" ".")

Target.create "Bundle" (fun _ ->
    [ "server", Tools.dotnet $"publish -c Release -o \"{deployPath}\"" serverPath
      "client", Tools.npm "run build" __SOURCE_DIRECTORY__ ]
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
        ==> "InstallClient"
        ==> "Bundle"

    "Azure"

    "Clean"
        ==> "InstallClient"
        ==> "Run"

    "InstallClient"
        ==> "RunTests"
]

[<EntryPoint>]
let main args = runOrDefault args
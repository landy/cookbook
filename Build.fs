open Fake.Core
open Fake.IO
open Farmer
open Farmer.Builders

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
    run dotnet "fable clean --yes" clientPath // Delete *.fs.js files created by Fable
)

Target.create "InstallClient" (fun _ -> run npm "install" ".")

Target.create "BundleApi" (fun _ ->
    [ "server", dotnet $"publish -c Release -o \"{deployPath}\"" serverPath
      "client", npm "run build" __SOURCE_DIRECTORY__ ]
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
    run dotnet "build" sharedPath
    [ "server", dotnet "watch run" serverPath
      "client", npm "start" __SOURCE_DIRECTORY__ ]
    |> runParallel
)

Target.create "RunTests" (fun _ ->
    run dotnet "build" sharedTestsPath
    [ "server", dotnet "watch run" serverTestsPath
      "client", dotnet "fable watch --run webpack-dev-server --config ../../webpack.tests.config.js" clientTestsPath ]
    |> runParallel
)

Target.create "Format" (fun _ ->
    run dotnet "fantomas . -r" "src"
)

open Fake.Core.TargetOperators

let dependencies = [
    "Clean"
        ==> "InstallClient"
        ==> "BundleApi"

    "Azure"

    "Clean"
        ==> "InstallClient"
        ==> "Run"

    "InstallClient"
        ==> "RunTests"
]

[<EntryPoint>]
let main args = runOrDefault args
#r "paket: groupref build //"
#load "./.fake/build.fsx/intellisense.fsx"

#if !FAKE
#r "netstandard"
#r "Facades/netstandard" // https://github.com/ionide/ionide-vscode-fsharp/issues/839#issuecomment-396296095
#endif

open System

open Fake.Core
open Fake.DotNet
open Fake.IO
#load @"paket-files/build/CompositionalIT/fshelpers/src/FsHelpers/ArmHelper/ArmHelper.fs"
#load "Infrastructure.fsx"
open Cit.Helpers.Arm
open Cit.Helpers.Arm.Parameters
open Microsoft.Azure.Management.ResourceManager.Fluent.Core


Target.initEnvironment ()

let serverPath = Path.getFullName "./src/Server"
let graphQLGeneratorPath = Path.getFullName "./src/GraphQLSchemaGenerator"
let clientPath = Path.getFullName "./src/Client"
let clientDeployPath = Path.getFullName "./dist"
let deployDir = Path.getFullName "./deploy"
let gqlClientPath = Path.getFullName "./gql-output"
let schemaFile = Path.getFullName "./cookbookSchema.json"

let release = ReleaseNotes.load "RELEASE_NOTES.md"

let platformTool tool winTool =
    let tool = if Environment.isUnix then tool else winTool
    match ProcessUtils.tryFindFileOnPath tool with
    | Some t -> t
    | _ ->
        let errorMsg =
            tool + " was not found in path. " +
            "Please install it and make sure it's available from your path. " +
            "See https://safe-stack.github.io/docs/quickstart/#install-pre-requisites for more info"
        failwith errorMsg

let nodeTool = platformTool "node" "node.exe"
let npmTool = platformTool "npm" "npm.cmd"

let runTool cmd args workingDir =
    let arguments = args |> String.split ' ' |> Arguments.OfArgs
    Command.RawCommand (cmd, arguments)
    |> CreateProcess.fromCommand
    |> CreateProcess.withWorkingDirectory workingDir
    |> CreateProcess.ensureExitCode
    |> Proc.run
    |> ignore

let runDotNet cmd workingDir =
    let result =
        DotNet.exec (DotNet.Options.withWorkingDirectory workingDir) cmd ""
    if result.ExitCode <> 0 then failwithf "'dotnet %s' failed in %s" cmd workingDir

let runDotNetWithArgs cmd workingDir args =
    let result =
        DotNet.exec (DotNet.Options.withWorkingDirectory workingDir) cmd args
    if result.ExitCode <> 0 then failwithf "'dotnet %s' failed in %s with args %s" cmd workingDir args

let openBrowser url =
    //https://github.com/dotnet/corefx/issues/10361
    Command.ShellCommand url
    |> CreateProcess.fromCommand
    |> CreateProcess.ensureExitCodeWithMessage "opening browser failed"
    |> Proc.run
    |> ignore


Target.create "Clean" (fun _ ->
    [ deployDir
      clientDeployPath
      gqlClientPath ]
    |> Shell.deleteDirs

    File.delete schemaFile
)

Target.create "InstallClient" (fun _ ->
    printfn "Node version:"
    runTool nodeTool "--version" __SOURCE_DIRECTORY__
    printfn "Yarn version:"
    runTool npmTool "--version" __SOURCE_DIRECTORY__
    runTool npmTool "install --frozen-lockfile" __SOURCE_DIRECTORY__
)

Target.create "Build" (fun _ ->
    runDotNet "build" serverPath
    Shell.regexReplaceInFileWithEncoding
        "let app = \".+\""
       ("let app = \"" + release.NugetVersion + "\"")
        System.Text.Encoding.UTF8
        (Path.combine clientPath "Version.fs")
    runTool npmTool "run build" __SOURCE_DIRECTORY__
)

Target.create "Run" (fun _ ->
    let server = async {
        runDotNet "watch run" serverPath
    }
    let client = async {
        runTool npmTool "start" __SOURCE_DIRECTORY__
    }
    let browser = async {
        do! Async.Sleep 5000
        openBrowser "http://localhost:8080"
    }

    let vsCodeSession = Environment.hasEnvironVar "vsCodeSession"
    let safeClientOnly = Environment.hasEnvironVar "safeClientOnly"

    let tasks =
        [ if not safeClientOnly then yield server
          yield client
          if not vsCodeSession then yield browser ]

    tasks
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore
)

Target.create "RunClient" (fun _ ->

    let client = async {
        runTool npmTool "webpack-dev-server" __SOURCE_DIRECTORY__
    }
    let browser = async {
        do! Async.Sleep 5000
        openBrowser "http://localhost:8080"
    }

    let tasks =
        [ client; browser]

    tasks
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore
)


Target.create "Bundle" (fun _ ->
    let serverDir = deployDir
    let publicDir = Path.combine deployDir "public"

    let publishArgs = sprintf "publish -c Release -o \"%s\"" serverDir
    runDotNet publishArgs serverPath

    Shell.copyDir publicDir clientDeployPath FileFilter.allFiles
)

type ArmOutput =
    { WebAppName : ParameterValue<string>
      WebAppPassword : ParameterValue<string> }
let mutable deploymentOutputs : ArmOutput option = None

Target.create "ArmTemplate" (fun _ ->
    let environment = Environment.environVarOrDefault "environment" (Guid.NewGuid().ToString().ToLower().Split '-' |> Array.head)
    let resourceGroupName = "cookbook-" + environment

    let authCtx =
        // You can safely replace these with your own subscription and client IDs hard-coded into this script.
        let subscriptionId = try Environment.environVar "subscriptionId" |> Guid.Parse with _ -> failwith "Invalid Subscription ID. This should be your Azure Subscription ID."
        let clientId = try Environment.environVar "clientId" |> Guid.Parse with _ -> failwith "Invalid Client ID. This should be the Client ID of an application registered in Azure with permission to create resources in your subscription."
        let clientSecret = try Environment.environVar "clientSecret" with _ -> failwith "Invalid Client Secret. This should be the Client Secret of an application registered in Azure with permission to create resources in your subscription."
        let tenantId =
            try Environment.environVar "tenantId" |> Guid.Parse
            with _ -> failwith "Invalid TenantId ID. This should be the Tenant ID of an application registered in Azure with permission to create resources in your subscription."

        let credentials = { ClientId = clientId; ClientSecret = clientSecret; TenantId = tenantId }
        Trace.tracefn "Deploying template to resource group '%s' in subscription '%O'..." resourceGroupName subscriptionId
        subscriptionId
        |> authenticate credentials

    let armTemplate =
        environment
        |> Infrastructure.deployment
        |> Infrastructure.toTemplate
        |> Farmer.Writer.toJson


    let deployment =
        let location = Environment.environVarOrDefault "location" Region.EuropeWest.Name
        { DeploymentName = "Cookbook deployment"
          ResourceGroup = New(resourceGroupName, Region.Create location)
          ArmTemplate = armTemplate
          Parameters = (Simple List.empty)
          DeploymentMode = Incremental }

    deployment
    |> deployWithProgress authCtx
    |> Seq.iter(function
        | DeploymentInProgress (state, operations) -> Trace.tracefn "State is %s, completed %d operations." state operations
        | DeploymentError (statusCode, message) -> Trace.traceError <| sprintf "DEPLOYMENT ERROR: %s - '%s'" statusCode message
        | DeploymentCompleted d -> deploymentOutputs <- d)
)

Target.create "GenerateArm" (fun _ ->
    "dev"
    |> Infrastructure.deployment
    |> Infrastructure.toTemplate
    |> Farmer.Writer.toJson
    |> File.replaceContent "arm-template.json"
)

open Fake.IO.Globbing.Operators
open System.Net

// https://github.com/SAFE-Stack/SAFE-template/issues/120
// https://stackoverflow.com/a/6994391/3232646
type TimeoutWebClient() =
    inherit WebClient()
    override this.GetWebRequest uri =
        let request = base.GetWebRequest uri
        request.Timeout <- 30 * 60 * 1000
        request

Target.create "AppService" (fun _ ->
    let zipFile = "deploy.zip"
    IO.File.Delete zipFile
    Zip.zip deployDir zipFile !!(deployDir + @"\**\**")

    let appName = deploymentOutputs.Value.WebAppName.value
    let appPassword = deploymentOutputs.Value.WebAppPassword.value

    let destinationUri = sprintf "https://%s.scm.azurewebsites.net/api/zipdeploy" appName
    let client = new TimeoutWebClient(Credentials = NetworkCredential("$" + appName, appPassword))
    Trace.tracefn "Uploading %s to %s" zipFile destinationUri
    client.UploadData(destinationUri, IO.File.ReadAllBytes zipFile) |> ignore)



open Fake.Core.TargetOperators

"Clean"
    ==> "InstallClient"
    ==> "Build"
    ==> "Bundle"
//    ==> "ArmTemplate"
//    ==> "AppService"


"Clean"
    ==> "InstallClient"
    ==> "Run"

"Clean"
    ==> "InstallClient"
    ==> "RunClient"

Target.runOrDefaultWithArguments "Build"

namespace CRQCIndex.CLI

open System
open System.IO
open System.Text.Json

module Config =

    type CloudflareConfig = {
        ApiToken: string
        AccountId: string
    }

    type ResourceNames = {
        PagesProjectName: string
    }

    let defaultResourceNames = {
        PagesProjectName = "crqcindex"
    }

    /// Deployment scope determined by git diff analysis
    type DeploymentScope =
        | PagesOnly           // Only site changes - deploy to Pages
        | FullDeploy          // Worker/CLI changes - full deployment
        | NoDeploy            // No relevant changes detected

    type DeploymentState = {
        PagesDeployed: bool
        PagesProjectUrl: string option
        LastDeployedCommit: string option
        LastDeployTimestamp: DateTime option
    }

    let loadConfig () : Result<CloudflareConfig, string> =
        let apiToken = Environment.GetEnvironmentVariable("CLOUDFLARE_API_TOKEN")
        let accountId = Environment.GetEnvironmentVariable("CLOUDFLARE_ACCOUNT_ID")

        match apiToken, accountId with
        | null, _ -> Error "CLOUDFLARE_API_TOKEN environment variable not set"
        | _, null -> Error "CLOUDFLARE_ACCOUNT_ID environment variable not set"
        | token, account ->
            Ok {
                ApiToken = token
                AccountId = account
            }

    let stateFilePath = ".crqcindex-deploy-state.json"

    let private jsonOptions = JsonSerializerOptions(WriteIndented = true)

    let loadState () : DeploymentState option =
        if File.Exists(stateFilePath) then
            try
                let json = File.ReadAllText(stateFilePath)
                Some (JsonSerializer.Deserialize<DeploymentState>(json, jsonOptions))
            with _ ->
                None
        else
            None

    let saveState (state: DeploymentState) : unit =
        let json = JsonSerializer.Serialize(state, jsonOptions)
        File.WriteAllText(stateFilePath, json)

    let defaultState = {
        PagesDeployed = false
        PagesProjectUrl = None
        LastDeployedCommit = None
        LastDeployTimestamp = None
    }

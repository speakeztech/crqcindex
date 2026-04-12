namespace CRQCIndex.CLI.Commands

open System
open CRQCIndex.CLI
open CRQCIndex.CLI.Core

module DeployPages =

    let execute (config: Config.CloudflareConfig) (siteDir: string) (projectName: string) (verbose: bool) : Async<Result<unit, string>> =
        async {
            printfn "Deploying CRQC Index site to Cloudflare Pages..."

            let distDir = System.IO.Path.Combine(siteDir, "dist")
            use httpClient = HttpHelpers.createHttpClient config.ApiToken

            let! result = PagesClient.deployPages httpClient config.AccountId projectName distDir verbose

            match result with
            | Ok () ->
                let state = Config.loadState () |> Option.defaultValue Config.defaultState
                let updated =
                    { state with
                        PagesDeployed = true
                        PagesProjectUrl = Some $"https://{projectName}.pages.dev"
                        LastDeployTimestamp = Some DateTime.UtcNow }
                Config.saveState updated
                printfn "Deployment complete: https://%s.pages.dev" projectName
                return Ok ()
            | Error e ->
                return Error e
        }

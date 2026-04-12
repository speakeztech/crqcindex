namespace CRQCIndex.CLI.Commands

open System
open System.Diagnostics
open System.IO
open System.Net.Http
open System.Net.Http.Headers
open CRQCIndex.CLI
open CRQCIndex.CLI.Core

module DeployPages =

    let private getGitHead () : string option =
        try
            let psi = ProcessStartInfo("git", "rev-parse HEAD")
            psi.RedirectStandardOutput <- true
            psi.UseShellExecute <- false
            psi.CreateNoWindow <- true
            use proc = Process.Start(psi)
            let output = proc.StandardOutput.ReadToEnd().Trim()
            proc.WaitForExit()
            if proc.ExitCode = 0 && output.Length > 0 then Some output else None
        with _ -> None

    let private runBuild (siteDir: string) (verbose: bool) : Result<unit, string> =
        try
            printfn "        Running npm install..."
            let psiInstall = ProcessStartInfo("npm", "install")
            psiInstall.WorkingDirectory <- siteDir
            psiInstall.RedirectStandardOutput <- true
            psiInstall.RedirectStandardError <- true
            psiInstall.UseShellExecute <- false
            psiInstall.CreateNoWindow <- true
            use procInstall = Process.Start(psiInstall)
            let _installOut = procInstall.StandardOutput.ReadToEnd()
            let installErr = procInstall.StandardError.ReadToEnd()
            procInstall.WaitForExit()
            if procInstall.ExitCode <> 0 then
                Error $"npm install failed: {installErr}"
            else

            printfn "        Running npm run build (Fable + Vite)..."
            let psi = ProcessStartInfo("npm", "run build")
            psi.WorkingDirectory <- siteDir
            psi.RedirectStandardOutput <- true
            psi.RedirectStandardError <- true
            psi.UseShellExecute <- false
            psi.CreateNoWindow <- true
            use proc = Process.Start(psi)
            let stdout = proc.StandardOutput.ReadToEnd()
            let stderr = proc.StandardError.ReadToEnd()
            proc.WaitForExit()
            if verbose && stdout.Trim().Length > 0 then
                printfn "        %s" (stdout.Trim())
            if proc.ExitCode <> 0 then
                let errMsg = if stderr.Trim().Length > 0 then stderr.Trim() else stdout.Trim()
                Error $"Build failed: {errMsg}"
            else
                Ok ()
        with
        | ex -> Error $"Build process failed: {ex.Message}"

    let execute (config: Config.CloudflareConfig) (siteDir: string) (projectName: string) (force: bool) (skipBuild: bool) (verbose: bool) : Async<Result<unit, string>> =
        async {
            printfn "CRQC Index Pages Deployment"
            printfn ""

            let siteDir = Path.GetFullPath(siteDir)
            let distDir = Path.Combine(siteDir, "dist")

            // Check if anything changed since last deploy
            let currentCommit = getGitHead()
            let state = Config.loadState () |> Option.defaultValue Config.defaultState

            let needsDeploy =
                force ||
                not state.PagesDeployed ||
                (match currentCommit, state.LastDeployedCommit with
                 | Some current, Some last -> current <> last
                 | Some _, None -> true
                 | None, _ -> true)

            if not needsDeploy then
                printfn "  No changes since last deploy (commit: %s)"
                    (state.LastDeployedCommit |> Option.defaultValue "unknown")
                printfn "  Use --force to deploy anyway."
                return Ok ()
            else

            // Step 1: Build
            let buildResult =
                if not skipBuild then
                    printfn "  [1/3] Building site..."
                    if Directory.Exists(distDir) then
                        if verbose then printfn "        Cleaning dist directory..."
                        Directory.Delete(distDir, true)
                    match runBuild siteDir verbose with
                    | Error e -> Error e
                    | Ok () ->
                        if not (Directory.Exists(distDir)) then
                            Error "Build did not create dist directory"
                        else
                            let fileCount = Directory.GetFiles(distDir, "*", SearchOption.AllDirectories).Length
                            printfn "        Built %d files" fileCount
                            Ok ()
                else
                    printfn "  [1/3] Skipping build (--skip-build)"
                    if not (Directory.Exists(distDir)) then
                        Error $"dist directory not found: {distDir}. Run without --skip-build first."
                    else
                        Ok ()

            match buildResult with
            | Error e -> return Error e
            | Ok () ->

            // Step 2: Ensure project exists
            use httpClient = new HttpClient()
            httpClient.DefaultRequestHeaders.Authorization <- AuthenticationHeaderValue("Bearer", config.ApiToken)
            let pages = PagesUploader.PagesOperations(httpClient, config.AccountId)

            printfn "  [2/3] Checking project..."
            let! exists = pages.ProjectExists(projectName)
            let! ensureProject =
                if not exists then
                    async {
                        printfn "        Creating project '%s'..." projectName
                        return! pages.CreateProject(projectName, "main")
                    }
                else
                    async {
                        if verbose then printfn "        Project exists"
                        return Ok ()
                    }
            match ensureProject with
            | Error e -> return Error $"Failed to create project: {e}"
            | Ok () ->

            // Step 3: Deploy
            printfn "  [3/3] Deploying..."
            let progressCallback msg =
                printfn "        %s" msg

            let! result = pages.DeployDirectory projectName distDir verbose progressCallback

            match result with
            | Error e -> return Error e
            | Ok url ->

            let updated =
                { Config.defaultState with
                    PagesDeployed = true
                    PagesProjectUrl = Some $"https://{projectName}.pages.dev"
                    LastDeployedCommit = currentCommit
                    LastDeployTimestamp = Some DateTime.UtcNow }
            Config.saveState updated

            printfn ""
            printfn "  Deployment complete!"
            printfn "  Site URL: https://%s.pages.dev" projectName
            if url <> "Deployment created successfully" then
                printfn "  Preview: %s" url
            return Ok ()
        }

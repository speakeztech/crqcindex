namespace CRQCIndex.CLI.Commands

open CRQCIndex.CLI

module Migrate =

    let execute (config: Config.CloudflareConfig) (verbose: bool) : Async<Result<unit, string>> =
        async {
            printfn "CRQC Index Migration"
            printfn "===================="
            printfn ""

            // Step 1: Provision
            printfn "[1/2] Provisioning resources..."
            let! provisionResult = Provision.execute config verbose
            match provisionResult with
            | Error e -> return Error $"Provisioning failed: {e}"
            | Ok () ->

            // Step 2: Deploy Pages
            printfn "[2/2] Deploying site..."
            let! deployResult = DeployPages.execute config "./site" Config.defaultResourceNames.PagesProjectName verbose
            match deployResult with
            | Error e -> return Error $"Pages deployment failed: {e}"
            | Ok () ->

            printfn ""
            printfn "Migration complete."
            return Ok ()
        }

    let private runStep (name: string) (step: Async<Result<unit, string>>) : Async<Result<unit, string>> =
        async {
            printfn "[*] %s..." name
            let! result = step
            match result with
            | Error e -> return Error $"{name} failed: {e}"
            | Ok () -> return Ok ()
        }

    let executeSelective
        (config: Config.CloudflareConfig)
        (skipProvision: bool)
        (skipDeploy: bool)
        (verbose: bool) : Async<Result<unit, string>> =
        async {
            printfn "CRQC Index Migration (selective)"
            printfn "================================"
            printfn ""

            let mutable error: string option = None

            if not skipProvision && error.IsNone then
                let! r = runStep "Provisioning resources" (Provision.execute config verbose)
                match r with
                | Error e -> error <- Some e
                | Ok () -> ()

            if not skipDeploy && error.IsNone then
                let! r = runStep "Deploying site" (DeployPages.execute config "./site" Config.defaultResourceNames.PagesProjectName verbose)
                match r with
                | Error e -> error <- Some e
                | Ok () -> ()

            match error with
            | Some e ->
                return Error e
            | None ->
                printfn ""
                printfn "Selective migration complete."
                return Ok ()
        }

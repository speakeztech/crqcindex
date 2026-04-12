namespace CRQCIndex.CLI.Commands

open CRQCIndex.CLI

module Provision =

    let execute (config: Config.CloudflareConfig) (verbose: bool) : Async<Result<unit, string>> =
        async {
            printfn "Provisioning CRQC Index resources..."
            printfn "  Account: %s" config.AccountId

            // Phase 1: Pages project only
            // Future phases will add D1, R2, Vectorize, Workers
            let resources = Config.defaultResourceNames

            if verbose then
                printfn "  Pages project: %s" resources.PagesProjectName

            printfn "Provisioning complete."
            return Ok ()
        }

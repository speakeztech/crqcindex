namespace CRQCIndex.CLI.Commands

open System.Text.Json
open CRQCIndex.CLI

module Status =

    let executeText (_config: Config.CloudflareConfig) : Async<Result<unit, string>> =
        async {
            let state = Config.loadState () |> Option.defaultValue Config.defaultState

            printfn "CRQC Index Deployment Status"
            printfn "============================"
            printfn ""
            printfn "  Pages deployed: %b" state.PagesDeployed
            match state.PagesProjectUrl with
            | Some url -> printfn "  Pages URL: %s" url
            | None -> printfn "  Pages URL: (not deployed)"
            match state.LastDeployedCommit with
            | Some commit -> printfn "  Last commit: %s" commit
            | None -> printfn "  Last commit: (none)"
            match state.LastDeployTimestamp with
            | Some ts -> printfn "  Last deploy: %s" (ts.ToString("yyyy-MM-dd HH:mm:ss UTC"))
            | None -> printfn "  Last deploy: (never)"

            return Ok ()
        }

    let executeJson (_config: Config.CloudflareConfig) : Async<Result<unit, string>> =
        async {
            let state = Config.loadState () |> Option.defaultValue Config.defaultState
            let json = JsonSerializer.Serialize(state, JsonSerializerOptions(WriteIndented = true))
            printfn "%s" json
            return Ok ()
        }

namespace CRQCIndex.CLI

open System
open Argu

module Program =

    [<RequireQualifiedAccess>]
    type ProvisionArgs =
        | [<AltCommandLine("-v")>] Verbose

        interface IArgParserTemplate with
            member this.Usage =
                match this with
                | Verbose -> "Enable verbose output"

    [<RequireQualifiedAccess>]
    type DeployPagesArgs =
        | [<AltCommandLine("-d")>] Site_Dir of path: string
        | [<AltCommandLine("-n")>] Project_Name of name: string
        | [<AltCommandLine("-v")>] Verbose

        interface IArgParserTemplate with
            member this.Usage =
                match this with
                | Site_Dir _ -> "Site directory (default: ./site)"
                | Project_Name _ -> "Pages project name (default: crqcindex)"
                | Verbose -> "Enable verbose output"

    [<RequireQualifiedAccess>]
    type MigrateArgs =
        | [<AltCommandLine("-v")>] Verbose
        | Skip_Provision
        | Skip_Deploy

        interface IArgParserTemplate with
            member this.Usage =
                match this with
                | Verbose -> "Enable verbose output"
                | Skip_Provision -> "Skip resource provisioning"
                | Skip_Deploy -> "Skip site deployment"

    [<RequireQualifiedAccess>]
    type StatusArgs =
        | [<AltCommandLine("-j")>] Json

        interface IArgParserTemplate with
            member this.Usage =
                match this with
                | Json -> "Output status as JSON"

    [<RequireQualifiedAccess>]
    type CLIArgs =
        | [<CliPrefix(CliPrefix.None)>] Provision of ParseResults<ProvisionArgs>
        | [<CliPrefix(CliPrefix.None); CustomCommandLine("deploy-pages")>] DeployPages of ParseResults<DeployPagesArgs>
        | [<CliPrefix(CliPrefix.None)>] Migrate of ParseResults<MigrateArgs>
        | [<CliPrefix(CliPrefix.None)>] Status of ParseResults<StatusArgs>
        | [<AltCommandLine("-V")>] Version

        interface IArgParserTemplate with
            member this.Usage =
                match this with
                | Provision _ -> "Provision Cloudflare resources"
                | DeployPages _ -> "Deploy site to Cloudflare Pages"
                | Migrate _ -> "Full migration: provision -> deploy"
                | Status _ -> "Show deployment status"
                | Version -> "Show version"

    let private runAsync (computation: Async<Result<'a, string>>) : int =
        match Async.RunSynchronously computation with
        | Ok _ -> 0
        | Error e ->
            eprintfn "Error: %s" e
            1

    [<EntryPoint>]
    let main argv =
        let parser = ArgumentParser.Create<CLIArgs>(programName = "crqcindex")

        try
            let results = parser.ParseCommandLine(inputs = argv, raiseOnUsage = true)

            if results.Contains <@ CLIArgs.Version @> then
                printfn "CRQCIndex CLI v0.1.0"
                0
            else
                match results.GetSubCommand() with
                | CLIArgs.Provision args ->
                    match Config.loadConfig() with
                    | Error e ->
                        eprintfn "Error: %s" e
                        1
                    | Ok config ->
                        let verbose = args.Contains <@ ProvisionArgs.Verbose @>
                        Commands.Provision.execute config verbose
                        |> runAsync

                | CLIArgs.DeployPages args ->
                    match Config.loadConfig() with
                    | Error e ->
                        eprintfn "Error: %s" e
                        1
                    | Ok config ->
                        let siteDir = args.GetResult(<@ DeployPagesArgs.Site_Dir @>, "./site")
                        let projectName = args.GetResult(<@ DeployPagesArgs.Project_Name @>, "crqcindex")
                        let verbose = args.Contains <@ DeployPagesArgs.Verbose @>
                        Commands.DeployPages.execute config siteDir projectName verbose
                        |> runAsync

                | CLIArgs.Migrate args ->
                    match Config.loadConfig() with
                    | Error e ->
                        eprintfn "Error: %s" e
                        1
                    | Ok config ->
                        let verbose = args.Contains <@ MigrateArgs.Verbose @>
                        if args.Contains <@ MigrateArgs.Skip_Provision @> ||
                           args.Contains <@ MigrateArgs.Skip_Deploy @> then
                            Commands.Migrate.executeSelective
                                config
                                (args.Contains <@ MigrateArgs.Skip_Provision @>)
                                (args.Contains <@ MigrateArgs.Skip_Deploy @>)
                                verbose
                            |> runAsync
                        else
                            Commands.Migrate.execute config verbose
                            |> runAsync

                | CLIArgs.Status args ->
                    match Config.loadConfig() with
                    | Error e ->
                        eprintfn "Error: %s" e
                        1
                    | Ok config ->
                        let json = args.Contains <@ StatusArgs.Json @>
                        if json then
                            Commands.Status.executeJson config |> runAsync
                        else
                            Commands.Status.executeText config |> runAsync

                | CLIArgs.Version ->
                    printfn "CRQCIndex CLI v0.1.0"
                    0

        with
        | :? ArguParseException as e ->
            printfn "%s" e.Message
            if e.ErrorCode = ErrorCode.HelpText then 0 else 1
        | ex ->
            eprintfn "Unexpected error: %s" ex.Message
            1

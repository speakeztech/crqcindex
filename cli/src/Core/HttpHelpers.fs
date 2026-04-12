namespace CRQCIndex.CLI.Core

open System.Net.Http
open System.Net.Http.Headers

module HttpHelpers =

    let createHttpClient (apiToken: string) =
        let client = new HttpClient()
        client.DefaultRequestHeaders.Authorization <- AuthenticationHeaderValue("Bearer", apiToken)
        client.BaseAddress <- System.Uri("https://api.cloudflare.com/client/v4/")
        client

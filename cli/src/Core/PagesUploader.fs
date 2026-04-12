module CRQCIndex.CLI.Core.PagesUploader

open System
open System.IO
open System.Net.Http
open System.Net.Http.Headers
open System.Text
open System.Text.Json

type FileInfo = {
    RelativePath: string
    FullPath: string
    Hash: string
    Size: int64
    ContentType: string
}

type FileUpload = {
    Hash: string
    Content: byte[]
    ContentType: string
    FilePath: string
}

let computeFileHash (filePath: string) : string =
    let content = File.ReadAllBytes(filePath)
    let base64Content = Convert.ToBase64String(content)
    let extension = Path.GetExtension(filePath).TrimStart('.').ToLowerInvariant()
    let dataToHash = Encoding.UTF8.GetBytes(base64Content + extension)
    let hashBytes = Blake3.Hasher.Hash(dataToHash)
    BitConverter.ToString(hashBytes.AsSpan().Slice(0, 16).ToArray())
        .Replace("-", "").ToLowerInvariant()

let getContentType (filePath: string) =
    match Path.GetExtension(filePath).ToLowerInvariant() with
    | ".html" | ".htm" -> "text/html"
    | ".css" -> "text/css"
    | ".js" -> "application/javascript"
    | ".mjs" -> "application/javascript"
    | ".json" -> "application/json"
    | ".png" -> "image/png"
    | ".jpg" | ".jpeg" -> "image/jpeg"
    | ".gif" -> "image/gif"
    | ".svg" -> "image/svg+xml"
    | ".ico" -> "image/x-icon"
    | ".woff" -> "font/woff"
    | ".woff2" -> "font/woff2"
    | ".ttf" -> "font/ttf"
    | ".eot" -> "application/vnd.ms-fontobject"
    | ".xml" -> "application/xml"
    | ".txt" -> "text/plain"
    | ".map" -> "application/json"
    | ".webp" -> "image/webp"
    | ".pdf" -> "application/pdf"
    | _ -> "application/octet-stream"

let collectFiles (directory: string) : FileInfo list =
    Directory.GetFiles(directory, "*", SearchOption.AllDirectories)
    |> Array.filter (fun f ->
        not (f.Contains(".git")) &&
        not (f.EndsWith(".DS_Store")) &&
        not (f.EndsWith("~")))
    |> Array.map (fun fullPath ->
        let relativePath =
            fullPath.Substring(directory.Length).TrimStart(Path.DirectorySeparatorChar, '/')
            |> fun p -> "/" + p.Replace("\\", "/")
        let hash = computeFileHash fullPath
        let size = System.IO.FileInfo(fullPath).Length
        let contentType = getContentType fullPath
        { RelativePath = relativePath
          FullPath = fullPath
          Hash = hash
          Size = size
          ContentType = contentType })
    |> Array.toList

let buildManifest (files: FileInfo list) : string =
    let entries =
        files
        |> List.map (fun f -> sprintf "\"%s\":\"%s\"" f.RelativePath f.Hash)
        |> String.concat ","
    sprintf "{%s}" entries

type PagesOperations(httpClient: HttpClient, accountId: string) =

    member this.GetUploadToken(projectName: string) : Async<Result<string, string>> =
        async {
            try
                let url = $"https://api.cloudflare.com/client/v4/accounts/{accountId}/pages/projects/{projectName}/upload-token"
                let! response = httpClient.GetAsync(url) |> Async.AwaitTask
                let! responseBody = response.Content.ReadAsStringAsync() |> Async.AwaitTask

                if response.IsSuccessStatusCode then
                    use doc = JsonDocument.Parse(responseBody)
                    let root = doc.RootElement
                    let mutable successProp = Unchecked.defaultof<JsonElement>
                    if root.TryGetProperty("success", &successProp) && successProp.GetBoolean() then
                        let mutable resultProp = Unchecked.defaultof<JsonElement>
                        if root.TryGetProperty("result", &resultProp) then
                            let mutable jwtProp = Unchecked.defaultof<JsonElement>
                            if resultProp.TryGetProperty("jwt", &jwtProp) then
                                return Ok (jwtProp.GetString())
                            else
                                return Error "JWT not found in response"
                        else
                            return Error "Result not found in response"
                    else
                        return Error $"API returned failure: {responseBody}"
                else
                    return Error $"HTTP {int response.StatusCode}: {responseBody}"
            with
            | ex -> return Error $"Failed to get upload token: {ex.Message}"
        }

    member this.CheckMissingAssets(jwt: string, hashes: string list) : Async<Result<string list, string>> =
        async {
            try
                let url = "https://api.cloudflare.com/client/v4/pages/assets/check-missing"
                let payload = sprintf """{"hashes":[%s]}""" (hashes |> List.map (sprintf "\"%s\"") |> String.concat ",")

                use jwtClient = new HttpClient()
                jwtClient.DefaultRequestHeaders.Authorization <- AuthenticationHeaderValue("Bearer", jwt)

                use content = new StringContent(payload, Encoding.UTF8, "application/json")
                let! response = jwtClient.PostAsync(url, content) |> Async.AwaitTask
                let! responseBody = response.Content.ReadAsStringAsync() |> Async.AwaitTask

                if response.IsSuccessStatusCode then
                    use doc = JsonDocument.Parse(responseBody)
                    let root = doc.RootElement
                    let mutable successProp = Unchecked.defaultof<JsonElement>
                    if root.TryGetProperty("success", &successProp) && successProp.GetBoolean() then
                        let mutable resultProp = Unchecked.defaultof<JsonElement>
                        if root.TryGetProperty("result", &resultProp) && resultProp.ValueKind = JsonValueKind.Array then
                            let missing =
                                resultProp.EnumerateArray()
                                |> Seq.map (fun e -> e.GetString())
                                |> Seq.toList
                            return Ok missing
                        else
                            return Ok []
                    else
                        return Error $"API returned failure: {responseBody}"
                else
                    return Error $"HTTP {int response.StatusCode}: {responseBody}"
            with
            | ex -> return Error $"Failed to check missing assets: {ex.Message}"
        }

    member this.UploadAssetBatch(jwt: string, files: FileUpload list) : Async<Result<unit, string>> =
        async {
            try
                let url = "https://api.cloudflare.com/client/v4/pages/assets/upload"

                let fileEntries =
                    files
                    |> List.map (fun f ->
                        let base64Content = Convert.ToBase64String(f.Content)
                        sprintf """{"key":"%s","value":"%s","metadata":{"contentType":"%s"},"base64":true}"""
                            f.Hash base64Content f.ContentType)
                    |> String.concat ","

                let payload = sprintf "[%s]" fileEntries

                use jwtClient = new HttpClient()
                jwtClient.DefaultRequestHeaders.Authorization <- AuthenticationHeaderValue("Bearer", jwt)

                use content = new StringContent(payload, Encoding.UTF8, "application/json")
                let! response = jwtClient.PostAsync(url, content) |> Async.AwaitTask
                let! responseBody = response.Content.ReadAsStringAsync() |> Async.AwaitTask

                if response.IsSuccessStatusCode then
                    return Ok ()
                else
                    return Error $"Upload failed - HTTP {int response.StatusCode}: {responseBody}"
            with
            | ex -> return Error $"Failed to upload assets: {ex.Message}"
        }

    member this.UpsertHashes(jwt: string, hashes: string list) : Async<Result<unit, string>> =
        async {
            try
                let url = "https://api.cloudflare.com/client/v4/pages/assets/upsert-hashes"
                let payload = sprintf """{"hashes":[%s]}""" (hashes |> List.map (sprintf "\"%s\"") |> String.concat ",")

                use jwtClient = new HttpClient()
                jwtClient.DefaultRequestHeaders.Authorization <- AuthenticationHeaderValue("Bearer", jwt)

                use content = new StringContent(payload, Encoding.UTF8, "application/json")
                let! response = jwtClient.PostAsync(url, content) |> Async.AwaitTask
                let! responseBody = response.Content.ReadAsStringAsync() |> Async.AwaitTask

                if response.IsSuccessStatusCode then
                    return Ok ()
                else
                    return Error $"Upsert hashes failed - HTTP {int response.StatusCode}: {responseBody}"
            with
            | ex -> return Error $"Failed to upsert hashes: {ex.Message}"
        }

    member this.ProjectExists(projectName: string) : Async<bool> =
        async {
            try
                let url = $"https://api.cloudflare.com/client/v4/accounts/{accountId}/pages/projects/{projectName}"
                let! response = httpClient.GetAsync(url) |> Async.AwaitTask
                return response.IsSuccessStatusCode
            with
            | _ -> return false
        }

    member this.CreateProject(projectName: string, productionBranch: string) : Async<Result<unit, string>> =
        async {
            try
                let url = $"https://api.cloudflare.com/client/v4/accounts/{accountId}/pages/projects"
                let payload = sprintf """{"name":"%s","production_branch":"%s"}""" projectName productionBranch

                use content = new StringContent(payload, Encoding.UTF8, "application/json")
                let! response = httpClient.PostAsync(url, content) |> Async.AwaitTask
                let! responseBody = response.Content.ReadAsStringAsync() |> Async.AwaitTask

                if response.IsSuccessStatusCode then
                    use doc = JsonDocument.Parse(responseBody)
                    let root = doc.RootElement
                    let mutable successProp = Unchecked.defaultof<JsonElement>
                    if root.TryGetProperty("success", &successProp) && successProp.GetBoolean() then
                        return Ok ()
                    else
                        return Error $"Create project API error: {responseBody}"
                else
                    return Error $"Create project HTTP {int response.StatusCode}: {responseBody}"
            with
            | ex -> return Error $"Failed to create project: {ex.Message}"
        }

    member this.DeployDirectory
        (projectName: string)
        (directory: string)
        (verbose: bool)
        (progressCallback: string -> unit)
        : Async<Result<string, string>> =
        async {
            try
                progressCallback "Getting upload token..."
                let! tokenResult = this.GetUploadToken(projectName)
                match tokenResult with
                | Error e -> return Error $"Failed to get upload token: {e}"
                | Ok jwt ->
                if verbose then progressCallback $"Got JWT token ({jwt.Length} chars)"

                progressCallback "Collecting files..."
                let files = collectFiles directory
                let allHashes = files |> List.map (fun f -> f.Hash) |> List.distinct

                progressCallback $"Found {files.Length} files, {allHashes.Length} unique hashes"

                progressCallback "Checking for existing assets..."
                let! missingResult = this.CheckMissingAssets(jwt, allHashes)

                match missingResult with
                | Error e -> return Error $"Failed to check missing assets: {e}"
                | Ok missingHashes ->

                progressCallback $"{missingHashes.Length} files need uploading"

                let! uploadOk =
                    if missingHashes.Length > 0 then
                        async {
                            progressCallback $"Uploading {missingHashes.Length} files..."

                            let missingHashSet = Set.ofList missingHashes
                            let filesToUpload =
                                files
                                |> List.filter (fun f -> missingHashSet.Contains(f.Hash))
                                |> List.map (fun f ->
                                    { Hash = f.Hash
                                      Content = File.ReadAllBytes(f.FullPath)
                                      ContentType = f.ContentType
                                      FilePath = f.RelativePath })

                            let maxBatchSize = 50L * 1024L * 1024L
                            let maxFilesPerBatch = 100

                            let rec uploadBatches (remaining: FileUpload list) (batchNum: int) =
                                async {
                                    match remaining with
                                    | [] -> return Ok ()
                                    | _ ->
                                        let mutable batchSize = 0L
                                        let mutable batchCount = 0
                                        let batch, rest =
                                            remaining
                                            |> List.fold (fun (batch, rest) file ->
                                                let newSize = batchSize + int64 file.Content.Length
                                                if batchCount < maxFilesPerBatch && newSize < maxBatchSize then
                                                    batchSize <- newSize
                                                    batchCount <- batchCount + 1
                                                    (file :: batch, rest)
                                                else
                                                    (batch, file :: rest)
                                            ) ([], [])
                                            |> fun (b, r) -> (List.rev b, List.rev r)

                                        progressCallback $"Uploading batch {batchNum} ({batch.Length} files, {batchSize / 1024L}KB)..."

                                        let! uploadResult = this.UploadAssetBatch(jwt, batch)
                                        match uploadResult with
                                        | Error e -> return Error e
                                        | Ok () -> return! uploadBatches rest (batchNum + 1)
                                }

                            return! uploadBatches filesToUpload 1
                        }
                    else
                        async {
                            progressCallback "All files already uploaded"
                            return Ok ()
                        }

                match uploadOk with
                | Error e -> return Error e
                | Ok () ->

                progressCallback "Registering assets..."
                let! upsertResult = this.UpsertHashes(jwt, allHashes)
                match upsertResult with
                | Error e -> return Error $"Failed to register assets: {e}"
                | Ok () ->

                progressCallback "Creating deployment..."
                let manifest = buildManifest files

                let url = $"https://api.cloudflare.com/client/v4/accounts/{accountId}/pages/projects/{projectName}/deployments"

                use formContent = new MultipartFormDataContent()
                formContent.Add(new StringContent(manifest), "manifest")
                formContent.Add(new StringContent("main"), "branch")

                let! response = httpClient.PostAsync(url, formContent) |> Async.AwaitTask
                let! responseBody = response.Content.ReadAsStringAsync() |> Async.AwaitTask

                if response.IsSuccessStatusCode then
                    use doc = JsonDocument.Parse(responseBody)
                    let root = doc.RootElement
                    let mutable successProp = Unchecked.defaultof<JsonElement>
                    if root.TryGetProperty("success", &successProp) && successProp.GetBoolean() then
                        let mutable resultProp = Unchecked.defaultof<JsonElement>
                        if root.TryGetProperty("result", &resultProp) then
                            let mutable urlProp = Unchecked.defaultof<JsonElement>
                            if resultProp.TryGetProperty("url", &urlProp) then
                                return Ok (urlProp.GetString())
                            else
                                return Ok "Deployment created successfully"
                        else
                            return Ok "Deployment created successfully"
                    else
                        return Error $"Deployment failed: {responseBody}"
                else
                    return Error $"Deployment failed - HTTP {int response.StatusCode}: {responseBody}"
            with
            | ex -> return Error $"Failed to deploy: {ex.Message}"
        }

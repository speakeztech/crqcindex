namespace Partas.Solid.CRQCIndex

open Fable.Core
open Fable.Core.JsInterop
open Partas.Solid

/// Content index for CRQC Index.
/// Uses Vite's import.meta.glob to eagerly load all markdown content at build time.
/// solid-marked compiles each .md into a SolidJS component module with:
///   - default export: the rendered Component
///   - named export: frontmatter (parsed YAML)
///   - named export: TableOfContents (auto-generated heading nav)
module Content =

    /// Vite's import.meta.glob — eagerly imports all content markdown files at build time
    [<Emit("import.meta.glob('/content/**/*.md', { eager: true })")>]
    let private contentModules : obj = jsNative

    /// A single content entry with its compiled component and metadata
    type Entry =
        { Component: obj
          TableOfContents: obj
          frontmatter: obj
          section: string
          slug: string
          path: string }

    let private contentIndex = JsInterop.createObj []
    let private sectionIndex = JsInterop.createObj [
        "bulletins", box (ResizeArray<Entry>())
        "analysis", box (ResizeArray<Entry>())
        "methodology", box (ResizeArray<Entry>())
        "fud-files", box (ResizeArray<Entry>())
    ]

    /// Build the content and section indexes from the glob-imported modules
    let private buildIndex () =
        let entries = JS.Constructors.Object.entries contentModules
        for kv in entries do
            let path: string = !!kv?(0)
            let md: obj = !!kv?(1)
            let stripped = path.Replace("/content/", "").Replace(".md", "")
            let parts = stripped.Split('/')
            let section = parts.[0]
            let slug = if parts.Length > 1 then parts.[1..] |> String.concat "/" else ""
            let entryPath = section + "/" + slug

            let entry =
                { Component = md?``default``
                  TableOfContents = md?TableOfContents
                  frontmatter = if isNull md?frontmatter then createObj [] else md?frontmatter
                  section = section
                  slug = slug
                  path = entryPath }

            contentIndex?(entryPath) <- entry
            let sectionList: ResizeArray<Entry> option = !!sectionIndex?(section)
            match sectionList with
            | Some list -> list.Add(entry)
            | None -> ()

        // Sort each section by date descending
        let sections = JS.Constructors.Object.values sectionIndex
        for section in sections do
            let list: ResizeArray<Entry> = !!section
            let sorted =
                list
                |> Seq.sortByDescending (fun e ->
                    let date: string = !!e.frontmatter?date
                    if isNull date then "" else date)
                |> Seq.toArray
            list.Clear()
            for item in sorted do
                list.Add(item)

    // Build on module load
    do buildIndex()

    let getContent (sectionSlug: string) : obj =
        let entry = contentIndex?(sectionSlug)
        if isNull entry then null else !!entry

    let getSection (section: string) : obj array =
        let list: ResizeArray<Entry> option = !!sectionIndex?(section)
        match list with
        | Some l -> l.ToArray() |> unbox
        | None -> [||]

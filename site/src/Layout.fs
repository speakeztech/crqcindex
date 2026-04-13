namespace Partas.Solid.CRQCIndex

open Fable.Core
open Fable.Core.JsInterop
open Partas.Solid
open Partas.Solid.ArkUI

module Navigation =
    [<Import("createSignal", "solid-js")>]
    let createSignal<'T> (initialValue: 'T) : Accessor<'T> * ('T -> unit) = jsNative

    [<Import("createEffect", "solid-js")>]
    let createEffect (fn: unit -> unit) : unit = jsNative

    /// Read the current hash route
    let getCurrentRoute () =
        let hash = Browser.Dom.window.location.hash
        if hash.Length > 1 then hash.[1..] else ""

/// Sidebar navigation component
[<Erase>]
type Sidebar() =
    inherit nav()

    [<Erase>]
    member val currentRoute: Accessor<string> = Unchecked.defaultof<_> with get, set

    [<Erase>]
    member val onNavigate: (string -> unit) = Unchecked.defaultof<_> with get, set

    [<SolidTypeComponent>]
    member props.constructor =
        let isActive (route: string) =
            let current = props.currentRoute()
            if route = "" then current = ""
            else current.StartsWith(route)

        let navClass (route: string) =
            if isActive route then
                "flex items-center gap-3 px-3 py-2 rounded-lg text-sm font-medium bg-crqc-indigo/10 text-crqc-indigo dark:text-crqc-indigo-light"
            else
                "flex items-center gap-3 px-3 py-2 rounded-lg text-sm text-speakez-neutral/70 dark:text-speakez-neutral-light/70 hover:bg-speakez-neutral/5 dark:hover:bg-speakez-neutral-light/5 transition-colors"

        nav(class' = "h-full py-6 px-4 overflow-y-auto bg-white/50 dark:bg-speakez-neutral/50") {
            div(class' = "mb-6") {
                div(class' = "flex items-center gap-2 px-2 mb-4") {
                    div(class' = "w-8 h-8 rounded-lg bg-gradient-to-br from-crqc-indigo to-speakez-teal flex items-center justify-center") {
                        span(class' = "text-white font-bold font-mono text-sm") { "Q" }
                    }
                    span(class' = "text-sm font-bold font-heading text-speakez-neutral dark:text-speakez-neutral-light") {
                        "CRQC Index"
                    }
                }
            }
            div(class' = "space-y-1") {
                a(href = "#/", onClick = (fun _ -> props.onNavigate ""), class' = navClass "") {
                    span() { "\uD83C\uDFE0" }
                    "Dashboard"
                }
                a(href = "#/bulletins", onClick = (fun _ -> props.onNavigate "bulletins"), class' = navClass "bulletins") {
                    span() { "\u26A1" }
                    "Bulletins"
                }
                a(href = "#/analysis", onClick = (fun _ -> props.onNavigate "analysis"), class' = navClass "analysis") {
                    span() { "\uD83D\uDD2C" }
                    "Analysis"
                }
                a(href = "#/methodology", onClick = (fun _ -> props.onNavigate "methodology"), class' = navClass "methodology") {
                    span() { "\uD83D\uDCCA" }
                    "Methodology"
                }
                a(href = "#/fud-files", onClick = (fun _ -> props.onNavigate "fud-files"), class' = navClass "fud-files") {
                    span() { "\uD83D\uDEE1\uFE0F" }
                    "FUD Files"
                }
            }
        }

/// Dashboard home view — the original splash content
[<Erase>]
type DashboardView() =
    inherit div()

    [<SolidTypeComponent>]
    member props.constructor =
        div(class' = "py-8") {
            // Hero section
            div(class' = "text-center py-8 sm:py-12") {
                h1(class' = Styles.heading) {
                    "CRQC Index"
                }
                p(class' = Styles.subheading + " mt-4 max-w-2xl mx-auto") {
                    "Monitoring convergence toward cryptographically relevant quantum computation"
                }
                div(class' = Styles.divider + " mt-8")
            }

            // Z-estimate instrument
            div(class' = "mb-12") {
                ZEstimateInstrument()
            }

            // Mosca theorem explainer
            div(class' = "mb-12") {
                MoscaExplainer()
            }

            // Conclave callout
            div(class' = Styles.card + " p-6 mb-12") {
                div(class' = "flex items-center gap-3 mb-3") {
                    div(class' = "w-8 h-8 rounded-lg bg-gradient-to-br from-speakez-teal to-speakez-blue flex items-center justify-center") {
                        span(class' = "text-white font-bold text-sm") { "C" }
                    }
                    h2(class' = "text-xl font-bold font-heading text-speakez-neutral dark:text-speakez-neutral-light") {
                        "A Conclave Project"
                    }
                }
                p(class' = Styles.bodyText) {
                    "Built on SpeakEZ's "
                    a(href = "https://speakez.tech/blog/conclave-a-speakez-platform-service/", target = "_blank", class' = "link-crqc font-semibold") { "Conclave platform " }
                    "for agentic AI systems on Cloudflare's global edge."
                }
            }

            // Sections grid
            div(class' = "mb-16") {
                h2(class' = "text-2xl font-bold font-heading text-speakez-neutral dark:text-speakez-neutral-light mb-6") {
                    "What's Ahead"
                }
                div(class' = "grid grid-cols-1 md:grid-cols-2 gap-4") {
                    SectionPreview(
                        title = "Bulletins",
                        description = "Short-form signal feed tracking hardware milestones, error correction breakthroughs, and cryptanalytic developments.",
                        icon = "\u26A1"
                    )
                    SectionPreview(
                        title = "Analysis",
                        description = "Long-form synthesis connecting patterns across signals, with transparent methodology and source attribution.",
                        icon = "\uD83D\uDD2C"
                    )
                    SectionPreview(
                        title = "Methodology",
                        description = "Reference documentation on scoring rubric, signal taxonomy, Z estimation model, and editorial scope.",
                        icon = "\uD83D\uDCCA"
                    )
                    SectionPreview(
                        title = "FUD Files",
                        description = "Dedicated analysis of excluded signals: marketing inflation, dismissal campaigns, and hype amplification.",
                        icon = "\uD83D\uDEE1\uFE0F"
                    )
                }
            }

            SiteFooter()
        }

/// Reactive content router — imported from JSX for proper SolidJS signal tracking
[<Erase; Import("ContentRouter", "./router.jsx")>]
type ContentRouter() =
    interface HtmlTag

    [<DefaultValue>]
    val mutable currentRoute: Accessor<string>

    [<DefaultValue>]
    val mutable dashboard: HtmlElement

/// Main app shell with Ark UI Splitter for sidebar/content layout
[<Erase>]
type AppShell() =
    inherit div()

    [<Erase>]
    member val isDark: Accessor<bool> = Unchecked.defaultof<_> with get, set

    [<Erase>]
    member val onToggleTheme: (unit -> unit) = Unchecked.defaultof<_> with get, set

    [<SolidTypeComponent>]
    member props.constructor =
        let currentRoute, setRoute = Navigation.createSignal (Navigation.getCurrentRoute())

        // Listen for hash changes
        Navigation.createEffect (fun () ->
            let handler = fun _ -> setRoute (Navigation.getCurrentRoute())
            Browser.Dom.window.addEventListener("hashchange", handler)
        )

        div(class' = Styles.pageBackground) {
            NavHeader(isDark = props.isDark, onToggleTheme = props.onToggleTheme)

            div(class' = "flex-1", style = "height: calc(100vh - 72px)") {
                Splitter(
                    orientation = SplitterOrientation.Horizontal,
                    defaultSize = [| 20.0; 80.0 |],
                    panels = [|
                        PanelData(id = "sidebar", minSize = 15.0, maxSize = 30.0, collapsible = true, collapsedSize = 0.0)
                        PanelData(id = "content", minSize = 50.0)
                    |]
                ) {
                    Splitter.Panel(id = "sidebar") {
                        Sidebar(currentRoute = currentRoute, onNavigate = fun r -> setRoute r)
                    }

                    Splitter.ResizeTrigger(
                        id = "sidebar:content",
                        class' = "w-1.5 bg-speakez-neutral/10 dark:bg-speakez-neutral-light/10 hover:bg-crqc-indigo/30 dark:hover:bg-crqc-indigo-light/30 transition-colors cursor-col-resize flex items-center justify-center"
                    ) {
                        Splitter.ResizeTriggerIndicator(
                            class' = "w-0.5 h-8 rounded-full bg-speakez-neutral/30 dark:bg-speakez-neutral-light/30"
                        )
                    }

                    Splitter.Panel(id = "content") {
                        main(class' = "h-full overflow-y-auto") {
                            div(class' = Styles.container) {
                                ContentRouter(currentRoute = currentRoute, dashboard = DashboardView())
                            }
                        }
                    }
                }
            }
        }

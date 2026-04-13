namespace Partas.Solid.CRQCIndex

open Fable.Core
open Fable.Core.JsInterop
open Partas.Solid

module Navigation =
    [<Import("createSignal", "solid-js")>]
    let createSignal<'T> (initialValue: 'T) : Accessor<'T> * ('T -> unit) = jsNative

    [<Import("createEffect", "solid-js")>]
    let createEffect (fn: unit -> unit) : unit = jsNative

    /// Read the current hash route
    let getCurrentRoute () =
        let hash = Browser.Dom.window.location.hash
        if hash.Length > 1 then hash.[1..] else ""

/// Animated hamburger icon that morphs between menu and X
[<Erase>]
type HamburgerIcon() =
    inherit button()

    [<Erase>]
    member val isOpen: Accessor<bool> = Unchecked.defaultof<_> with get, set

    [<Erase>]
    member val onToggle: (unit -> unit) = Unchecked.defaultof<_> with get, set

    [<SolidTypeComponent>]
    member props.constructor =
        let barBase = "block h-0.5 w-6 rounded-full bg-speakez-neutral dark:bg-speakez-neutral-light transition-all duration-300 ease-in-out"
        button(
            class' = "flex flex-col items-center justify-center w-10 h-10 rounded-lg hover:bg-speakez-neutral/10 dark:hover:bg-speakez-neutral-light/10 transition-colors gap-1.5 z-50",
            onClick = (fun _ -> props.onToggle()),
            title = "Toggle navigation"
        ) {
            span(class' =
                if props.isOpen() then barBase + " rotate-45 translate-y-2"
                else barBase
            )
            span(class' =
                if props.isOpen() then barBase + " opacity-0 scale-x-0"
                else barBase
            )
            span(class' =
                if props.isOpen() then barBase + " -rotate-45 -translate-y-2"
                else barBase
            )
        }

/// Sidebar navigation component — supports expanded (icons+text) and collapsed (icons only)
[<Erase>]
type Sidebar() =
    inherit nav()

    [<Erase>]
    member val currentRoute: Accessor<string> = Unchecked.defaultof<_> with get, set

    [<Erase>]
    member val onNavigate: (string -> unit) = Unchecked.defaultof<_> with get, set

    [<Erase>]
    member val expanded: Accessor<bool> = Unchecked.defaultof<_> with get, set

    [<SolidTypeComponent>]
    member props.constructor =
        let isActive (route: string) =
            let current = props.currentRoute()
            if route = "" then current = ""
            else current.StartsWith(route)

        let navClass (route: string) =
            let expanded = props.expanded()
            let baseClass =
                if expanded then "flex items-center gap-4 px-4 py-3 rounded-lg text-base font-medium transition-colors"
                else "flex items-center justify-center w-12 h-12 rounded-lg transition-colors"
            if isActive route then
                baseClass + " bg-crqc-indigo/10 text-crqc-indigo dark:text-crqc-indigo-light font-semibold"
            else
                baseClass + " text-speakez-neutral/70 dark:text-speakez-neutral-light/70 hover:bg-speakez-neutral/5 dark:hover:bg-speakez-neutral-light/5"

        let navItem (route: string) (icon: string) (label: string) =
            a(href = "#/" + route, onClick = (fun _ -> props.onNavigate route), class' = navClass route, title = label) {
                span(class' = "text-xl flex-shrink-0") { icon }
                if props.expanded() then
                    span() { label }
            }

        nav(class' = "h-full py-6 overflow-y-auto bg-white/50 dark:bg-speakez-neutral/50") {
            // Logo — only in expanded mode
            if props.expanded() then
                div(class' = "mb-8 px-4") {
                    div(class' = "flex items-center gap-3 px-2 mb-6") {
                        div(class' = "w-10 h-10 rounded-lg bg-gradient-to-br from-crqc-indigo to-speakez-teal flex items-center justify-center") {
                            span(class' = "text-white font-bold font-mono text-base") { "Q" }
                        }
                        span(class' = "text-base font-bold font-heading text-speakez-neutral dark:text-speakez-neutral-light") {
                            "CRQC Index"
                        }
                    }
                }

            div(class' = (if props.expanded() then "space-y-1 px-4" else "space-y-2 flex flex-col items-center px-1")) {
                navItem "" "\uD83C\uDFE0" "Dashboard"
                navItem "bulletins" "\u26A1" "Bulletins"
                navItem "analysis" "\uD83D\uDD2C" "Analysis"
                navItem "methodology" "\uD83D\uDCCA" "Methodology"
                navItem "fud-files" "\uD83D\uDEE1\uFE0F" "FUD Files"
            }
        }

/// Dashboard home view — the original splash content
[<Erase>]
type DashboardView() =
    inherit div()

    [<SolidTypeComponent>]
    member props.constructor =
        div(class' = "py-8") {
            div(class' = "text-center py-8 sm:py-12") {
                h1(class' = Styles.heading) { "CRQC Index" }
                p(class' = Styles.subheading + " mt-4 max-w-2xl mx-auto") {
                    "Monitoring convergence toward cryptographically relevant quantum computation"
                }
                div(class' = Styles.divider + " mt-8")
            }
            div(class' = "mb-12") { ZEstimateInstrument() }
            div(class' = "mb-12") { MoscaExplainer() }
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
            div(class' = "mb-16") {
                h2(class' = "text-2xl font-bold font-heading text-speakez-neutral dark:text-speakez-neutral-light mb-6") {
                    "What's Ahead"
                }
                div(class' = "grid grid-cols-1 md:grid-cols-2 gap-4") {
                    SectionPreview(title = "Bulletins", description = "Short-form signal feed tracking hardware milestones, error correction breakthroughs, and cryptanalytic developments.", icon = "\u26A1")
                    SectionPreview(title = "Analysis", description = "Long-form synthesis connecting patterns across signals, with transparent methodology and source attribution.", icon = "\uD83D\uDD2C")
                    SectionPreview(title = "Methodology", description = "Reference documentation on scoring rubric, signal taxonomy, Z estimation model, and editorial scope.", icon = "\uD83D\uDCCA")
                    SectionPreview(title = "FUD Files", description = "Dedicated analysis of excluded signals: marketing inflation, dismissal campaigns, and hype amplification.", icon = "\uD83D\uDEE1\uFE0F")
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

/// Main app shell with collapsible sidebar
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
        let sidebarOpen, setSidebarOpen = Navigation.createSignal true

        Navigation.createEffect (fun () ->
            let handler = fun _ -> setRoute (Navigation.getCurrentRoute())
            Browser.Dom.window.addEventListener("hashchange", handler)
        )

        let toggleSidebar () = setSidebarOpen (not (sidebarOpen()))

        div(class' = Styles.pageBackground) {
            // Header with hamburger
            header(class' = "py-4 border-b border-speakez-neutral/10 dark:border-speakez-neutral-light/10") {
                div(class' = "max-w-full mx-auto px-4") {
                    div(class' = "flex justify-between items-center") {
                        div(class' = "flex items-center gap-3") {
                            HamburgerIcon(isOpen = sidebarOpen, onToggle = toggleSidebar)
                            // Show logo text in header when sidebar is collapsed
                            div(class' = if sidebarOpen() then "hidden" else "flex items-center gap-3") {
                                div(class' = "w-8 h-8 rounded-lg bg-gradient-to-br from-crqc-indigo to-speakez-teal flex items-center justify-center") {
                                    span(class' = "text-white font-bold font-mono text-sm") { "Q" }
                                }
                                span(class' = "text-lg font-bold font-heading text-speakez-neutral dark:text-speakez-neutral-light hidden sm:inline") {
                                    "CRQC Index"
                                }
                            }
                        }
                        nav(class' = "flex items-center gap-4") {
                            span(class' = Styles.comingSoonBadge + " hidden sm:inline-flex") { "Coming Soon" }
                            ThemeToggle(isDark = props.isDark, onToggle = props.onToggleTheme)
                        }
                    }
                }
            }

            let sidebarClass =
                let baseClass = "border-r border-speakez-neutral/10 dark:border-speakez-neutral-light/10 transition-all duration-300 ease-in-out overflow-hidden flex-shrink-0 "
                if sidebarOpen() then
                    baseClass + "fixed md:relative z-40 md:z-auto w-64 h-full"
                else
                    baseClass + "hidden md:block w-16"

            let onNav = fun r ->
                setRoute r
                if Browser.Dom.window.innerWidth < 768.0 then
                    setSidebarOpen false

            div(class' = "flex relative", style = "height: calc(100vh - 65px)") {
                // Mobile overlay backdrop
                Show(when' = sidebarOpen()) {
                    div(
                        class' = "fixed inset-0 bg-black/30 z-30 md:hidden transition-opacity duration-300",
                        onClick = (fun _ -> setSidebarOpen false)
                    )
                }

                // Sidebar — desktop: inline with transition, mobile: overlay
                div(class' = sidebarClass) {
                    Sidebar(
                        currentRoute = currentRoute,
                        onNavigate = onNav,
                        expanded = sidebarOpen
                    )
                }

                // Main content
                main(class' = "flex-1 h-full overflow-y-auto") {
                    div(class' = Styles.container + " py-4") {
                        ContentRouter(currentRoute = currentRoute, dashboard = DashboardView())
                    }
                }
            }
        }

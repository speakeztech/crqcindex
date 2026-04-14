namespace Partas.Solid.CRQCIndex

open Fable.Core
open Fable.Core.JsInterop
open Partas.Solid
open Partas.Solid.Router
open Partas.Solid.Meta

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

/// Sidebar navigation — reads route location directly for active state
[<Erase>]
type Sidebar() =
    inherit nav()

    [<Erase>]
    member val onNavigate: (unit -> unit) = Unchecked.defaultof<_> with get, set

    [<Erase>]
    member val expanded: Accessor<bool> = Unchecked.defaultof<_> with get, set

    [<SolidTypeComponent>]
    member props.constructor =
        let location = Hooks.useLocation()

        let isActive (path: string) =
            let current = location.pathname
            if path = "/" then current = "/"
            else current.StartsWith(path)

        let navClass (path: string) =
            let expanded = props.expanded()
            let baseClass =
                if expanded then "flex items-center gap-4 px-4 py-3 rounded-lg text-base font-medium transition-colors"
                else "flex items-center justify-center w-12 h-12 rounded-lg transition-colors"
            if isActive path then
                baseClass + " bg-crqc-indigo/10 text-crqc-indigo dark:text-crqc-indigo-light font-semibold"
            else
                baseClass + " text-speakez-neutral/70 dark:text-speakez-neutral-light/70 hover:bg-speakez-neutral/5 dark:hover:bg-speakez-neutral-light/5"

        // Regular <a> tags — the Router intercepts clicks for client-side navigation.
        // onClick closes sidebar on mobile only; does not preventDefault.
        let navItem (path: string) (icon: string) (label: string) =
            a(href = path, onClick = (fun _ ->
                if Browser.Dom.window.innerWidth < 768.0 then
                    props.onNavigate()
            ), class' = navClass path, title = label) {
                span(class' = "text-xl flex-shrink-0") { icon }
                if props.expanded() then
                    span() { label }
            }

        nav(class' = "h-full py-6 overflow-y-auto bg-white dark:bg-speakez-neutral md:bg-white/50 md:dark:bg-speakez-neutral/50") {
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
                navItem "/" "\uD83C\uDFE0" "Dashboard"
                navItem "/bulletins" "\u26A1" "Bulletins"
                navItem "/analysis" "\uD83D\uDD2C" "Analysis"
                navItem "/methodology" "\uD83D\uDCCA" "Methodology"
                navItem "/fud-files" "\uD83D\uDEE1\uFE0F" "FUD Files"
            }
        }

module internal Interop =
    /// Prevents the FablePlugin from inlining a closure body when passed as a
    /// prop to a SolidTypeComponent. Without this, the plugin evaluates the
    /// closure during render instead of passing a function reference.
    [<Emit("$0")>]
    let callback (fn: unit -> unit) : (unit -> unit) = fn

    /// Accesses props.children directly, bypassing splitProps. The SolidJS
    /// compiler recognizes props.children and handles it reactively for route
    /// transitions. splitProps produces PARTAS_LOCAL.children which the Solid
    /// compiler treats as a static const member access.
    [<Emit("$0.children")>]
    let propsChildren (props: obj) : HtmlElement = jsNative

/// Main app shell — used as Router's root layout.
/// Owns theme state, renders sidebar + header, and renders route-matched
/// content. Children are accessed via Interop.propsChildren to bypass
/// splitProps and preserve SolidJS children reactivity.
[<Erase>]
type AppShell() =
    inherit div()

    [<SolidTypeComponent>]
    member props.constructor =
        // Access route content directly from props, bypassing splitProps
        let routeContent () : HtmlElement = Interop.propsChildren props

        // Re-trigger page-enter animation on every route change
        let location = Hooks.useLocation()
        Solid.createEffect (fun () ->
            let _ = location.pathname
            let el = Browser.Dom.document.querySelector(".route-transition")
            if not (isNull el) then
                el.classList.remove("route-transition")
                ignore (el?offsetHeight) // force reflow
                el.classList.add("route-transition")
        )

        // Theme state
        let initialDark =
            let stored = Browser.Dom.window.localStorage.getItem("theme")
            stored <> "light"

        let isDark, setIsDark = Solid.createSignal initialDark

        let toggleTheme () =
            let newDark = not (isDark())
            setIsDark newDark
            if newDark then
                Browser.Dom.document.documentElement.classList.add("dark")
                Browser.Dom.window.localStorage.setItem("theme", "dark")
            else
                Browser.Dom.document.documentElement.classList.remove("dark")
                Browser.Dom.window.localStorage.setItem("theme", "light")

        // Sidebar state — closed by default on mobile and narrow desktop
        let sidebarOpen, setSidebarOpen = Solid.createSignal (Browser.Dom.window.innerWidth >= 1024.0)
        let toggleSidebar () = setSidebarOpen (not (sidebarOpen()))

        MetaProvider() {
            div(class' = Styles.pageBackground) {
                // Header with hamburger
                header(class' = "py-4 border-b border-speakez-neutral/10 dark:border-speakez-neutral-light/10") {
                    div(class' = "max-w-full mx-auto px-4") {
                        div(class' = "flex justify-between items-center") {
                            div(class' = "flex items-center gap-3") {
                                HamburgerIcon(isOpen = sidebarOpen, onToggle = Interop.callback toggleSidebar)
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
                                ThemeToggle(isDark = isDark, onToggle = Interop.callback toggleTheme)
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

                div(class' = "flex relative", style = "height: calc(100vh - 65px)") {
                    // Mobile overlay backdrop
                    Show(when' = sidebarOpen()) {
                        div(
                            class' = "fixed inset-0 bg-black/30 z-30 md:hidden transition-opacity duration-300",
                            onClick = (fun _ -> setSidebarOpen false)
                        )
                    }

                    // Sidebar
                    div(class' = sidebarClass) {
                        Sidebar(
                            onNavigate = Interop.callback (fun () -> setSidebarOpen false),
                            expanded = sidebarOpen
                        )
                    }

                    // Main content — renders the route-matched component
                    main(class' = "flex-1 h-full overflow-y-auto") {
                        div(class' = Styles.container + " py-4 route-transition") {
                            routeContent()
                        }
                    }
                }
            }
        }

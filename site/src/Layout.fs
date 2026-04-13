namespace Partas.Solid.CRQCIndex

open Fable.Core
open Fable.Core.JsInterop
open Partas.Solid
open Partas.Solid.Router

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
                navItem "/" "\uD83C\uDFE0" "Dashboard"
                navItem "/bulletins" "\u26A1" "Bulletins"
                navItem "/analysis" "\uD83D\uDD2C" "Analysis"
                navItem "/methodology" "\uD83D\uDCCA" "Methodology"
                navItem "/fud-files" "\uD83D\uDEE1\uFE0F" "FUD Files"
            }
        }

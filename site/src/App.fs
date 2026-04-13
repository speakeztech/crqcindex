namespace Partas.Solid.CRQCIndex

open Fable.Core
open Fable.Core.JsInterop
open Browser.Dom
open Browser.Types
open Partas.Solid

module App =

    /// Import SolidJS render function
    [<Import("render", "solid-js/web")>]
    let render (app: unit -> HtmlElement) (container: Element) : unit = jsNative

    /// Import SolidJS createSignal
    [<Import("createSignal", "solid-js")>]
    let createSignal<'T> (initialValue: 'T) : Accessor<'T> * ('T -> unit) = jsNative

    /// Main application component with Splitter layout
    [<SolidComponent>]
    let App () =
        let initialDark =
            let stored = window.localStorage.getItem("theme")
            stored <> "light"

        let isDark, setIsDark = createSignal initialDark

        let toggleTheme () =
            let newDark = not (isDark())
            setIsDark newDark
            if newDark then
                document.documentElement.classList.add("dark")
                window.localStorage.setItem("theme", "dark")
            else
                document.documentElement.classList.remove("dark")
                window.localStorage.setItem("theme", "light")

        AppShell(isDark = isDark, onToggleTheme = toggleTheme)

    // Application entry point
    let main () =
        let root = document.getElementById("app")
        if not (isNull root) then
            render (fun () -> App() :> HtmlElement) root

    // Auto-start the application
    main()

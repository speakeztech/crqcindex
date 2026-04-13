namespace Partas.Solid.CRQCIndex

open Fable.Core
open Fable.Core.JsInterop
open Browser.Dom
open Browser.Types
open Partas.Solid

/// Router component — defined in app-router.jsx because the Partas.Solid
/// FablePlugin does not yet erase the Router builder pattern into JSX.
/// All page components and the app shell are F#; only Route wiring is JSX.
[<Erase; Import("AppRouter", "./app-router.jsx")>]
type AppRouter() =
    interface HtmlTag

module App =

    /// Import SolidJS render function
    [<Import("render", "solid-js/web")>]
    let render (app: unit -> HtmlElement) (container: Element) : unit = jsNative

    [<SolidComponent>]
    let App () =
        AppRouter()

    // Application entry point
    let main () =
        let root = document.getElementById("app")
        if not (isNull root) then
            render (fun () -> App() :> HtmlElement) root

    // Auto-start the application
    main()

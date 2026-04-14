namespace Partas.Solid.CRQCIndex

open Fable.Core
open Fable.Core.JsInterop
open Browser.Dom
open Browser.Types
open Partas.Solid
open Partas.Solid.Router

module App =

    /// Import SolidJS render function
    [<Import("render", "solid-js/web")>]
    let render (app: unit -> HtmlElement) (container: Element) : unit = jsNative

    [<SolidComponent>]
    let App () =
        Router(root = !!jsConstructor<AppShell>) {
            Route(path = "/", component' = !!jsConstructor<DashboardView>)
            Route(path = "/:section", component' = !!jsConstructor<SectionListPage>)
            Route(path = "/:section/:slug", component' = !!jsConstructor<ArticlePage>)
        }

    // Application entry point
    let main () =
        let root = document.getElementById("app")
        if not (isNull root) then
            render (fun () -> App() :> HtmlElement) root

    // Auto-start the application
    main()

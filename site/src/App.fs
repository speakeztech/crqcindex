namespace CRQCIndex.Site

open Fable.Core
open Fable.Core.JsInterop
open Browser.Dom
open Browser.Types
open global.Partas.Solid

module App =

    /// Import SolidJS render function
    [<Import("render", "solid-js/web")>]
    let render (app: unit -> HtmlElement) (container: Element) : unit = jsNative

    /// Main application component - splash page
    [<SolidComponent>]
    let App () =
        div(class' = Styles.pageBackground) {
            // Navigation header
            NavHeader()

            // Main content
            main(class' = Styles.container) {
                // Hero section
                div(class' = "text-center py-12 sm:py-16 lg:py-20") {
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
                        "Built on SpeakEZ's Conclave platform for agentic AI systems on Cloudflare's global edge. Specialized actors handle signal ingestion, scoring, deduplication, and Z-estimate synthesis through tell-first messaging with capability-based security."
                    }
                }

                // Upcoming sections grid
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
            }

            // Footer
            div(class' = Styles.container + " pb-8") {
                SiteFooter()
            }
        }

    // Application entry point
    let main () =
        let root = document.getElementById("app")
        if not (isNull root) then
            render (fun () -> App() :> HtmlElement) root

    // Auto-start the application
    main()

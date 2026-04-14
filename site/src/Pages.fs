namespace Partas.Solid.CRQCIndex

open Fable.Core
open Fable.Core.JsInterop
open Partas.Solid
open Partas.Solid.Router
open Partas.Solid.Meta

module Solid =
    [<Import("createSignal", "solid-js")>]
    let createSignal<'T> (initialValue: 'T) : Accessor<'T> * ('T -> unit) = jsNative

    [<Import("createEffect", "solid-js")>]
    let createEffect (fn: unit -> unit) : unit = jsNative

    [<Import("createMemo", "solid-js")>]
    let createMemo<'T> (fn: unit -> 'T) : Accessor<'T> = jsNative

// Content module is defined in Content.fs — same namespace, no import needed

module PageMeta =
    let siteTitle = "CRQC Index - Quantum Threat Monitoring"
    let siteDescription = "Tracking how close quantum computers are to breaking today's encryption. Signal analysis across ECC, RSA, and AES to help organizations plan their post-quantum migration."
    let defaultOgImage = "https://crqcindex.com/images/crcq_index.png"
    let siteUrl = "https://crqcindex.com"

module SectionMeta =
    let titleFor section =
        match section with
        | "bulletins" -> "Bulletins"
        | "analysis" -> "Analysis"
        | "methodology" -> "Methodology"
        | "fud-files" -> "FUD Files"
        | s -> s

    let iconFor section =
        match section with
        | "bulletins" -> "\u26A1"
        | "analysis" -> "\uD83D\uDD2C"
        | "methodology" -> "\uD83D\uDCCA"
        | "fud-files" -> "\uD83D\uDEE1\uFE0F"
        | _ -> "\uD83D\uDCC4"

    let descriptionFor section =
        match section with
        | "bulletins" -> "Short-form signal feed tracking hardware milestones, error correction breakthroughs, and cryptanalytic developments."
        | "analysis" -> "Long-form synthesis connecting patterns across signals, with transparent methodology and source attribution."
        | "methodology" -> "Reference documentation on scoring rubric, signal taxonomy, Z estimation model, and editorial scope."
        | "fud-files" -> "Dedicated analysis of excluded signals: marketing inflation, dismissal campaigns, and hype amplification."
        | _ -> ""

/// Dashboard home view
[<Erase>]
type DashboardView() =
    inherit div()

    [<SolidTypeComponent>]
    member props.constructor =
        div(class' = "py-8") {
            Title() { PageMeta.siteTitle }
            Meta(name = "description", content = PageMeta.siteDescription)
            Meta(property = "og:type", content = "website")
            Meta(property = "og:title", content = PageMeta.siteTitle)
            Meta(property = "og:description", content = PageMeta.siteDescription)
            Meta(property = "og:image", content = PageMeta.defaultOgImage)
            Meta(property = "og:url", content = PageMeta.siteUrl)
            Meta(name = "twitter:card", content = "summary_large_image")
            Meta(name = "twitter:title", content = PageMeta.siteTitle)
            Meta(name = "twitter:description", content = PageMeta.siteDescription)
            Meta(name = "twitter:image", content = PageMeta.defaultOgImage)

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

/// Section list page — derived functions for reactive param tracking.
/// @solidjs/router reuses the component when params change within the same
/// route pattern, so all param-derived values must be functions (not let bindings).
/// Function calls in the builder compile to {fn()} in JSX, which the Solid
/// compiler wraps in reactive effects.
[<Erase>]
type SectionListPage() =
    inherit div()

    [<SolidTypeComponent>]
    member props.constructor =
        let parameters = Hooks.useParams()
        let location = Hooks.useLocation()

        let section () : string = !!parameters?section
        let items () = Content.getSection (section())
        let sectionTitle () = SectionMeta.titleFor (section())
        let sectionIcon () = SectionMeta.iconFor (section())
        let sectionDesc () = SectionMeta.descriptionFor (section())
        let pageTitle () = sectionTitle() + " - CRQC Index"
        let canonicalUrl () = PageMeta.siteUrl + (!!location?pathname : string)

        div(class' = "py-8") {
            Title() { pageTitle() }
            Meta(name = "description", content = sectionDesc())
            Meta(property = "og:type", content = "website")
            Meta(property = "og:title", content = pageTitle())
            Meta(property = "og:description", content = sectionDesc())
            Meta(property = "og:image", content = PageMeta.defaultOgImage)
            Meta(property = "og:url", content = canonicalUrl())
            Meta(name = "twitter:card", content = "summary_large_image")
            Meta(name = "twitter:title", content = pageTitle())
            Meta(name = "twitter:description", content = sectionDesc())
            Meta(name = "twitter:image", content = PageMeta.defaultOgImage)

            div(class' = "flex items-center gap-3 mb-8") {
                div(class' = "w-10 h-10 rounded-lg bg-crqc-indigo/10 dark:bg-crqc-indigo-light/10 flex items-center justify-center") {
                    span(class' = "text-lg") { sectionIcon() }
                }
                h1(class' = "text-3xl font-bold font-heading text-speakez-neutral dark:text-speakez-neutral-light") {
                    sectionTitle()
                }
            }
            div(class' = "space-y-4") {
                For(each = items()) {
                    yield fun item _ ->
                        let fm: obj = item?frontmatter
                        let itemTitle: string = !!fm?title
                        let date: string = !!fm?date
                        let description: string = !!fm?description
                        let path: string = !!item?path
                        a(
                            href = "/" + path,
                            class' = Styles.card + " block p-6 hover:shadow-xl transition-shadow group"
                        ) {
                            div(class' = "flex justify-between items-start") {
                                div() {
                                    h3(class' = "font-semibold font-heading text-speakez-neutral dark:text-speakez-neutral-light mb-1 group-hover:text-crqc-indigo dark:group-hover:text-crqc-indigo-light transition-colors") {
                                        itemTitle
                                    }
                                    if not (isNull description) && description <> "" then
                                        p(class' = "text-sm text-speakez-neutral/60 dark:text-speakez-neutral-light/60 mb-2 line-clamp-2") {
                                            description
                                        }
                                    span(class' = "text-sm text-speakez-neutral/50 dark:text-speakez-neutral-light/50 font-mono") {
                                        date
                                    }
                                }
                                span(class' = "text-speakez-neutral/30 dark:text-speakez-neutral-light/30 group-hover:text-crqc-indigo transition-colors") {
                                    "\u2192"
                                }
                            }
                        }
                }
            }
        }

/// Article page — derived functions for reactive param tracking.
/// The if/else is INSIDE the builder so it compiles to a JSX ternary
/// (reactive), not a JS if/else at the component level (static).
[<Erase>]
type ArticlePage() =
    inherit div()

    [<SolidTypeComponent>]
    member props.constructor =
        let parameters = Hooks.useParams()
        let location = Hooks.useLocation()

        // Derived functions — reactive when called inside JSX expressions
        let section () : string = !!parameters?section
        let slug () : string = !!parameters?slug
        let content () = Content.getContent (section() + "/" + slug())
        let fm () : obj = if isNull (content()) then null else content()?frontmatter
        let articleTitle () : string = if isNull (fm()) then "" else !!fm()?title
        let date () : string = if isNull (fm()) then "" else !!fm()?date
        let description () : string = if isNull (fm()) then "" else !!fm()?description
        let ogImage () : string =
            if isNull (fm()) then PageMeta.defaultOgImage
            else
                let img: string = !!fm()?og_image
                if isNull img then PageMeta.defaultOgImage else img
        let sectionName () = SectionMeta.titleFor (section())
        let pageTitle () =
            if isNull (content()) then "Not Found - CRQC Index"
            else articleTitle() + " - CRQC Index"
        let canonicalUrl () = PageMeta.siteUrl + (!!location?pathname : string)

        // Single top-level element — the if/else inside the builder compiles
        // to a reactive JSX ternary, not a static JS branch.
        div() {
            Title() { pageTitle() }
            Meta(name = "description", content = description())
            Meta(property = "og:type", content = "article")
            Meta(property = "og:title", content = articleTitle())
            Meta(property = "og:description", content = description())
            Meta(property = "og:image", content = ogImage())
            Meta(property = "og:url", content = canonicalUrl())
            Meta(property = "article:published_time", content = date())
            Meta(name = "twitter:card", content = "summary_large_image")
            Meta(name = "twitter:title", content = articleTitle())
            Meta(name = "twitter:description", content = description())
            Meta(name = "twitter:image", content = ogImage())

            if isNull (content()) then
                div(class' = Styles.container + " py-12") {
                    div(class' = Styles.card + " p-8 text-center") {
                        h2(class' = "text-2xl font-bold font-heading text-speakez-neutral dark:text-speakez-neutral-light mb-4") {
                            "Content Not Found"
                        }
                        p(class' = Styles.bodyText) { "The requested article could not be located." }
                    }
                }
            else
                div(class' = "py-8") {
                    header(class' = "mb-8") {
                        div(class' = "flex items-center gap-2 mb-3") {
                            span(class' = Styles.tierBadge + " bg-crqc-indigo/10 text-crqc-indigo dark:text-crqc-indigo-light") {
                                sectionName()
                            }
                            span(class' = "text-sm text-speakez-neutral/50 dark:text-speakez-neutral-light/50 font-mono") {
                                date()
                            }
                        }
                        h1(class' = "text-3xl sm:text-4xl font-bold font-heading text-speakez-neutral dark:text-speakez-neutral-light") {
                            articleTitle()
                        }
                        div(class' = Styles.divider + " mt-6 mx-0")
                    }
                    div() { !!(content()?Component) }
                }
        }

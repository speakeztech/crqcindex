namespace Partas.Solid.CRQCIndex

open Fable.Core
open Fable.Core.JsInterop
open Partas.Solid

module Content =
    [<Import("getContent", "./content.js")>]
    let getContent (sectionSlug: string) : obj = jsNative

    [<Import("getSection", "./content.js")>]
    let getSection (section: string) : obj array = jsNative

/// Renders a single content article using the compiled MDX component
[<Erase>]
type ArticleView() =
    inherit div()

    [<Erase>]
    member val section: string = "" with get, set

    [<Erase>]
    member val slug: string = "" with get, set

    [<SolidTypeComponent>]
    member props.constructor =
        let content = Content.getContent (props.section + "/" + props.slug)
        if isNull content then
            div(class' = Styles.container + " py-12") {
                div(class' = Styles.card + " p-8 text-center") {
                    h2(class' = "text-2xl font-bold font-heading text-speakez-neutral dark:text-speakez-neutral-light mb-4") {
                        "Content Not Found"
                    }
                    p(class' = Styles.bodyText) { "The requested article could not be located." }
                }
            }
        else
            let fm: obj = content?frontmatter
            let title: string = fm?title
            let date: string = fm?date
            let sectionName: string = props.section
            let ContentComponent: obj = content?Component
            div(class' = "py-8") {
                header(class' = "mb-8") {
                    div(class' = "flex items-center gap-2 mb-3") {
                        span(class' = Styles.tierBadge + " bg-crqc-indigo/10 text-crqc-indigo dark:text-crqc-indigo-light") {
                            sectionName
                        }
                        span(class' = "text-sm text-speakez-neutral/50 dark:text-speakez-neutral-light/50 font-mono") {
                            date
                        }
                    }
                    h1(class' = "text-3xl sm:text-4xl font-bold font-heading text-speakez-neutral dark:text-speakez-neutral-light") {
                        title
                    }
                    div(class' = Styles.divider + " mt-6 mx-0")
                }
                // Render the MDX component directly as a function call
                div() { !!ContentComponent }
            }

/// Lists all articles in a section
[<Erase>]
type SectionListView() =
    inherit div()

    [<Erase>]
    member val section: string = "" with get, set

    [<Erase>]
    member val sectionTitle: string = "" with get, set

    [<Erase>]
    member val sectionIcon: string = "" with get, set

    [<SolidTypeComponent>]
    member props.constructor =
        let items = Content.getSection props.section
        div(class' = "py-8") {
            div(class' = "flex items-center gap-3 mb-8") {
                div(class' = "w-10 h-10 rounded-lg bg-crqc-indigo/10 dark:bg-crqc-indigo-light/10 flex items-center justify-center") {
                    span(class' = "text-lg") { props.sectionIcon }
                }
                h1(class' = "text-3xl font-bold font-heading text-speakez-neutral dark:text-speakez-neutral-light") {
                    props.sectionTitle
                }
            }
            div(class' = "space-y-4") {
                For(each = items) {
                    yield fun item _ ->
                        let fm: obj = item?frontmatter
                        let title: string = fm?title
                        let date: string = fm?date
                        let path: string = item?path
                        a(
                            href = "#/" + path,
                            class' = Styles.card + " block p-6 hover:shadow-xl transition-shadow group"
                        ) {
                            div(class' = "flex justify-between items-start") {
                                div() {
                                    h3(class' = "font-semibold font-heading text-speakez-neutral dark:text-speakez-neutral-light mb-1 group-hover:text-crqc-indigo dark:group-hover:text-crqc-indigo-light transition-colors") {
                                        title
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

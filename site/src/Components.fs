namespace Partas.Solid.CRQCIndex

open Fable.Core
open Fable.Core.JsInterop
open Partas.Solid

/// Theme toggle button component
[<Erase>]
type ThemeToggle() =
    inherit button()

    [<Erase>]
    member val isDark: Accessor<bool> = Unchecked.defaultof<_> with get, set

    [<Erase>]
    member val onToggle: (unit -> unit) = Unchecked.defaultof<_> with get, set

    [<SolidTypeComponent>]
    member props.constructor =
        let titleText = if props.isDark() then "Switch to light mode" else "Switch to dark mode"
        button(
            class' = "p-2 rounded-lg bg-white/50 dark:bg-speakez-neutral hover:bg-white dark:hover:bg-speakez-neutral/80 transition-colors text-xl border border-speakez-neutral/10 dark:border-speakez-neutral-light/10",
            onClick = (fun _ -> props.onToggle()),
            title = titleText
        ) {
            Show(when' = props.isDark()) {
                span(class' = "select-none") { "\u2600\uFE0F" }
            }
            Show(when' = not (props.isDark())) {
                span(class' = "select-none") { "\uD83C\uDF19" }
            }
        }

/// Navigation header component
[<Erase>]
type NavHeader() =
    inherit header()

    [<Erase>]
    member val isDark: Accessor<bool> = Unchecked.defaultof<_> with get, set

    [<Erase>]
    member val onToggleTheme: (unit -> unit) = Unchecked.defaultof<_> with get, set

    [<SolidTypeComponent>]
    member props.constructor =
        header(class' = "py-6") {
            div(class' = Styles.container) {
                div(class' = "flex justify-between items-center") {
                    // Logo / site title
                    div(class' = "flex items-center gap-3") {
                        div(class' = "w-10 h-10 rounded-lg bg-gradient-to-br from-crqc-indigo to-speakez-teal flex items-center justify-center") {
                            span(class' = "text-white font-bold font-mono text-lg") { "Q" }
                        }
                        div() {
                            span(class' = "text-lg font-bold font-heading text-speakez-neutral dark:text-speakez-neutral-light") {
                                "CRQC Index"
                            }
                        }
                    }
                    // Nav controls
                    nav(class' = "flex items-center gap-4") {
                        span(class' = Styles.comingSoonBadge + " hidden sm:inline-flex") { "Coming Soon" }
                        ThemeToggle(isDark = props.isDark, onToggle = props.onToggleTheme)
                    }
                }
            }
        }

/// Z-estimate visual instrument - the central splash element
[<Erase>]
type ZEstimateInstrument() =
    inherit div()

    [<SolidTypeComponent>]
    member props.constructor =
        div(class' = Styles.card) {
            div(class' = Styles.zEstimateContainer) {
                // Subtitle above the Z value
                p(class' = "text-sm uppercase tracking-widest font-semibold text-crqc-indigo dark:text-crqc-indigo-light mb-6") {
                    "Estimated Time to CRQC"
                }

                // Z estimate display
                div(class' = "flex items-baseline gap-2 mb-2") {
                    span(class' = Styles.zValueDisplay + " text-crqc-indigo dark:text-crqc-indigo-light") {
                        "Z"
                    }
                }

                // Confidence interval visualization
                div(class' = "w-full max-w-lg mt-6 mb-4") {
                    div(class' = Styles.confidenceBar)
                    div(class' = "flex justify-between mt-2 text-xs font-mono text-speakez-neutral/50 dark:text-speakez-neutral-light/50") {
                        span() { "p10" }
                        span() { "p50" }
                        span() { "p90" }
                    }
                }

                // Per-primitive labels
                div(class' = "flex flex-wrap justify-center gap-3 mt-4") {
                    span(class' = Styles.tierBadge + " bg-crqc-critical/10 text-crqc-critical") { "ECC P-256" }
                    span(class' = Styles.tierBadge + " bg-crqc-warning/10 text-crqc-warning") { "RSA-2048" }
                    span(class' = Styles.tierBadge + " bg-crqc-indigo/10 text-crqc-indigo") { "ECC P-384" }
                    span(class' = Styles.tierBadge + " bg-crqc-safe/10 text-crqc-safe") { "AES-256" }
                }

                // Explanation
                p(class' = "mt-8 text-sm text-center max-w-lg " + Styles.bodyText) {
                    "The CRQC Index tracks convergence toward cryptographically relevant quantum computation across multiple cryptographic primitives, providing per-primitive estimates with confidence intervals."
                }
            }
        }

/// Section card describing a future site section
[<Erase>]
type SectionPreview() =
    inherit div()

    [<Erase>]
    member val title: string = "" with get, set

    [<Erase>]
    member val description: string = "" with get, set

    [<Erase>]
    member val icon: string = "" with get, set

    [<SolidTypeComponent>]
    member props.constructor =
        div(class' = Styles.card + " p-6 hover:shadow-xl transition-shadow") {
            div(class' = "flex items-start gap-4") {
                // Icon container
                div(class' = "w-10 h-10 rounded-lg bg-crqc-indigo/10 dark:bg-crqc-indigo-light/10 flex items-center justify-center flex-shrink-0") {
                    span(class' = "text-lg") { props.icon }
                }
                div() {
                    h3(class' = "font-semibold font-heading text-speakez-neutral dark:text-speakez-neutral-light mb-1") {
                        props.title
                    }
                    p(class' = "text-sm " + Styles.bodyText) {
                        props.description
                    }
                }
            }
        }

/// Mosca theorem brief explanation
[<Erase>]
type MoscaExplainer() =
    inherit div()

    [<SolidTypeComponent>]
    member props.constructor =
        div(class' = Styles.card + " p-8") {
            h2(class' = "text-2xl font-bold font-heading text-speakez-neutral dark:text-speakez-neutral-light mb-4") {
                "Mosca's Theorem"
            }
            div(class' = Styles.divider + " mb-6 mx-0")
            p(class' = Styles.bodyText + " mb-4") {
                "If X + Y > Z, your exposure window is already open. Data encrypted years ago is already at risk to harvest-now-decrypt-later (HNDL) collection."
            }
            div(class' = "flex flex-wrap gap-6 mt-6") {
                div(class' = "flex-1 min-w-[140px]") {
                    div(class' = "text-3xl font-bold font-mono text-crqc-indigo dark:text-crqc-indigo-light") { "X" }
                    p(class' = "text-sm " + Styles.bodyText) { "How long your data must stay secret" }
                }
                div(class' = "flex-1 min-w-[140px]") {
                    div(class' = "text-3xl font-bold font-mono text-speakez-teal") { "Y" }
                    p(class' = "text-sm " + Styles.bodyText) { "How long migration will take" }
                }
                div(class' = "flex-1 min-w-[140px]") {
                    div(class' = "text-3xl font-bold font-mono text-crqc-critical") { "Z" }
                    p(class' = "text-sm " + Styles.bodyText) { "How long until a CRQC arrives" }
                }
            }
        }
/// Site footer
[<Erase>]
type SiteFooter() =
    inherit footer()

    [<SolidTypeComponent>]
    member props.constructor =
        footer(class' = Styles.footer) {
            div(class' = "mb-4") {
                p(class' = "font-semibold text-speakez-neutral dark:text-speakez-neutral-light") {
                    "SpeakEZ Technologies\u2122"
                }
                p(class' = "text-sm text-speakez-neutral/70 dark:text-speakez-neutral-light/70 mt-1") {
                    "\u00A9 2026 SpeakEZ Technologies. All rights reserved."
                }
            }
            nav() {
                div(class' = "flex flex-wrap justify-center gap-4") {
                    a(href = "https://speakez.tech/company/tos", target = "_blank", class' = Styles.navLink) { "Terms" }
                    a(href = "https://speakez.tech/company/disclaimer", target = "_blank", class' = Styles.navLink) { "Disclaimer" }
                    a(href = "https://speakez.tech/company/privacy", target = "_blank", class' = Styles.navLink) { "Privacy" }
                    a(href = "https://speakez.tech/company/contact", target = "_blank", class' = Styles.navLink) { "Contact" }
                }
            }
        }

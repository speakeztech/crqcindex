namespace CRQCIndex.Site

/// Tailwind CSS class definitions for CRQC Index components
module Styles =

    /// Page background
    let pageBackground =
        "min-h-screen bg-speakez-neutral-light dark:bg-speakez-neutral-dark transition-colors"

    /// Container with responsive padding
    let container =
        "max-w-5xl mx-auto px-4 sm:px-6 lg:px-8"

    /// Main heading style
    let heading =
        "text-4xl sm:text-5xl lg:text-6xl font-bold font-heading text-speakez-neutral dark:text-speakez-neutral-light tracking-tight"

    /// Subheading style
    let subheading =
        "text-xl sm:text-2xl font-heading text-speakez-neutral/80 dark:text-speakez-neutral-light/80"

    /// Body text
    let bodyText =
        "text-base text-speakez-neutral/70 dark:text-speakez-neutral-light/70"

    /// Card component
    let card =
        "bg-white dark:bg-speakez-neutral rounded-lg shadow-lg overflow-hidden border border-speakez-neutral/10 dark:border-speakez-neutral-light/10"

    /// Z-estimate display - the central metric
    let zEstimateContainer =
        "relative flex flex-col items-center justify-center py-12 px-8"

    /// Z value large display
    let zValueDisplay =
        "text-7xl sm:text-8xl lg:text-9xl font-bold font-mono tabular-nums tracking-tighter"

    /// Z label
    let zLabel =
        "text-sm uppercase tracking-widest font-semibold text-crqc-slate dark:text-crqc-slate-light mt-2"

    /// Confidence interval bar
    let confidenceBar =
        "w-full max-w-md h-2 rounded-full bg-gradient-to-r from-crqc-safe via-crqc-warning to-crqc-critical opacity-80"

    /// Signal tier badge base
    let tierBadge =
        "inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"

    /// Navigation link
    let navLink =
        "text-sm text-speakez-neutral/70 dark:text-speakez-neutral-light/70 hover:text-speakez-teal dark:hover:text-speakez-teal transition-colors"

    /// Footer style
    let footer =
        "mt-16 pt-8 border-t border-speakez-neutral/20 dark:border-speakez-neutral-light/20 text-center"

    /// Accent divider
    let divider =
        "w-16 h-1 mx-auto rounded-full bg-gradient-to-r from-crqc-indigo to-speakez-teal"

    /// Coming soon badge
    let comingSoonBadge =
        "inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold bg-crqc-indigo/10 text-crqc-indigo dark:bg-crqc-indigo-light/10 dark:text-crqc-indigo-light"

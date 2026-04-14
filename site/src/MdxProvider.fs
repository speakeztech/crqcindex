namespace Partas.Solid.CRQCIndex

open Fable.Core
open Fable.Core.JsInterop
open Partas.Solid
open Partas.Solid.SolidMarked

/// MDX builtins for CRQC Index.
/// Referenced by vite-plugin-solid-marked via the @mdx alias in vite.config.js.
///
/// solid-marked compiles markdown → SolidJS components that call useMDX()
/// to look up the renderer for each node type. This module exports useMDX
/// with builtins styled in the site's Tailwind visual language.
///
/// Element creation uses emitJsExpr because the FablePlugin only transforms
/// Partas.Solid elements inside [<SolidTypeComponent>] members. Module-level
/// code that creates elements would reference erased builder types that don't
/// produce JS output. emitJsExpr generates the JSX at compile time from F#.
module MdxProvider =

    let private builtins =
        MDXBuiltinComponents(
            Root = (fun (props: ParentProps) ->
                emitJsExpr props """<div class="max-w-none">{$0.children}</div>"""
            ),
            Heading = (fun (props: HeadingProps) ->
                let baseClass = "font-heading text-speakez-neutral dark:text-speakez-neutral-light"
                let sizeClass =
                    match props.depth with
                    | 1 -> "text-3xl sm:text-4xl font-bold mt-8 mb-4"
                    | 2 -> "text-2xl font-bold mt-8 mb-3"
                    | 3 -> "text-xl font-semibold mt-6 mb-2"
                    | _ -> "text-lg font-semibold mt-4 mb-2"
                let cls = baseClass + " " + sizeClass
                match props.depth with
                | 1 -> emitJsExpr (cls, props) """<h1 id={$1.id} class={$0}>{$1.children}</h1>"""
                | 2 -> emitJsExpr (cls, props) """<h2 id={$1.id} class={$0}>{$1.children}</h2>"""
                | 3 -> emitJsExpr (cls, props) """<h3 id={$1.id} class={$0}>{$1.children}</h3>"""
                | 4 -> emitJsExpr (cls, props) """<h4 id={$1.id} class={$0}>{$1.children}</h4>"""
                | 5 -> emitJsExpr (cls, props) """<h5 id={$1.id} class={$0}>{$1.children}</h5>"""
                | _ -> emitJsExpr (cls, props) """<h6 id={$1.id} class={$0}>{$1.children}</h6>"""
            ),
            Paragraph = (fun (props: ParentProps) ->
                emitJsExpr props """<p class="text-base text-speakez-neutral/80 dark:text-speakez-neutral-light/80 leading-relaxed mb-4">{$0.children}</p>"""
            ),
            Blockquote = (fun (props: ParentProps) ->
                emitJsExpr props """<blockquote class="border-l-4 border-crqc-indigo/30 dark:border-crqc-indigo-light/30 pl-4 my-4 italic text-speakez-neutral/70 dark:text-speakez-neutral-light/70">{$0.children}</blockquote>"""
            ),
            Code = (fun (props: CodeProps) ->
                let langClass =
                    match props.lang with
                    | Some lang -> " language-" + lang
                    | None -> ""
                let cls = "font-mono text-sm text-speakez-neutral-light" + langClass
                emitJsExpr (cls, props) """<pre class="bg-speakez-neutral-dark rounded-lg p-4 my-4 overflow-x-auto"><code class={$0}>{$1.children}</code></pre>"""
            ),
            InlineCode = (fun (props: LiteralProps) ->
                emitJsExpr props """<code class="font-mono text-sm bg-speakez-neutral/10 dark:bg-speakez-neutral-light/10 px-1.5 py-0.5 rounded">{$0.children}</code>"""
            ),
            Link = (fun (props: LinkProps) ->
                emitJsExpr props """<a href={$0.url} title={$0.title} class="text-crqc-indigo dark:text-crqc-indigo-light hover:underline font-medium">{$0.children}</a>"""
            ),
            Image = (fun (props: ImageProps) ->
                emitJsExpr props """<img src={$0.url} alt={$0.alt} title={$0.title} class="rounded-lg my-4 max-w-full" />"""
            ),
            List = (fun (props: ListProps) ->
                let listClass = "list-inside space-y-1 mb-4 text-speakez-neutral/80 dark:text-speakez-neutral-light/80"
                if props.ordered then
                    emitJsExpr (listClass, props) """<ol class={"list-decimal " + $0} start={$1.start}>{$1.children}</ol>"""
                else
                    emitJsExpr (listClass, props) """<ul class={"list-disc " + $0}>{$1.children}</ul>"""
            ),
            ListItem = (fun (props: ListItemProps) ->
                emitJsExpr props """<li class="leading-relaxed">{$0.children}</li>"""
            ),
            ThematicBreak = (fun _ ->
                emitJsExpr () """<hr class="my-8 border-t border-speakez-neutral/20 dark:border-speakez-neutral-light/20" />"""
            ),
            Strong = (fun (props: ParentProps) ->
                emitJsExpr props """<strong class="font-semibold text-speakez-neutral dark:text-speakez-neutral-light">{$0.children}</strong>"""
            ),
            Emphasis = (fun (props: ParentProps) ->
                emitJsExpr props """<em>{$0.children}</em>"""
            ),
            Delete = (fun (props: ParentProps) ->
                emitJsExpr props """<del class="text-speakez-neutral/50 dark:text-speakez-neutral-light/50">{$0.children}</del>"""
            ),
            Table = (fun (props: TableProps) ->
                emitJsExpr props """<div class="overflow-x-auto my-4"><table class="min-w-full border border-speakez-neutral/10 dark:border-speakez-neutral-light/10 rounded-lg">{$0.children}</table></div>"""
            ),
            TableHead = (fun (props: ParentProps) ->
                emitJsExpr props """<thead class="bg-speakez-neutral/5 dark:bg-speakez-neutral-light/5">{$0.children}</thead>"""
            ),
            TableBody = (fun (props: ParentProps) ->
                emitJsExpr props """<tbody>{$0.children}</tbody>"""
            ),
            TableRow = (fun (props: ParentProps) ->
                emitJsExpr props """<tr class="border-b border-speakez-neutral/10 dark:border-speakez-neutral-light/10">{$0.children}</tr>"""
            ),
            TableCell = (fun (props: TableCellProps) ->
                emitJsExpr props """<td class="px-4 py-2 text-sm">{$0.children}</td>"""
            ),
            Break = (fun _ ->
                emitJsExpr () """<br />"""
            )
        )

    /// Called by solid-marked compiled components to look up renderers
    let useMDX () =
        MDXProps(builtins = builtins)

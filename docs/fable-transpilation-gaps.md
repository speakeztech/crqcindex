---
title: "Fable/FablePlugin Transpilation Gaps — Lessons from CRQC Index"
description: "Documented gaps between F# intent and JS output when using Fable + Partas.Solid FablePlugin. Reference for JSIR migration planning."
date: 2026-04-13
authors: ["Houston Haynes"]
status: "Reference"
tags: ["Fable", "Partas.Solid", "JSIR", "transpilation", "architecture"]
---

## Context

CRQC Index is built with Fable (F# → JS) and Partas.Solid (F# DSL for SolidJS). During the router and meta tag migration (April 2026), several gaps surfaced between what F# expresses and what the transpilation pipeline produces. These are documented here as reference for future JSIR tooling decisions.

## Gap 1: Builder Pattern Extension Methods Not Erased

**What happened:** The `@solidjs/router` Router uses F#'s computation expression builder pattern. The `Run` method (which finalizes the builder) was defined as an extension method on a separate `Extensions` type. The FablePlugin did not erase it — the compiled JS referenced a non-exported function.

**Root cause:** The FablePlugin's `ImportedContainerExtensionName` pattern matcher checks import selectors against hardcoded prefixes (`HtmlContainerExtensions_`, `BindingsModule_Extensions`). The Router bindings' compiled selector (`Bindings_Extensions_Run_...`) didn't match either prefix.

**Fix applied:** Added `Bindings_Extensions` to the selector pattern in `Spec.fs`. The `Run` extension method is now matched and erased correctly.

**Lesson:** Erased extension methods are fragile — they depend on compiled name prefixes that change with module/namespace structure. A JSIR compiler should erase by attribute/semantic, not by string prefix matching.

## Gap 2: splitProps Breaks SolidJS Children Reactivity

**What happened:** When a Partas.Solid component receives children (e.g., route content from `@solidjs/router`'s `root` prop), the FablePlugin generates `splitProps(props, ["children"])`. The extracted `PARTAS_LOCAL.children` is a const member access that the SolidJS compiler treats as static. Route transitions never update the content area.

**Root cause:** SolidJS's JSX compiler has special handling for `props.children` — it recognizes the `props` identifier and tracks reactivity. But `PARTAS_LOCAL.children` (from splitProps) is a const member access that the Solid compiler treats as a one-time read.

**Workaround applied:** `Interop.propsChildren` uses `[<Emit("$0.children")>]` to access `props.children` directly, bypassing splitProps. The children are then called as a function (`routeContent()`) inside the builder, which the Solid compiler wraps in a reactive effect.

**Lesson:** Intermediary destructuring (splitProps) breaks framework-specific compiler heuristics. A JSIR compiler should understand the target framework's reactivity model and preserve the idioms the framework compiler expects (e.g., `props.x` not `destructured.x`).

## Gap 3: FablePlugin Inlines Closures Passed as Component Props

**What happened:** Local closures like `let toggleSidebar () = setSidebarOpen (not (sidebarOpen()))` passed as props to custom components (`HamburgerIcon(onToggle = toggleSidebar)`) were inlined by the FablePlugin. The compiled output executed `setSidebarOpen(!sidebarOpen())` during render instead of emitting `() => setSidebarOpen(!sidebarOpen())`.

**Root cause:** The FablePlugin inlines simple function bodies when they're passed as prop expressions to SolidTypeComponents. It doesn't distinguish between value props and callback props. DOM event handlers (`onClick`) are handled separately and preserve the function wrapper.

**Workaround applied:** `Interop.callback` uses `[<Emit("$0")>]` as an identity function that prevents the plugin from seeing through the wrapper.

**Lesson:** A JSIR compiler must distinguish between "evaluate this expression now" and "pass this function to be called later." The distinction is semantic (callback vs. value), not syntactic. TypeScript's type system captures this via function types in interfaces; F#'s type system has the same information but the plugin ignores it.

## Gap 4: Element Creation Outside SolidTypeComponent Produces Dead References

**What happened:** MDX builtins defined as module-level functions (not SolidTypeComponents) used Partas.Solid element constructors (`div()`, `h1()`, etc.). The compiled output imported from erased builder types (`Tags.fs.jsx`, `HtmlAttributes.fs.jsx`) that don't produce JS output.

**Root cause:** The FablePlugin transforms element creation into JSX only inside `[<SolidTypeComponent>]` members. Module-level code that creates elements compiles to builder function calls referencing erased types.

**Workaround applied:** MDX builtins use `emitJsExpr` to emit JSX directly. The source is F#; the JSX is generated at compile time by Fable.

**Lesson:** If a type is erased, ALL uses of that type must be transformed — not just uses within a specific context. A JSIR compiler should track erased types globally and transform every creation site, regardless of whether it's inside a component or a module function.

## Gap 5: Reactive Params Require Derived Functions, Not Let Bindings

**What happened:** Route params from `useParams()` captured in `let` bindings (`let section = params.section`) were read once at component creation. When the Router reused the component with different params (same route pattern, different values), the content didn't update.

**Root cause:** SolidJS components run once — reactivity comes from signals read inside tracking scopes (JSX expressions, effects, memos). `let` bindings in Fable compile to `const` in JS, which the Solid compiler treats as static.

**Fix applied:** All param-derived values are defined as functions (`let section () = params.section`), called inside builder expressions where the Solid compiler wraps them in reactive effects. `createMemo` was tried but caused disposal crashes during route transitions.

**Lesson:** A JSIR compiler targeting a reactive framework must understand the framework's tracking model. Values derived from reactive sources need to be emitted as getter functions or accessors, not as const bindings. The compiler should infer this from the type system (signal reads should produce reactive derivations).

## Gap 6: Conditional Rendering Position Matters

**What happened:** `if/else` at the SolidTypeComponent constructor level compiled to a JS `if/else` (static branch). The same `if/else` inside a builder compiled to a JSX ternary (reactive, tracked by the Solid compiler).

**Root cause:** The FablePlugin transforms builder content into JSX but doesn't transform top-level constructor logic. A top-level `if isNull content then <NotFound/> else <Article/>` runs once at component creation — the branch is chosen permanently.

**Fix applied:** Moved all conditionals inside the builder so they compile to JSX ternaries.

**Lesson:** In reactive frameworks, conditional rendering must be in the reactive layer (JSX), not the imperative layer (component body). A JSIR compiler should understand which expressions are "setup" (run once) vs. "render" (tracked reactively) and ensure conditional rendering lands in the render path.

## Upstream Fixes Applied to Partas.Solid (Local)

These were applied to the local Partas.Solid repo at `/home/hhh/repos/Partas.Solid`:

| Fix | File | Description |
|-----|------|-------------|
| MetaHTMLAttributes | HtmlAttributes.fs | `name`, `content`, `charset`, `httpEquiv`, `media` moved from MenuHTMLAttributes to MetaHTMLAttributes |
| Router Hooks rename | SolidRouterBindings.fs | `type Bindings` renamed to `type Hooks` to avoid collision with `module Bindings` |
| Run selector pattern | Spec.fs (FablePlugin) | Added `Bindings_Extensions` to ImportedContainerExtensionName pattern |
| FSharp.Core pin | FablePlugin.fsproj | Explicit FSharp.Core 9.0.303 + DisableImplicitFSharpCoreReference for .NET 10 SDK compatibility |

## Summary for JSIR Planning

The common thread: **the transpilation pipeline doesn't understand the target framework's runtime semantics.** Fable translates F# syntax to JS syntax. The FablePlugin adds SolidJS-specific transforms. But neither understands SolidJS's reactivity model, disposal lifecycle, or compiler heuristics at a deep level.

A JSIR compiler that targets SolidJS (or any reactive framework) needs to:

1. **Track reactive data flow** — know which values are signals, which are derived, and emit the right idioms (getters, not consts)
2. **Respect framework compiler expectations** — preserve `props.x` patterns the framework compiler recognizes as reactive
3. **Erase by semantics, not by name** — erased types should be transformed everywhere, not just in tagged contexts
4. **Distinguish callbacks from values** — function-typed props that are callbacks must preserve the function wrapper
5. **Understand component lifecycle** — reactive computations that fire during disposal corrupt the DOM; the compiler should prevent this structurally

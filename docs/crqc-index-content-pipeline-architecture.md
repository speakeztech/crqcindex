---
title: "CRQC Index: Content Pipeline & Agent Execution Architecture"
description: "Realized architecture for the SPEC stack content pipeline, Cloudflare Sandbox integration, and agentic content lifecycle"
date: 2026-04-13
authors: ["Houston Haynes"]
status: active
tags: ["architecture", "SPEC", "Cloudflare", "Sandboxes", "solid-marked", "content-pipeline"]
supersedes: []
related: ["crqc-index-design-scaffold.md", "crqc-index-methodology-primer.md"]
---

## Overview

This document captures the realized architecture for the CRQC Index content pipeline as implemented through the SPEC stack (SolidJS, Partas.Solid, Elmish, Fidelity.CloudEdge). It supersedes implicit assumptions in the Design Scaffold about content generation and publication, and introduces Cloudflare Sandboxes as the agent execution environment for Conclave actors.

The architecture serves dual purpose: CRQC Index is the first consumer of patterns that will generalize into the Victor CLI tool for all SpeakEZ sites.

## Architecture Decision Record

### ADR-1: Partas.Solid as the rendering layer (not Hugo, not Astro)

**Context:** The site needed a content rendering pipeline. Hugo was the existing pattern from SpeakEZ and clef-lang-site. Cloudflare's EmDash (Astro-based) was evaluated.

**Decision:** Partas.Solid with solid-marked for MDX compilation. Content is authored as markdown, compiled to SolidJS components at build time via Vite, and rendered through customizable builtin components.

**Rationale:**
- EmDash is tightly coupled to Astro and uses Portable Text, not markdown — poor fit for agent-produced content
- Hugo would require maintaining a separate Go-template ecosystem alongside the F#/SolidJS stack
- solid-marked compiles markdown to native SolidJS component trees with no `Dynamic` indirection, no virtual DOM, no `innerHTML` — pure signals-based direct DOM
- MDX enables embedding interactive SolidJS components directly in markdown for future "literate compute" pages
- The `useMDX` provider pattern gives per-site visual identity through F#-defined builtin renderers

**Consequences:** Content must be available at build time (or fetched and compiled in a Sandbox). Runtime content delivery from R2/Workers requires a separate rendering path (pre-compiled JSON or SSR).

### ADR-2: Cloudflare Sandboxes for agent execution (not Workers alone)

**Context:** The Design Scaffold assumed Conclave actors (Prospero, Sentinel, Analyst, Librarian) would run as Workers and Durable Objects. Cloudflare Sandboxes reached GA on April 13, 2026, providing full compute environments with secure credential injection.

**Decision:** Conclave actors that perform engineering work (content generation, build verification, git operations) execute in Sandboxes. Workers remain for stateless request handling (Content API, search, notification webhooks). Durable Objects remain for stateful coordination (Prospero's Z estimate, Librarian's corpus).

**Rationale:**
- Sandboxes provide terminal, filesystem, git, interpreters — the full loop agents need: clone → analyze → write → build → verify → push
- Secure credential injection via egress proxy means agents never see tokens for Forgejo, APIs, or notification services
- Dynamic egress lockdown enables the security posture the Design Scaffold requires: open network for clone/push, locked down during analysis
- Live preview URLs allow agents to verify rendered output before committing
- Sleep-on-idle, wake-on-demand economics align with signal-driven activation (no cost between signals)

**Consequences:**
- Fidelity.CloudEdge needs Sandbox management API bindings (see Infrastructure Updates section)
- BAREWire-encoded LARP messages between actors may need adaptation for Sandbox ↔ Worker communication boundaries
- The approval workflow's trust boundary (human review before publication) is strengthened: Sandbox can build and verify but cannot deploy without approval

### ADR-3: Fidelity.CloudEdge runtime bindings standardized on Xantham (May 2026)

**Context:** The Conclave actor constellation (Prospero, Sentinels, Analyst, Librarian, Approval Gate) requires the full Cloudflare runtime SDK surface — not a subset. Workers types (Durable Objects, KV, R2, D1, Email, Vectorize, Container, DurableObjectFacets, WorkerLoader), `agents-sdk` (Agent base class, RPC, callable decorator), `dynamic-workflows` (workflow orchestration for the approval pipeline), and Sandboxes (where Conclave actors execute) all need to compose with consistent type identity at the cross-reference boundaries. Mixed-tooling generation (Glutinum for some, Xantham for others) breaks those boundaries because the two generators emit different module paths and different type shapes for the same TypeScript declarations.

**Decision:** Fidelity.CloudEdge has standardized its TypeScript→F# binding generation on Xantham. See [Fidelity.CloudEdge/docs/00 Decision 7](../../Fidelity.CloudEdge/docs/00_architecture_decisions.md) for the formal record and rationale. CRQC Index is the validation target for that decision: the workers buildout in `workers/` (currently `.gitkeep` only) is the natural compile-end-to-end surface for the Xantham-generated binding stack.

**Rationale:**
- CRQC Index needs all four runtime surfaces (workers-types, agents-sdk, dynamic-workflows, Sandboxes-pending). There is no partial-application path: Conclave's actor model + approval pipeline + content sandbox execution all have to compose.
- Xantham's hierarchical module tree (from npm package structure) and its multi-file type-graph crawler keep cross-package references resolvable when all bindings come from the same generator. Glutinum's flat-with-post-processing output would force per-package shim layers at every boundary.
- The `workers/` folder is empty as of April 2026 — there is no legacy F# worker code locked into a specific binding shape. Xantham bindings can land directly without a migration step on the CRQC Index side.

**Consequences:**
- The `Fidelity.CloudEdge.Agents` and `Fidelity.CloudEdge.DynamicWorkflows` packages currently ship as hand-curated `Types.fs` (Glutinum crashed on these surfaces; Xantham requires four bug fixes that are tracked in [Fidelity.CloudEdge/docs/06 §"Xantham: Capabilities, Architecture, and Tracked Issues"](../../Fidelity.CloudEdge/docs/06_tool_status.md)). The hand-curated bindings work today and are sufficient for the workers buildout to begin.
- The Xantham migration is sequenced through phases A-F per [Fidelity.CloudEdge/docs/12 §9 "May 2026 Verified State and Roadmap"](../../Fidelity.CloudEdge/docs/12_xantham_glutinum_replacement_assessment.md). CRQC Index workers compile against the hand-curated bindings until the Xantham-generated equivalents land in Phase E; the binding-side change is largely transparent to worker code that consumes the published interface.
- The Sandboxes binding (see Infrastructure Updates section below) will be Xantham-generated from day one, not added as a new Glutinum binding.

### ADR-4: Git (Forgejo via Tunnel) as source of truth, not R2

**Context:** Earlier designs considered R2 as the working store with git as publication record.

**Decision:** Git is the source of truth for published content. Sandboxes interact with git directly. R2 stores media assets, build artifacts, and cached rendered output — not canonical content.

**Rationale:**
- Sandboxes have native git support with credential injection via egress proxy
- Forgejo (self-hosted via Cloudflare Tunnel) provides full git history, branch-based workflows, and webhook triggers without Microsoft dependency
- The site's `site/content/` directory IS the git-managed content store, same as Hugo's model
- Agents create draft branches; human approval merges to main; Pages deploy triggers on merge

**Consequences:** Bidirectional sync between R2 and git is unnecessary. R2 becomes a cache/CDN layer, not a content authority.

## Content Lifecycle

```
Signal arrives
    │
    ▼
Sandbox wakes (zero cost until triggered)
    │  ┌─ Egress: Forgejo tunnel host only
    │  ├─ Credentials: injected via proxy, never visible to agent
    │  └─ Compute: full terminal, F#, Node.js, git
    │
    ├── git clone content repo (from Forgejo via Tunnel)
    ├── Sentinel: ingest signal, extract entities, classify
    ├── Analyst: score signal, compute Z delta, draft markdown
    │     └── writes site/content/{section}/{slug}.md
    │         with extended frontmatter (status, provenance, source_signals)
    ├── Librarian: deduplicate against corpus, verify citations
    ├── dotnet fable && vite build (verify content compiles)
    ├── Preview URL: agent verifies rendered output
    ├── git commit to draft branch with provenance metadata
    ├── git push to Forgejo
    │     └── Egress locked to tunnel host + notification webhook only
    ├── Webhook → Notification Worker → alert Houston
    │
    ▼
Sandbox sleeps
    │
    ▼
Human review (Houston)
    ├── Review draft branch in Forgejo
    ├── Check preview, edit if needed
    ├── Merge to main
    │
    ▼
Forgejo webhook → Pages deploy
    ├── Fable compile → solid-marked compile → Vite build
    ├── Deploy to Cloudflare Pages
    ├── Search indexing triggered (D1 FTS5 + Vectorize)
    │
    ▼
Published
```

### Content Directory Structure

```
site/content/
├── bulletins/           # Short-form signal reports
│   └── 2026-04-12-nist-boulder-fiber.md
├── analysis/            # Long-form synthesis
│   └── convergent-quantum-conundrum.md
├── methodology/         # Scoring rubric, Z model, taxonomy
│   └── z-estimation-model.md
└── fud-files/           # Excluded signal analysis
    └── quantum-supremacy-marketing.md
```

### Extended Frontmatter Schema

```yaml
---
title: "Document Title"
status: draft | review | approved | published
author: winterthunder              # human author or agent ID
agent: analyst-v2                   # Conclave actor that produced this
date: 2026-04-12
reviewed_by: winterthunder
reviewed_at: 2026-04-13T02:30:00Z
source_signals:
  - id: sig-nist-boulder-2026-04
    confidence: 0.87
    url: https://...
provenance:
  chain: [sentinel-arxiv, analyst, librarian]
  sandbox_id: sb-a1b2c3d4
  preview_verified: true
  build_hash: abc123
tags: [quantum, fiber, CRQC]
---
```

## Rendering Pipeline

### Build-Time Flow

```
site/content/*.md
    │
    ▼
vite-plugin-solid-marked (build time)
    ├── Parses frontmatter (YAML/TOML)
    ├── Compiles markdown → SolidJS component via micromark/mdast
    ├── noDynamicComponents: true (no Dynamic wrapper)
    ├── Resolves useMDX() from mdx-provider.jsx
    │
    ▼
SolidJS component tree
    ├── Each markdown node → builtin component function
    ├── Heading, Paragraph, Code, Link, List, etc.
    ├── Styled per CRQC Index visual identity
    ├── MDX: embedded SolidJS components for interactivity
    │
    ▼
Vite bundle
    ├── Pre-compiled components (no runtime parsing)
    ├── Content index built via import.meta.glob
    └── Hash-based routing for SPA-like navigation
```

### MDX Provider Architecture

The `useMDX()` hook supplies builtin renderers for every markdown node type. This is the site's visual identity layer — each site (CRQC Index, SpeakEZ, clef-lang) defines its own builtins with its own typography, colors, and interactive components.

Current implementation: `site/src/mdx-provider.jsx` (JSX for Vite plugin compatibility)
Future: F# via `Partas.Solid.SolidMarked` bindings when the provider can be compiled through Fable

### Partas.Solid Binding Packages

Created on the `ark-ui-bindings` branch of `speakeztech/Partas.Solid`:

| Package | npm Source | Components | Purpose |
|---------|-----------|------------|---------|
| Partas.Solid.ArkUI | @ark-ui/solid | Splitter, Tabs, TreeView, Collapsible, Menu | Layout scaffolding, resizable panels, hierarchical navigation |
| Partas.Solid.SolidMarked | solid-marked | MDXProvider, useMDX, MDXBuiltinComponents | MDX/Markdown content rendering with customizable builtins |

These are PR-able independently to upstream (shayanhabibi/Partas.Solid). The TanStack.Store binding (PR #44, draft) is also on the local fork.

## Infrastructure Updates Required

### Fidelity.CloudEdge: Sandbox API Bindings

**Current state:** Fidelity.CloudEdge covers 32 Cloudflare management services and 727 runtime types. No Sandbox compute API exists in the package. The existing "Containers" service covers the Container Registry (image management), not Sandbox execution.

**Required additions:**

1. **CloudEdge.Runtime — Container/Sandbox types** (available now in `workers-types 4.20260413.1`):
   - `Container.interceptOutboundHttp(addr, binding)` — per-address egress proxy for credential injection
   - `Container.interceptAllOutboundHttp(binding)` — global egress proxy for policy enforcement
   - `Container.interceptOutboundHttps(addr, binding)` — HTTPS-specific secure credential injection
   - `Container.snapshotDirectory()` / `Container.snapshotContainer()` — state snapshots for warm rotation
   - `ContainerStartupOptions.enableInternet` — internet access toggle for isolation
   - `ContainerStartupOptions.directorySnapshots` — restore snapshots on startup
   - `WorkerLoaderStartupOptions.globalOutbound` — egress control for replica Oliviers
   - `WorkerLoaderStartupOptions.streamingTails` — streaming actor diagnostics

2. **CloudEdge.Runtime — DurableObjectFacets** (available now):
   - `DurableObjectFacets` — dynamic DO instantiation from within a DO (`get()`, `abort()`, `delete()`)
   - `FacetStartupOptions<T>` — configuration for facet creation (id, class)
   - Direct actor model integration: Prospero can spawn Olivier workers as Facets with isolated SQLite
   - Maps to ChildSpec in the Conclave supervision tree

3. **CloudEdge.Agents — agents-sdk binding** (0.3.0, hand-curated):
   - `Agent<'Env, 'State>` interface, `Schedule<'T>`, `ScheduleKind`, `RPCRequest`, `RPCResponse` DU, `CallableAttribute`, `AgentNamespace`, `Routing` module
   - Maps directly to Conclave actor roles: Sentinel/Analyst/Librarian/Approval Gate all consume this interface
   - Currently hand-curated because Glutinum crashed on `agents-sdk`. Xantham-generated equivalent lands in Phase E of the Xantham migration ([Fidelity.CloudEdge/docs/12 §9](../../Fidelity.CloudEdge/docs/12_xantham_glutinum_replacement_assessment.md)). The published interface is stable; consumer code is unaffected by the binding-source switch.

4. **CloudEdge.DynamicWorkflows — dynamic-workflows binding** (0.3.0, hand-curated):
   - `WorkflowEventLike<'T>`, `WorkflowRunner<'T,'R>`, `LoadWorkflowRunnerContext<'Env>`, `DynamicWorkflowBinding`, `MissingDispatcherMetadataError`, `Api` module
   - Required for the Approval Gate pipeline: Sentinel scheduling, retroactive review windows, escalated investigation holds
   - Same migration story as agents-sdk: hand-curated today, Xantham-generated in Phase E

5. **CloudEdge.Management — Sandbox orchestration** (pending; will be Xantham-generated):
   - No dedicated Sandbox management paths in the public OpenAPI spec as of GA day
   - The Sandbox product is a composition of existing primitives, not a new service: DurableObjectFacets for dynamic DO instantiation + Container egress interception for the proxy layer
   - Management operations (create/wake/sleep/destroy sandbox instances) will likely land as extensions to existing Container or DO management paths
   - When the Cloudflare Sandboxes API surface formalizes, the binding will be Xantham-generated from day one (per ADR-3) — tracked as G7 in the Fidelity.CloudEdge gap analysis. Provision via dashboard or wrangler in the interim.

**Status (May 2026):** Fidelity.CloudEdge 0.3.0 released.
- Runtime: `workers-types 4.20260413.1`. All Container, DurableObjectFacets, and egress interception types landed.
- Runtime additions: `Fidelity.CloudEdge.Agents` (agents-sdk) and `Fidelity.CloudEdge.DynamicWorkflows` shipped as hand-curated `Types.fs`.
- Management: 121 new paths (AI model endpoints, AI Search, Browser Rendering). 32 services covered.
- Architectural decision: TypeScript→F# binding generation standardized on Xantham (per ADR-3). The hand-curated Agents and DynamicWorkflows bindings are bridge artifacts; durable form is Xantham-generated output once renderer bug fixes land.
- The runtime types ARE the Sandbox building blocks. Conclave can begin using `DurableObjectFacets.get()` to spawn agent instances and `Container.interceptOutboundHttp()` for egress proxy credential injection today.

### Design Scaffold Reconciliation

The following assumptions in `crqc-index-design-scaffold.md` require updates:

1. **Agent execution model** (Section: Conclave Architecture)
   - **Was:** All actors as Workers/Durable Objects
   - **Now:** Engineering actors (Sentinel scraping, Analyst writing, Librarian verifying) execute in Containers/Sandboxes. Coordination actors (Prospero) remain as Durable Objects and can now spawn Olivier workers as **DurableObjectFacets** — native actor model with isolated SQLite per facet. This maps directly to ChildSpec in the supervision tree. Stateless serving (Content API, search) remains as Workers.

2. **Actor supervision** (Section: Conclave Architecture — new consideration)
   - **Was:** Implicit supervision via Durable Object alarm scheduling
   - **Now:** `DurableObjectFacets` provides `get()`, `abort()`, `delete()` — a native supervision primitive. Prospero as a parent DO can instantiate, monitor, and terminate child Facets. Container snapshots (`snapshotDirectory()`, `snapshotContainer()`) enable warm restart semantics for stateful actors.

3. **Publication pipeline** (Section: Content Publishing)
   - **Was:** Atomic pipeline within Workers (write D1 → generate Bulletin → recalculate Z → notifications)
   - **Now:** Container/Sandbox produces content + pushes to git → human approves → merge triggers build + deploy + search indexing. Z recalculation may still occur in a Durable Object (Prospero) triggered by the merge webhook.

4. **Inter-actor communication** (Section: Signal Flow)
   - **Was:** BAREWire-encoded LARP messages between Durable Objects
   - **Now:** Container ↔ Worker communication via `interceptOutboundHttp` egress proxy to coordination Workers. DurableObjectFacets enable intra-DO actor spawning without external messaging. BAREWire/LARP may apply for structured data exchange through the egress proxy layer.

5. **Content storage** (implicit)
   - **Was:** D1 as content store, R2 for archives
   - **Now:** Git (Forgejo) as content source of truth. D1 for search index, provenance registry, and Z estimate history. R2 for media assets and cached build artifacts. Container snapshots for actor working state.

### Methodology Primer Reconciliation

The GNN/embedding architecture in `crqc-index-methodology-primer.md` is largely unaffected — it describes the analytical engine, not the content pipeline. However:

1. **Embedding archive permanence** — If embeddings are computed in Sandboxes, the archive must persist beyond Sandbox lifecycle. D1 or R2 remains the durable store; Sandbox is ephemeral compute.

2. **R-GCN inference location** — Current design runs R-GCN in Workers via WASM/SIMD. Sandboxes offer an alternative execution environment with full CPU access. Decision: keep inference in Workers for latency (always-on edge), use Sandboxes for training/retraining (batch, ephemeral).

## Search Infrastructure

The search pipeline follows the proven pattern from SpeakEZ and clef-lang-site:

- **CLI indexing:** Split markdown at H2 boundaries, strip formatting, POST sections to search Worker
- **D1 FTS5:** BM25 full-text search with weighted columns (title=10, section=5, content=1)
- **Vectorize:** Semantic embeddings via Workers AI (bge-base-en-v1.5)
- **Hybrid retrieval:** Reciprocal Rank Fusion (RRF k=60) combining BM25 and vector results
- **AI synthesis:** Workers AI generates contextual summaries from ranked results
- **Frontend:** Partas.Solid search modal component (replacing vanilla JS from Hugo sites)

Search indexing triggers on content publication (merge to main → build → index).

## Security Model

### Sandbox Egress Policy

```
Phase 1 (Setup):
  allowed: [forgejo.internal (via Tunnel), npmjs.org]
  credentials: Forgejo token injected via proxy

Phase 2 (Analysis):
  allowed: [] (no egress — agent processes data in isolation)

Phase 3 (Push):
  allowed: [forgejo.internal (via Tunnel), notification-webhook.workers.dev]
  credentials: Forgejo token + webhook secret injected via proxy
```

### Trust Boundaries

1. **Sandbox → Git:** Agent can push to draft branches only. Merge to main requires human approval.
2. **Sandbox → Deployment:** No direct path. Deployment triggers only on merge to main via webhook.
3. **Sandbox → Credentials:** Never visible to agent code. Injected at the egress proxy layer.
4. **Content → Publication:** Every document passes through the approval workflow defined in the Design Scaffold. Sandboxes do not bypass the Approval Gate.

## Roadmap

### Immediate (realized)
- [x] Partas.Solid.ArkUI bindings (Splitter, Tabs, TreeView, Collapsible, Menu)
- [x] Partas.Solid.SolidMarked bindings (MDXProvider, useMDX, builtins)
- [x] solid-marked content pipeline (build-time MDX compilation, zero Dynamic)
- [x] Collapsible sidebar with animated hamburger (icons-only collapsed mode, mobile overlay)
- [x] Hash-based routing with reactive content switching
- [x] Content directory structure with extended frontmatter schema

### Near-term
- [x] Fidelity.CloudEdge runtime bindings regenerated (workers-types 4.20260413.1 — DurableObjectFacets, Container egress, snapshots)
- [x] Fidelity.CloudEdge.Agents (agents-sdk) hand-curated binding shipped in 0.3.0
- [x] Fidelity.CloudEdge.DynamicWorkflows hand-curated binding shipped in 0.3.0
- [ ] Xantham migration Phases A-D (Fidelity.CloudEdge-side; bug fixes upstream + per-surface validation). Tracked in [Fidelity.CloudEdge/docs/12 §9.5](../../Fidelity.CloudEdge/docs/12_xantham_glutinum_replacement_assessment.md). Transparent to CRQC Index workers
- [ ] Fidelity.CloudEdge management API for Sandbox orchestration (pending OpenAPI spec publication)
- [ ] Search Worker deployment (D1 FTS5 + Vectorize, proven pattern)
- [ ] Forgejo instance provisioned via Cloudflare Tunnel
- [ ] Content Worker for R2-cached pre-rendered content (runtime delivery path)
- [ ] @solidjs/router migration (replace hash routing with proper client-side router)

### Medium-term
- [ ] Xantham migration Phase E: cross-link validation. Compile a representative CRQC Index worker against Xantham-generated workers-types + agents-sdk + dynamic-workflows. CRQC Index is the validation target for [Fidelity.CloudEdge/docs/00 Decision 7](../../Fidelity.CloudEdge/docs/00_architecture_decisions.md).
- [ ] G7: Cloudflare Sandboxes binding (Xantham-generated, per ADR-3). Required before Sandbox-resident Conclave actors can be implemented
- [ ] Conclave Sandbox orchestration (Prospero triggers Sandbox wake via webhook)
- [ ] Sentinel agent implementation (signal source scraping in Sandbox)
- [ ] Analyst agent implementation (scoring + content generation in Sandbox)
- [ ] Victor CLI extraction (generalize CRQC Index patterns into reusable dotnet tool)
- [ ] MDX provider migration to F# (via Partas.Solid.SolidMarked bindings)

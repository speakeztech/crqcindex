# CRQC Index

Monitoring convergence toward cryptographically relevant quantum computation.

The CRQC Index is a signal detection and analysis system that tracks progress toward cryptographically relevant quantum computers (CRQCs) across multiple cryptographic primitives. It provides per-primitive Z estimates (time to CRQC) with confidence intervals, helping organizations understand their exposure window under Mosca's theorem.

## A Conclave Project

The CRQC Index is built on [Conclave](https://speakez.tech/portfolio/conclave/), SpeakEZ's managed platform for agentic AI systems on Cloudflare's global edge. The system implements a multi-agent architecture where specialized actors handle distinct responsibilities:

- **Prospero**: Coordinator Durable Object that owns the canonical Z estimate
- **Sentinel agents**: Source-specific scrapers (arXiv, patents, government advisories, vendor announcements)
- **Analyst agent**: Signal scorer and classifier, computes Z deltas per primitive
- **Librarian agent**: Deduplication, citation graph maintenance, provenance tracking

These actors communicate via Conclave's tell-first messaging pattern, with capability-based security enforced at the platform level through Fidelity.CloudEdge bindings and Cloudflare's Worker Loader API.

## Project Structure

```
crqcindex/
├── site/           Partas.Solid frontend (F# + SolidJS via Fable)
├── workers/        Cloudflare Workers (F# via Fable) - Conclave agents
├── cli/            F# CLI for Cloudflare deployment via Fidelity.CloudEdge
├── scripts/        Shell wrappers for CLI commands
└── docs/           Design documents and methodology
```

## Tech Stack

- **Frontend**: F# with [Partas.Solid](https://github.com/parta-solid/Partas.Solid) (SolidJS bindings), Vite, Tailwind CSS
- **Backend**: Conclave actor agents on Cloudflare Workers + Durable Objects
- **CLI**: .NET 10.0 F# with [Fidelity.CloudEdge.Management](https://www.nuget.org/packages/Fidelity.CloudEdge.Management)
- **Deployment**: Cloudflare Pages + Workers, no Wrangler

## Prerequisites

- .NET 10.0 SDK
- Node.js 18+
- Cloudflare account with API token

## Getting Started

```bash
# Restore .NET tools (Fable)
dotnet tool restore

# Install npm dependencies
cd site && npm install && cd ..

# Development server
cd site && npm run dev:watch

# Build for production
cd site && npm run build
```

## Deployment

```bash
# Set environment variables
export CLOUDFLARE_API_TOKEN="your-token"
export CLOUDFLARE_ACCOUNT_ID="your-account-id"

# Full migration
./scripts/migrate.sh -v

# Or individual steps
./scripts/provision.sh -v
./scripts/deploy-pages.sh -v
./scripts/status.sh
```

## Site Sections (Planned)

- **Splash/Landing**: Visual Z-estimate instrument with per-primitive confidence intervals
- **Bulletins**: Short-form signal feed (hardware milestones, error correction, cryptanalysis)
- **Analysis**: Long-form synthesis connecting patterns across signals
- **Methodology**: Scoring rubric, signal taxonomy, Z estimation model
- **FUD Files**: Analysis of excluded signals (marketing inflation, hype amplification)

## License

Proprietary - SpeakEZ Technologies

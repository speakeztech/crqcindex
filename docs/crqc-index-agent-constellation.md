---
title: "CRQC Index: Agent Constellation"
linkTitle: "Agent Constellation"
description: "Mermaid diagrams of the agent constellation, input vectors, and editorial pushback flow"
date: 2026-04-14
authors: ["Houston Haynes"]
status: "Design Draft"
tags: ["Architecture", "Diagrams", "Agents"]
---

## Overview

The CRQC Index agent constellation runs on Cloudflare as a set of Durable Objects coordinated by Prospero. Three input vectors feed the Analyst → Librarian → Approval Gate pipeline. The editorial input vector is bidirectional: agents push back when editorial claims contradict the established corpus.

These diagrams complement the prose architecture in `crqc-index-design-scaffold.md`. They are the canonical visual reference for how signals move through the system.

## Full System Flow

```mermaid
flowchart TB
    %% Input Vectors
    subgraph INPUTS["Input Vectors"]
        direction TB
        ARXIV["arXiv Sentinel<br/><i>hourly</i>"]
        PATENTS["Patent Sentinel<br/><i>daily</i>"]
        GOVT["Government Sentinel<br/><i>daily</i>"]
        VENDOR["Vendor Sentinel<br/><i>daily</i>"]
        SOCIAL["Social Sentinel<br/><i>15 min</i>"]
        FUNDING["Funding Sentinel<br/><i>weekly</i>"]
        SUBMIT["External Submission<br/><i>public form</i>"]
        EDITORIAL["Editorial Input<br/><i>Houston authors</i>"]
    end

    %% Validation for external submissions
    subgraph VALIDATION["Submission Validation"]
        direction TB
        DEDUP["Deduplication"]
        VERIFY["Source Verification"]
        MOTIVE["Motivation Analysis<br/><small>vendor amp / FUD /<br/>prompt injection / coordinated</small>"]
        TRIAGE{"Triage"}
    end

    %% Core agents
    subgraph CONSTELLATION["Agent Constellation (Private)"]
        direction TB
        ANALYST["<b>Analyst</b><br/>Structured extraction<br/>4-dim scoring<br/>Tier classification<br/>QFT→Shor's proxy eval"]
        LIBRARIAN["<b>Librarian</b><br/>Corpus check<br/>Citation graph<br/>Provenance chain<br/>Conflict detection"]
        PROSPERO["<b>Prospero</b><br/>Coordinator<br/>Z model owner<br/>Sentinel scheduling"]
    end

    %% Approval Gate
    GATE["<b>Approval Gate</b><br/>(Durable Object)<br/>Queue + private notify<br/>Approve / Reject / Hold / Reclassify"]

    %% Houston (human)
    HOUSTON((("👤 Houston<br/>Editorial &<br/>Approval")))

    %% Public site
    PUBLIC[["<b>Public Site</b><br/>Workers + Pages + D1 + R2<br/><i>only approved content</i>"]]

    %% Sentinel flows
    ARXIV --> ANALYST
    PATENTS --> ANALYST
    GOVT --> ANALYST
    VENDOR --> ANALYST
    SOCIAL --> ANALYST
    FUNDING --> ANALYST

    %% External submission flow
    SUBMIT --> DEDUP
    DEDUP --> VERIFY
    VERIFY --> MOTIVE
    MOTIVE --> TRIAGE
    TRIAGE -->|"legitimate"| ANALYST
    TRIAGE -->|"flagged"| HOUSTON
    TRIAGE -.->|"auto-close"| ARCHIVE[("Archive")]

    %% Editorial flow (bidirectional)
    EDITORIAL --> ANALYST
    LIBRARIAN -.->|"conflict detected"| HOUSTON
    HOUSTON -.->|"stand by / revise / defer"| EDITORIAL

    %% Internal pipeline
    ANALYST <--> LIBRARIAN
    ANALYST --> PROSPERO
    PROSPERO --> GATE

    %% Approval cycle
    GATE -->|"notify"| HOUSTON
    HOUSTON -->|"approve"| GATE
    HOUSTON -->|"reject / reclassify"| ANALYST

    %% Publication
    GATE ==>|"approved"| PUBLIC

    %% Styling
    classDef sentinel fill:#1e3a5f,stroke:#4a90e2,color:#e0e0e0
    classDef agent fill:#3d2d5c,stroke:#9b6dd6,color:#e0e0e0
    classDef gate fill:#5c2d2d,stroke:#d66d6d,color:#e0e0e0
    classDef public fill:#2d5c3d,stroke:#6dd69b,color:#e0e0e0
    classDef human fill:#5c4d2d,stroke:#d6b56d,color:#e0e0e0
    classDef validation fill:#2d4d5c,stroke:#6db5d6,color:#e0e0e0

    class ARXIV,PATENTS,GOVT,VENDOR,SOCIAL,FUNDING,SUBMIT,EDITORIAL sentinel
    class ANALYST,LIBRARIAN,PROSPERO agent
    class GATE,TRIAGE gate
    class PUBLIC public
    class HOUSTON human
    class DEDUP,VERIFY,MOTIVE validation
```

## Editorial Pushback Flow

The editorial input vector is the only one where agents push back to the human. This sequence diagram shows the dialogue.

```mermaid
sequenceDiagram
    actor H as Houston
    participant A as Analyst
    participant L as Librarian
    participant C as Corpus (D1)
    participant G as Approval Gate
    participant P as Public Site

    H->>A: Editorial input (analysis / commentary)
    Note over A: source: editorial<br/>extract claims<br/>evaluate specificity

    A->>L: Submit claims for corpus validation
    L->>C: Cross-reference established signals

    alt Claims align with corpus
        L-->>A: No conflict
        A->>G: Forward scored signal
        G->>H: Approval notification
        H->>G: Approve
        G->>P: Publish
    else Claims contradict corpus
        L-->>A: Conflict flagged
        A->>H: Structured pushback<br/>(claim / corpus / conflict)

        alt Houston stands by editorial
            H->>A: Reasoning ("agents missed X")
            Note over A: Calibration data captured
            A->>G: Forward with editorial override
            G->>H: Approval notification
            H->>G: Approve
            G->>P: Publish
        else Houston revises
            H->>A: Revised editorial input
            A->>L: Re-validate
            Note over A,L: Loop until resolved
        else Houston defers
            H-->>A: Hold as draft
            Note over A: Revisit when more signals arrive
        end
    end
```

## Z Estimation Signal Flow

How signals push or pull Z, including the secondary calibration role of application-level benchmarks.

```mermaid
flowchart LR
    subgraph PRIMARY["Primary Signals (Tier 1)"]
        direction TB
        HW["Hardware Milestones<br/><small>qubit counts, fidelity</small>"]
        EC["Error Correction<br/><small>codes, decoders, thresholds</small>"]
        CRYPT["Cryptanalysis<br/><small>resource estimates</small>"]
        INFRA["Infrastructure<br/><small>networking, control</small>"]
    end

    subgraph SECONDARY["Secondary Calibration"]
        QFT["QFT Benchmarks<br/><i>Shor's subroutine</i>"]
        VQE["VQE / Algorithm Accuracy<br/><i>necessary-sufficient gap</i>"]
        TTS["Time-to-Solution<br/><i>full-stack overhead</i>"]
    end

    subgraph CONTEXT["Contextual (Tier 2)"]
        FUND["Funding"]
        HIRE["Hiring"]
        PAT["Patents"]
        SUPPLY["Supply Chain"]
    end

    NOISE["Tier 3: Noise<br/><small>marketing / FUD / hype</small><br/><i>filtered, published</i>"]

    Z((("<b>Z Factor</b><br/>per-primitive<br/>under tension")))

    PRIMARY -->|"push / pull"| Z
    SECONDARY -.->|"corroborate or<br/>contradict push"| Z
    CONTEXT -.->|"adjust confidence<br/>intervals"| Z
    NOISE -.->|"excluded"| FILTER["FUD Files"]

    Z --> MOSCA["Mosca: X + Y > Z<br/><i>per-organization slide rule</i>"]

    classDef primary fill:#3d2d5c,stroke:#9b6dd6,color:#e0e0e0
    classDef secondary fill:#2d4d5c,stroke:#6db5d6,color:#e0e0e0
    classDef context fill:#1e3a5f,stroke:#4a90e2,color:#e0e0e0
    classDef noise fill:#5c2d2d,stroke:#d66d6d,color:#e0e0e0
    classDef z fill:#5c4d2d,stroke:#d6b56d,color:#e0e0e0

    class HW,EC,CRYPT,INFRA primary
    class QFT,VQE,TTS secondary
    class FUND,HIRE,PAT,SUPPLY context
    class NOISE,FILTER noise
    class Z,MOSCA z
```

## Approval Tier Routing

How signals flow through the three approval tiers based on Analyst scoring and characteristics.

```mermaid
flowchart TB
    SIGNAL["Scored Signal<br/>(from Analyst)"]
    DECISION{"Tier classification<br/>+ source check<br/>+ anomaly flags"}

    AUTO["<b>Auto-publish</b><br/><small>known source<br/>Tier 2, Z impact ≤ 2<br/>no anomalies</small>"]
    STANDARD["<b>Standard Review</b><br/><small>Tier 1<br/>OR new source<br/>OR Tier 3 noise<br/>OR Librarian flags</small>"]
    ESCALATED["<b>Escalated Review</b><br/><small>Z impact ≥ 8<br/>OR source anomalies<br/>OR low confidence<br/>OR contradicts corpus<br/>OR prompt injection detected</small>"]

    PUBLISH[["Public Site"]]
    RETRO["Retrospective Review<br/><i>within 24h</i>"]
    HOUSTON_R((("Houston<br/>Review")))
    HELD[("Held: unverified<br/>indefinite")]

    SIGNAL --> DECISION
    DECISION --> AUTO
    DECISION --> STANDARD
    DECISION --> ESCALATED

    AUTO ==> PUBLISH
    AUTO -.-> RETRO
    RETRO -.->|"problem found"| RETRACT["Retract +<br/>correction notice"]

    STANDARD --> HOUSTON_R
    HOUSTON_R -->|"approve"| PUBLISH
    HOUSTON_R -->|"reject"| ARCHIVE[("Archive")]
    HOUSTON_R -->|"hold"| HELD

    ESCALATED --> HOUSTON_R
    HELD -.->|"corroborating evidence"| HOUSTON_R

    classDef tier1 fill:#2d5c3d,stroke:#6dd69b,color:#e0e0e0
    classDef tier2 fill:#5c4d2d,stroke:#d6b56d,color:#e0e0e0
    classDef tier3 fill:#5c2d2d,stroke:#d66d6d,color:#e0e0e0
    classDef pub fill:#2d5c3d,stroke:#6dd69b,color:#e0e0e0

    class AUTO tier1
    class STANDARD tier2
    class ESCALATED tier3
    class PUBLISH pub
```

## Notes

- All Durable Objects communicate via BAREWire-encoded LARP messages internally; the diagrams omit message format for clarity.
- The `Archive` data store is shared across triage paths but holds different metadata depending on rejection reason (spam, duplicate, fabricated source, manual reject).
- Sentinel cadences shown are targets; actual scheduling is managed by Prospero based on source rate-limit constraints.
- The editorial pushback dialogue is private — none of it appears on the public site unless Houston approves the resulting signal for publication.

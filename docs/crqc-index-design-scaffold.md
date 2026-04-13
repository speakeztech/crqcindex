---
title: "CRQC Index: Design Scaffold"
linkTitle: "CRQC Index"
description: "Agentic signal detection for CRQC emergence tracking"
date: 2026-04-11
authors: ["Houston Haynes"]
status: "Design Draft"
tags: ["Security", "Quantum", "Conclave", "LARP", "Architecture"]
---

## Purpose

A continuously-operating, publicly-accessible signal detection system that tracks
the convergence of quantum computing capabilities toward cryptographically relevant
quantum computation (CRQC). The monitor maintains a live estimate of Mosca's Z
variable (time to CRQC) by ingesting, classifying, and scoring signals from
academic publications, patent filings, social media, vendor announcements, and
government advisories.

The monitor's defining property is adversarial epistemics: it actively identifies
and filters FUD, marketing inflation, and hype from genuine technical signals. The
output is a curated, scored, source-attributed timeline with confidence intervals,
not a news feed.

## Core Metric

The monitor tracks Z (time to CRQC) with per-primitive granularity.
X (secrecy horizon) and Y (migration duration) are user-configurable
inputs. The public dashboard shows:

- Current Z estimate with confidence intervals (p10, p50, p90)
- Historical Z trajectory with driving signals annotated
- Per-primitive breakdown (RSA-2048, ECC P-256, ECC P-384, AES-256)

## Threat Surface

Z is not a single threshold. The monitor decomposes it into a
per-primitive, per-vector, per-sector structure.

### Primitive Dependency Map

| Primitive | Falls at (est. qubits) | Depends on it |
|-----------|----------------------|---------------|
| ECC P-256 | ~4,000 logical | TLS 1.3 key exchange (ECDHE), FIDO2/WebAuthn passkeys, Apple/Google Pay, code signing (Android, iOS, Windows), Signal Protocol, WireGuard, SSH ECDSA keys, Ethereum transaction signatures, X.509 leaf certificates |
| ECC P-384 | ~6,000 logical | Government PKI (CNSA suite), enterprise VPN concentrators, some HSM root keys |
| RSA-2048 | ~4,099 logical (Shor) | Legacy TLS, PGP/GPG email encryption, DNSSEC (many zones), older X.509 roots, document signing, some SCADA/ICS authentication |
| RSA-4096 | ~8,198 logical (Shor) | High-security PKI roots, some CA root certificates, long-lived signing keys |
| AES-128 | ~2,953 logical (Grover) | Symmetric encryption (reduced security margin but likely safe) |
| AES-256 | ~6,681 logical (Grover) | Symmetric encryption (remains secure; Grover gives 2^128 effective strength) |

The practical consequence: when ECC P-256 falls, TLS key exchange, passkey
authentication, mobile payments, and code signing are all compromised
simultaneously. RSA-2048 falling is a separate event affecting different
infrastructure. The monitor's Threat Assessment presents these as distinct
timelines with distinct impact profiles, not a single "quantum day."

### Attack Vector Taxonomy

Not all quantum attacks require the same capability. The Threat Assessment
classifies attack vectors by their quantum resource requirements and their
operational prerequisites:

**Tier A: Stored-ciphertext decryption (HNDL payoff).**
Requirements: a CRQC capable of running Shor's algorithm on the key
exchange primitive used when the ciphertext was captured. No access to
the target system is needed at the time of decryption; the adversary
already has the ciphertext.

This is the retroactive threat. Every TLS session captured today that
used ECDHE key exchange becomes readable when ECC P-256 falls. The
attack surface is the entire archive of captured traffic, which may
span years or decades.

Impact: confidentiality breach of historical communications. No
integrity impact (the communications already happened). No
availability impact.

**Tier B: Live session interception (man-in-the-middle).**
Requirements: a CRQC capable of breaking the key exchange in real time
(or near-real-time), plus active network position. The adversary must
be on the network path and must break the key exchange fast enough to
establish the MITM before the session times out.

This is harder than Tier A because it requires both quantum capability
and network position simultaneously. But the NIST SMF-28 result shows
that the network position can be established over existing telecom
fiber, and the classical stabilization infrastructure is commodity
hardware.

Impact: confidentiality and integrity breach of live communications.
The adversary can read and modify traffic in transit.

**Tier C: Signature forgery.**
Requirements: a CRQC capable of computing the private key from a
public key (ECDSA or RSA). No access to the signing system is needed;
the public key is, by definition, public.

When ECC P-256 falls, an adversary can forge code signatures, TLS
certificates, FIDO2 authentication tokens, and blockchain transactions.
The public key is, by definition, public; no access to the signing
system is needed.

Impact: integrity and authentication collapse across all ECDSA-dependent
infrastructure.

**Tier D: Symmetric key search (Grover).**
Requirements: a CRQC with enough qubits and circuit depth to run
Grover's algorithm against AES. The quadratic speedup reduces
AES-128's effective security to 64 bits (insecure) but leaves
AES-256 at 128 bits (secure for the foreseeable future).

This is the lowest-urgency vector. AES-256 is not at risk from
any projected quantum capability. AES-128 has reduced margin but
would require a much larger quantum computer than ECC or RSA attacks.

Impact: varies by symmetric key length. AES-256 users are not
affected. AES-128 users have reduced but still substantial margin.

**Tier E: On-setup attack.**
Source: SpeakEZ WireGuard PSK analysis (March 2025), "Achieving
Safety in a Universally Contested Future" (October 2025).

Requirements: CRQC capable of breaking the key exchange used during
initial system provisioning, plus knowledge of when provisioning
occurred. Targets the key ceremony: generation, distribution, and
installation of long-lived keys.

If the adversary captured provisioning traffic (VPN tunnel
establishment, HSM initialization, PKI root key distribution),
they retroactively derive the long-lived keys and compromise every
subsequent operation. Specialized HNDL targeting infrastructure
setup, not operational traffic. KeyStation's air-gapped key
ceremony is the direct countermeasure: no network traffic to capture.

Impact: total compromise of the provisioning trust chain.

### Active HNDL Monitoring

Observable indicators of ongoing HNDL activity, tracked by source type:

**Direct indicators (rare, high-confidence):**
- Disclosed breaches where encrypted data was exfiltrated but not
  decrypted (the adversary took ciphertext, not plaintext)
- Nation-state APT reports identifying data exfiltration patterns
  consistent with bulk ciphertext collection
- Intelligence community advisories naming specific HNDL campaigns

**Indirect indicators (more common, lower confidence):**
- Anomalous traffic volume at IX peering points or submarine cable
  landing stations reported in network observatory data
- Increases in state-sponsored storage infrastructure procurement
  (cloud storage contracts, data center construction in specific
  jurisdictions)
- Changes in national data localization laws that create legal
  frameworks for bulk data capture
- Targeted intrusions into telecom infrastructure (carrier-grade
  NAT, lawful intercept systems, optical tap points)

**Sector-specific exposure assessment:**
The Threat Assessment maintains a sector-by-sector exposure profile:

| Sector | Primary primitive exposure | HNDL attractiveness | Migration complexity |
|--------|--------------------------|--------------------|--------------------|
| Financial services | ECC (payments, TLS) | Very high (transaction data) | High (regulatory compliance) |
| Healthcare | RSA (legacy systems), ECC (modern) | Very high (30-year data retention) | Very high (device lifecycles) |
| Defense/intelligence | ECC P-384 (CNSA), RSA | Maximum (by definition) | Medium (mandate-driven) |
| Critical infrastructure | RSA (SCADA/ICS), mixed | High (disruption value) | Very high (20-30 year lifecycles) |
| Technology | ECC (code signing, auth) | High (IP, source code) | Medium (engineering capacity) |
| Legal | RSA/ECC (document signing) | High (privilege, M&A) | High (legacy document stores) |
| Telecommunications | ECC (network auth) | Maximum (access enables further HNDL) | High (scale) |

Sector exposure is structural analysis, not a risk score. Drives the
authenticated organizational assessment.

### Probing Signals

The Threat Assessment tracks signals that specific organizations or sectors
are being actively probed or targeted:

- Public breach disclosures that mention encrypted data exfiltration
- CISA/FBI advisories naming targeted sectors
- Academic publications analyzing traffic patterns at specific
  infrastructure points
- Vendor threat intelligence reports (with source credibility scoring
  from the FUD Files methodology; vendor threat reports are frequently
  marketing vehicles)

These signals do not alter the Z estimate (they don't change when CRQC
arrives). They alter the urgency assessment for specific sectors and
organizations: "CRQC may be 4 years away, but your data was exfiltrated
last year, so your effective exposure window is already open."

In the authenticated view, probing signals are filtered to the
organization's declared sector. A healthcare organization sees HNDL
indicators relevant to healthcare; a telecom provider sees telecom
infrastructure probing. Probing signals in an organization's sector
trigger dedicated alerts distinct from Z-change notifications: "new
CISA advisory identifies active targeting of [your sector] with
encrypted data exfiltration patterns consistent with HNDL collection."

## Signal Taxonomy

### Tier 1: Primary Signals (direct evidence of capability advancement)

These signals directly alter the Z estimate. Each must be attributable to a
specific publication, dataset, or verifiable demonstration.

**Hardware milestones**
- Qubit count (logical and physical) by modality (superconducting, trapped-ion, photonic, neutral-atom)
- Gate fidelity improvements (single-qubit, two-qubit, mid-circuit measurement)
- Coherence time improvements
- Connectivity topology changes (all-to-all vs. nearest-neighbor)

**Error correction breakthroughs**
- New code families with improved encoding rates (qLDPC, BB codes, lifted-product)
- Decoder improvements (Cascade-class results that change achievable logical error rates)
- Threshold improvements (higher physical error rates tolerated)
- Reduction in physical-to-logical qubit ratios

**Cryptanalytic resource estimates**
- Revised qubit/gate counts for breaking specific primitives
- New quantum algorithms or algorithmic improvements
- Circuit depth optimizations for Shor's algorithm variants
- Resource estimates for ECC vs. RSA vs. lattice-based systems

**Physical infrastructure**
- Quantum networking demonstrations (fiber, satellite, free-space)
- Entanglement distribution fidelity and distance records
- Interconnect technology for multi-chip/multi-node quantum systems
- Classical control system improvements (cryogenic CMOS, room-temp control)

**Government/institutional signals**
- Migration deadline announcements (Google 2029, NSA CNSA 2.0)
- Standards body actions (NIST PQC finalization, new algorithm selections)
- Intelligence community advisories (CISA, NSA, GCHQ guidance)
- Export control changes affecting quantum hardware or algorithms

### Tier 2: Contextual Signals (indirect evidence, requires interpretation)

These signals provide context but do not directly change Z. They inform confidence
intervals and trend direction.

- Funding rounds for quantum hardware companies (size, investors, valuation)
- Hiring patterns at quantum labs (growth rate, specialization mix)
- Patent filing patterns (claims, assignees, priority dates)
- Conference talk acceptance patterns (topic distribution shifts)
- Supply chain indicators (dilution refrigerator orders, laser system procurement)
- National quantum strategy announcements and budget allocations

### Tier 3: Noise (actively filtered)

These signals are identified and tagged but excluded from Z estimation. The
monitor publishes its noise classifications to demonstrate filtering rigor.

**Marketing inflation**
- Vendor press releases claiming "quantum advantage" without peer-reviewed evidence
- Qubit count announcements that conflate physical and logical qubits
- "Quantum-ready" product claims without specified PQC algorithms
- Roadmap projections presented as achieved milestones

**FUD (Fear, Uncertainty, Doubt)**
- Claims that quantum computing is "impossible" or "decades away" without
  engaging current resource estimates
- Dismissals citing decoherence without addressing error correction advances
- Arguments from historical disappointment ("quantum has been 10 years away
  for 30 years") without engaging specific technical progress
- Conflation of universal fault-tolerant QC with CRQC (the latter requires
  far fewer resources than the former)

The monitor treats theoretical impossibility arguments with particular
skepticism. Lord Kelvin declared heavier-than-air flight impossible in
1895. The Wright brothers flew in 1903. Theory is context; engineering
is signal.

**Hype amplification**
- Social media posts that amplify Tier 1 signals without adding analysis
- News articles that restate press releases without verification
- Commentary that extrapolates single results to general conclusions
- Influencer content driven by engagement metrics, not technical substance

## Signal Scoring

Each ingested signal receives a composite score across four dimensions:

**Provenance (0-10)**
- 10: Peer-reviewed journal (Nature, Science, PRL, Optica)
- 8: arXiv preprint from established research group
- 6: Government/standards body publication (NIST, NSA, CISA)
- 4: Conference proceedings (QIP, QEC, APS March Meeting)
- 2: Vendor technical report with reproducible claims
- 0: Press release, social media, news article without primary source

**Specificity (0-10)**
- 10: Concrete resource estimate with stated assumptions
- 8: Demonstrated result with measured performance metrics
- 6: Theoretical improvement with complexity analysis
- 4: Architectural proposal with plausible scaling argument
- 2: Qualitative claim with directional argument only
- 0: Vague assertion without technical content

**Reproducibility (0-10)**
- 10: Open-source code/data, independently verified
- 8: Detailed methodology, reproducible in principle
- 6: Sufficient detail for expert evaluation
- 4: Key details omitted but claims are internally consistent
- 2: Claims not independently verifiable
- 0: No methodology provided

**Impact on Z (0-10)**
- 10: Changes Z estimate by > 2 years
- 8: Changes Z estimate by 1-2 years
- 6: Changes Z estimate by 6-12 months
- 4: Changes Z estimate by 1-6 months
- 2: Confirms existing trend without changing estimate
- 0: No impact on Z estimate

Signals with composite score below 12 (out of 40) are classified as noise and
filtered. Signals scoring 12-24 are contextual. Signals scoring above 24 are
primary and trigger Z re-estimation.

## Agent Architecture (Conclave on Cloudflare)

### Actor Constellation

```
┌─────────────────────────────────────────────────────────────┐
│                    Prospero (Coordinator)                    │
│                                                             │
│  Maintains Z estimate state                                 │
│  Triggers re-estimation on primary signal ingestion         │
│  Publishes dashboard updates via LARP                       │
│  Manages agent scheduling and rate limiting                 │
└──────────┬──────────────┬──────────────┬────────────────────┘
           │              │              │
     ┌─────▼─────┐ ┌─────▼─────┐ ┌─────▼─────┐
     │  Sentinel  │ │  Sentinel  │ │  Sentinel  │    ... (N agents)
     │  arXiv     │ │  Patents   │ │  Social    │
     │  quant-ph  │ │  USPTO     │ │  X/Twitter │
     │  cr.CR     │ │  EPO       │ │  BlueSky   │
     └─────┬─────┘ └─────┬─────┘ └─────┬─────┘
           │              │              │
     ┌─────▼─────┐ ┌─────▼─────┐ ┌─────▼─────┐
     │  Sentinel  │ │  Sentinel  │ │  Sentinel  │
     │  Govt/Std  │ │  Vendor    │ │  Funding   │
     │  NIST,CISA │ │  IBM,Goog  │ │  Crunchbase│
     │  NSA,GCHQ  │ │  MSFT,NVDA │ │  PitchBook │
     └─────┬─────┘ └─────┬─────┘ └─────┬─────┘
           │              │              │
           └──────────────┼──────────────┘
                          │
                    ┌─────▼─────┐
                    │  Analyst  │
                    │           │
                    │  Signal   │
                    │  scoring  │
                    │  Noise    │
                    │  filtering│
                    │  Z delta  │
                    │  estimate │
                    └─────┬─────┘
                          │
                    ┌─────▼─────┐
                    │  Librarian│
                    │           │
                    │  Dedup    │
                    │  Citation │
                    │  graph    │
                    │  History  │
                    └───────────┘
```

### Actor Roles

**Prospero (Coordinator)**
Durable Object. Singleton. Owns the canonical Z estimate and the full signal
history. Receives scored signals from the Analyst, updates the Z model, and
pushes dashboard state via LARP to the public-facing edge. Manages scheduling
cadence for each Sentinel (arXiv: hourly; patents: daily; social: every 15
minutes; government: daily; vendor: daily; funding: weekly).

**Sentinel Agents (Scrapers)**
Durable Objects, one per source category. Each Sentinel:
- Scrapes its assigned sources on schedule
- Extracts candidate signals (title, abstract, authors, date, claims)
- Performs initial relevance filtering (keyword + semantic match against
  the signal taxonomy)
- Emits candidate signals to the Analyst via BAREWire-encoded LARP messages
- Maintains cursor state (last-seen arXiv ID, last patent publication date,
  last tweet ID) for incremental scraping

Sentinels do not score or interpret. They detect and forward.

**Analyst Agent (Scorer/Filter)**
Durable Object. Receives candidate signals from all Sentinels. For each:
- Classifies into Tier 1 / Tier 2 / Tier 3 (noise)
- Scores across the four dimensions (provenance, specificity,
  reproducibility, Z impact)
- For Tier 3 signals, tags the noise category (marketing, FUD, hype)
  and publishes the classification with reasoning (transparency)
- For Tier 1 signals, computes a Z delta estimate with confidence
  bounds and forwards to Prospero
- For Tier 2 signals, stores as contextual and adjusts confidence
  intervals on existing Z estimate

The Analyst uses structured extraction prompts against an LLM API for
paper triage. The prompts are versioned and auditable. The Analyst does
not hallucinate Z estimates; it extracts specific claims from papers
and maps them to the signal taxonomy. The Z delta computation is
deterministic given the extracted claims.

**Librarian Agent (Deduplication and Citation Graph)**
Durable Object. Maintains the full corpus of ingested signals:
- Deduplicates across sources (same paper appearing on arXiv, Twitter,
  and a news article)
- Builds citation graph (which papers cite which; which results
  supersede which)
- Tracks provenance chains (a social media post → a news article →
  the actual paper)
- Resolves conflicting claims between papers
- Maintains the historical Z trajectory with full attribution

### Data Model

```
Signal {
    id: UUID
    source: SourceType          // arXiv, USPTO, Twitter, NIST, vendor, etc.
    source_url: URL
    ingestion_time: Timestamp
    publication_date: Date
    authors: string[]
    title: string
    
    // Extracted claims
    claims: Claim[]
    
    // Scoring
    tier: 1 | 2 | 3
    provenance_score: 0-10
    specificity_score: 0-10
    reproducibility_score: 0-10
    z_impact_score: 0-10
    composite_score: 0-40
    
    // For Tier 3 (noise)
    noise_category: Marketing | FUD | Hype | null
    noise_reasoning: string | null
    
    // For Tier 1 (primary)
    z_delta: Duration | null     // estimated change to Z
    z_confidence: float | null   // confidence in the delta
    primitive_affected: Primitive[] | null
    
    // Librarian metadata
    supersedes: Signal[] | null
    superseded_by: Signal | null
    citation_refs: Signal[]
    duplicate_of: Signal | null
}

Claim {
    type: ClaimType             // qubit_count, gate_fidelity, resource_estimate, etc.
    primitive: Primitive | null  // RSA-2048, ECC-P256, AES-256, general
    metric: string              // what was measured
    value: number               // the measurement
    unit: string                // qubits, percent, hours, etc.
    comparison_baseline: string // what this improves upon
    improvement_factor: number  // magnitude of improvement
}

ZEstimate {
    timestamp: Timestamp
    p10: Duration               // 10th percentile (optimistic)
    p50: Duration               // median
    p90: Duration               // 90th percentile (conservative)
    per_primitive: Map<Primitive, {p10, p50, p90}>
    driving_signals: Signal[]   // which signals produced this estimate
}
```

## Site Architecture

### Splash / Landing

The entry point. Minimal text. Two visual elements:

1. **The Instrument.** Either a countdown range or a trajectory line chart
   (TBD based on design). Shows the current Z estimate with confidence
   band, not a single number. "CRQC for ECC P-256: 3-7 years (p50: 4.5
   years)." Updated in real time as signals arrive. The per-primitive
   breakdown (RSA-2048, ECC P-256, ECC P-384, AES-256) is visible at a
   glance; ECC leads, which is the key visual takeaway.

2. **The Mosca Calculator.** Interactive. User inputs X and Y. Returns
   margin result with plain-language output: "Your exposure window opened
   18 months ago" or "approximately 2 years of margin remaining."

Navigation from the splash leads to the three content sections plus
reference material.

### Bulletins

The signal feed. Each Bulletin is a short-form entry (300-800 words)
anchored to a specific captured signal: a paper, a patent filing, a
government advisory, a vendor announcement, a social media post from a
principal investigator.

**Purpose:** capture individual spikes in the signal field as they occur.
Demystify academic jargon into language a non-specialist security
professional can act on.

**Signal scope:** Bulletins cover two categories:

1. *Quantum capability signals.* Papers, patents, hardware milestones,
   resource estimates, infrastructure demonstrations. These directly
   affect the Z estimate.

2. *Classical security failures relevant to the migration context.*
   Breaches, architectural vulnerabilities, and trust-model gaps that
   illustrate why the PQC migration is an opportunity for comprehensive
   posture reset, not just algorithm replacement. These do not affect
   the Z estimate. They affect the urgency argument: organizations
   that have not fixed classical gaps will carry them through the
   quantum transition. The Mercor/Tailscale breach, key ceremony
   failures, unrotated signing roots, and credential lifecycle gaps
   fall in this category.

   Classical security Bulletins are tagged distinctly from quantum
   signals. They appear in the feed because the site's thesis is
   that every environment is contested, and the migration forces
   organizations to touch their entire cryptographic infrastructure.
   Documenting what goes wrong classically strengthens the case for
   the comprehensive approach.

3. *Ecosystem remediation signals.* Proposals, standards drafts, and
   deployed mitigations from specific ecosystems (Bitcoin soft forks,
   FIDO PQC authenticator specs, TLS post-quantum drafts, Signal PQXDH
   deployment). Classified as Tier 2 (contextual). They inform Y
   (migration duration) for organizations that depend on those
   ecosystems. All ecosystems receive equal coverage depth. The monitor
   tracks the primitive timeline, not ecosystem-internal debates. A
   remediation proposal is noted when it produces a concrete outcome;
   it is not tracked as an ongoing narrative.

**Structure per Bulletin:**
- Headline: plain-language statement of what changed
- Source: linked primary source with publication date
- Signal classification: Tier 1 (primary) / Tier 2 (contextual) /
  Tier 3 (noise), with the tier reasoning visible
- What it means: 2-3 paragraphs translating the technical content into
  operational terms. What primitive is affected? By how much? What
  should a CISO do differently because of this?
- Z impact: did this signal change the estimate? By how much? If not,
  why not?
- For Tier 3 (noise) signals: what category (marketing / FUD / hype)
  and why the classification was made. These are published with the
  same rigor as primary signals. The noise log is not hidden; it is
  interleaved with the real signals, clearly tagged, so readers can
  calibrate their own intake against the monitor's filtering.

**Cadence:** event-driven. A Bulletin publishes when a signal arrives
that scores above the ingestion threshold, or when a notable noise
signal warrants public classification. Not on a fixed schedule.

**Tone:** factual, concise, accessible. No editorializing. The
Bulletin states what was published, what it means for Z, and stops.
The reader draws their own conclusions.

### Analysis

The deep-read layer. Analysis entries are longer-form treatments
(1,500-4,000 words) that stitch together multiple Bulletins into a
coherent narrative. This is where the dots get connected.

**Purpose:** synthesize patterns across signals. Identify convergences
that individual Bulletins don't capture. Provide the strategic context
that a security architect or a board-level briefing requires.

Analysis entries may connect quantum and classical signals. An entry
might link a quantum capability milestone (Cascade decoder results
compressing Z) with a classical failure pattern (unrotated signing
roots across an industry sector) to make the argument: "this sector's
exposure window is shorter than estimated because the migration will
take longer due to accumulated cryptographic debt."

Analysis entries also cover the behavioral dimension: security
culture, ceremony compliance, credential hygiene, and the gap between
available tools and actual practice. The quantum deadline reframes
these as urgent because post-quantum key material is larger,
ceremonies are more complex, and compromise consequences are
retroactive. An Analysis entry on credential mismanagement is not
a digression from the quantum mission; it is the operational
corollary. The best algorithm is irrelevant if the key is on a
sticky note.

**Structure:**
- An Analysis entry references multiple Bulletins by link
- It identifies the convergence pattern: "these three independent
  results, from different teams, using different methods, all point
  in the same direction"
- It updates the Z trajectory with a composite assessment, not just
  individual signal deltas
- It may revise the per-primitive breakdown when multiple signals
  affect the same primitive
- It contextualizes against the historical trajectory: "six months ago,
  the median estimate was X. Three Bulletins have moved it to Y.
  Here's why."

**Dashboard component:** the Analysis section has its own landing page
with a dashboard view showing:
- Current Z estimate with confidence intervals (p10, p50, p90)
- Historical Z trajectory chart, annotated with the Analysis entries
  that caused each inflection
- Per-primitive breakdown with trend arrows
- Links to the full Analysis entries below the dashboard

**Cadence:** periodic, driven by accumulation. An Analysis entry
publishes when enough Bulletins have accumulated to form a pattern,
or when a single Bulletin is significant enough to warrant immediate
strategic context. Roughly monthly, but not on a fixed schedule.

**Tone:** rigorous, structured, longer-horizon. Still factual, but
permits synthesis and pattern identification. Clearly distinguished
from opinion: "these signals converge on X" is a factual observation;
"organizations should do Y" is reserved for the product pages.

### Methodology

The reference layer. Longest shelf life. Most general. Updated
infrequently, only when the scoring rubric, signal taxonomy, or
estimation model changes.

**Purpose:** explain, from the inside out, how the monitor determines
a change in the Z estimate. Any reader who questions a Bulletin's
classification or an Analysis entry's conclusion can trace the
reasoning back to the Methodology.

**Contents:**
- **The Mosca Inequality:** formal statement of X + Y > Z with worked
  examples for different organizational profiles (healthcare, finance,
  defense, critical infrastructure)
- **Signal Taxonomy:** the full Tier 1 / Tier 2 / Tier 3 classification
  with definitions and boundary cases
- **Scoring Rubric:** the four-dimension scoring system (provenance,
  specificity, reproducibility, Z impact) with the 0-10 scales and
  threshold definitions
- **Noise Classification:** formal definitions of marketing inflation,
  FUD, and hype amplification, with historical examples of each
- **Z Estimation Model:** how individual signal scores aggregate into
  the composite Z estimate. The mathematical model, its assumptions,
  and its limitations. This section is explicit about what the monitor
  does not know and cannot estimate.
- **Changelog:** versioned record of every change to the methodology,
  with rationale. If the scoring rubric changes, the old version and
  the new version are both visible, with an explanation of why the
  change was made and how it affects historical scores.

**Tone:** technical, precise, auditable.

### FUD Files

A dedicated section that elevates excluded signals from a filtering
byproduct to a first-class analytical category. The thesis: what
organizations and individuals choose to deny, dismiss, inflate, or
fabricate is itself a signal about the state of the field. Analyzing
what's excluded from the Z estimate is as informative as analyzing
what's included.

**Purpose:** make the negative space visible. Show readers not just
what's happening in quantum, but what specific actors are trying to
make people believe is or isn't happening, and why.

**Content categories:**

*Marketing Inflation.*
Vendor claims that overstate capability. Each entry shows: the claim
as made, the primary source it references (if any), what the primary
source actually says, and where the inflation occurs. Example: a
press release claiming "1,000-qubit quantum computer" when the
system has 1,000 physical qubits with no error correction, encoding
zero logical qubits. The FUD File entry shows the press release,
the technical spec, and the gap between them.

Over time, the Marketing Inflation entries create a per-vendor
credibility record. Readers can see which companies consistently
overstate results and which report accurately. This record is
assembled from public statements; it is not editorial opinion.

*Dismissal Campaigns.*
Coordinated or patterned messaging that quantum computing is further
away than evidence suggests. Each entry identifies: the claim, the
claimant, their economic interest in the claim (e.g., GPU revenue
at risk), and the specific Tier 1 signals that contradict the
dismissal. The entry does not assert motive; it presents the
conflict of interest alongside the technical evidence and lets the
reader evaluate.

Tracking dismissal campaigns over time reveals inflection patterns.
A company that dismisses quantum in Q1, acquires a quantum startup
in Q2, and sets an internal PQC migration deadline in Q3 has
produced three FUD File entries that, read in sequence, tell a
story no single entry conveys. Dismissals that cite theoretical
impossibility are scored against the same historical pattern:
Lord Kelvin's 1895 declaration that heavier-than-air flight was
impossible preceded the Wright brothers by eight years. The
monitor does not argue with dismissals; it places them alongside
the engineering timeline and lets the record accumulate.

*Adversarial Content.*
Suspected fabricated or manipulated signals. These are signals that
the escalated review process identified as potentially forged:
synthetic arXiv preprints, fabricated social media personas, papers
with anomalous provenance. Each entry documents: what triggered
the escalation, what investigation was performed, what the
conclusion was (confirmed fabrication, inconclusive, cleared as
legitimate).

This category is handled with extreme care. The monitor does not
accuse individuals of fraud without evidence. Entries in this
category describe the anomalies detected and the investigation
process. If a signal is cleared as legitimate after investigation,
the FUD File entry documents that outcome, which itself builds
credibility: the process catches real anomalies and does not
produce false accusations.

*Hype Amplification.*
Content that takes a legitimate result and extrapolates beyond
what the result supports. The entry shows: the original result
(with its actual claims), the amplified version (with the
extrapolated claims), and where the extrapolation breaks. This
category is especially common after major Tier 1 signals, when
social media and news outlets amplify the result into territory
the original authors would not endorse.

**Analytical layer.** The FUD Files have their own periodic
analysis entries (distinct from the main Analysis section) that
identify patterns:

- Volume trends: is the rate of marketing inflation increasing
  or decreasing? A spike in dismissal content after a major
  Tier 1 signal is a reaction pattern worth documenting.
- Source clustering: are the dismissals coming from a specific
  industry segment (GPU manufacturers, classical cloud providers)
  or distributed broadly?
- Temporal correlation: do marketing inflation spikes precede
  vendor earnings calls or funding announcements?
- Narrative shifts: when does a specific actor's messaging change
  from dismissal to acknowledgment? That transition point is
  itself a high-value signal.

**Tone.** Clinical. The FUD Files present evidence and pattern,
not judgment. The entry format is: "this was claimed; this is the
evidence; here is the discrepancy." Readers draw their own
conclusions. The monitor's role is to make the discrepancies
visible, not to editorialize about them.

**Relationship to Z estimation.** FUD Files content does not
directly alter the Z estimate. It informs the confidence
intervals indirectly: if dismissal campaigns are intensifying
from actors with known economic exposure to quantum disruption,
that behavioral signal corroborates the technical signals that
are compressing Z. The FUD Files are the monitor's peripheral
vision.

### Terms and Glossary

Standalone reference section. Alphabetical. Each entry is 1-3 sentences.
Written for the reader who landed on the site because their CISO
forwarded a Bulletin and they don't know what CRQC means.

Core terms: CRQC, HNDL, PQC, QEC, qLDPC, FIDO2, ECC, RSA, ECDSA,
surface code, logical qubit, physical qubit, Shor's algorithm, Grover's
algorithm, Mosca's theorem, gate fidelity, coherence time, T-gate,
error correction threshold, code distance, stabilizer, syndrome,
decoder, lattice-based cryptography, hash-based signatures, BB code,
qubit modality (superconducting, trapped-ion, neutral-atom, photonic).

Terms link to relevant Bulletins and Analysis entries where the concept
appears in context.

### References

Curated list of primary sources organized by category:

- **Foundational papers:** Mosca 2015, Shor 1994, Grover 1996,
  Abramsky-Coecke 2004
- **Government advisories:** NIST PQC standards, NSA CNSA 2.0,
  CISA infrastructure assessment, GAO 2024 report, G7 Treasury
  statement, Google 2029 migration deadline
- **Cryptanalytic resource estimates:** Kim et al. ECC estimates,
  Iceberg Pinnacle RSA-2048, Google ECDLP-256
- **Error correction:** Cascade (arXiv:2604.08358), BB code
  constructions, surface code threshold results
- **Infrastructure:** NIST SMF-28 fiber stabilization,
  modality-specific hardware milestones
- **SpeakEZ publications:** blog timeline with links, establishing
  the monitor's own provenance

Each reference includes: citation, link, date, and a one-sentence
annotation stating its relevance to Z estimation.

### Authenticated View (QuantumCredential integration)

Organizations that authenticate with QuantumCredential get:

- Saved X and Y parameters with organizational profile
- Alert thresholds: notify when Z drops below X + Y + margin
- Private annotations on Bulletins and Analysis entries
- Export of signal history for compliance documentation
- Integration with KeyStation for migration planning triggers
- **Organizational threat assessment:** Threat Surface section
  customized to declared sector, primitive inventory, data
  retention requirements. Per-primitive exposure windows,
  recommended migration sequencing.
- **Attack vector prioritization:** Tier A-E taxonomy applied to
  the organization's profile. Sector-specific HNDL indicators.
- **Migration planning dashboard:** Y decomposed into phases,
  overlaid with Z trajectory. Visual answer to "will we finish
  before Z arrives?"
- **Sector-filtered probing alerts:** HNDL indicators filtered
  to the organization's declared sector.

The public site provides the instrument. The authenticated view
provides the organizational interpretation.

## Backplane: Orchestration and Approval Workflow

### Principle

The monitor collects continuously, publishes only with approval. The
public-facing site is a curated output, not a live feed of agent
activity. The Sentinels scrape, the Analyst scores, the Librarian
deduplicates, but nothing reaches the public site until Houston
reviews and approves. Automated updates to metrics and visuals
(Z estimate recalculation, trajectory charts, per-primitive
breakdowns) are triggered by approved signals, not by raw ingestion.

This is not a bottleneck; it is a trust boundary. The monitor's
credibility depends on every published signal being verified by a
human who understands the domain well enough to detect forgeries,
adversarial content, and edge cases that automated scoring cannot
resolve.

### Threat Model for the Monitor Itself

The monitor is an attractive target for manipulation:

**False urgency.** An adversary (or a vendor seeking attention)
injects a fabricated result that compresses Z. Organizations panic,
make hasty purchasing decisions, or lose trust in the monitor when
the signal is debunked. The monitor's credibility is destroyed.

**False complacency.** An adversary injects plausible-looking
dismissals or inflated resource estimates that expand Z. Organizations
delay migration. The monitor becomes complicit in the delay.

**Poisoned AI agents.** Papers, preprints, or social media posts
designed specifically to exploit LLM-based scoring. Adversarial
text that causes the Analyst agent to misclassify a noise signal
as Tier 1, or a genuine signal as noise. Prompt injection embedded
in paper abstracts or social media posts.

**Synthetic personas.** Fake researcher accounts posting technically
plausible content that the Sentinel ingests. The content may be
partially correct (to pass automated plausibility checks) with
critical details fabricated.

**Retracted or corrected papers.** A legitimate paper is ingested
and scored. The paper is later retracted or significantly corrected.
If the monitor doesn't track retractions, the Z estimate retains
the influence of the withdrawn result.

### Architecture

```
┌──────────────────────────────────────────────────────────┐
│                  Public-Facing Site                       │
│         (Workers + Pages + D1 + R2)                      │
│                                                          │
│   Only contains approved content                         │
│   Metrics/visuals update on approval events              │
│   Static between approvals                               │
└──────────────────────┬───────────────────────────────────┘
                       │ publish event (approved signal)
                       │
┌──────────────────────┴───────────────────────────────────┐
│              Approval Gate (Durable Object)               │
│                                                          │
│   Receives scored signals from Analyst                   │
│   Queues them in review pipeline                         │
│   Notifies Houston via private channel                   │
│   Accepts approve / reject / hold / reclassify           │
│   On approve: triggers publication + metric update       │
│   On reject: archives with rejection reasoning           │
│   On hold: keeps in queue for further investigation      │
│   On reclassify: returns to Analyst with override tier   │
└──────────────────────┬───────────────────────────────────┘
                       │ scored signal
                       │
┌──────────────────────┴───────────────────────────────────┐
│              Agent Constellation (private)                │
│                                                          │
│   Sentinels → Analyst → Librarian → Approval Gate        │
│                                                          │
│   All agent activity is internal                         │
│   Nothing from this layer reaches the public site        │
│   without passing through the Approval Gate              │
└──────────────────────────────────────────────────────────┘
```

### Approval Tiers

Not all signals require the same level of review. The approval
workflow has three tiers based on the Analyst's composite score
and the signal's characteristics:

**Auto-publish (with retrospective review).**
Signals that meet all of the following criteria:
- Source is a known, verified primary source (established arXiv
  author, NIST/NSA/CISA publication, peer-reviewed journal)
- Composite score falls within the Tier 2 (contextual) range
- Z impact score is ≤ 2 (confirms existing trend, no estimate change)
- No anomalies flagged by the Librarian (no duplicate concerns,
  no citation inconsistencies)

These signals are published automatically and queued for
retrospective review within 24 hours. If review identifies a
problem, the signal is retracted with a public correction notice.
This tier handles the high-volume, low-impact flow of confirmatory
results that keep the feed current without creating a review backlog.

**Approval required (standard review).**
Signals that meet any of the following criteria:
- Tier 1 classification (primary signal, Z impact > 2)
- Source is new or unverified (first appearance of an author,
  institution, or publication venue in the corpus)
- Tier 3 classification with noise category (marketing, FUD, hype);
  noise classifications are editorially sensitive and require review
- Librarian flags a potential duplicate, retraction, or citation
  inconsistency

Houston receives a private notification (see below) with the
signal, the Analyst's scoring, the Librarian's metadata, and a
one-tap approve/reject/hold/reclassify interface. Standard review
target: within 4 hours during waking hours.

**Escalated review (investigation required).**
Signals that meet any of the following criteria:
- Z impact score ≥ 8 (signal would change Z estimate by > 1 year)
- Source exhibits anomalies: paper has no institutional affiliation,
  arXiv account was created recently, social media account has
  anomalous follower/engagement patterns, claims are extraordinary
  relative to prior art
- Analyst confidence is low (the structured extraction produced
  ambiguous or contradictory claims)
- The signal contradicts multiple established Tier 1 signals
  without explaining the discrepancy
- The Analyst detects potential prompt injection or adversarial
  formatting in the source text

Escalated signals are held indefinitely until Houston completes
investigation. Investigation may include: verifying the paper's
authors through independent channels, checking institutional
affiliations, looking for corroborating results from other groups,
or reaching out to domain contacts for assessment. The signal is
not published until investigation is complete. If investigation
is inconclusive, the signal is archived with a "held: unverified"
status and revisited if corroborating evidence appears.

### Private Notification Channel

The Approval Gate notifies Houston through a private channel
separate from the public notification infrastructure:

**Primary:** private email to a dedicated address (not the public
SpeakEZ inbox). The email contains the full signal data, Analyst
scoring, Librarian metadata, and approval tier classification.
For standard and escalated signals, the email includes a secure
link to the approval interface.

**Secondary:** push notification via a lightweight private
dashboard (Workers-hosted, authenticated, not publicly accessible).
The dashboard shows the review queue, pending signals sorted by
approval tier, and one-tap approve/reject/hold controls for
standard-tier signals.

**Fallback:** if no action is taken on a standard-tier signal
within 8 hours, the Approval Gate sends a reminder. If no action
within 24 hours, the signal is automatically held (not published,
not rejected) and flagged for batch review. The system never
auto-publishes a signal that requires approval, regardless of
how long the queue sits.

### Publication Pipeline

On approval, the following happen atomically:

1. The signal is written to the public D1 database
2. The Bulletin is generated (templated from the signal data +
   Analyst scoring + any editorial notes Houston added during review)
3. The Z estimate is recalculated if the signal has Z impact
4. Dashboard metrics and visuals are regenerated (trajectory chart,
   per-primitive breakdown, confidence intervals)
5. Subscriber notifications are dispatched (email, Atom feed update,
   webhook POSTs) with the configured coalescing window
6. The signal's approval status, timestamp, and any editorial notes
   are written to the audit log

Steps 1-4 are synchronous (the public site is consistent after
publication). Steps 5-6 are async (notifications may arrive after
the site is updated).

### Rejection and Correction

**Rejected signals** are archived with rejection reasoning. The
reasoning is private (not published) unless the signal was a notable
noise classification, in which case the Tier 3 Bulletin is published
with the rejection reasoning as the noise classification rationale.

**Corrections to published signals** follow a strict protocol:
- The original Bulletin remains visible, marked with a correction
  banner
- A correction note is appended with the date and nature of the
  correction
- If the correction changes the Z impact, the Z estimate is
  recalculated and a new Bulletin is published explaining the
  revision
- Subscribers who received the original notification receive a
  correction notification
- The Atom feed entry is updated with the correction (per Atom
  spec, the `<updated>` timestamp changes)

**Retractions** (a published signal is found to be based on
retracted or fraudulent source material):
- The Bulletin is marked as retracted, not deleted
- A retraction Bulletin is published explaining what happened
  and why the original passed review
- The Z estimate is recalculated excluding the retracted signal
- The Methodology changelog records the incident and any process
  changes it motivates

## Content Policy

The monitor extracts, scores, and presents. It does not editorialize,
predict, or advocate. All scoring methodology is public and versioned.
Noise classifications include auditable reasoning.

## Commercial Disclosure

The CRQC Index is a service provided by SpeakEZ Technologies, Inc.
This disclosure is permanent, visible on every page, and not buried in a
footer or terms-of-service document.

**What we sell.** SpeakEZ builds post-quantum security infrastructure:
QuantumCredential (post-quantum credential format), KeyStation (air-gapped
HSM for key ceremonies), and the Fidelity Framework (verified compilation
for heterogeneous computing, including quantum targets). These products
exist because we believe the threat this monitor tracks is real and
approaching faster than industry consensus reflects.

**Why we built this monitor.** We built it because the signal field is
noisy, the threat timeline is uncertain, and organizations need a
rigorous, continuously-updated instrument to calibrate their own
migration urgency. We also built it because we believe that when
organizations see a clear, honest accounting of the convergence toward
CRQC, some of them will conclude they need the products we sell. We
are comfortable with that alignment.

**What this means for the content.** The monitor's scoring rubric,
signal taxonomy, noise classification criteria, and Z estimation model
are public, versioned, and auditable. Every Bulletin, Analysis entry,
and noise classification includes full reasoning. Readers can verify
every assessment against the primary sources. The monitor does not
inflate threat estimates to sell products; inflated estimates would
undermine the rigor that is the monitor's only asset.

**The line.** The monitor measures and reports. Product information
lives on speakez.tech, linked from site navigation, clearly separated
from signal content. Bulletins and Analysis entries never recommend
SpeakEZ products. The authenticated view requires QuantumCredential;
that relationship is stated plainly.

## Notification and Subscription Infrastructure

### Public Subscriptions (unauthenticated)

**Email notifications.** Visitors can subscribe with an email address
to receive notifications when:
- A Tier 1 signal is published (primary signals that move the Z estimate)
- An Analysis entry is published (periodic synthesis)
- The Z estimate changes by more than a configurable threshold

Subscription management is self-service. Subscribers select their
notification level:
- **Critical only:** Tier 1 signals that change Z by > 6 months
- **All primary:** all Tier 1 signals
- **Full feed:** Tier 1 + Tier 2 + Analysis entries

Implementation: Cloudflare Email Service (Email Sending from Workers).
Transactional email via the SEND_EMAIL binding, no external provider
dependency. Email templates are plain-text-first with optional HTML,
minimal formatting. The email contains the Bulletin headline, the
signal classification, the Z impact summary, and a link to the full
entry. No tracking pixels. No marketing content in notification
emails. The notification is the content; the content is the funnel.

**Atom/RSS feed.** Standard Atom 1.0 feed served from a Worker route.
Separate feeds for:
- `/feed/bulletins` — all Bulletins (Tier 1, 2, and 3)
- `/feed/primary` — Tier 1 Bulletins only
- `/feed/analysis` — Analysis entries only
- `/feed/all` — everything

Each feed entry includes: title, publication date, signal tier,
Z impact summary, and full content. The feed is the canonical
machine-readable interface; security teams that run their own
aggregation infrastructure can consume it directly without email.

**Webhook endpoint.** For organizations that want to integrate
signal ingestion into their own monitoring infrastructure:
`POST` to a registered webhook URL when a signal matching their
configured criteria is published. Payload is the structured Signal
object from the data model (JSON). Webhook registration requires
email verification but not QuantumCredential authentication.

### Authenticated Subscriptions (QuantumCredential)

Organizations authenticated via QuantumCredential receive all public
notification capabilities plus:

- **Threshold alerts.** "Notify me when Z drops below X + Y + margin
  for my saved organizational parameters." This is the migration
  trigger: the email that says "your exposure window is now open"
  or "your margin has fallen below 12 months."
- **Digest reports.** Weekly or monthly summary of all signals with
  cumulative Z trajectory, formatted for board-level briefing.
  Exportable as PDF for compliance documentation.
- **Custom noise filters.** Organizational allow/block lists for
  specific sources, vendors, or topic areas. "Don't notify me about
  photonic modality results; we're tracking superconducting only."

### Implementation Notes

**Subscriber storage:** D1 database on Cloudflare. Schema:
```
Subscriber {
    id: UUID
    email: string (hashed for privacy, cleartext for sending)
    notification_level: critical | primary | full
    webhook_url: string | null
    org_id: string | null          // QuantumCredential org, if authenticated
    x_param: Duration | null       // secrecy horizon, if set
    y_param: Duration | null       // migration time, if set
    margin_threshold: Duration | null
    created_at: Timestamp
    verified: boolean
}
```

**Rate limiting:** notifications are batched if multiple signals arrive
within a short window. A 15-minute coalescing window prevents email
floods during high-activity periods (e.g., a major conference with
multiple relevant papers). The batch email lists all signals with
individual classifications.

**Unsubscribe:** one-click, every email, no friction. The subscriber
list is not a marketing asset; it is a notification service. Subscribers
who unsubscribe are removed immediately and permanently. No "are you
sure" interstitials. No win-back sequences.

**Privacy:** subscriber emails are used exclusively for monitor
notifications. They are not shared with third parties, not used for
SpeakEZ product marketing (unless the subscriber separately opts
in on speakez.tech), and not sold. This policy is stated on the
subscription form and in the site footer.

## Naming

Internal project name: **Canary**

Candidates for public-facing name: TBD. The name should convey watchfulness
without alarmism. It should be instantly understood by a CISO who has never
heard of Mosca's theorem but knows they have a quantum risk they can't quantify.

## Implementation Phases

**Phase 1: Proof of signal extraction.**
Single Sentinel (arXiv quant-ph + cr.CR). Manual Analyst scoring. Static
dashboard. Validates that the extraction pipeline produces useful structured
signals from real papers. Deployed on Cloudflare Workers + D1.

**Phase 2: Automated scoring.**
LLM-assisted Analyst with structured extraction prompts. Automated Tier
classification and scoring. Noise log. Historical Z trajectory from
backfilled signals (your blog publication timeline as seed data).

**Phase 3: Full constellation.**
All Sentinel agents operational. Patent monitoring. Social media signal
detection with FUD/hype filtering. Government advisory tracking. Real-time
dashboard with LARP-driven updates. QuantumCredential authentication
integration.

**Phase 4: Organizational profiles and threat assessment.**
Authenticated view with saved parameters. Organizational threat
assessment (customized Threat Surface per organization's sector,
primitives, and retention requirements). Attack vector prioritization
per profile. Migration planning dashboard. Alert thresholds.
Compliance export. KeyStation migration planning integration.
This is the revenue layer.

## Provenance

Seed data for historical Z trajectory backfill:

| Date | Signal | Source |
|------|--------|--------|
| 2025-03-xx | WireGuard PSK trust gap identified | SpeakEZ blog |
| 2025-06-01 | Mosca Moment thesis published | SpeakEZ blog |
| 2025-10-30 | Universal contestation thesis published | SpeakEZ blog |
| 2025-10-xx | Google acquires Atlantic Quantum (fluxonium superconducting) | Google |
| 2025-11-xx | Gelsinger "two years to quantum mainstream" (FT interview) | Financial Times |
| 2025-12-28 | QRNG XOR entropy case study published | SpeakEZ blog |
| 2026-02-xx | Kim et al. ECC resource estimates | eprint/arXiv |
| 2026-02-xx | Iceberg Pinnacle RSA-2048 < 100K qubits | Iceberg Quantum |
| 2026-03-25 | Google 2029 PQC migration deadline | Google blog |
| 2026-03-27 | Google announces dual-modality (superconducting + neutral atom) | Google Quantum AI blog |
| 2026-03-28 | SpeakEZ March 2026 update (ECC urgency) | SpeakEZ blog |
| 2026-03-31 | Google ECDLP-256 break | Google whitepaper |
| 2026-04-02 | NIST SMF-28 fiber stabilization | Optica Quantum |
| 2026-04-09 | Cascade neural decoder | arXiv:2604.08358 |

---
title: "CRQC Index: Methodology Primer"
linkTitle: "Methodology Primer"
description: "How the CRQC Index uses temporal graph neural networks and language models to detect capability convergence"
date: 2026-04-12
authors: ["Houston Haynes"]
status: "Design Draft"
tags: ["Methodology", "AI", "GNN", "OSINT"]
---

## What This System Does

The CRQC Index tracks how fast the world is approaching
a cryptographically relevant quantum computer. It does
this by monitoring academic papers, patent filings,
government advisories, vendor announcements, and social
media from researchers in the field. Hundreds of signals
arrive weekly. Most are noise. Some are incremental. A
few change the timeline. The system's job is to sort them,
score them, and maintain a running estimate of when
specific cryptographic primitives become vulnerable.

This primer explains the two AI components that power
that process and how they divide the work between them.

## Why This Monitor Exists

The quantum threat is the forcing function. It is not
the only problem.

Most organizations have accumulated decades of
cryptographic debt: key ceremonies that exist on paper
but are not followed, trust models that assume network
boundaries that no longer hold, credential lifecycles
that have never been audited, and key material that has
never been rotated. These deficiencies are not quantum
vulnerabilities. They are classical vulnerabilities that
have persisted because the cost of fixing them exceeded
the perceived risk of leaving them in place.

The Mercor/Tailscale breach of 2025 illustrates the
pattern. The compromise was not quantum-related. It
exploited a classical trust-model gap in WireGuard's
PSK handling that Tailscale's architecture left open.
No quantum computer was involved. No advanced
cryptanalysis was required. The attacker walked through
a door that was architecturally unlocked. Tailscale
still has not shipped PSK support over a year later.

This is typical, not exceptional. The cryptographic
infrastructure of most organizations contains gaps
that predate the quantum threat by years or decades:
certificates that chain to roots nobody can account for,
VPN tunnels authenticated by shared secrets that were
distributed insecurely, HSMs running firmware that has
never been verified, signing keys that have never been
rotated because the rotation procedure was never
written.

The PQC migration forces a complete inventory. Every
certificate chain must be audited. Every key exchange
protocol must be evaluated. Every signing root must be
examined. Every credential lifecycle must be documented.
This inventory work is identical whether the motivation
is quantum resistance or basic hygiene. The quantum
deadline provides the urgency; the work itself is
overdue regardless.

This monitor tracks the quantum timeline because that
is the clock. But the operational thesis is broader:
every environment is a contested environment, contested
by classical adversaries today and quantum-capable
adversaries on a compressing timeline. The security
posture required to survive the quantum transition is
the same posture required to survive the classical
threat landscape that already exists. The organizations
that treat PQC migration as a cryptographic algorithm
swap will fix one problem and leave the rest. The
organizations that treat it as a comprehensive security
posture reset will fix everything, because the migration
forces them to touch everything.

The CRQC Index exists to provide the timeline pressure
that motivates the comprehensive approach. The Z
estimate is a number. The response it should provoke is
not "swap algorithms before Z arrives." It is "audit
everything, fix everything, verify everything, because
you are going to touch it all anyway."

## The Third Problem: Behavior

Algorithms can be replaced. Infrastructure can be
audited. Neither matters if the people operating the
systems do not change how they work.

The strongest post-quantum credential scheme is
defeated by a recovery key written on a sticky note.
The most rigorous key ceremony is undermined by an
administrator who shares the HSM PIN over Slack. The
most carefully designed zero-trust architecture
collapses when a developer hardcodes a secret in a
repository because the secrets management tool was
inconvenient.

These are not hypothetical failures. They are the
dominant cause of breaches today, in a classical threat
environment. The quantum transition does not fix them.
It amplifies them: post-quantum key material is larger,
ceremonies are more complex, and the consequences of
compromise are retroactive.

The PQC migration creates a window for cultural change
precisely because it forces organizations to retrain
their staff. Every engineer who touches a certificate
chain during migration is a candidate for better
security habits. Every key ceremony that gets rewritten
for PQC compliance can be rewritten to include the
physical verification steps that should have been there
from the start. Every credential lifecycle that gets
audited for quantum vulnerability can be audited for
rotation compliance at the same time.

The tools for safe practice exist. FIDO2 hardware tokens
eliminate password reuse. HSMs with physical presence
requirements eliminate remote key extraction. Air-gapped
key ceremonies eliminate network-based key compromise.
Secrets management systems eliminate hardcoded
credentials. None of these are new. None require quantum
awareness. All are resisted because they add friction
to workflows that people have optimized for convenience.

The quantum deadline changes the cost-benefit
calculation. When the consequence of a compromised
signing root is retroactive exposure of every artifact
ever signed with that root, the friction of a proper
key ceremony becomes trivially justified. When the
consequence of a harvested VPN establishment is
retroactive decryption of every session that tunnel
carried, the inconvenience of proper PSK management
becomes trivially justified.

The CRQC Index tracks the technical timeline. The
Analysis section connects that timeline to operational
reality, including the behavioral changes that the
timeline demands. Bulletins that document classical
failures (credential mismanagement, ceremony shortcuts,
trust-model gaps) are published alongside quantum
capability signals because they share the same
conclusion: the security posture required to survive
the quantum transition is comprehensive, and
"comprehensive" includes the people, not just the
protocols.

## Editorial Scope

The CRQC Index tracks cryptographic primitives, not
the ecosystems built on them. ECC P-256 is a primitive.
Bitcoin, FIDO2, TLS, Signal, WireGuard, and Ethereum
are ecosystems that depend on it. The monitor covers the
primitive timeline with equal treatment across all
affected ecosystems.

**Primitive dependency coverage.** Each vulnerable
primitive has a dependency table entry listing the
ecosystems and protocols that rely on it. Bitcoin's
Taproot (Schnorr on secp256k1), FIDO2/WebAuthn
(ECDSA on P-256), TLS 1.3 (ECDHE), Signal Protocol
(Curve25519), and Ethereum (ECDSA on secp256k1) all
appear with equal weight. No ecosystem is elevated
because its community is large or vocal, and no
ecosystem is suppressed because its discourse is
contentious.

**Remediation coverage.** When ecosystem-specific
remediation proposals appear (Bitcoin soft fork
proposals, FIDO PQC authenticator specifications,
TLS 1.4 post-quantum drafts, Signal's PQXDH
deployment), they are noted in Bulletins as Tier 2
contextual signals. They inform the Y variable
(migration duration) for organizations that depend
on those ecosystems. The monitor does not track the
internal remediation debates of any ecosystem in
ongoing depth. A Bitcoin soft fork debate, a FIDO
working group discussion, and a TLS standardization
process all receive the same treatment: noted when
they produce a concrete outcome that affects
migration timelines, not covered as ongoing
narratives.

**No ecosystem advocacy.** The monitor does not
recommend specific remediation approaches for any
ecosystem. It does not evaluate whether a particular
proposal is adequate. It documents the primitive
vulnerability, notes the proposed responses, and
reports the timeline. The ecosystems own their
migrations.

**Commercial interest disclosure.** SpeakEZ builds
QuantumCredential, which operates in the FIDO2/WebAuthn
ecosystem. This interest is disclosed in the site's
commercial disclosure section. The editorial policy
ensures that FIDO2 remediation coverage does not receive
deeper or more favorable treatment than coverage of any
other ecosystem's remediation efforts. The disclosure
makes the interest visible; the editorial policy makes
the coverage even.

**Scope of quantum coverage.** The CRQC Index tracks
the aspects of quantum computing development that are
relevant to the cryptographic threat timeline. It does
not cover quantum computing as a field of general
scientific interest. Papers on quantum advantage
demonstrations, fundamental physics implications,
quantum simulation for chemistry, or quantum machine
learning are in scope only when they produce a result
that changes the resource estimate for breaking a
specific cryptographic primitive or removes a
bottleneck on the path to CRQC. The site works to
establish a reliable timeline for the aspects of
quantum that bear on cryptographic security.

**Engineering over theory.** The monitor scores
hardware milestones, resource estimates, and
demonstrated results. It does not score theoretical
arguments about what quantum computers can or cannot
do in principle. Theoretical impossibility arguments
have a poor historical track record against
engineering timelines. In 1895, Lord Kelvin declared
heavier-than-air flying machines impossible; the
Wright brothers flew at Kitty Hawk eight years later.
The machine did not wait for the theory to be revised.
The CRQC Index applies the same principle: theory is
context, engineering is signal. When a theoretical
paper argues that quantum computers at a certain scale
have implications for fundamental physics, that is
noted as context. When an engineering team demonstrates
a new logical qubit count or a decoder that changes
achievable error rates, that moves Z.

## Two Kinds of AI, Two Kinds of Work

The system uses two distinct forms of AI for two distinct
tasks. They are not interchangeable.

**A language model** handles text. It reads papers, extracts
claims, classifies sources, identifies noise, and drafts
summaries. Language models are good at this because the
input is unstructured text and the output is structured
data: "this paper claims X about primitive Y with Z
supporting evidence."

**A graph neural network** handles relationships. It takes
the structured data the language model produced and
analyzes how researchers, institutions, and technical
areas connect to each other and how those connections
change over time. Graph neural networks are good at this
because the input is a network of relationships and the
output is a set of structural patterns: "this researcher
moved from a civilian lab to a defense-affiliated entity,"
"this technical area went quiet at three institutions
simultaneously," "this cluster of co-authors is
consolidating."

The language model operates on documents. The graph neural
network operates on the map of who produced those documents,
with whom, and when.

## The Language Model as Boundary Manager

The language model sits at two boundaries in the pipeline.

### Boundary 1: Ingestion

Raw signals arrive as unstructured text: paper abstracts,
patent claims, news articles, social media posts, government
advisories. The language model's job at this boundary is
extraction and entitization:

- **Extract claims.** From a paper abstract, identify the
  specific technical claim: "achieved logical error rate
  of 10⁻¹⁰ at physical error rate 0.1% on the [144,12,12]
  Gross code using a convolutional neural network decoder."
  Not a summary. A structured claim with a metric, a
  context, and a comparison baseline.

- **Entitize.** Identify the named entities: researchers
  (by name), institutions (by affiliation), technical
  areas (by IPC class or keyword), and primitives affected
  (by cryptographic system). Map these to canonical
  identifiers in the entity resolution table. Flag new
  entities that don't exist in the table for manual
  resolution.

- **Classify.** Score the signal across provenance,
  specificity, reproducibility, and impact. Assign a
  tier: primary (changes the estimate), contextual
  (informs confidence), or noise (marketing, FUD, hype).
  For noise signals, identify the noise category and
  state the reasoning.

- **Detect adversarial content.** Flag signals with
  anomalous provenance: recently created accounts,
  papers without institutional affiliation, claims
  that contradict established results without
  explanation, or text patterns consistent with
  fabrication or prompt injection.

The language model does not decide what gets published.
It produces structured records that enter the approval
workflow. A human reviews and approves or rejects.

### Boundary 2: Aggregation

After the graph neural network has analyzed the
collaboration network and produced structural findings
(anomaly scores, cluster changes, cadence deviations,
migration flows), the language model's job at this
boundary is interpretation and drafting:

- **Cross-reference.** Connect the GNN's structural
  findings to the ingested signals. "The GNN detected
  cluster consolidation in quantum error correction
  among three Hefei institutions. This correlates with
  a cadence increase in IPC class G06N 10/ from the
  same entities and two Tier 1 Bulletins from this
  quarter."

- **Draft.** Produce candidate text for Analysis entries
  and Bulletins. The language model writes; a human
  edits and approves.

- **Summarize.** Generate the dashboard narrative: what
  changed this week, what drove the change, what the
  current Z estimate means in plain language.

The language model at this boundary does not generate
analysis. It translates structural findings into readable
text. The analysis lives in the temporal comparison layer
described below.

## Three Analytical Layers

The system's analytical core has three layers with
distinct responsibilities. The separation matters because
it determines what each component can and cannot do.

### Layer 1: GNN as feature encoder

A graph neural network learns from network structure.
Given a graph where nodes are entities (researchers,
institutions, companies) and edges are relationships
(co-authored a paper, co-filed a patent, share an
affiliation), the GNN learns a vector representation
of each node that captures its structural context: who
it connects to, how densely, through what kinds of
relationships, and how that pattern compares to other
nodes.

The core operation is message passing. Each node
collects information from its neighbors, transforms it
through a learned function, and updates its own
representation. After several rounds of message passing,
each node's representation reflects not just its
immediate neighbors but the broader network topology
around it.

The GNN is a feature encoder. It takes a graph snapshot
and produces an embedding matrix: one vector per node.
It has no memory. It has no temporal awareness. Each
daily snapshot is processed independently. The GNN's
only job is to produce a consistent embedding space
where structurally similar nodes land near each other.

The CRQC Index uses a relational GNN (R-GCN) that
learns separate weight matrices for each edge type:

| Edge type | What it represents | What changes in it signal |
|-----------|-------------------|-------------------------|
| Co-authorship | Joint publication | Active technical collaboration |
| Co-inventorship | Joint patent filing | Shared IP development |
| Affiliation | Employment or appointment | Institutional alignment |
| Citation | Reference in a paper | Intellectual influence |
| Funding | Shared grant or program | Coordinated investment |

Separate weight matrices per relation type let the
encoder distinguish structural patterns that arise from
different kinds of relationships. A co-authorship cluster
in quantum error correction produces different embeddings
than a citation cluster in the same area.

The GNN is trained in F# using the Furnace autodiff
library, exported to ONNX, and runs inference in a
Cloudflare Worker via WebAssembly with SIMD acceleration.

### Layer 2: Temporal comparison

This is where the analysis happens. Layer 2 operates
on the archive of daily embeddings that Layer 1 produces.
It is not a neural network. It is a set of comparison
operations over a stored history.

**Daily PCA.** Each day's embedding matrix is compared
to recent history via principal component analysis. The
PCA extracts the dominant modes of structural variation.
A week where PC1 explains 80% of variance has one
dominant structural event. A week where variance is
distributed across PC1-3 has multiple concurrent shifts.

**PC drift.** The cosine angle between successive
periods' principal components measures how much the
dominant structural dynamic has changed character. A
large drift triggers deeper analysis.

**Recursive re-evaluation.** This is the property that
distinguishes the system from a simple anomaly detector.
When new context arrives (a new signal, a new
collaboration, a cadence anomaly), Layer 2 does not
just evaluate the new snapshot. It re-evaluates
historical embeddings against the current context.

A paper published in January produces a node embedding
that looks unremarkable against January's graph. In
April, a new collaboration forms, a cadence anomaly
fires, and suddenly the January embedding is
structurally significant in retrospect. The January
embedding didn't change. The context for evaluating
it did.

This means the embedding archive is not a historical
record. It is an active analytical resource. Every
analysis cycle that detects a significant structural
change runs a backward pass over the archive, asking:
does this new context change the significance of any
previous snapshot? Signals that faded can become
resonant. Connections that looked incidental can
become patterns.

The backward pass is computationally cheap: vector
comparisons and nearest-neighbor searches over stored
embeddings. It does not re-run the GNN on old graphs.
The embeddings are fixed once produced; only their
interpretation changes.

**Cadence analysis.** Rolling statistical measures
(mean, standard deviation, trend) over filing and
publication counts per entity per technical area. This
is time series arithmetic, not neural network inference.
It runs alongside the embedding comparison and
produces complementary signals: the GNN captures
structural topology; cadence analysis captures activity
volume.

**Absence detection.** When the cadence analysis flags
a silence (a previously active entity stops filing in a
specific area), Layer 2 cross-references the silence
against the embedding archive: did this entity's
structural position change before it went quiet? Did
its co-authors also go quiet, or did they continue
independently? The combination of cadence silence and
embedding trajectory narrows the interpretation space
(classification vs. program failure vs. strategic
concealment).

Most analysis cycles are routine: the PC drift is
below threshold, the cadence metrics are within
baseline, and Layer 2 produces a summary confirming
stability. When PC drift exceeds threshold or a
cadence anomaly fires, Layer 2 runs the full recursive
pass and produces a structured finding for Layer 3.

### Layer 3: LLM aggregation

The language model at Boundary 2. It receives Layer 2's
structured findings (anomaly scores, cluster changes,
cadence deviations, re-evaluated historical signals)
and the scored signal corpus from Boundary 1.

- **Cross-reference.** Connect Layer 2's structural
  findings to specific ingested signals. "The
  embedding drift this week correlates with two Tier 1
  Bulletins and a cadence anomaly in IPC class G06N 10/."

- **Draft.** Produce candidate text for Analysis entries
  and Bulletins.

- **Surface re-evaluated signals.** When Layer 2's
  backward pass identifies a historical signal that
  has become newly significant, Layer 3 drafts an
  update that explains the recontextualization.

Layer 3 does not generate analysis. It narrates the
analysis that Layer 2 produced.

## How They Work Together

```
Raw Signals (papers, patents, posts, advisories)
    │
    ▼
┌─────────────────────────────────┐
│  LLM Boundary 1: Ingestion     │
│                                 │
│  Extract claims                 │
│  Entitize (researchers,         │
│    institutions, areas)         │
│  Classify (tier, noise type)    │
│  Detect anomalies               │
│                                 │
│  Output: structured signal      │
│  records + entity graph edges   │
└──────────────┬──────────────────┘
               │
               ▼
┌─────────────────────────────────┐
│  Graph Construction             │
│                                 │
│  Build daily adjacency from     │
│  cumulative edge log            │
│  Resolve entities to canonical  │
│  node IDs                       │
│  Compute cadence metrics        │
│  per entity, per area           │
└──────────────┬──────────────────┘
               │
               ▼
┌─────────────────────────────────┐
│  Layer 1: GNN Encoder           │
│                                 │
│  R-GCN message passing          │
│  Daily node embeddings          │
│                                 │
│  Output: embedding matrix       │
│  (one vector per node)          │
│  Stored to embedding archive    │
└──────────────┬──────────────────┘
               │
               ▼
┌─────────────────────────────────┐
│  Layer 2: Temporal Comparison   │
│                                 │
│  PCA over embedding stack       │
│  PC drift measurement           │
│  Recursive re-evaluation of     │
│    historical embeddings        │
│  Cadence analysis               │
│  Absence-of-signal detection    │
│  Cluster detection              │
│  Migration flow analysis        │
│                                 │
│  Output: structural findings    │
│  (current + recontextualized    │
│   historical signals)           │
└──────────────┬──────────────────┘
               │
               ▼
┌─────────────────────────────────┐
│  Layer 3: LLM Aggregation       │
│                                 │
│  Cross-reference findings       │
│  with ingested signals          │
│  Draft Bulletins and Analysis   │
│  Surface re-evaluated signals   │
│                                 │
│  Output: candidate content for  │
│  approval workflow              │
└──────────────┬──────────────────┘
               │
               ▼
        Approval Gate
        (human review)
               │
               ▼
        Publication
```

The GNN and the temporal comparison layer never merge.
The GNN produces embeddings; it does not compare them.
The comparison layer compares embeddings; it does not
produce them. This separation is what makes the
recursive re-evaluation possible: the historical
embeddings are stable artifacts that can be
reinterpreted indefinitely without recomputation.

## What This System Does Not Do

**It does not predict breakthroughs.** Layer 2 detects
structural changes in the collaboration network that
may precede capability advances. Structural changes can
have explanations unrelated to technical progress
(funding cycles, policy changes, personnel retirements).

**It does not attribute intent.** When a researcher's
embedding trajectory shifts toward military-affiliated
clusters, the system records the migration. It does not
assert why. When cadence drops to zero, the system flags
the anomaly. It does not assert classification. The
system presents pattern; interpretation is the analyst's
responsibility.

**It does not replace domain expertise.** The GNN
encodes graph structure into vectors. Layer 2 compares
vectors over time. Neither understands quantum physics.
A domain expert reviews every finding before publication.

**It does not make the Z estimate autonomously.** Layer 2's
structural findings contribute to confidence intervals
on Z. They do not override the direct technical signals
(resource estimates, hardware milestones) that anchor
the point estimate.

**It does not forget.** The embedding archive is
permanent. Historical embeddings are not discarded after
analysis. They are re-evaluable indefinitely, because
the recursive comparison is the mechanism by which
faded signals become resonant when new context arrives.

## Technical Summary

| Component | Layer | Role | Input | Output |
|-----------|-------|------|-------|--------|
| Language model | Boundary 1 | Ingestion | Unstructured text | Structured signal records, entity graph edges |
| Graph construction | — | Data engineering | Edge log from D1 | Daily adjacency matrices per relation type |
| R-GCN | Layer 1 | Feature encoding | Single graph snapshot | Embedding matrix (one vector per node) |
| PCA | Layer 2 | Compression | Embedding stack | Principal components, variance ratios |
| Temporal comparison | Layer 2 | Analysis | Embedding archive + current snapshot | Structural findings, recontextualized signals |
| Cadence analysis | Layer 2 | Time series | Monthly counts per entity/area | Deviation scores, absence flags |
| Language model | Layer 3 | Aggregation | Structural findings + signal corpus | Candidate Bulletins, Analysis entries |
| Hypergraph cache | — | Serving | All Layer 2 outputs | Keyed bundles served to dashboard |

The language model is a commercial API (Anthropic Claude).
The R-GCN is trained in F# using the Furnace autodiff
library on local hardware, exported to ONNX, and runs
inference in a Cloudflare Worker via WebAssembly with
SIMD acceleration. Layer 2 runs in the same Worker as
arithmetic over the stored embedding archive.

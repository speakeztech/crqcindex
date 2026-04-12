---
title: "CRQC Index: OSINT Module — China Quantum Signal Monitoring"
linkTitle: "China OSINT Module"
description: "Patent cadence analysis, absence-of-signal detection, and disinformation filtering for Chinese quantum programs"
date: 2026-04-12
authors: ["Houston Haynes"]
status: "Design Draft"
tags: ["Security", "OSINT", "Quantum", "Patents", "China"]
---

## Scope

Companion module to the CRQC Index monitor. Dedicated to tracking
Chinese quantum capability development through open-source
intelligence: patent filings, academic publications, state media,
procurement signals, and social media from Chinese researchers
and institutions.

Two objectives with equal weight:

1. **Signal detection.** Identify genuine capability advances
   that affect the Z estimate, sourced from Chinese institutions
   and companies.

2. **Absence detection.** Identify when previously active public
   filing or publication cadences go quiet in specific technical
   areas, which may indicate classification, military transfer,
   or strategic concealment.

Both the presence and absence of signal are informative. The
module treats silence as data.

## Landscape Context

China accounts for approximately 60% of global quantum patent
filings by raw count. This volume is driven by incentive
structures (university and state institute programs that reward
filing volume), state funding ($15B+ allocated to quantum
technologies), and deliberate national strategy (quantum
designated as a strategic technology in the 14th Five-Year Plan).

Volume and quality diverge. CNIPA-granted patents tend toward
narrower claim scope than USPTO equivalents. Many filings are
incremental variations on published academic research. The
module must distinguish between volume-driven filings (noise)
and substantive capability claims (signal).

Key entities:

**State labs and universities**
- University of Science and Technology of China (USTC), Hefei
  (Jiuzhang photonic, Zuchongzhi superconducting)
- National Laboratory for Quantum Information Sciences, Hefei
- Chinese Academy of Sciences (CAS) and affiliated institutes
- Tsinghua, Peking, Zhejiang, Nanjing universities

**Hardware companies**
- Origin Quantum (superconducting, Wukong 72-qubit, IPO pending)
- CIQTEK (instruments and sensing, STAR Market IPO approved Dec 2025)
- QuantumCTek (QKD, first pure-quantum public listing, now under
  China Telecom control)
- Hyqubit, QuDoor, Huayi Quantum (trapped ion)
- CAS Cold Atom, MatriQ, Buchou Quantum (neutral atom)
- QBoson, TuringQ (photonic)

**QKD and networking**
- Guoke Quantum (provincial backbone networks, 1,700+ km Hubei network)
- Qasky
- Beijing-Shanghai QKD backbone (2,000+ km, operational)

**State actors**
- PLA Strategic Support Force (cyber and space operations)
- Ministry of State Security (intelligence)
- China Electronics Technology Group Corporation (CETC)

## Source Architecture

### Patent Monitoring

**Primary source: CNIPA**
China National Intellectual Property Administration. Patent
publications are available through CNIPA's online search
system and through commercial aggregators (PatSnap, Orbit,
Google Patents with jurisdiction filter).

Monitoring parameters:
- IPC/CPC class filters: H04L 9/ (cryptographic mechanisms),
  G06N 10/ (quantum computing), G06F 21/ (security arrangements),
  H04B 10/ (optical transmission), G01V (geophysics/sensing)
- Assignee filters: tracked entity list (see above)
- Keyword filters: Chinese-language terms for quantum computing
  (量子计算), quantum key distribution (量子密钥分发), quantum
  error correction (量子纠错), post-quantum cryptography
  (后量子密码), qubit (量子比特), entanglement (量子纠缠)
- Filing date tracking: weekly cadence measurement per entity
  and per technical area

**Secondary: PCT filings**
PCT applications from Chinese priority filings entering
national phase at USPTO, EPO, JPO. These indicate which
Chinese inventions are considered commercially or strategically
valuable enough to pursue international protection. A Chinese
patent that stays domestic-only is less significant than one
entering PCT national phase.

**Tertiary: USPTO and EPO filings by Chinese entities**
Direct filings (not PCT-routed) by Chinese companies or
individuals at foreign patent offices. Rare but high-signal.

### Academic Publication Monitoring

- arXiv: quant-ph, cs.CR, cs.ET with Chinese institutional
  affiliations
- Chinese Journal of Quantum Electronics (量子电子学报)
- Science China: Physics, Mechanics & Astronomy
- Chinese Physics B, Chinese Physics Letters
- National Science Review
- Preprint servers: ChinaXiv

Track publication cadence per institution and per topic area.
Measure co-authorship networks. Flag papers with PLA-affiliated
authors or dual-use framing.

### State Media and Policy Monitoring

- Xinhua (新华社): official state news agency
- People's Daily (人民日报): CCP official newspaper
- Science and Technology Daily (科技日报): MOST-affiliated
- CNIPA press briefings and annual reports
- State Council policy documents
- Five-Year Plan quantum technology sections
- Provincial government quantum investment announcements
  (Anhui/Hefei cluster is primary)

These sources provide the official narrative. Divergence
between the official narrative and the patent/publication
data is itself a signal.

### Procurement and Infrastructure Signals

- Dilution refrigerator procurement (import records, since
  China imports most cryogenic equipment)
- Laser system procurement for neutral atom and photonic programs
- Cleanroom construction and expansion at known quantum facilities
- Fiber deployment contracts for QKD network expansion
- Data center construction in proximity to quantum research
  facilities (potential HNDL storage infrastructure)

### Social Media and Researcher Activity

- Chinese researcher accounts on X, Weibo, WeChat public
  accounts, Zhihu
- Conference attendance and presentation patterns
- Lab website updates and personnel changes
- Graduate student recruitment postings (specialization
  shifts indicate research direction changes)

## Cadence Analysis

The core analytical method. For each tracked entity and each
tracked technical area, maintain a time series of filing/publication
frequency. The baseline cadence is established from historical
data (minimum 24 months).

### Positive signals (cadence increase)

A sustained increase in filing rate in a specific technical area
indicates active development. Scored by:
- Magnitude of increase relative to baseline
- Breadth of claims (narrow incremental vs. broad architectural)
- Whether PCT national phase entry follows domestic filing
- Whether peer-reviewed publications accompany the patents
- Co-filing patterns (multiple entities filing in the same
  area simultaneously suggests coordinated program)

### Negative signals (cadence decrease or cessation)

A sustained decrease or cessation of filing in a previously
active area is potentially the highest-value signal the module
produces. Possible interpretations:

**Classification.** Work has been moved behind state secrecy
barriers. Chinese patent law (Article 4) allows the State
Intellectual Property Office to classify inventions affecting
national security. Classified patents are not published. A
research group that was filing quarterly on quantum error
correction and stops filing while continuing to publish
general-interest papers on unrelated topics may have had their
QEC work classified.

**Military transfer.** Research has been absorbed by PLA or
defense-affiliated entities that do not file public patents.
CETC and its subsidiaries operate partially outside the public
patent system. A civilian lab whose researchers begin
co-authoring with CETC-affiliated authors, followed by
cessation of independent patent activity, suggests transfer.

**Program failure.** The research direction was abandoned.
This interpretation is distinguishable from classification by
examining whether the researchers pivot to new topics (program
failure) or simply go quiet across all topics (classification
or transfer).

**Strategic concealment.** The entity has decided that
publication reveals too much to foreign intelligence. This
is distinguishable from classification by examining whether
the concealment is selective (specific technical areas go
quiet while others continue) or comprehensive (all activity
ceases).

The module does not assert which interpretation is correct.
It flags the cadence anomaly, presents the available evidence
for each interpretation, and leaves assessment to the analyst
(Houston, via the approval workflow).

### Cadence metrics

```
EntityCadence {
    entity: string
    technical_area: IPC_class
    period: monthly
    filing_count: int
    baseline_mean: float     // 24-month rolling average
    baseline_stddev: float
    current_deviation: float // (current - mean) / stddev
    trend: increasing | stable | decreasing | ceased
    months_since_last_filing: int | null
    alert_threshold: 2.0     // standard deviations
}
```

Alert triggers:
- `current_deviation > 2.0` (positive): surge in activity
- `current_deviation < -2.0` (negative): significant decrease
- `months_since_last_filing > 2 * baseline_period` AND
  `baseline_mean > 1.0`: previously active entity has gone quiet
- `trend == ceased` AND entity still active in other areas:
  selective concealment pattern

## Disinformation Detection

Chinese quantum announcements carry specific disinformation
patterns that the module must identify and classify:

**Capability inflation.** State media reports of quantum
"breakthroughs" that overstate the technical content of the
underlying paper. The module compares the state media claim
against the actual paper (if available) and flags discrepancies.
This is the Chinese equivalent of vendor marketing inflation
in the main CRQC Index, but with state propaganda apparatus
behind it.

**Strategic ambiguity.** Deliberate vagueness about whether a
result has military applications. Papers published in civilian
journals with dual-use framing (e.g., "secure communications
for critical infrastructure") that could apply to either
commercial or military deployments. The module flags dual-use
language without asserting intent.

**Decoy publications.** High-volume filing of narrow,
incremental patents that create noise in the patent landscape,
making it harder for foreign analysts to identify which
filings represent genuine capability advances. The module
scores Chinese patents using the same four-dimension rubric
(provenance, specificity, reproducibility, Z impact) as the
main CRQC Index, which naturally filters volume-driven noise.

**Mirror claims.** Chinese papers or patents that closely
replicate Western results without attribution or with
minimal modification. These are sometimes filed to establish
domestic prior art or to create negotiating leverage in
future cross-licensing disputes. The Librarian agent's
citation graph analysis detects these by comparing claim
structure against the existing corpus.

**Coordinated narrative shifts.** When multiple state media
outlets simultaneously change their framing of quantum
technology (e.g., shifting from "decades away" to "strategic
urgency"), this indicates a policy-level decision that may
reflect non-public information about capability status.
Track narrative framing over time and flag coordinated shifts.

## Collaboration Network Analysis

### Structure

The module maintains a temporal graph of the quantum research
collaboration landscape. Nodes are entities at three levels:

- **Individual researchers** (authors on papers and named
  inventors on patents)
- **Institutions** (universities, companies, state labs,
  military-affiliated entities)
- **Programs** (identifiable research programs within
  institutions, tracked by funding acknowledgment strings,
  lab names, or grant numbers in paper metadata)

Edges are co-occurrence relationships:

- Co-authorship on a paper
- Co-inventorship on a patent
- Shared institutional affiliation
- Shared funding source
- Citation relationships (directed)
- Acknowledged collaboration in paper text

Every edge has a timestamp (publication or filing date).
The graph is temporal: edges appear, persist, and disappear.
The evolution of the graph over time is the primary
analytical object.

### What changes in the graph reveal

**Affiliation migration.** A researcher who publishes from
USTC in 2024 and from a CETC subsidiary in 2025 has moved
from civilian to defense-affiliated work. The migration
itself is a data point. The pattern of migrations across
many researchers reveals directional flow: is talent moving
from civilian to military, or the reverse? Is it
concentrated in specific technical areas?

**Collaboration formation.** When two previously unconnected
institutions begin co-authoring, a new program or partnership
has formed. The technical area of the co-authored work
indicates what the partnership targets. Google's acquisition
of Atlantic Quantum (October 2025) and subsequent neutral
atom partnership with CU Boulder (March 2026) would appear
as two new high-weight edges forming in rapid succession,
connecting previously separate clusters.

**Collaboration dissolution.** When co-authorship between
two entities ceases after a sustained period of activity,
the partnership has ended or the work has been classified.
Combined with cadence analysis on the corresponding patent
area, dissolution patterns help distinguish program failure
from classification.

**Cluster formation.** When multiple entities in the same
technical area begin cross-collaborating, a research cluster
is consolidating. The Hefei quantum cluster (USTC, CAS,
Origin Quantum, provincial government funding) is visible
in the graph as a dense subgraph with increasing internal
edge density over time.

**Isolation events.** When a previously well-connected
researcher or institution becomes disconnected from the
collaboration graph (stops co-authoring with external
partners, stops appearing at conferences, stops publishing),
the isolation is a signal. Combined with cadence analysis,
isolation of a key node suggests the node's work has been
absorbed into a classified or restricted program.

### Corporate maneuver tracking

The collaboration graph applies globally, not just to China.
Corporate M&A, hiring, and partnership signals are the
"rough cut" version of the same analysis:

- **Acquisitions:** Google → Atlantic Quantum (superconducting
  fluxonium, Oct 2025). Creates new edge between Google's
  quantum graph cluster and Atlantic Quantum's personnel
  and IP.
- **Hiring:** Google hires Adam Kaufman from CU Boulder/JILA
  for neutral atom program (March 2026). Individual node
  migrates from academic to corporate cluster.
- **Partnerships:** PennyLane/Catalyst + Open Quantum Design
  trapped-ion integration (Jan 2026). New edge between
  Xanadu's software cluster and OQD's hardware cluster.
- **Investment:** Playground Global portfolio includes
  PsiQuantum and xLight. Gelsinger node connects to photonic
  and semiconductor quantum clusters through investment edges.

These maneuvers are publicly announced and easily captured.
They serve as ground truth for calibrating the temporal graph
model before applying it to the harder problem of Chinese
collaboration network analysis where visibility is lower.

### Temporal graph representation

```
Node {
    id: UUID
    type: researcher | institution | program
    name: string
    aliases: string[]          // name variants, transliterations
    affiliations: Affiliation[]  // timestamped list
    technical_areas: IPC_class[]
    first_seen: Date
    last_seen: Date
    activity_status: active | quiet | ceased
}

Affiliation {
    institution_id: UUID
    role: string | null
    start_date: Date | null
    end_date: Date | null
    classification: civilian | military | dual_use | unknown
}

Edge {
    source: UUID
    target: UUID
    type: coauthor | coinventor | affiliation | citation |
          funding | acquisition | hiring | partnership
    timestamp: Date
    weight: int              // number of co-occurrences
    technical_area: IPC_class | null
    source_document: UUID    // paper or patent that created this edge
}

GraphSnapshot {
    timestamp: Date
    nodes: Node[]
    edges: Edge[]
    clusters: Cluster[]      // detected communities
    anomalies: Anomaly[]     // migration, dissolution, isolation events
}
```

### Analysis pipeline

**Extraction.** Sentinel agents extract author names,
affiliations, and funding acknowledgments from papers and
patents. Entity resolution maps name variants and
transliterations to canonical node IDs. This is the hardest
unsolved problem in the pipeline: Chinese researcher names
have many romanization variants, and institutional names
change frequently.

**Graph construction.** Extracted entities and
relationships are added to the edge log. New edges and
node attribute changes (affiliation migrations) are flagged
as events. Daily adjacency matrices are built per
relation type.

**GNN encoding (Layer 1).** The R-GCN processes each
daily graph snapshot independently and produces an
embedding matrix: one vector per node. The GNN has no
memory and no temporal awareness. It is a feature
encoder that maps graph topology to vectors.

Trained on the historical collaboration graph
(backfilled from arXiv and patent data going back to 2015)
with known events (acquisitions, program launches,
classification events that were later confirmed) as
supervision signal where available.

**Temporal comparison (Layer 2).** Operates on the
embedding archive, not on raw graphs. Detects:

- Cluster formation and dissolution
- Affiliation migration flows (civilian → military direction
  and rate)
- Collaboration density changes per technical area
- Node isolation events
- Correlation between embedding trajectory changes and
  cadence anomalies from the patent monitoring system
- Recursive re-evaluation: when new context arrives,
  historical embeddings are reassessed for significance
  that was not apparent at the time of encoding

The temporal comparison layer is arithmetic over
stored vectors, not neural network inference. It runs
in the same Worker as the GNN encoder.

**LLM aggregation (Layer 3).** Receives Layer 2's
structured findings and cross-references them:

- Against cadence analysis from patent monitoring
- Against state media narrative shifts
- Against procurement signals
- Generates candidate Bulletin text for the approval workflow
- Flags anomalies that require escalated review (e.g.,
  a key QEC researcher at USTC migrates to an unidentified
  institution and simultaneously stops publishing)
- Surfaces historical signals that Layer 2's backward pass
  has recontextualized

The language model does not assess intent or make
intelligence judgments. It correlates structured signals
and drafts descriptions for human review.

### Entity resolution challenges

Chinese researcher names present specific difficulties:

- Multiple valid romanizations (Pinyin, Wade-Giles, local
  conventions)
- Common surnames create high collision rates (Wang, Li,
  Zhang account for ~22% of the Chinese population)
- Institutional affiliations are often listed differently
  across papers (abbreviated, translated, or using
  historical names)
- Some researchers publish under different name orderings
  for domestic vs. international venues

The module maintains an entity resolution table with
confirmed mappings (manually verified) and candidate
mappings (automated, pending verification). ORCID IDs
resolve unambiguously when available but adoption is
incomplete in Chinese quantum research.

### Scope limitation

The collaboration network tracks publicly visible
relationships. It cannot see classified collaborations,
internal communications, or unpublished joint work. The
analytical value is in tracking the *boundary* between
visible and invisible: when a visible collaboration goes
dark, that boundary has moved, and the movement is the
signal.

## Integration with CRQC Index

Signals from the China OSINT module feed into the main CRQC
Index through the same Analyst → Approval Gate pipeline.
They appear in Bulletins with source attribution and are
scored identically.

Absence-of-signal alerts appear in the FUD Files section
under a dedicated "Signal Gaps" subcategory, since they are
analytical observations about what is *not* being published,
which is structurally parallel to the FUD Files' function of
analyzing what is being excluded from the Z estimate.

The Threat Surface section's HNDL monitoring incorporates
China-specific indicators:
- Beijing-Shanghai QKD backbone operational status and
  expansion (indicates investment in quantum-secure
  communications for state use)
- Provincial QKD network build-outs (Hubei, Anhui, Guangdong)
- Chinese submarine cable investments and IX presence in
  third countries (potential HNDL collection infrastructure)

## Language and Translation

Chinese-language sources require translation for analysis.

**Automated translation:** LLM-assisted translation of patent
abstracts, paper titles, and state media excerpts. Sufficient
for initial relevance screening by Sentinel agents.

**Human review:** technical claims extracted from Chinese
patents or papers that score above the Tier 1 threshold
receive human review of the original Chinese text before
publication. Machine translation of technical quantum
terminology is error-prone; a mistranslated claim could
produce a false Z delta.

**Terminology concordance:** maintain a Chinese-English
quantum terminology dictionary, updated as new terms emerge.
Core terms are established; emerging terminology (new
algorithm names, hardware architecture terms, program
codenames) must be tracked as it appears.

## Operational Security

The module monitors state-level actors who actively conduct
counterintelligence. Operational considerations:

- Scraping infrastructure should not be attributable to
  SpeakEZ or to the CRQC Index domain
- CNIPA queries should be distributed across time and
  IP addresses to avoid pattern detection
- Researcher social media monitoring should use read-only
  access patterns indistinguishable from normal academic
  interest
- The module's existence and methodology are public (per
  the CRQC Index transparency policy), but the specific
  query patterns and scraping schedules are not published
- No attempt to access non-public Chinese systems,
  classified databases, or restricted networks. The module
  operates exclusively on open-source intelligence.

## Implementation

**Phase 1:** CNIPA patent monitoring via commercial aggregator
API (PatSnap or similar). Cadence baseline establishment for
top 20 tracked entities across 5 IPC classes. Manual analysis.

**Phase 2:** arXiv Chinese-affiliation monitoring added to
existing Sentinel. Cadence analysis automated. Absence-of-signal
alerts operational.

**Phase 3:** State media monitoring. Disinformation detection.
Narrative shift tracking. Integration with FUD Files.

**Phase 4:** Procurement signal monitoring. Social media
researcher tracking. Full cadence dashboard with per-entity,
per-area time series visualization.

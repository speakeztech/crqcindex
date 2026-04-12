---
title: "Proof of Concept: Dimensionally-Verified Quantum Tail Risk Amplification"
linkTitle: "Quantum Tail Risk PoC"
description: "Scaffold for a multi-target quantum amplitude amplification proof-of-concept on AMD Strix Halo"
date: 2026-04-11
authors: ["Houston Haynes"]
status: "Design Draft"
tags: ["Quantum", "Architecture", "Fidelity", "PoC"]
---

## Objective

Demonstrate the Fidelity Framework's DTS and multi-target compilation capabilities through a quantum amplitude amplification algorithm applied to financial tail-risk estimation. The program compiles from Clef source to three execution backends on a single AMD Strix Halo (gfx1151) machine:

- **CPU (x86-64):** Classical Monte Carlo baseline and classical optimizer loop via LLVM
- **GPU (RDNA 3.5):** Quantum circuit simulation of the amplitude amplification oracle via Vulkan compute shaders
- **NPU (XDNA2):** Syndrome-style post-processing and threshold classification via MLIR-AIE

The DTS annotations verify dimensional consistency across all three targets and across the classical/quantum simulation boundary. The coeffect system tracks precision profiles per target.

## Motivation

Classical Monte Carlo tail-risk estimation requires O(1/p) samples to reliably observe events at probability p. For black swan events at p = 10⁻⁶, this demands ~10⁶ samples per evaluation cycle. Quantum amplitude amplification achieves the same statistical coverage in O(1/√p) iterations, a quadratic speedup that reduces the requirement to ~10³ iterations.

This PoC does not require quantum hardware. The amplitude amplification algorithm runs on a GPU-simulated quantum circuit. The algorithmic structure is identical regardless of backend; only the fidelity profile changes. This makes it a clean demonstration of the Fidelity Framework's retargetability thesis: same source, same dimensional guarantees, different hardware with target-specific precision characteristics.

The financial tail-risk domain is chosen because:

1. Dimensional annotations are load-bearing (currency units, probability measures, time horizons)
2. The quadratic speedup is commercially meaningful at realistic problem sizes
3. The classical/quantum boundary has a clear transfer profile (parameter encoding, measurement decoding)
4. Regulatory compliance (Basel III/IV) requires auditable methodology, which DTS verification provides

## Architecture Overview

```
┌─────────────────────────────────────────────────────────┐
│                    Clef Source Program                   │
│  ┌───────────────────────────────────────────────────┐  │
│  │  module TailRisk                                  │  │
│  │    open Fidelity.DTS                              │  │
│  │    open Fidelity.Quantum                          │  │
│  │                                                   │  │
│  │    // Dimensionally-typed risk parameters         │  │
│  │    let threshold : USD = 1_000_000.0<USD>         │  │
│  │    let horizon   : Days = 10<Days>                │  │
│  │    let confidence: Dimensionless = 0.99           │  │
│  │                                                   │  │
│  │    // Oracle: marks states where loss > threshold  │  │
│  │    let tailOracle (state: QState<N>) : QState<N>  │  │
│  │                                                   │  │
│  │    // Amplitude amplification with DTS tracking   │  │
│  │    let amplify (dist: Distribution<USD>)          │  │
│  │              : AmplifiedResult<USD, Probability>  │  │
│  └───────────────────────────────────────────────────┘  │
└──────────────────────┬──────────────────────────────────┘
                       │ Firefly (parse) → PSG
                       ▼
┌─────────────────────────────────────────────────────────┐
│                 Alex (Middle-End)                        │
│  ┌─────────────┐ ┌──────────────┐ ┌──────────────────┐ │
│  │ DTS         │ │ Coeffect     │ │ Target           │ │
│  │ Verification│ │ Analysis     │ │ Partitioning     │ │
│  │             │ │              │ │                  │ │
│  │ USD ∘ Days  │ │ Precision:   │ │ CPU: optimizer   │ │
│  │ consistency │ │  CPU: f64    │ │ GPU: oracle sim  │ │
│  │ through Q   │ │  GPU: f32   │ │ NPU: classifier  │ │
│  │ boundary    │ │  NPU: i16   │ │                  │ │
│  └─────────────┘ └──────────────┘ └──────────────────┘ │
└──────────────────────┬──────────────────────────────────┘
                       │ MLIR dialect emission
                       ▼
┌─────────────────────────────────────────────────────────┐
│                   Composer (Backend)                     │
│                                                         │
│  ┌───────────┐   ┌────────────────┐   ┌──────────────┐ │
│  │ LLVM      │   │ Vulkan SPIR-V  │   │ MLIR-AIE     │ │
│  │           │   │                │   │              │ │
│  │ Classical │   │ Quantum sim    │   │ Threshold    │ │
│  │ optimizer │   │ oracle kernel  │   │ classifier   │ │
│  │ loop      │   │ amplitude amp  │   │ syndrome     │ │
│  │           │   │                │   │ scoring      │ │
│  │ Target:   │   │ Target:        │   │ Target:      │ │
│  │ x86-64    │   │ gfx1151        │   │ XDNA2 NPU   │ │
│  └─────┬─────┘   └───────┬────────┘   └──────┬───────┘ │
└────────┼─────────────────┼────────────────────┼─────────┘
         │                 │                    │
         ▼                 ▼                    ▼
┌─────────────────────────────────────────────────────────┐
│              BAREWire IPC / Shared Memory                │
│  Zero-copy transfer with dimensional tag preservation   │
└─────────────────────────────────────────────────────────┘
```

## Component Design

### 1. Clef Source: Dimensionally-Typed Tail Risk Oracle

The oracle is the core quantum subroutine. It marks computational basis states whose associated portfolio loss exceeds a dimensionally-typed threshold.

```
// Risk factor encoding: N risk factors → N qubits
// Each qubit encodes a discretized range of one risk factor
type RiskFactor =
    | EquityReturn   of range: Interval<Percent>
    | InterestRate   of range: Interval<BasisPoints>
    | FXRate         of range: Interval<Dimensionless>
    | Volatility     of range: Interval<Percent>

// Portfolio loss function: maps risk factor states to USD loss
// DTS verifies: all factor contributions sum to USD
let portfolioLoss (factors: RiskFactor array) : USD =
    factors
    |> Array.map (fun f ->
        match f with
        | EquityReturn r -> position.equity * r.value    // USD * Percent → USD
        | InterestRate r -> position.bonds * duration * r.value  // USD * Years * BasisPoints → USD
        | FXRate r       -> position.forex * r.value     // USD * Dimensionless → USD
        | Volatility r   -> position.options * vega * r.value)   // USD * (USD/Percent) * Percent → USD
    |> Array.sum  // USD + USD + ... → USD

// Oracle: phase-flip states where loss > threshold
// DTS verifies: threshold and portfolioLoss have same dimension (USD)
let tailOracle (threshold: USD) (state: QState<N>) : QState<N> =
    let loss = portfolioLoss (decodeFactors state)
    if loss > threshold then phaseFlip state
    else state
```

The dimensional annotations are not decorative. The compiler statically verifies:

- `position.equity * r.value` produces USD (USD × Percent → USD)
- `position.bonds * duration * r.value` produces USD (USD × Years × BasisPoints → USD, with BasisPoints = Percent/100 and duration in Years cancelling appropriately)
- The `sum` aggregation is over homogeneous USD terms
- The comparison `loss > threshold` compares USD to USD

A unit error (e.g., comparing USD to EUR, or passing BasisPoints where Percent is expected) is a compile-time failure. No existing quantum compilation framework catches this.

### 2. Amplitude Amplification Loop

The classical optimizer loop runs on CPU. It invokes the quantum oracle simulation on GPU, collects measurement results, and iterates.

```
// Optimal iteration count: O(1/√p) where p is tail probability
// DTS verifies: iterations is Dimensionless (count)
let optimalIterations (tailProb: Probability) : int =
    int (Math.PI / (4.0 * sqrt tailProb))

// Main amplification loop
// Runs on CPU; dispatches oracle to GPU
let amplify (portfolio: Portfolio<USD>)
            (threshold: USD)
            (tailProb: Probability)
            : AmplifiedResult<USD, Probability> =

    let iters = optimalIterations tailProb
    let mutable state = uniformSuperposition portfolio.factorCount

    for _ in 1 .. iters do
        // GPU dispatch: oracle evaluation
        state <- tailOracle threshold state     // → Vulkan compute
        // GPU dispatch: diffusion operator
        state <- diffusionOperator state         // → Vulkan compute

    // Measurement: collapse to classical outcome
    // Transfer boundary: QState<N> → MeasurementOutcome<USD>
    let outcome = measure state  // classical result with USD annotation preserved

    { TailProbability = estimateProbability outcome
      ExpectedLoss = estimateConditionalLoss outcome threshold
      IterationsUsed = iters
      FidelityProfile = currentTarget.precisionBounds }
```

### 3. Target Partitioning and Transfer Boundaries

The `fidproj` target declaration specifies the three-target layout:

```
[fidproj]
name = "tail-risk-poc"
version = "0.1.0"

[targets.cpu]
arch = "x86-64"
backend = "llvm"
precision = "f64"
role = "host"  # classical optimizer, orchestration

[targets.gpu]
arch = "gfx1151"
backend = "vulkan-spirv"
precision = "f32"
role = "quantum-sim"  # oracle evaluation, amplitude amplification
compute_api = "vulkan-1.3"

[targets.npu]
arch = "xdna2"
backend = "mlir-aie"
precision = "i16"  # fixed-point for threshold classification
role = "classifier"  # post-measurement syndrome scoring

[transfer.cpu-gpu]
protocol = "barewire"
encoding = "f64-to-f32"
fidelity = { max_ulp_error = 1 }
dimensional_tags = "preserved"

[transfer.gpu-npu]
protocol = "barewire"
encoding = "f32-to-i16-fixed"
fidelity = { scale_factor = 1000, max_quantization_error = 0.001 }
dimensional_tags = "preserved"

[transfer.npu-cpu]
protocol = "barewire"
encoding = "i16-fixed-to-f64"
fidelity = { lossless_in_range = true }
dimensional_tags = "preserved"
```

Each transfer boundary has a DTS-verified encoding profile. The compiler confirms that dimensional tags survive the encoding: a value annotated as USD on the CPU side arrives as USD on the GPU side, even though the numeric representation changes from f64 to f32. The coeffect system tracks the precision loss introduced at each boundary.

### 4. GPU Target: Vulkan Quantum Simulator Kernel

The quantum circuit simulation maps to Vulkan compute shaders. The state vector is a complex-valued array of size 2^N, stored in GPU memory.

```
// Pseudocode for the Vulkan compute shader generated by Composer
// Actual output is SPIR-V via Vulkan MLIR dialect

// State vector: 2^N complex amplitudes
layout(set=0, binding=0) buffer StateVector {
    vec2 amplitudes[];  // vec2 = (real, imag) as f32 pairs
};

// Oracle kernel: phase-flip marked states
layout(local_size_x = 256) in;
void oracleKernel() {
    uint idx = gl_GlobalInvocationID.x;
    if (idx >= stateSize) return;

    // Decode basis state to risk factors
    float loss = evaluatePortfolioLoss(idx);

    // Phase flip if loss > threshold
    if (loss > threshold) {
        amplitudes[idx] = -amplitudes[idx];
    }
}

// Diffusion kernel: reflect about mean amplitude
layout(local_size_x = 256) in;
void diffusionKernel() {
    uint idx = gl_GlobalInvocationID.x;
    if (idx >= stateSize) return;

    // 2|ψ_mean⟩⟨ψ_mean| - I
    vec2 mean = meanAmplitude;  // computed via reduction
    amplitudes[idx] = 2.0 * mean - amplitudes[idx];
}
```

For N = 20 risk factors, the state vector has 2²⁰ = 1,048,576 complex entries, requiring ~8 MB of GPU memory at f32 precision. This is trivially within the RDNA 3.5 capacity. The parallelism maps naturally to GPU workgroups: each work item processes one basis state independently in the oracle kernel.

### 5. NPU Target: Threshold Classification

The NPU handles the post-measurement classification step. After the GPU produces measurement outcomes (sampled basis states with associated loss values), the NPU runs a fixed-point threshold classifier that bins outcomes into risk categories.

This is a lightweight, high-throughput, low-latency task; exactly the NPU's operational profile. The classifier is a decision tree over quantized loss values:

```
// MLIR-AIE target: threshold classifier
// Input: quantized loss values (i16 fixed-point, scale 1000)
// Output: risk category (i8 enum)

// Categories aligned with Basel III/IV risk buckets
type RiskCategory =
    | Normal      = 0   // loss < VaR_95
    | Elevated    = 1   // VaR_95 ≤ loss < VaR_99
    | Severe      = 2   // VaR_99 ≤ loss < VaR_99.9
    | BlackSwan   = 3   // loss ≥ VaR_99.9

// DTS verifies: thresholds are in USD (quantized)
// Coeffect: i16 precision sufficient for bucket boundaries
let classify (loss_q: i16) (thresholds: RiskThresholds<USD>) : RiskCategory =
    if loss_q < thresholds.var95_q then Normal
    elif loss_q < thresholds.var99_q then Elevated
    elif loss_q < thresholds.var999_q then Severe
    else BlackSwan
```

The NPU processes the classification stream at wire speed, freeing the CPU from per-sample branching and the GPU from non-parallel conditional logic.

### 6. BAREWire Transfer Protocol

All inter-target communication uses BAREWire zero-copy IPC with dimensional tag preservation.

```
// BAREWire message schema for oracle results
schema OracleResult {
    basis_state: uint32          // which basis state was measured
    loss_value: float32          // portfolio loss in USD (f32 from GPU)
    amplitude_sq: float32        // |α|² probability estimate
    dimensional_tag: DimTag      // preserved: USD for loss, Dimensionless for probability
}

// BAREWire message schema for classification results
schema ClassificationResult {
    basis_state: uint32
    category: uint8              // RiskCategory enum
    loss_quantized: int16        // loss in USD, fixed-point scale 1000
    dimensional_tag: DimTag      // preserved: USD
}
```

The dimensional tags travel with the data through the IPC channel. The receiving target's BAREWire decoder verifies tag consistency before admitting the value into typed computation. A tag mismatch (e.g., a GPU producing a value tagged as EUR that the NPU expects as USD) is a runtime assertion failure with a clear diagnostic trace back to the source annotation.

## Compilation Pipeline

```
Clef source
    │
    ▼
Firefly ──→ PSG (Program Semantic Graph)
                │
                ├─ DTS verification pass (dimensional consistency, all targets)
                ├─ Coeffect analysis (precision profiles per target)
                ├─ Target partitioning (CPU/GPU/NPU role assignment)
                │
                ▼
           Alex (middle-end)
                │
                ├─ Quantum dialect emission (oracle, diffusion as MLIR ops)
                ├─ Classical dialect emission (optimizer loop, orchestration)
                ├─ Classifier dialect emission (threshold tree)
                │
                ▼
           Composer (backend)
                │
                ├─→ LLVM IR ──→ x86-64 object code (CPU)
                ├─→ Vulkan MLIR ──→ SPIR-V ──→ Vulkan compute shader (GPU)
                └─→ AIE MLIR ──→ XDNA2 instruction stream (NPU)
                │
                ▼
           Linker
                │
                ├─ CPU host binary (orchestrator + classical optimizer)
                ├─ SPIR-V shader module (quantum simulation kernels)
                ├─ AIE binary overlay (classifier)
                └─ BAREWire channel descriptors (transfer boundaries)
```

## What This Demonstrates

1. **DTS across the classical/quantum boundary.** Dimensional annotations propagate from Clef source through MLIR lowering to all three targets. Unit errors are caught at compile time regardless of which target executes the computation.

2. **Multi-target compilation from single source.** One Clef program produces three binaries for three architecturally distinct processors on one SoC. The `fidproj` target declaration governs partitioning; the compiler handles the rest.

3. **Coeffect-tracked precision profiles.** The f64→f32→i16 precision cascade is explicit in the compilation output. The coeffect system reports the cumulative precision budget at each transfer boundary. A computation that would exceed acceptable precision loss at f32 (e.g., distinguishing two tail probabilities separated by less than f32 ULP) produces a coeffect warning.

4. **BAREWire zero-copy IPC with dimensional tags.** Inter-target data transfer preserves type safety without serialization overhead. The dimensional tags are metadata, not payload; they add no runtime cost but provide full audit traceability.

5. **Retargetability.** The same Clef source, with a different `fidproj` declaration, could target:
   - A Catalyst-compatible MLIR backend for execution on trapped-ion hardware
   - A CUDA-Q Quake dialect for NVIDIA GPU simulation
   - A future QPU backend when hardware is available

   The DTS guarantees hold for all targets. Only the fidelity profile changes.

## Scope and Limitations

**What this PoC is:**
- A design-level validation of DTS, coeffect tracking, and multi-target compilation applied to a quantum-relevant algorithm
- A demonstration that dimensional verification catches real errors in hybrid quantum-classical programs
- A concrete artifact for investor and technical audiences showing the framework's differentiation from CUDA-Q, Catalyst, and QIR

**What this PoC is not:**
- A production risk management system
- A claim of quantum advantage (the simulation has no speedup over classical; the algorithmic structure is what transfers to hardware)
- A replacement for existing quantum SDKs (it targets their MLIR dialects as backends, not as competitors)

## Dependencies and Feasibility

| Component | Status | Notes |
|-----------|--------|-------|
| Clef parser (Firefly) | In development | Subset sufficient for PoC |
| DTS verification | Specified (arXiv:2603.16437) | Core pass; PoC exercises dimensional arithmetic |
| Alex MLIR emission | In development | Needs quantum dialect ops (small surface area) |
| Vulkan compute dispatch | Available | RDNA 3.5 Vulkan 1.3 driver on Arch/Omarchy |
| MLIR-AIE toolchain | Available | AMD upstream; targets XDNA2 NPU on Strix Halo |
| LLVM x86-64 backend | Mature | Standard path |
| BAREWire IPC | Specified | Shared-memory path sufficient for single-SoC PoC |
| Quantum MLIR ops | Minimal | Oracle + diffusion; can borrow from Catalyst or define custom |

## Relation to Existing Work

The De Marzo et al. Zipf-Mandelbrot framework provides the diagnostic basis for when amplitude amplification is warranted: when the sampling quality metric Q < 5, classical Monte Carlo is provably undersampling the tail, and quantum amplification provides the corrective. The PoC implements the computational response to that diagnostic.

The Cascade paper (arXiv:2604.08358) is relevant at the retargeting horizon. When the PoC retargets from GPU simulation to QPU hardware, the decoder quality determines the achievable logical error rate, which enters the `fidproj` fidelity profile as a target-specific parameter. The DTS/coeffect machinery treats decoder-induced error the same way it treats f32 quantization error: as a precision bound that propagates through the computation and is reported to the programmer.

## The Erosion of Locality: Three Converging Vectors

The PoC is designed against a single-SoC target today. Its retargeting path assumes that quantum computation will eventually span multiple nodes, not just multiple processors on one chip. Three independent lines of recent work are compressing the timeline for that transition by eroding the locality assumptions that have historically confined quantum computing to single-lab, single-chip experiments.

### Vector 1: Non-local error correction (Cascade, arXiv:2604.08358)

Cascade's neural decoder learns non-local correlations in the error structure of quantum codes. The "waterfall" regime demonstrates that practical logical error rates exceed the naive distance scaling, because the decoder exploits the full coset structure of the code, not just the minimum-weight representatives. This relaxes the assumption that error correction must be local: the decoder's learned message-passing rules capture long-range correlations that fixed-rule decoders miss. The practical effect is that smaller codes achieve lower logical error rates than distance-based estimates predict, reducing the overhead for fault-tolerant operation.

### Vector 2: Non-local encoding (qLDPC and bivariate bicycle codes)

The [144,12,12] Gross code and related qLDPC families encode logical qubits using stabilizers with non-local connectivity, a departure from the surface code's nearest-neighbor-on-a-2D-grid assumption. The encoding rate (12 logical qubits in 144 physical qubits) is dramatically more efficient than the surface code's ~1,000:1 ratio at comparable error rates, precisely because the code exploits non-local check relationships. This is the encoding-side complement to Cascade's decoding-side result: both benefit from abandoning strict locality.

### Vector 3: Non-local qubit distribution (NIST fiber stabilization)

Nardelli et al. (Optica Quantum, April 2026) demonstrated that single photons can traverse 2 km of noisy optical fiber and arrive with >99% indistinguishability and timing jitter below 100 attoseconds. The technique adapts atomic clock stabilization methods: a bright classical reference laser interleaves with single quantum photons thousands of times per second, measuring and correcting fiber-induced phase distortions in real time. Isolation between the classical stabilization channel and the quantum signal channel exceeds 80 billion to one.

This result relaxes the assumption that qubits must be co-located on a single chip. Path-entangled photonic states, where the quantum information is encoded in which fiber the photon traverses, can now be distributed across kilometer-scale networks with fidelity sufficient for entanglement distribution protocols.

### Structural implications for the framework

The NIST stabilization protocol is itself a classical/quantum hybrid control loop: a high-power classical channel (phase reference) coexisting with a single-photon quantum channel over the same physical medium, with real-time feedback correction and an isolation budget between the two. This is a transfer boundary in the Fidelity Framework's vocabulary. The coeffect system would track the classical stabilization channel (high bandwidth, f64 phase measurements) and the quantum signal channel (single photon, path-entangled state) as distinct precision regimes sharing a physical medium, with the isolation ratio entering the fidelity profile as a target-specific parameter.

The Prospero/Olivier actor model extends naturally to this multi-node topology. Prospero orchestrates computation across local processors (CPU, GPU, NPU) via Olivier proxies today. The same model applies when an Olivier proxy addresses a remote quantum node over a stabilized fiber link: the proxy's BAREWire contract specifies the fidelity profile of the link (indistinguishability, timing jitter, isolation ratio), and the coeffect system propagates the link's precision characteristics through the computation graph. The extension from single-SoC to networked quantum is an expansion of the actor topology, not an architectural change.

The convergence of these three vectors, non-local decoding, non-local encoding, and non-local physical distribution, means that the framework's multi-target, transfer-boundary-aware compilation architecture is positioned for the transition from single-chip quantum to distributed quantum without requiring structural rework. The PoC validates the single-SoC case; the architecture accommodates the networked case when the links are ready.

## Next Steps

1. Define the minimal quantum MLIR dialect surface for oracle + diffusion ops
2. Implement the Vulkan compute shader for state vector simulation (standalone, pre-Composer)
3. Implement the BAREWire schema for inter-target transfer
4. Wire the MLIR-AIE classifier as a standalone threshold engine
5. Integrate through Alex once the target partitioning pass is operational
6. Benchmark: classical Monte Carlo (CPU only) vs. simulated amplitude amplification (CPU+GPU+NPU)

---
title: "The Necessary-Sufficient Gap: What Application-Level Benchmarks Mean for CRQC Timelines"
description: "Analysis of how application-level quantum benchmarking reveals the gap between having enough qubits and producing correct answers — and what that means for the CRQC Index's assessment of close rate across cryptographic primitives."
og_image: "/images/crcq_index.png"
status: published
author: winterthunder
date: 2026-04-14
tags: [quantum, benchmarking, CRQC, methodology, QFT, close-rate]
---

## The Gap We've Been Measuring Around

The CRQC Index tracks signals that push or pull Z — the time-to-CRQC factor in Mosca's inequality. Hardware milestones, error correction breakthroughs, cryptanalytic resource estimates, and infrastructure developments all feed into the agents' assessment of how fast the close rate is moving for each cryptographic primitive.

But there's a structural gap in what those signals capture. Qubit counts tell you whether a machine *could* run Shor's algorithm. Gate fidelities tell you whether individual operations are accurate enough. Error correction ratios tell you how many physical qubits you need per logical qubit. These are necessary conditions. They answer: "does this hardware have the right specs?"

They do not answer: "does this hardware produce correct answers when you actually run the algorithm?"

That distinction — between necessary and sufficient conditions for cryptanalytic capability — is the gap that application-level benchmarking measures. A new framework from IonQ (arXiv:2604.11781) provides methodology for quantifying it, and the results are instructive for how the CRQC Index agents should weigh hardware signals.

## Same Qubits, Different Answers

The IonQ paper presents 13 benchmark families spanning chemistry, optimization, machine learning, simulation, and foundational subroutines. The benchmarks measure application-level performance: not gate fidelity in isolation, but whether a complete algorithm running on real hardware produces a correct result.

The most revealing finding is also the simplest. Running a Variational Quantum Eigensolver (VQE) for hydrogen chain ground-state energies at 18 qubits, three IonQ platforms — Aria, Forte, and Forte Enterprise — produce substantially different energy errors. Same qubit count. Same algorithm. Same pre-optimized parameters. Different accuracy.

This is the necessary-sufficient gap made visible. All three platforms have "enough qubits" for the problem. The variable is whether those qubits, wired together in the actual circuit, accumulate errors fast enough to destroy the answer. Qubit count predicts nothing about this. Gate fidelity in isolation predicts some of it. But the full picture only emerges when you run the application and measure what comes out.

For the CRQC Index, this means that a hardware announcement claiming N qubits at fidelity F is a necessary-condition signal. It pushes Z only if there's reason to believe the sufficient conditions are converging at a comparable rate. Application-level benchmarks are how you check.

## QFT: The Canary for Shor's Algorithm

Among the 13 benchmark families, one has direct cryptanalytic relevance: the Quantum Fourier Transform (QFT).

QFT is not a benchmark chosen for convenience. It is a core subroutine of Shor's algorithm — the algorithm that breaks RSA and ECC, and the reason the CRQC Index exists. When Shor's algorithm factors an integer, QFT is the operation that extracts the period of the modular exponentiation function. Without a working QFT at scale, Shor's algorithm does not produce correct factors.

The IonQ paper includes two QFT benchmark variants: Cosine QFT (loading a cosine plane wave and recovering its frequency) and Hidden Phase QFT (recovering a hidden phase through Fourier and Hadamard transforms). Both are designed to resist compiler shortcuts that would trivialize the benchmark — the circuits maintain enough structural complexity that the QFT subroutine is genuinely exercised.

The results show exponential fidelity decay with qubit count. At 12 qubits, Cosine QFT scores are around 0.59. At 36 qubits, they drop to 0.034. The Hidden Phase variant shows a similar trajectory. These are on IonQ's best current hardware.

For context: breaking ECC P-256 requires Shor's algorithm operating on approximately 4,000 logical qubits, each of which requires QFT as a subroutine. The benchmark results at 36 qubits — two orders of magnitude below the target — already show significant fidelity degradation.

This does not mean Shor's is impossible. Error correction exists precisely to address this decay. But it means that the close rate for Shor's viability depends not just on scaling qubit counts but on maintaining QFT fidelity as those counts grow. QFT benchmark performance at increasing scales is the leading indicator the agents should track. When QFT fidelity stops decaying exponentially with qubit count — because error correction is actually working at scale — that is a signal that the sufficient conditions are converging with the necessary ones.

## Time-to-Solution: The Overhead Stack

The paper introduces Time-to-Solution (TTS) as a metric distinct from raw execution time. Execution time measures how long the QPU spends on quantum operations. TTS measures the full pipeline: compilation, optimization, queue wait, QPU fetch, waveform generation, execution, measurement aggregation, post-processing, and result storage.

For a Shor's attack on RSA-2048, TTS matters because the threat model requires the computation to complete in a tractable timeframe. A machine with enough logical qubits to run Shor's but requiring centuries per factorization does not push Z. The temporal component of Mosca's inequality — the Z factor itself — implicitly assumes the computation finishes within a relevant window.

The IonQ paper's TTS measurements show that for current hardware, the full pipeline overhead is substantial. The execution duty cycle on Forte-class systems yields approximately 40-50 minutes of aggregate execution time per hour, with the remainder consumed by calibration, parameter checks, and inter-job overhead. At 36 qubits, per-shot times for QFT benchmarks are measured in hundreds of microseconds, but the total pipeline from submission to results retrieval includes parallelized classical infrastructure that adds its own latency.

TTS extrapolations at cryptographically relevant scales don't exist yet — the hardware isn't there. But the framework establishes the methodology for making them. As hardware scales, TTS measurements will tell us whether the full classical/quantum stack is converging toward tractable Shor's execution times, or whether overhead is growing faster than circuit execution is shrinking. The agents should factor TTS trends into their assessment of whether hardware advances are actually accelerating the close rate.

## Source Credibility

The CRQC Index applies adversarial epistemics to every signal source, and this paper is no exception.

**Commercial interest.** The authors are IonQ personnel. IonQ is a publicly traded quantum computing company (NYSE: IONQ) with direct financial interest in demonstrating hardware differentiation. Benchmark results that show IonQ systems outperforming competitors serve the company's market positioning.

**Mitigating factors.** The framework is open-source, with code available for independent reproduction. The methodology is explicit — benchmark definitions, scoring rubrics, and circuit specifications are fully documented. Critically, the results include IonQ's own older hardware (Aria) underperforming its newer systems (Forte, Forte Enterprise), and in some benchmarks, all systems performing poorly at scale. A vendor publishing unflattering self-comparisons is not typical marketing behavior.

**The MLPerf precedent.** The framework's closed/open division structure is borrowed from MLPerf, the established AI benchmarking standard. This governance model — where closed benchmarks fix the implementation for fair comparison, and open benchmarks fix the success criterion while permitting algorithmic innovation — has proven effective at resisting vendor gaming in the AI hardware market. Its adoption in quantum computing is a structural positive for benchmark credibility.

**Assessment.** The specific benchmark results on IonQ hardware are vendor data and should be treated accordingly. The *methodology* — application-level benchmarking with necessary-sufficient gap measurement, QFT as a Shor's proxy, TTS as a full-stack metric — is the contribution that matters for the CRQC Index. The framework's value is independent of which vendor's hardware produces the best numbers.

## Integration into Z Estimation

Application-level benchmarks give the CRQC Index agents a secondary calibration dimension for assessing signals that push or pull Z.

The mechanism is straightforward. When a hardware signal arrives — a new qubit count milestone, a fidelity improvement, an error correction demonstration — the agents currently assess it against resource estimates for each cryptographic primitive. "Does this bring us closer to 4,000 logical qubits for ECC P-256?" That assessment captures the necessary conditions.

Application-level benchmark data adds a question: "On hardware with these specs, does QFT actually work at the scales demonstrated?" If the answer is yes and fidelity is improving, the hardware signal's push on Z is corroborated — the sufficient conditions are converging. If the answer is no — if QFT fidelity is plateauing or degrading despite improving specs — then the hardware signal is moving the necessary line without moving the sufficient line, and the agents should widen their confidence interval on the close rate.

This is not a rigid classification. The agents assess the landscape from the data. As QFT benchmarks scale toward cryptographically relevant sizes — hundreds and eventually thousands of logical qubits — these signals naturally carry more weight. The transition from contextual signal to primary signal is not a gate to pass through; it's a gradient the agents navigate as the evidence accumulates.

For organizations using Mosca's inequality to assess their own exposure: the necessary-sufficient gap is a reminder that Z is not a single number arriving on a single day. It's a factor under tension, and the distance between "hardware exists with enough qubits" and "hardware produces correct Shor's factorizations" may itself span years. Your migration timeline (Y) should account for both.

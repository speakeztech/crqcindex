---
title: "Z Estimation Model"
description: "Technical reference for the CRQC Index Z estimate methodology — signal taxonomy across hardware, error correction, cryptanalysis, and infrastructure dimensions with per-primitive scoring for ECC, RSA, and AES."
og_image: "/images/crcq_index.png"
status: published
author: winterthunder
date: 2026-04-01
tags: [methodology, Z-estimate, scoring]
---

## Overview

The Z estimate represents the CRQC Index's central output: a per-primitive estimate of time-to-CRQC with confidence intervals. It is not a prediction but a synthesis of observable signals weighted by reliability, recency, and corroboration.

## Signal Taxonomy

Signals are classified across four dimensions:

- **Hardware Milestones**: Qubit counts, coherence times, gate fidelities, architectural innovations
- **Error Correction**: Logical qubit demonstrations, threshold results, new codes
- **Cryptanalysis**: Algorithm improvements, novel attack vectors, resource estimates
- **Infrastructure**: Networking, interconnects, control systems, supply chain indicators
- **Application-Level Benchmarks** (secondary calibration): QFT fidelity at scale, algorithm accuracy across platforms, time-to-solution measurements — these help the agents assess whether hardware signals are closing the necessary-sufficient gap

## Scoring Rubric

Each signal receives a composite score based on:

1. **Source reliability** (0-1): Peer-reviewed publication, preprint, conference talk, press release, rumor
2. **Reproducibility** (0-1): Independent verification, replication attempts, open methodology
3. **Recency decay**: Exponential decay with 18-month half-life
4. **Corroboration bonus**: Signals reinforced by independent sources receive multiplicative weight

## Secondary Calibration Signals

Primary signals (hardware milestones, error correction, cryptanalysis, infrastructure) push or pull Z directly. Secondary calibration signals don't independently move Z — they help the agents assess whether primary signals are actually closing the gap between necessary and sufficient conditions for cryptanalytic capability.

Having enough qubits to run Shor's algorithm is necessary. Producing correct factorizations on real hardware at scale is sufficient. The distance between those two conditions is not zero, and it may itself span years. Secondary calibration signals measure that distance.

**Application-level benchmarking** is the first named class of secondary calibration signal. When a hardware signal pushes Z — a new qubit count milestone, a fidelity improvement — application-level benchmark data tells the agents whether the sufficient conditions are converging at the same rate as the necessary conditions. If QFT fidelity is improving at scale, the push is corroborated. If QFT fidelity is plateauing despite improving specs, the agents widen their confidence interval on the close rate.

This is not a rigid gate. The agents assess the landscape from the data. As application-level benchmarks scale toward cryptographically relevant sizes, they naturally carry more weight in the agents' assessment of close rate.

## Per-Primitive Estimates

Z is computed independently for each cryptographic primitive:

- **ECC P-256**: Most exposed to Shor's algorithm improvements. QFT benchmark results at scale are the application-level proxy for Shor's viability against this primitive — QFT is a direct subroutine of Shor's algorithm.
- **RSA-2048**: Classical factoring baseline, large key size provides some buffer. Same QFT dependency as ECC — Shor's algorithm requires functioning QFT at the logical qubit scale needed for factorization.
- **ECC P-384**: Higher security margin but same algorithmic vulnerability class
- **AES-256**: Grover's algorithm provides quadratic speedup; effective security reduces to 128-bit equivalent

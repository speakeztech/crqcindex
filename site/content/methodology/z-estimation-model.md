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

## Scoring Rubric

Each signal receives a composite score based on:

1. **Source reliability** (0-1): Peer-reviewed publication, preprint, conference talk, press release, rumor
2. **Reproducibility** (0-1): Independent verification, replication attempts, open methodology
3. **Recency decay**: Exponential decay with 18-month half-life
4. **Corroboration bonus**: Signals reinforced by independent sources receive multiplicative weight

## Per-Primitive Estimates

Z is computed independently for each cryptographic primitive:

- **ECC P-256**: Most exposed to Shor's algorithm improvements
- **RSA-2048**: Classical factoring baseline, large key size provides some buffer
- **ECC P-384**: Higher security margin but same algorithmic vulnerability class
- **AES-256**: Grover's algorithm provides quadratic speedup; effective security reduces to 128-bit equivalent

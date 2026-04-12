---
title: "The Price of Retrofit: What $200 Bitcoin Transactions Teach About PQC Migration"
date: 2026-04-12
authors: ["Houston Haynes"]
status: "Draft Bulletin / Commentary"
tags: ["Security", "Quantum", "Bitcoin", "Migration"]
---

On April 9, 2026, StarkWare researcher Avihu Levy published
a working scheme for quantum-safe Bitcoin transactions that
requires no changes to the Bitcoin protocol. The cryptographic
construction is genuinely clever: it replaces elliptic curve
security assumptions with hash-based puzzles and Lamport
signatures, fits within Bitcoin's legacy scripting constraints
(201 opcodes, 10,000 bytes), and is deployable today.

Each transaction costs $75 to $200 in GPU compute.

A standard Bitcoin transaction costs about 33 cents.

That ratio, 200x to 600x, is not a commentary on the quality
of Levy's work. He solved an extraordinarily constrained
optimization problem and produced a result that many assumed
was impossible. The ratio is a commentary on what happens when
post-quantum security is retrofitted onto infrastructure that
was never designed to accommodate it.

## The Constraint Creates the Cost

Bitcoin's scripting system was specified in 2009. Its limits,
201 opcodes and 10,000 bytes, were designed for an era when
the largest foreseeable cryptographic operation was a single
ECDSA signature verification. Post-quantum signature schemes
(SPHINCS+, for example) produce signatures that are tens of
kilobytes. They do not fit. Lamport signatures fit, barely,
but only by requiring the transaction creator to solve a
brute-force hash puzzle that takes 70 trillion attempts to
find a valid solution. That puzzle is the $200.

The cost is not GPU cycles. GPU cycles are cheap and getting
cheaper. The cost is the gap between what the system was built
to do and what it is now being asked to do. Every dollar of
that $200 is a dollar of architectural debt, accumulated over
17 years of operating a cryptographic system whose foundational
assumptions were known to have a shelf life.

Levy himself frames QSB as a "last resort measure." He
co-authored BIP-360, the protocol-level quantum-resistant
address proposal that was merged into Bitcoin's BIP repository
in February 2026. BIP-360 is the correct long-term solution.
It requires a soft fork. Soft forks require community consensus.
Community consensus on Bitcoin takes years. QSB exists because
the governance timeline and the threat timeline may not align.

## The Universal Lesson

Bitcoin is an extreme case, but the dynamic is not unique.
Every system that defers PQC migration will eventually face
its own version of the $200 transaction.

The cost takes different forms in different contexts:

**Financial services.** Re-encrypting 20 years of archived
transaction records with post-quantum algorithms. The records
were encrypted with RSA or ECC key exchange. The keys must be
rederived or the ciphertext must be decrypted and re-encrypted
under new keys. The computational cost scales with the volume
of the archive. The operational cost scales with the number of
systems that must be taken offline, migrated, validated, and
returned to production. Organizations that begin migration
now, while their systems are operational and their teams are
available, pay the cost incrementally. Organizations that wait
until a CRQC is demonstrated pay the cost under emergency
conditions, competing with every other organization for the
same scarce expertise, under regulatory pressure, with their
historical data already exposed.

**Healthcare.** Patient records carry 30-year confidentiality
requirements. HIPAA compliance demands data protection for the
lifetime of the record. Records encrypted with ECC in 2020 and
subject to HNDL collection are already potentially compromised;
the exposure just hasn't materialized yet. Re-encrypting
decades of medical records is operationally complex, requires
system-by-system validation, and cannot be done while systems
are serving patients without careful orchestration. The longer
the deferral, the larger the archive, and the narrower the
window.

**Critical infrastructure.** SCADA and ICS systems have
20-30 year lifecycles. Many are running cryptographic
implementations that predate the PQC standardization effort.
Some cannot be updated without physical site visits. Some
cannot be updated at all and must be replaced. The replacement
cost is the retrofit cost, and it grows with every year of
continued operation on vulnerable primitives.

**Code signing.** Every signed firmware image, every signed
software update, every signed driver distributed since the
signing root was established is retroactively compromisable
if the signing key is derived from a captured key exchange.
The cost of rotating a signing root is not the cryptographic
operation. It is the downstream validation: every device that
trusts the old root must be updated to trust the new root.
For a platform with billions of deployed devices, this is
a logistics problem, not a cryptography problem. And it gets
harder, not easier, with time.

## The Compounding Dynamic

Architectural debt compounds. A system that defers PQC
migration by one year accumulates one additional year of
data encrypted under vulnerable primitives, one additional
year of signed artifacts chained to vulnerable roots, one
additional year of key material that must be rotated, and
one additional year of operational procedures that must be
retrained.

The $200 transaction is what compounding looks like at the
limit. Bitcoin's scripting system has not changed in 17
years. The cryptographic threat has evolved continuously.
The gap between the two is now wide enough that bridging
it, even temporarily, even partially, even by the most
skilled engineer available, costs 600 times what the
operation should cost.

The question for every organization is not "will we face
a $200 transaction equivalent?" It is "how large is our
gap, and how fast is it growing?" The organizations
currently deferring PQC migration are accumulating
architectural debt at the same rate Bitcoin has accumulated
it since 2009. The difference is that most of them have the
ability to soft-fork their own infrastructure. Bitcoin's
governance model makes protocol changes extraordinarily
difficult. An enterprise controls its own systems. It can
rotate keys, update protocols, re-encrypt archives, and
retrain staff on its own schedule.

That ability is the advantage. Using it is the decision.

## What This Means for the CRQC Index

The QSB paper is a Tier 2 contextual signal. It does not
change the Z estimate. It does not affect when any
primitive falls. It demonstrates what the cost function
looks like for one specific ecosystem that has deferred
migration until the constraint envelope is nearly
exhausted. The CRQC Index tracks the timeline. The QSB
cost is what the timeline means in practice for systems
that wait.

The Bulletin for this signal will note the paper, state
the security parameters (118-bit classical, ~59-bit
effective under Grover, below NIST's 128-bit PQ floor),
and present the $200 cost as a data point on the
universal cost curve of deferred migration. Equal
treatment with all other ecosystem remediation signals.
The lesson is not about Bitcoin. The lesson is about
the price of waiting.

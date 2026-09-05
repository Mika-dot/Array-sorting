# Array Sorting — algorithms, benchmarks & modern research

A comparative sorting laboratory in C#. The repository started as a catalogue of classic algorithms and now also contains reproducible benchmarks, adaptive sorting, parallel sorting and learning-augmented experiments.

> `main` is the project index. Research implementations stay in dedicated branches so the original code and measurements remain traceable.

[![Modern sorting CI](https://github.com/Mika-dot/Array-sorting/actions/workflows/modern-sorting-ci.yml/badge.svg?branch=feature%2Fmodern-sorting-research)](https://github.com/Mika-dot/Array-sorting/actions/workflows/modern-sorting-ci.yml)

## Repository map

| Branch | Purpose | Highlights |
|---|---|---|
| [`main`](https://github.com/Mika-dot/Array-sorting/tree/main) | Original collection + global index | 27 classic/educational algorithms, original synthetic and real timing plots |
| [`feature/extended-sorting-algorithms`](https://github.com/Mika-dot/Array-sorting/tree/feature/extended-sorting-algorithms) | First benchmark/research extension | Heap, Cycle, Patience, parallel merge sort, deterministic Python benchmark suite, scaling/input-shape/operation-count charts |
| [`feature/modern-sorting-research`](https://github.com/Mika-dot/Array-sorting/tree/feature/modern-sorting-research) | Current research | PowerSort, PSRS, IntroSort, PDQ-inspired sort, learning-augmented bucket baseline, signed LSD radix, .NET 8 smoke tests and CI |

The old long-form README is preserved permanently in Git history: [README before the research-index rewrite](https://github.com/Mika-dot/Array-sorting/blob/6ba3943fe25cbc481ececda2051d014f9d30e46d/README.md).

---

## Algorithm catalogue

### Classic collection (`main`)

These algorithms are implemented in [`sorting/sorting/SortingSmart.cs`](sorting/sorting/SortingSmart.cs). Complexity below describes the algorithm family and, where relevant, the behaviour of the current implementation rather than an idealized textbook variant.

| Algorithm | Family | Best | Average | Worst | Extra memory | Stable | Notes |
|---|---|---:|---:|---:|---:|:---:|---|
| Bubble | exchange | `O(n²)`* | `O(n²)` | `O(n²)` | `O(1)` | Yes | Current legacy method has no early-exit optimization. |
| Shaker | bidirectional exchange | `O(n)` | `O(n²)` | `O(n²)` | `O(1)` | Yes | Stops when a complete pass has no swaps. |
| Insertion | insertion | `O(n)` | `O(n²)` | `O(n²)` | `O(1)` | Yes | Very useful for short/nearly sorted ranges. |
| Stooge | recursive curiosity | `Θ(n^2.709…)` | `Θ(n^2.709…)` | `Θ(n^2.709…)` | recursion | No | Educational, intentionally impractical. |
| Pancake | prefix reversal | `O(n²)` | `O(n²)` | `O(n²)` | `O(1)` | No | Sorting using prefix flips. |
| Shell | gap insertion | gap-dependent | gap-dependent | typically `O(n²)` | `O(1)` | No | Result strongly depends on gap sequence. |
| Merge | merge | `O(n log n)` | `O(n log n)` | `O(n log n)` | `O(n)` | implementation-dependent | Predictable comparison sorting. |
| Selection | selection | `Θ(n²)` | `Θ(n²)` | `Θ(n²)` | `O(1)` | No | Few writes, many comparisons. |
| Quick | partition | `O(n log n)` | `O(n log n)` | `O(n²)` | recursion | No | Legacy Lomuto-style implementation can degrade on adversarial order. |
| Gnome | exchange/insertion | `O(n)` | `O(n²)` | `O(n²)` | `O(1)` | Yes | Simple adjacent-swap method. |
| Tree | BST | `O(n log n)` | `O(n log n)` | `O(n²)` | `O(n)` | No | Unbalanced tree can degenerate. |
| Comb | diminishing-gap exchange | near `O(n log n)` on favorable data | implementation-dependent | `O(n²)` | `O(1)` | No | Removes turtles faster than Bubble. |
| BasicCounting | counting | `O(n + k)` | `O(n + k)` | `O(n + k)` | `O(k)`+ | depends | Integer-domain sort; `k` is value range. |
| CombinedBubble | exchange hybrid | implementation-dependent | `O(n²)` | `O(n²)` | `O(1)` | depends | Legacy experimental Bubble variant. |
| Heapify | heap | `O(n log n)` | `O(n log n)` | `O(n log n)` | `O(1)` | No | Heap-based ordering. |
| Cocktail | bidirectional exchange | `O(n)` | `O(n²)` | `O(n²)` | `O(1)` | Yes | Cocktail/Shaker family. |
| OddEven | odd-even transposition | `O(n)` | `O(n²)` | `O(n²)` | `O(1)` | Yes | Parallel-friendly phase structure. |
| Tim | adaptive merge/insertion | `O(n)` | `O(n log n)` | `O(n log n)` | `O(n)` | Yes | Run-oriented hybrid family. |
| Counting | counting | `O(n + k)` | `O(n + k)` | `O(n + k)` | `O(n + k)` | Yes* | Efficient when the key range is moderate. |
| Radix | digit distribution | `O(d(n+b))` | `O(d(n+b))` | `O(d(n+b))` | `O(n+b)` | Yes* | `d` passes, radix/base `b`. |
| Bucket | distribution | `O(n+k)` expected | `O(n+k)` expected | `O(n²)` | `O(n+k)` | depends | Performance depends on distribution and bucket policy. |
| BinaryInsertion | insertion | `O(n)` | `O(n²)` | `O(n²)` | `O(1)` | Yes* | Binary search reduces comparisons, **not element shifts**. |
| Bogo | randomized curiosity | `O(n)` | `O(n·n!)` expected | unbounded | `O(1)` | No | Demonstration only. |
| Cycle | minimum-write | `O(n²)` | `O(n²)` | `O(n²)` | `O(1)` | No | Designed to minimize writes. |
| Exchange | exchange | `Θ(n²)` | `Θ(n²)` | `Θ(n²)` | `O(1)` | No | Simple pairwise exchange sort. |
| Heap | heap | `O(n log n)` | `O(n log n)` | `O(n log n)` | `O(1)` | No | Deterministic worst-case bound. |
| MSDRadix | MSD distribution | `O(d(n+b))` typical | `O(d(n+b))` typical | distribution-dependent | `O(n+b)` | depends | Most-significant-digit radix family. |

`*` Stability/best-case details can depend on the exact implementation. Legacy code is retained as historical/educational material; the modern branch has dedicated correctness CI.

### Extended and modern research

| Algorithm | Branch | Stable | Complexity | Extra memory | Why it is here |
|---|---|:---:|---|---|---|
| Patience | extended | No | `O(n log n)` with efficient pile/merge structures | `O(n)` | Pile-based sorting; closely related to LIS techniques. |
| Multithreaded merge | extended | Yes | `O(n log n)` work | `O(n)` | Simple parallel merge-sort baseline. |
| IntroSort | modern | No | `O(n log n)` worst-case | `O(log n)` | Quicksort speed + heapsort fallback + insertion cutoff. |
| PDQ-inspired | modern | No | `O(n)` ordered fast path; `O(n log n)` fallback | `O(log n)` | Pattern-aware introspective experiment; explicitly **not** an exact pdqsort port. |
| PowerSort | modern | Yes | `O(n)` on one natural run; `O(n log n)` worst | `O(n)` | Adaptive natural-run merge policy; used as the merge-policy direction in modern Python sorting. |
| PSRS | modern | No | `O(n log n)` local work + sampling/merge | `O(n+p²)` here | Parallel Sorting by Regular Sampling for multicore arrays. |
| LinearModelBuckets | modern | No | distribution-sensitive, exact `O(n log n)` fallback | `O(n+b)` | Transparent learning-augmented / model-guided sorting baseline. |
| SignedLsdRadix | modern | Yes | four `O(n+256)` passes for `Int32` | `O(n+256)` | Predictable linear-time integer sorting independent of input order. |

### Research backlog — deliberately **not** marked implemented

| Algorithm / direction | Status | Reason |
|---|---|---|
| Smoothsort | Not implemented | The old `Smooth()` function was only `return Heap(array)` and therefore was mislabeled. It is now disabled in the research branches until a real Leonardo-heap implementation exists. |
| Han's sort | Not implemented | Kept as an advanced integer-sorting research topic rather than a fake checkbox. |
| Virtual-Memory Powersort (2026) | Next target | Recent low-extra-memory Powersort variant. |
| Exact pdqsort / driftsort comparison | Next target | Requires faithful engineering-level ports and C# microbenchmarks, not simplified textbook aliases. |
| IPS4o-style parallel sample sort | Next target | Useful multicore comparison against PSRS. |

---

## Benchmarks and graphs

There are three generations of measurements in the repository. Do not compare their absolute milliseconds directly unless they were produced by the same benchmark harness and machine.

### 1. Original repository measurements (`main`)

The original per-algorithm plots are retained in:

- [`complexity in graphics/`](complexity%20in%20graphics/) — synthetic measurements.
- [`complexity in graphics real/`](complexity%20in%20graphics%20real/) — original real-run measurements.

Representative examples:

| Bubble | Heap |
|---|---|
| ![Legacy Bubble graph](complexity%20in%20graphics%20real/Bubble.PNG) | ![Legacy Heap graph](complexity%20in%20graphics%20real/Heap.PNG) |

### 2. Deterministic comparative benchmark (`feature/extended-sorting-algorithms`)

This branch introduced fixed-seed inputs, correctness checks, source CSV files and plots that compare multiple algorithms on identical arrays.

![Efficient algorithms scaling](https://github.com/Mika-dot/Array-sorting/blob/feature/extended-sorting-algorithms/docs/assets/benchmarks/efficient-scaling.png?raw=1)

![Input order sensitivity](https://github.com/Mika-dot/Array-sorting/blob/feature/extended-sorting-algorithms/docs/assets/benchmarks/input-shapes.png?raw=1)

Additional extended plots:

- [Quadratic scaling](https://github.com/Mika-dot/Array-sorting/blob/feature/extended-sorting-algorithms/docs/assets/benchmarks/quadratic-scaling.png)
- [Comparison/write growth](https://github.com/Mika-dot/Array-sorting/blob/feature/extended-sorting-algorithms/docs/assets/benchmarks/operation-growth.png)
- [Sorting process visualization](https://github.com/Mika-dot/Array-sorting/blob/feature/extended-sorting-algorithms/docs/assets/benchmarks/sorting-process.png)
- [Benchmark generator](https://github.com/Mika-dot/Array-sorting/blob/feature/extended-sorting-algorithms/tools/generate_sorting_benchmarks.py)

### 3. Modern research benchmark (`feature/modern-sorting-research`)

The modern branch adds adaptive, integer, model-guided and introspective algorithms. Inputs use deterministic seed `20260905`; every measured result is checked against the reference sorted array.

![Modern scaling](https://github.com/Mika-dot/Array-sorting/blob/feature/modern-sorting-research/docs/assets/modern/modern-scaling.svg?raw=1)

![Modern input sensitivity](https://github.com/Mika-dot/Array-sorting/blob/feature/modern-sorting-research/docs/assets/modern/modern-input-shapes.svg?raw=1)

Source data:

- [modern-scaling.csv](https://github.com/Mika-dot/Array-sorting/blob/feature/modern-sorting-research/docs/assets/modern/modern-scaling.csv)
- [modern-input-shapes.csv](https://github.com/Mika-dot/Array-sorting/blob/feature/modern-sorting-research/docs/assets/modern/modern-input-shapes.csv)
- [generator](https://github.com/Mika-dot/Array-sorting/blob/feature/modern-sorting-research/tools/generate_modern_benchmarks.py)

### What the modern experiment shows

The reference benchmark is intentionally not used to claim that Python implementations predict C# wall-clock speed. It is useful for algorithm behaviour:

- PowerSort collapses toward linear work on already sorted/reverse-natural-run inputs because it discovers long runs.
- The PDQ-inspired experiment has a strong ordered fast path but can still be slower on patterns such as reverse order; that is exactly why it is labelled “inspired” rather than passed off as the production pdqsort implementation.
- Signed LSD radix performs almost the same amount of work regardless of ordering or duplicate structure.
- The linear model bucket baseline can be competitive on distributions that fit its coarse model, while skewed data exposes the weakness of a one-parameter predictor.

---

## Recent sorting research tracked by this repository

### Powersort and CPython

Powersort was proposed as a nearly-optimal adaptive merge policy and later became the merge policy used by CPython's list sort. The modern branch implements the natural-run/node-power core so its behaviour can be measured alongside the older algorithms.

- Paper: https://arxiv.org/abs/1805.04154
- CPython design notes: https://github.com/python/cpython/blob/main/Objects/listsort.txt

### Pattern-defeating quicksort

Pattern-defeating quicksort is a modern quicksort family designed to keep strong practical quicksort performance while defending against partition patterns that normally produce bad behaviour.

- Paper: https://arxiv.org/abs/2106.05123

### Rust's newer sorting implementation

Rust replaced its standard-library sorting internals in the Rust 1.81 timeframe. The current stable-sort implementation is built around `driftsort`, a hybrid combining merge-sort and quicksort ideas and using a Powersort-style heuristic. This repository does not pretend that a short C# function is a faithful driftsort port; it is listed as a dedicated future implementation/benchmark target.

- Rust 1.81 release notes: https://blog.rust-lang.org/2024/09/05/Rust-1.81.0/
- Current driftsort source: https://doc.rust-lang.org/src/core/slice/sort/stable/drift.rs.html

### Parallel sample sorting / IPS4o

IPS4o is an in-place parallel super-scalar sample-sort design intended for multicore and cache-efficient execution. The repository currently uses PSRS as the simpler parallel baseline before attempting an IPS4o-style implementation.

- Paper: https://arxiv.org/abs/1705.02257

### Learning-augmented sorting

Learned sorting can be viewed as model-guided SampleSort: a model estimates approximate ranks/buckets and an exact sorting stage finishes the result. `LinearModelBuckets` is intentionally simple so failure modes remain visible rather than hidden behind a large ML dependency.

- Analysis and parallelization of LearnedSort: https://arxiv.org/abs/2307.08637

### Virtual-Memory Powersort — 2026

A paper published at ESA 2026 introduces **Virtual-Memory Powersort**, reducing the auxiliary buffer requirement of Powersort from roughly half the input to `O(sqrt(n log n))` while retaining near-optimal merge behaviour up to additive linear overhead. This is the newest concrete algorithmic direction currently queued for this repository.

- ESA 2026 paper: https://doi.org/10.4230/LIPIcs.ESA.2026.14

---

## Reproduce the experiments

### Original C# project

The historical `main` project targets `.NET Core 3.1` and is intentionally left as the original baseline.

### Modern C# research

```bash
git switch feature/modern-sorting-research
dotnet run --project sorting/tests/SortingSmoke/SortingSmoke.csproj -c Release
```

The modern branch targets `.NET 8` and has GitHub Actions CI. The smoke suite checks empty/singleton inputs, negatives, duplicates, sorted/reverse order and deterministic random arrays up to 10,000 elements without allowing algorithms to mutate their input.

### Extended benchmark plots

```bash
git switch feature/extended-sorting-algorithms
python3 -m pip install -r tools/requirements.txt
python3 tools/generate_sorting_benchmarks.py
```

### Modern benchmark plots

```bash
git switch feature/modern-sorting-research
python3 -m pip install -r tools/requirements.txt
python3 tools/generate_modern_benchmarks.py
```

The Python plots are **reference-algorithm experiments**, not C# deployment benchmarks. The next benchmarking step is BenchmarkDotNet with wall-clock time, allocations, hardware counters and thread scaling on the C# implementations.

---

## Current roadmap

- [x] Preserve the original sorting catalogue and per-algorithm plots.
- [x] Add deterministic comparative benchmarks and CSV source data.
- [x] Add Heap, Cycle, Patience and multithreaded merge-sort experiments.
- [x] Remove the falsely labelled HeapSort-as-Smoothsort implementation.
- [x] Add IntroSort.
- [x] Add a pattern-aware PDQ-inspired introspective sort.
- [x] Add adaptive PowerSort.
- [x] Add PSRS multicore sorting.
- [x] Add signed 32-bit LSD radix sort.
- [x] Add a transparent model-guided sorting baseline.
- [x] Move the modern research branch to `.NET 8` and add CI/smoke tests.
- [ ] Implement real Leonardo-heap Smoothsort.
- [ ] Implement Virtual-Memory Powersort (ESA 2026).
- [ ] Add a faithful driftsort/pdqsort comparison branch.
- [ ] Add BenchmarkDotNet and multicore scaling plots for the C# implementations.
- [ ] Compare PSRS with an IPS4o-style parallel sample sort.
- [ ] Add a stronger monotone/piecewise learned rank model and adversarial-distribution tests.

---

## Project principle

A sorting method is marked **implemented** only when there is actual code for that method. If an implementation is a substitute, approximation or research-inspired variant, the README says so explicitly. Benchmarks keep source data and fixed seeds so plots can be regenerated instead of being decorative images.

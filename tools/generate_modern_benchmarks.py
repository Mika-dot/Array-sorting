#!/usr/bin/env python3
"""Deterministic reference benchmarks for the modern sorting research branch.

These are Python reference implementations for algorithmic comparison only.
They are not intended to predict .NET wall-clock performance.
"""
from __future__ import annotations

import csv
import math
import random
import statistics
import time
from dataclasses import dataclass
from pathlib import Path
from typing import Callable

import matplotlib.pyplot as plt

ROOT = Path(__file__).resolve().parents[1]
OUTPUT = ROOT / "docs" / "assets" / "modern"
SEED = 20260905
INSERTION_THRESHOLD = 24


def insertion_range(a: list[int], lo: int, hi: int) -> None:
    for i in range(lo + 1, hi + 1):
        value = a[i]
        j = i - 1
        while j >= lo and a[j] > value:
            a[j + 1] = a[j]
            j -= 1
        a[j + 1] = value


def sift_down(a: list[int], root: int, count: int, offset: int) -> None:
    while True:
        child = root * 2 + 1
        if child >= count:
            return
        if child + 1 < count and a[offset + child] < a[offset + child + 1]:
            child += 1
        if a[offset + root] >= a[offset + child]:
            return
        a[offset + root], a[offset + child] = a[offset + child], a[offset + root]
        root = child


def heap_range(a: list[int], lo: int, hi: int) -> None:
    count = hi - lo + 1
    for i in range(count // 2 - 1, -1, -1):
        sift_down(a, i, count, lo)
    for end in range(count - 1, 0, -1):
        a[lo], a[lo + end] = a[lo + end], a[lo]
        sift_down(a, 0, end, lo)


def partition_median3(a: list[int], lo: int, hi: int) -> int:
    mid = lo + (hi - lo) // 2
    if a[mid] < a[lo]:
        a[lo], a[mid] = a[mid], a[lo]
    if a[hi] < a[lo]:
        a[lo], a[hi] = a[hi], a[lo]
    if a[hi] < a[mid]:
        a[mid], a[hi] = a[hi], a[mid]
    pivot = a[mid]
    a[mid], a[hi - 1] = a[hi - 1], a[mid]
    i, j = lo, hi - 1
    while True:
        i += 1
        while a[i] < pivot:
            i += 1
        j -= 1
        while a[j] > pivot:
            j -= 1
        if i >= j:
            break
        a[i], a[j] = a[j], a[i]
    a[i], a[hi - 1] = a[hi - 1], a[i]
    return i


def _intro(a: list[int], lo: int, hi: int, depth: int) -> None:
    while hi - lo > INSERTION_THRESHOLD:
        if depth == 0:
            heap_range(a, lo, hi)
            return
        depth -= 1
        pivot = partition_median3(a, lo, hi)
        if pivot - lo < hi - pivot:
            _intro(a, lo, pivot - 1, depth)
            lo = pivot + 1
        else:
            _intro(a, pivot + 1, hi, depth)
            hi = pivot - 1
    if lo < hi:
        insertion_range(a, lo, hi)


def intro_sort(values: list[int]) -> list[int]:
    a = values.copy()
    if len(a) > 1:
        _intro(a, 0, len(a) - 1, 2 * (len(a).bit_length() - 1))
    return a


def _is_sorted(a: list[int], lo: int, hi: int) -> bool:
    return all(a[i - 1] <= a[i] for i in range(lo + 1, hi + 1))


def _pdq_inspired(a: list[int], lo: int, hi: int, budget: int) -> None:
    while hi - lo > INSERTION_THRESHOLD:
        if _is_sorted(a, lo, hi):
            return
        if budget == 0:
            heap_range(a, lo, hi)
            return
        pivot = partition_median3(a, lo, hi)
        left = pivot - lo
        right = hi - pivot
        if min(left, right) < (hi - lo + 1) // 8:
            budget -= 1
        if left < right:
            _pdq_inspired(a, lo, pivot - 1, budget)
            lo = pivot + 1
        else:
            _pdq_inspired(a, pivot + 1, hi, budget)
            hi = pivot - 1
    if lo < hi:
        insertion_range(a, lo, hi)


def pdq_inspired(values: list[int]) -> list[int]:
    a = values.copy()
    if len(a) > 1:
        _pdq_inspired(a, 0, len(a) - 1, len(a).bit_length() - 1)
    return a


@dataclass
class Run:
    start: int
    length: int
    power: int = 0

    @property
    def end(self) -> int:
        return self.start + self.length


def find_run(a: list[int], start: int) -> Run:
    n = len(a)
    if start >= n:
        return Run(start, 0)
    if start == n - 1:
        return Run(start, 1)
    end = start + 2
    if a[start + 1] < a[start]:
        while end < n and a[end] < a[end - 1]:
            end += 1
        a[start:end] = reversed(a[start:end])
    else:
        while end < n and a[end] >= a[end - 1]:
            end += 1
    return Run(start, end - start)


def node_power(start: int, len1: int, len2: int, n: int) -> int:
    a = 2 * start + len1
    b = a + len1 + len2
    power = 0
    while True:
        power += 1
        if a >= n:
            a -= n
            b -= n
        elif b >= n:
            return power
        a <<= 1
        b <<= 1


def merge_runs(a: list[int], left: Run, right: Run) -> Run:
    assert left.end == right.start
    i, j = left.start, right.start
    left_end, right_end = left.end, right.end
    tmp: list[int] = []
    while i < left_end and j < right_end:
        if a[i] <= a[j]:
            tmp.append(a[i])
            i += 1
        else:
            tmp.append(a[j])
            j += 1
    tmp.extend(a[i:left_end])
    tmp.extend(a[j:right_end])
    a[left.start:right_end] = tmp
    return Run(left.start, len(tmp))


def power_sort(values: list[int]) -> list[int]:
    a = values.copy()
    n = len(a)
    if n < 2:
        return a
    stack: list[Run] = []
    current = find_run(a, 0)
    while current.end < n:
        nxt = find_run(a, current.end)
        power = node_power(current.start, current.length, nxt.length, n)
        while stack and stack[-1].power > power:
            current = merge_runs(a, stack.pop(), current)
        current.power = power
        stack.append(current)
        current = nxt
    while stack:
        current = merge_runs(a, stack.pop(), current)
    return a


def radix_sort(values: list[int]) -> list[int]:
    src = values.copy()
    n = len(src)
    if n < 2:
        return src
    dst = [0] * n
    mask32 = 0xFFFFFFFF
    for shift in (0, 8, 16, 24):
        counts = [0] * 256
        for value in src:
            key = ((value & mask32) ^ 0x80000000)
            counts[(key >> shift) & 0xFF] += 1
        total = 0
        for i, count in enumerate(counts):
            counts[i] = total
            total += count
        for value in src:
            key = ((value & mask32) ^ 0x80000000)
            digit = (key >> shift) & 0xFF
            dst[counts[digit]] = value
            counts[digit] += 1
        src, dst = dst, src
    return src


def model_bucket_sort(values: list[int]) -> list[int]:
    if len(values) < 2:
        return values.copy()
    low, high = min(values), max(values)
    if low == high:
        return values.copy()
    count = max(8, min(2048, int(math.sqrt(len(values)))))
    buckets: list[list[int]] = [[] for _ in range(count)]
    span = high - low + 1
    for value in values:
        bucket = min(count - 1, (value - low) * count // span)
        buckets[bucket].append(value)
    result: list[int] = []
    for bucket in buckets:
        if bucket:
            result.extend(intro_sort(bucket))
    return result


ALGORITHMS: dict[str, Callable[[list[int]], list[int]]] = {
    "Python built-in": sorted,
    "PowerSort ref": power_sort,
    "IntroSort ref": intro_sort,
    "PDQ-inspired ref": pdq_inspired,
    "Linear model buckets": model_bucket_sort,
    "Signed LSD radix": radix_sort,
}


def random_data(size: int, offset: int = 0) -> list[int]:
    rng = random.Random(SEED + size * 37 + offset)
    return [rng.randint(-(2**31), 2**31 - 1) for _ in range(size)]


def benchmark(fn: Callable[[list[int]], list[int]], values: list[int], repeats: int = 3) -> float:
    expected = sorted(values)
    timings: list[float] = []
    for _ in range(repeats):
        start = time.perf_counter_ns()
        result = fn(values)
        timings.append((time.perf_counter_ns() - start) / 1_000_000)
        if result != expected:
            raise AssertionError(f"{fn.__name__} failed correctness check")
    return statistics.median(timings)


def write_csv(path: Path, rows: list[dict[str, object]]) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    with path.open("w", newline="", encoding="utf-8") as fh:
        writer = csv.DictWriter(fh, fieldnames=list(rows[0].keys()))
        writer.writeheader()
        writer.writerows(rows)


def scaling() -> list[dict[str, object]]:
    rows: list[dict[str, object]] = []
    for size in (1_000, 5_000, 20_000, 60_000):
        values = random_data(size)
        for name, fn in ALGORITHMS.items():
            rows.append({"algorithm": name, "size": size, "median_ms": benchmark(fn, values)})
    return rows


def shape_cases(size: int) -> dict[str, list[int]]:
    random_values = random_data(size, 11)
    ordered = sorted(random_values)
    reversed_values = list(reversed(ordered))
    nearly = ordered.copy()
    rng = random.Random(SEED + 77)
    for _ in range(max(1, size // 100)):
        i, j = rng.randrange(size), rng.randrange(size)
        nearly[i], nearly[j] = nearly[j], nearly[i]
    duplicates = [value % 32 for value in random_values]
    skewed = [int((rng.random() ** 8) * 1_000_000) for _ in range(size)]
    return {
        "Random": random_values,
        "Sorted": ordered,
        "Reverse": reversed_values,
        "Nearly sorted": nearly,
        "Many duplicates": duplicates,
        "Skewed": skewed,
    }


def shapes() -> list[dict[str, object]]:
    rows: list[dict[str, object]] = []
    size = 20_000
    cases = shape_cases(size)
    for name, fn in ALGORITHMS.items():
        timings = {
            shape: benchmark(fn, values, repeats=2)
            for shape, values in cases.items()
        }
        baseline = timings["Random"]
        for shape, elapsed in timings.items():
            rows.append({
                "algorithm": name,
                "input": shape,
                "size": size,
                "median_ms": elapsed,
                "vs_random": elapsed / baseline if baseline else 0.0,
            })
    return rows


def plot_scaling(rows: list[dict[str, object]]) -> None:
    fig, ax = plt.subplots(figsize=(12.5, 7))
    for name in ALGORITHMS:
        selected = [r for r in rows if r["algorithm"] == name]
        ax.plot(
            [int(r["size"]) for r in selected],
            [float(r["median_ms"]) for r in selected],
            marker="o",
            label=name,
        )
    ax.set_yscale("log")
    ax.set_title("Modern sorting research — deterministic Python reference scaling")
    ax.set_xlabel("Array length")
    ax.set_ylabel("Median time, ms (log scale)")
    ax.grid(True, alpha=0.3)
    ax.legend(ncol=2)
    fig.tight_layout()
    fig.savefig(OUTPUT / "modern-scaling.svg", format="svg")
    plt.close(fig)


def plot_shapes(rows: list[dict[str, object]]) -> None:
    shapes_order = ["Random", "Sorted", "Reverse", "Nearly sorted", "Many duplicates", "Skewed"]
    fig, ax = plt.subplots(figsize=(13, 7))
    x = range(len(shapes_order))
    width = 0.12
    for idx, name in enumerate(ALGORITHMS):
        selected = {str(r["input"]): float(r["vs_random"]) for r in rows if r["algorithm"] == name}
        positions = [i + (idx - 2.5) * width for i in x]
        ax.bar(positions, [selected[s] for s in shapes_order], width=width, label=name)
    ax.axhline(1.0, linewidth=1)
    ax.set_title("Input sensitivity — runtime relative to each algorithm's random input")
    ax.set_xticks(list(x))
    ax.set_xticklabels(shapes_order, rotation=15, ha="right")
    ax.set_ylabel("Relative runtime (random = 1.0×)")
    ax.grid(True, axis="y", alpha=0.3)
    ax.legend(ncol=2)
    fig.tight_layout()
    fig.savefig(OUTPUT / "modern-input-shapes.svg", format="svg")
    plt.close(fig)


def main() -> None:
    OUTPUT.mkdir(parents=True, exist_ok=True)
    scaling_rows = scaling()
    shape_rows = shapes()
    write_csv(OUTPUT / "modern-scaling.csv", scaling_rows)
    write_csv(OUTPUT / "modern-input-shapes.csv", shape_rows)
    plot_scaling(scaling_rows)
    plot_shapes(shape_rows)
    print(f"Wrote benchmark artifacts to {OUTPUT}")


if __name__ == "__main__":
    main()

#!/usr/bin/env python3
"""Generate deterministic sorting-algorithm charts and source CSV files."""

from __future__ import annotations

import csv
import heapq
import math
import random
import statistics
import time
from pathlib import Path
from typing import Callable, Iterable

import matplotlib.pyplot as plt
import numpy as np


ROOT = Path(__file__).resolve().parents[1]
OUTPUT = ROOT / "docs" / "assets" / "benchmarks"
SEED = 20260824

COLORS = {
    "Quick": "#2563eb",
    "Merge": "#0ea5e9",
    "Heap": "#8b5cf6",
    "Shell": "#14b8a6",
    "Counting": "#f59e0b",
    "Radix": "#ef4444",
    "Tim": "#10b981",
    "Patience": "#ec4899",
    "Bubble": "#dc2626",
    "Insertion": "#ea580c",
    "Selection": "#ca8a04",
    "Cocktail": "#7c3aed",
    "Comb": "#0891b2",
    "Cycle": "#475569",
}


def quick_sort(values: list[int]) -> list[int]:
    data = values.copy()
    if len(data) < 2:
        return data
    stack = [(0, len(data) - 1)]
    while stack:
        low, high = stack.pop()
        while low < high:
            mid = (low + high) // 2
            candidates = [(data[low], low), (data[mid], mid), (data[high], high)]
            _, pivot_index = sorted(candidates, key=lambda item: item[0])[1]
            data[pivot_index], data[high] = data[high], data[pivot_index]
            pivot = data[high]
            boundary = low
            for i in range(low, high):
                if data[i] <= pivot:
                    data[boundary], data[i] = data[i], data[boundary]
                    boundary += 1
            data[boundary], data[high] = data[high], data[boundary]
            if boundary - low < high - boundary:
                if boundary + 1 < high:
                    stack.append((boundary + 1, high))
                high = boundary - 1
            else:
                if low < boundary - 1:
                    stack.append((low, boundary - 1))
                low = boundary + 1
    return data


def merge_sort(values: list[int]) -> list[int]:
    data = values.copy()
    width = 1
    buffer = [0] * len(data)
    while width < len(data):
        for left in range(0, len(data), 2 * width):
            mid = min(left + width, len(data))
            right = min(left + 2 * width, len(data))
            i, j, k = left, mid, left
            while i < mid and j < right:
                if data[i] <= data[j]:
                    buffer[k] = data[i]
                    i += 1
                else:
                    buffer[k] = data[j]
                    j += 1
                k += 1
            while i < mid:
                buffer[k] = data[i]
                i += 1
                k += 1
            while j < right:
                buffer[k] = data[j]
                j += 1
                k += 1
        data, buffer = buffer, data
        width *= 2
    return data


def heap_sort(values: list[int]) -> list[int]:
    heap = values.copy()
    heapq.heapify(heap)
    return [heapq.heappop(heap) for _ in range(len(heap))]


def shell_sort(values: list[int]) -> list[int]:
    data = values.copy()
    gap = len(data) // 2
    while gap:
        for i in range(gap, len(data)):
            value = data[i]
            j = i
            while j >= gap and data[j - gap] > value:
                data[j] = data[j - gap]
                j -= gap
            data[j] = value
        gap //= 2
    return data


def counting_sort(values: list[int]) -> list[int]:
    if not values:
        return []
    low, high = min(values), max(values)
    counts = [0] * (high - low + 1)
    for value in values:
        counts[value - low] += 1
    result: list[int] = []
    for index, count in enumerate(counts):
        if count:
            result.extend([index + low] * count)
    return result


def radix_sort(values: list[int]) -> list[int]:
    # Four stable byte passes; xor converts signed order to unsigned order.
    data = [value ^ -(1 << 31) for value in values]
    for shift in (0, 8, 16, 24):
        counts = [0] * 256
        for value in data:
            counts[(value >> shift) & 0xFF] += 1
        total = 0
        for i, count in enumerate(counts):
            counts[i] = total
            total += count
        output = [0] * len(data)
        for value in data:
            digit = (value >> shift) & 0xFF
            output[counts[digit]] = value
            counts[digit] += 1
        data = output
    return [value ^ -(1 << 31) for value in data]


def tim_sort(values: list[int]) -> list[int]:
    return sorted(values)


def patience_sort(values: list[int]) -> list[int]:
    from bisect import bisect_left

    piles: list[list[int]] = []
    tops: list[int] = []
    for value in values:
        index = bisect_left(tops, value)
        if index == len(piles):
            piles.append([value])
            tops.append(value)
        else:
            piles[index].append(value)
            tops[index] = value
    heap = [(pile[-1], i) for i, pile in enumerate(piles)]
    heapq.heapify(heap)
    result: list[int] = []
    while heap:
        value, index = heapq.heappop(heap)
        result.append(value)
        piles[index].pop()
        if piles[index]:
            heapq.heappush(heap, (piles[index][-1], index))
    return result


def bubble_sort(values: list[int]) -> list[int]:
    data = values.copy()
    for end in range(len(data) - 1, 0, -1):
        swapped = False
        for i in range(end):
            if data[i] > data[i + 1]:
                data[i], data[i + 1] = data[i + 1], data[i]
                swapped = True
        if not swapped:
            break
    return data


def insertion_sort(values: list[int]) -> list[int]:
    data = values.copy()
    for i in range(1, len(data)):
        value = data[i]
        j = i - 1
        while j >= 0 and data[j] > value:
            data[j + 1] = data[j]
            j -= 1
        data[j + 1] = value
    return data


def selection_sort(values: list[int]) -> list[int]:
    data = values.copy()
    for i in range(len(data) - 1):
        minimum = i
        for j in range(i + 1, len(data)):
            if data[j] < data[minimum]:
                minimum = j
        if minimum != i:
            data[i], data[minimum] = data[minimum], data[i]
    return data


def cocktail_sort(values: list[int]) -> list[int]:
    data = values.copy()
    start, end = 0, len(data) - 1
    swapped = True
    while swapped:
        swapped = False
        for i in range(start, end):
            if data[i] > data[i + 1]:
                data[i], data[i + 1] = data[i + 1], data[i]
                swapped = True
        if not swapped:
            break
        end -= 1
        swapped = False
        for i in range(end, start, -1):
            if data[i - 1] > data[i]:
                data[i - 1], data[i] = data[i], data[i - 1]
                swapped = True
        start += 1
    return data


def comb_sort(values: list[int]) -> list[int]:
    data = values.copy()
    gap = len(data)
    swapped = True
    while gap > 1 or swapped:
        gap = max(1, int(gap / 1.3))
        swapped = False
        for i in range(len(data) - gap):
            if data[i] > data[i + gap]:
                data[i], data[i + gap] = data[i + gap], data[i]
                swapped = True
    return data


def cycle_sort(values: list[int]) -> list[int]:
    data = values.copy()
    for cycle_start in range(len(data) - 1):
        item = data[cycle_start]
        position = cycle_start + sum(value < item for value in data[cycle_start + 1 :])
        if position == cycle_start:
            continue
        while position < len(data) and item == data[position]:
            position += 1
        if position == len(data):
            continue
        data[position], item = item, data[position]
        while position != cycle_start:
            position = cycle_start + sum(value < item for value in data[cycle_start + 1 :])
            while position < len(data) and item == data[position]:
                position += 1
            if position == len(data):
                break
            data[position], item = item, data[position]
    return data


EFFICIENT: dict[str, Callable[[list[int]], list[int]]] = {
    "Quick": quick_sort,
    "Merge": merge_sort,
    "Heap": heap_sort,
    "Shell": shell_sort,
    "Counting": counting_sort,
    "Radix": radix_sort,
    "Tim": tim_sort,
    "Patience": patience_sort,
}

QUADRATIC: dict[str, Callable[[list[int]], list[int]]] = {
    "Bubble": bubble_sort,
    "Insertion": insertion_sort,
    "Selection": selection_sort,
    "Cocktail": cocktail_sort,
    "Comb": comb_sort,
    "Cycle": cycle_sort,
}


def random_array(size: int, seed_offset: int = 0) -> list[int]:
    rng = random.Random(SEED + size * 31 + seed_offset)
    return [rng.randint(-size * 2, size * 2) for _ in range(size)]


def benchmark(function: Callable[[list[int]], list[int]], data: list[int], repeats: int = 3) -> float:
    expected = sorted(data)
    timings = []
    for _ in range(repeats):
        start = time.perf_counter_ns()
        result = function(data)
        timings.append((time.perf_counter_ns() - start) / 1_000_000)
        if result != expected:
            raise AssertionError(f"{function.__name__} returned an incorrectly sorted array")
    return statistics.median(timings)


def scaling_benchmark(
    algorithms: dict[str, Callable[[list[int]], list[int]]], sizes: Iterable[int], repeats: int
) -> list[dict[str, float | int | str]]:
    rows = []
    for size in sizes:
        data = random_array(size)
        for name, function in algorithms.items():
            rows.append({"algorithm": name, "size": size, "median_ms": benchmark(function, data, repeats)})
    return rows


def input_shape_benchmark() -> list[dict[str, float | int | str]]:
    rows = []
    for name, function in {**EFFICIENT, **QUADRATIC}.items():
        size = 4_000 if name in EFFICIENT else 700
        random_values = random_array(size, 7)
        nearly_sorted = sorted(random_values)
        rng = random.Random(SEED + 99)
        for _ in range(max(1, size // 50)):
            a, b = rng.randrange(size), rng.randrange(size)
            nearly_sorted[a], nearly_sorted[b] = nearly_sorted[b], nearly_sorted[a]
        cases = {
            "Random": random_values,
            "Sorted": sorted(random_values),
            "Reverse": sorted(random_values, reverse=True),
            "Nearly sorted": nearly_sorted,
            "Many duplicates": [value % 16 for value in random_values],
        }
        for shape, data in cases.items():
            rows.append({"algorithm": name, "input": shape, "size": size, "median_ms": benchmark(function, data, 2)})
    return rows


def instrumented_counts(size: int) -> list[dict[str, int | str]]:
    original = random_array(size, 123)
    rows = []
    for name in ("Bubble", "Insertion", "Selection"):
        data = original.copy()
        comparisons = writes = 0
        if name == "Bubble":
            for end in range(size - 1, 0, -1):
                swapped = False
                for i in range(end):
                    comparisons += 1
                    if data[i] > data[i + 1]:
                        data[i], data[i + 1] = data[i + 1], data[i]
                        writes += 2
                        swapped = True
                if not swapped:
                    break
        elif name == "Insertion":
            for i in range(1, size):
                value, j = data[i], i - 1
                while j >= 0:
                    comparisons += 1
                    if data[j] <= value:
                        break
                    data[j + 1] = data[j]
                    writes += 1
                    j -= 1
                data[j + 1] = value
                writes += 1
        else:
            for i in range(size - 1):
                minimum = i
                for j in range(i + 1, size):
                    comparisons += 1
                    if data[j] < data[minimum]:
                        minimum = j
                if minimum != i:
                    data[i], data[minimum] = data[minimum], data[i]
                    writes += 2
        rows.append({"algorithm": name, "size": size, "comparisons": comparisons, "writes": writes})
    return rows


def style_axis(axis: plt.Axes, title: str) -> None:
    axis.set_title(title, loc="left", fontsize=12, fontweight="bold")
    axis.set_facecolor("#f8fafc")
    axis.grid(True, color="#cbd5e1", alpha=0.5, linewidth=0.7)
    axis.spines[["top", "right"]].set_visible(False)


def save_figure(figure: plt.Figure, filename: str) -> None:
    figure.tight_layout()
    figure.savefig(OUTPUT / filename, dpi=170, bbox_inches="tight", facecolor="white")
    plt.close(figure)


def plot_scaling(rows: list[dict[str, float | int | str]], filename: str, title: str, logarithmic: bool = False) -> None:
    figure, axis = plt.subplots(figsize=(13.5, 7.2))
    names = list(dict.fromkeys(str(row["algorithm"]) for row in rows))
    for name in names:
        selected = [row for row in rows if row["algorithm"] == name]
        axis.plot(
            [int(row["size"]) for row in selected],
            [float(row["median_ms"]) for row in selected],
            marker="o",
            markersize=4,
            linewidth=2,
            color=COLORS[name],
            label=name,
        )
    if logarithmic:
        axis.set_yscale("log")
    style_axis(axis, title)
    axis.set_xlabel("Array length, elements")
    axis.set_ylabel("Median time, ms" + (" (log scale)" if logarithmic else ""))
    axis.legend(ncol=4, frameon=False)
    save_figure(figure, filename)


def plot_input_shapes(rows: list[dict[str, float | int | str]]) -> None:
    names = list(dict.fromkeys(str(row["algorithm"]) for row in rows))
    shapes = ["Random", "Sorted", "Reverse", "Nearly sorted", "Many duplicates"]
    matrix = np.array([[float(next(row["median_ms"] for row in rows if row["algorithm"] == name and row["input"] == shape)) for shape in shapes] for name in names])
    normalized = matrix / matrix[:, [0]]
    figure, axis = plt.subplots(figsize=(12.5, 8.5))
    image = axis.imshow(normalized, cmap="RdYlGn_r", aspect="auto", vmin=0, vmax=min(4, float(normalized.max())))
    axis.set_xticks(range(len(shapes)), shapes)
    axis.set_yticks(range(len(names)), names)
    for i in range(len(names)):
        for j in range(len(shapes)):
            value = normalized[i, j]
            axis.text(j, i, f"{value:.2f}×", ha="center", va="center", fontsize=8, color="white" if value > 2.4 else "#0f172a")
    axis.set_title("Sensitivity to input order (relative to random input)", loc="left", fontsize=13, fontweight="bold")
    figure.colorbar(image, ax=axis, label="Runtime / random runtime")
    save_figure(figure, "input-shapes.png")


def plot_operations(rows: list[dict[str, int | str]]) -> None:
    names = list(dict.fromkeys(str(row["algorithm"]) for row in rows))
    figure, axes = plt.subplots(1, 2, figsize=(14, 6))
    for axis, metric, title in zip(axes, ("comparisons", "writes"), ("Comparisons", "Array writes")):
        for name in names:
            selected = [row for row in rows if row["algorithm"] == name]
            axis.plot([int(row["size"]) for row in selected], [int(row[metric]) for row in selected], marker="o", linewidth=2, color=COLORS[name], label=name)
        style_axis(axis, title)
        axis.set_xlabel("Array length")
        axis.set_ylabel("Operations")
        axis.legend(frameon=False)
    figure.suptitle("Why O(n²) sorts become expensive", fontsize=16, fontweight="bold", y=1.01)
    save_figure(figure, "operation-growth.png")


def bubble_snapshots(values: list[int]) -> list[list[int]]:
    data = values.copy()
    targets = {0, max(1, len(data) // 4), max(1, len(data) // 2), max(1, 3 * len(data) // 4), len(data) - 1}
    snapshots = []
    for pass_index, end in enumerate(range(len(data) - 1, 0, -1)):
        if pass_index in targets:
            snapshots.append(data.copy())
        for i in range(end):
            if data[i] > data[i + 1]:
                data[i], data[i + 1] = data[i + 1], data[i]
    snapshots.append(data)
    return snapshots[:5]


def plot_process() -> None:
    values = random_array(32, 321)
    snapshots = bubble_snapshots(values)
    figure, axes = plt.subplots(len(snapshots), 1, figsize=(13, 8), sharex=True)
    labels = ["Start", "After 25% of passes", "After 50% of passes", "After 75% of passes", "Sorted"]
    for axis, data, label in zip(axes, snapshots, labels):
        colors = plt.cm.viridis((np.array(data) - min(values)) / (max(values) - min(values)))
        axis.bar(np.arange(len(data)), data, color=colors, width=0.85)
        axis.axhline(0, color="#64748b", linewidth=0.6)
        axis.set_ylabel(label, rotation=0, ha="right", va="center", labelpad=20, fontsize=9)
        axis.set_yticks([])
        axis.spines[:].set_visible(False)
    axes[-1].set_xlabel("Array index")
    figure.suptitle("Bubble sort in motion: large values migrate to the right", fontsize=16, fontweight="bold", y=1.01)
    save_figure(figure, "sorting-process.png")


def write_csv(filename: str, rows: list[dict[str, float | int | str]]) -> None:
    with (OUTPUT / filename).open("w", newline="", encoding="utf-8") as stream:
        writer = csv.DictWriter(stream, fieldnames=list(rows[0].keys()))
        writer.writeheader()
        writer.writerows(rows)


def main() -> None:
    OUTPUT.mkdir(parents=True, exist_ok=True)
    plt.rcParams.update({"font.family": "DejaVu Sans", "text.color": "#0f172a", "axes.labelcolor": "#334155"})

    efficient = scaling_benchmark(EFFICIENT, [100, 250, 500, 1_000, 2_000, 5_000, 10_000, 20_000], 3)
    quadratic = scaling_benchmark(QUADRATIC, [50, 100, 200, 400, 800, 1_200, 1_600], 2)
    shapes = input_shape_benchmark()
    operations = [row for size in [25, 50, 100, 200, 400, 800] for row in instrumented_counts(size)]

    plot_scaling(efficient, "efficient-scaling.png", "Efficient algorithms: measured scaling", logarithmic=True)
    plot_scaling(quadratic, "quadratic-scaling.png", "Quadratic and near-quadratic algorithms: measured scaling")
    plot_input_shapes(shapes)
    plot_operations(operations)
    plot_process()

    write_csv("efficient-scaling.csv", efficient)
    write_csv("quadratic-scaling.csv", quadratic)
    write_csv("input-shapes.csv", shapes)
    write_csv("operation-counts.csv", operations)

    print(f"Generated benchmark artifacts in {OUTPUT.relative_to(ROOT)}")
    for row in efficient:
        if row["size"] == 20_000:
            print(f"{row['algorithm']:<10} n=20,000  {float(row['median_ms']):8.3f} ms")


if __name__ == "__main__":
    main()

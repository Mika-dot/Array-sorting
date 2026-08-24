import importlib.util
import unittest
from pathlib import Path


SCRIPT = Path(__file__).resolve().parents[1] / "tools" / "generate_sorting_benchmarks.py"
SPEC = importlib.util.spec_from_file_location("sorting_benchmarks", SCRIPT)
BENCH = importlib.util.module_from_spec(SPEC)
SPEC.loader.exec_module(BENCH)


class SortingReferenceTests(unittest.TestCase):
    def test_every_algorithm_handles_edge_cases(self):
        cases = [
            [],
            [1],
            [2, 1],
            [5, -1, 5, 0, -1, 9],
            list(range(40)),
            list(range(40, -1, -1)),
        ]
        for name, function in {**BENCH.EFFICIENT, **BENCH.QUADRATIC}.items():
            for values in cases:
                with self.subTest(algorithm=name, length=len(values)):
                    original = values.copy()
                    self.assertEqual(function(values), sorted(values))
                    self.assertEqual(values, original, "reference sort mutated its input")

    def test_random_arrays_are_sorted(self):
        for size in [3, 10, 31, 100]:
            values = BENCH.random_array(size)
            for name, function in {**BENCH.EFFICIENT, **BENCH.QUADRATIC}.items():
                with self.subTest(algorithm=name, length=size):
                    self.assertEqual(function(values), sorted(values))

    def test_fixed_seed_is_reproducible(self):
        self.assertEqual(BENCH.random_array(128), BENCH.random_array(128))


if __name__ == "__main__":
    unittest.main()

using System.Collections.Generic;
using UnityEngine;

namespace TiredDad
{
    /// <summary>
    /// Per-level, weighted pool of items the SpawnSystem draws from, plus the
    /// placement area and category caps. Built by the LevelGenerator.
    /// </summary>
    public class SpawnTable
    {
        public struct Entry
        {
            public ItemDefinition item;
            public float weight;
        }

        public readonly List<Entry> entries = new List<Entry>();

        /// <summary>World-space rectangle (XZ) items may spawn within.</summary>
        public Bounds area;

        /// <summary>Max simultaneously-live items per category.</summary>
        public int capPositive = 5;
        public int capNegative = 5;
        public int capHalfAndHalf = 5;

        public void Add(ItemDefinition item, float weight)
        {
            if (item == null || weight <= 0f) return;
            entries.Add(new Entry { item = item, weight = weight });
        }

        /// <summary>Weighted random pick; null if the table is empty.</summary>
        public ItemDefinition Pick(System.Random rng)
        {
            float total = 0f;
            foreach (var e in entries) total += e.weight;
            if (total <= 0f) return null;

            double roll = rng.NextDouble() * total;
            foreach (var e in entries)
            {
                roll -= e.weight;
                if (roll <= 0) return e.item;
            }
            return entries[entries.Count - 1].item;
        }

        public int CapFor(ItemCategory c) => c switch
        {
            ItemCategory.Positive    => capPositive,
            ItemCategory.Negative    => capNegative,
            ItemCategory.HalfAndHalf => capHalfAndHalf,
            _ => 5
        };
    }
}

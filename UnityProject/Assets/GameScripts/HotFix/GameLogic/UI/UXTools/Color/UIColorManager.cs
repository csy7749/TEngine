using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.UI.UXTools.Color
{
    public sealed class UIColorEntry
    {
        public UIColorEntry(string key, Color color)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Color key is required.", nameof(key));
            }

            Key = key;
            Color = color;
        }

        public string Key { get; }
        public Color Color { get; }
    }

    public sealed class UIColorConfig
    {
        public UIColorConfig(IReadOnlyList<UIColorEntry> entries)
        {
            if (entries == null || entries.Count == 0)
            {
                throw new ArgumentException("Color entries are required.", nameof(entries));
            }

            Entries = entries;
        }

        public IReadOnlyList<UIColorEntry> Entries { get; }
    }

    public sealed class UIColorManager
    {
        private readonly Dictionary<string, Color> _colors = new Dictionary<string, Color>(StringComparer.Ordinal);

        public UIColorManager(UIColorConfig config)
        {
            ApplyConfig(config);
        }

        public void ApplyConfig(UIColorConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            _colors.Clear();
            for (int i = 0; i < config.Entries.Count; i++)
            {
                var entry = config.Entries[i];
                _colors[entry.Key] = entry.Color;
            }
        }

        public bool TryGetColor(string key, out Color color)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                color = default;
                return false;
            }

            return _colors.TryGetValue(key, out color);
        }

        public Color GetColor(string key)
        {
            if (!TryGetColor(key, out var color))
            {
                throw new InvalidOperationException($"Color key not found: {key}.");
            }

            return color;
        }
    }
}

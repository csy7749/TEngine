using System;
using System.Collections.Generic;

namespace GameLogic.UI.UXTools.Widget
{
    public sealed class WidgetEntry
    {
        public WidgetEntry(string id, string assetPath)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Widget id is required.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(assetPath))
            {
                throw new ArgumentException("Asset path is required.", nameof(assetPath));
            }

            Id = id;
            AssetPath = assetPath;
        }

        public string Id { get; }
        public string AssetPath { get; }
    }

    public sealed class WidgetRepository
    {
        private readonly Dictionary<string, WidgetEntry> _entries = new Dictionary<string, WidgetEntry>(StringComparer.Ordinal);

        public void Register(WidgetEntry entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            _entries[entry.Id] = entry;
        }

        public bool TryGet(string id, out WidgetEntry entry)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                entry = null;
                return false;
            }

            return _entries.TryGetValue(id, out entry);
        }

        public WidgetEntry Get(string id)
        {
            if (!TryGet(id, out var entry))
            {
                throw new InvalidOperationException($"Widget not found: {id}.");
            }

            return entry;
        }
    }
}

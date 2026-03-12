using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.UI.UXTools.Color
{
    [Serializable]
    public sealed class UIColorEntryData
    {
        public string Key;
        public Color Color;
    }

    [CreateAssetMenu(menuName = "GameLogic/UI/UIColorConfig")]
    public sealed class UIColorConfigAsset : ScriptableObject
    {
        [SerializeField] private List<UIColorEntryData> _entries = new List<UIColorEntryData>();

        public IReadOnlyList<UIColorEntryData> Entries => _entries;

        public UIColorConfig ToConfig()
        {
            if (_entries == null || _entries.Count == 0)
            {
                throw new InvalidOperationException("UIColorConfigAsset has no entries.");
            }

            var list = new List<UIColorEntry>(_entries.Count);
            for (int i = 0; i < _entries.Count; i++)
            {
                var entry = _entries[i];
                list.Add(new UIColorEntry(entry.Key, entry.Color));
            }

            return new UIColorConfig(list);
        }
    }
}

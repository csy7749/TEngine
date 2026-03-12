using System;

namespace GameLogic.UI
{
    public enum UIWindowCacheMode
    {
        None = 0,
        Cache = 1,
    }

    public sealed class UIWindowOpenOptionsInput
    {
        public string PrefabPath { get; set; }
        public string PackageName { get; set; }
        public UILayer Layer { get; set; }
        public UIWindowCacheMode CacheMode { get; set; } = UIWindowCacheMode.Cache;
        public int SortingOrderOffset { get; set; }
        public string WindowId { get; set; }
    }

    public sealed class UIWindowOpenOptions
    {
        public UIWindowOpenOptions(UIWindowOpenOptionsInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (string.IsNullOrWhiteSpace(input.PrefabPath))
            {
                throw new ArgumentException("Prefab path is required.", nameof(input.PrefabPath));
            }

            PrefabPath = input.PrefabPath;
            PackageName = input.PackageName ?? string.Empty;
            Layer = input.Layer;
            CacheMode = input.CacheMode;
            SortingOrderOffset = input.SortingOrderOffset;
            WindowId = input.WindowId;
        }

        public string PrefabPath { get; }
        public string PackageName { get; }
        public UILayer Layer { get; }
        public UIWindowCacheMode CacheMode { get; }
        public int SortingOrderOffset { get; }
        public string WindowId { get; }
    }
}

using System;
using System.Collections.Generic;
using GameLogic.UI;
using UnityEngine;

namespace GameLogic.UI.Config
{
    [Serializable]
    public sealed class UILayerConfigEntry
    {
        public UILayer Layer;
        public string Name;
        public int SortingOrder;
    }

    [Serializable]
    public sealed class UIRootConfig
    {
        public RenderMode RenderMode = RenderMode.ScreenSpaceCamera;
        public Vector2 ReferenceResolution = new Vector2(1920f, 1080f);
        public float MatchWidthOrHeight = 0.5f;
    }

    [Serializable]
    public sealed class UICameraConfig
    {
        public string Name = "UICamera";
        public CameraClearFlags ClearFlags = CameraClearFlags.Depth;
        public Color BackgroundColor = Color.clear;
        public float OrthographicSize = 5f;
        public float NearClipPlane = 0.1f;
        public float FarClipPlane = 1000f;
        public float Depth = 100f;
    }

    [Serializable]
    public sealed class UIResourcePathConfig
    {
        public string PrefabRootPath = "";
        public string PackageName = "";

        public string ResolvePath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                throw new ArgumentException("Relative path is required.", nameof(relativePath));
            }

            if (string.IsNullOrWhiteSpace(PrefabRootPath))
            {
                return relativePath;
            }

            return PrefabRootPath.TrimEnd('/') + "/" + relativePath.TrimStart('/');
        }
    }

    [CreateAssetMenu(menuName = "GameLogic/UI/UIFrameworkConfig")]
    public sealed class UIFrameworkConfig : ScriptableObject
    {
        public const string DefaultHotUpdateAssemblyName = "GameLogic";

        [SerializeField] private bool _enableFramework = true;
        [SerializeField] private string _hotUpdateAssemblyName = DefaultHotUpdateAssemblyName;
        [SerializeField] private string _rootName = UIManagerOptions.DefaultRootName;
        [SerializeField] private string _layerName = UIManagerOptions.DefaultLayerName;
        [SerializeField] private UIRootConfig _rootConfig = new UIRootConfig();
        [SerializeField] private UICameraConfig _cameraConfig = new UICameraConfig();
        [SerializeField] private UIResourcePathConfig _resourcePathConfig = new UIResourcePathConfig();
        [SerializeField] private List<UILayerConfigEntry> _layers = new List<UILayerConfigEntry>
        {
            new UILayerConfigEntry { Layer = UILayer.Background, Name = UILayerSettings.BackgroundName, SortingOrder = UILayerSettings.BackgroundSortingOrder },
            new UILayerConfigEntry { Layer = UILayer.UI, Name = UILayerSettings.UIName, SortingOrder = UILayerSettings.UISortingOrder },
            new UILayerConfigEntry { Layer = UILayer.Popup, Name = UILayerSettings.PopupName, SortingOrder = UILayerSettings.PopupSortingOrder },
            new UILayerConfigEntry { Layer = UILayer.System, Name = UILayerSettings.SystemName, SortingOrder = UILayerSettings.SystemSortingOrder },
        };

        public bool EnableFramework => _enableFramework;
        public string HotUpdateAssemblyName => _hotUpdateAssemblyName;
        public string RootName => _rootName;
        public string LayerName => _layerName;
        public UIRootConfig RootConfig => _rootConfig;
        public UICameraConfig CameraConfig => _cameraConfig;
        public UIResourcePathConfig ResourcePathConfig => _resourcePathConfig;
        public IReadOnlyList<UILayerConfigEntry> Layers => _layers;

        public UIManagerOptions BuildManagerOptions(IUIResourceProvider resourceProvider, IUIWindowFactory windowFactory)
        {
            if (resourceProvider == null)
            {
                throw new ArgumentNullException(nameof(resourceProvider));
            }

            if (windowFactory == null)
            {
                throw new ArgumentNullException(nameof(windowFactory));
            }

            var layerSettings = BuildLayerSettings();
            var rootSettings = new UIRootSettings(_rootConfig.RenderMode, _rootConfig.ReferenceResolution, _rootConfig.MatchWidthOrHeight);
            var cameraSettings = new UICameraSettings(new UICameraSettingsInput
            {
                Name = _cameraConfig.Name,
                ClearFlags = _cameraConfig.ClearFlags,
                BackgroundColor = _cameraConfig.BackgroundColor,
                OrthographicSize = _cameraConfig.OrthographicSize,
                NearClipPlane = _cameraConfig.NearClipPlane,
                FarClipPlane = _cameraConfig.FarClipPlane,
                Depth = _cameraConfig.Depth,
            });

            return new UIManagerOptions(new UIManagerOptionsInput
            {
                RootName = _rootName,
                LayerName = _layerName,
                LayerSettings = layerSettings,
                RootSettings = rootSettings,
                CameraSettings = cameraSettings,
                ResourceProvider = resourceProvider,
                WindowFactory = windowFactory,
            });
        }

        public UIWindowOpenOptions CreateOpenOptions(UIWindowOpenOptionsInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var resolvedPath = _resourcePathConfig.ResolvePath(input.PrefabPath);
            var packageName = string.IsNullOrWhiteSpace(input.PackageName)
                ? _resourcePathConfig.PackageName
                : input.PackageName;

            var resolvedInput = new UIWindowOpenOptionsInput
            {
                PrefabPath = resolvedPath,
                PackageName = packageName,
                Layer = input.Layer,
                CacheMode = input.CacheMode,
                SortingOrderOffset = input.SortingOrderOffset,
                WindowId = input.WindowId,
            };

            return new UIWindowOpenOptions(resolvedInput);
        }

        public UILayerSettings BuildLayerSettings()
        {
            if (_layers == null || _layers.Count == 0)
            {
                throw new InvalidOperationException("Layer configuration is required.");
            }

            var definitions = new List<UILayerDefinition>(_layers.Count);
            for (int i = 0; i < _layers.Count; i++)
            {
                var entry = _layers[i];
                definitions.Add(new UILayerDefinition(entry.Layer, entry.Name, entry.SortingOrder));
            }

            return new UILayerSettings(definitions);
        }
    }
}

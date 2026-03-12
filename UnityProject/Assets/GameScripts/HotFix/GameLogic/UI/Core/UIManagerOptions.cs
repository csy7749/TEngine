using System;
using UnityEngine;

namespace GameLogic.UI
{
    public sealed class UIManagerOptionsInput
    {
        public string RootName { get; set; }
        public string LayerName { get; set; }
        public UILayerSettings LayerSettings { get; set; }
        public UIRootSettings RootSettings { get; set; }
        public UICameraSettings CameraSettings { get; set; }
        public IUIResourceProvider ResourceProvider { get; set; }
        public IUIWindowFactory WindowFactory { get; set; }
    }

    public sealed class UIManagerOptions
    {
        public const string DefaultRootName = "UIRoot";
        public const string DefaultLayerName = "UI";

        public UIManagerOptions(UIManagerOptionsInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            ResourceProvider = input.ResourceProvider ?? throw new ArgumentNullException(nameof(input.ResourceProvider));
            WindowFactory = input.WindowFactory ?? throw new ArgumentNullException(nameof(input.WindowFactory));
            LayerSettings = input.LayerSettings ?? UILayerSettings.Default;
            RootSettings = input.RootSettings ?? UIRootSettings.Default;
            CameraSettings = input.CameraSettings ?? UICameraSettings.Default;
            RootName = string.IsNullOrWhiteSpace(input.RootName) ? DefaultRootName : input.RootName;
            LayerName = string.IsNullOrWhiteSpace(input.LayerName) ? DefaultLayerName : input.LayerName;
        }

        public string RootName { get; }
        public string LayerName { get; }
        public UILayerSettings LayerSettings { get; }
        public UIRootSettings RootSettings { get; }
        public UICameraSettings CameraSettings { get; }
        public IUIResourceProvider ResourceProvider { get; }
        public IUIWindowFactory WindowFactory { get; }
    }

    public sealed class UIRootSettings
    {
        public const float DefaultMatchWidthOrHeight = 0.5f;
        public static readonly Vector2 DefaultReferenceResolution = new Vector2(1920f, 1080f);

        public static readonly UIRootSettings Default = new UIRootSettings(
            renderMode: RenderMode.ScreenSpaceCamera,
            referenceResolution: DefaultReferenceResolution,
            matchWidthOrHeight: DefaultMatchWidthOrHeight);

        public UIRootSettings(RenderMode renderMode, Vector2 referenceResolution, float matchWidthOrHeight)
        {
            RenderMode = renderMode;
            ReferenceResolution = referenceResolution;
            MatchWidthOrHeight = matchWidthOrHeight;
        }

        public RenderMode RenderMode { get; }
        public Vector2 ReferenceResolution { get; }
        public float MatchWidthOrHeight { get; }
    }

    public sealed class UICameraSettingsInput
    {
        public string Name { get; set; }
        public CameraClearFlags ClearFlags { get; set; } = CameraClearFlags.Depth;
        public Color BackgroundColor { get; set; } = Color.clear;
        public float OrthographicSize { get; set; } = UICameraSettings.DefaultOrthographicSize;
        public float NearClipPlane { get; set; } = UICameraSettings.DefaultNearClipPlane;
        public float FarClipPlane { get; set; } = UICameraSettings.DefaultFarClipPlane;
        public float Depth { get; set; } = UICameraSettings.DefaultDepth;
    }

    public sealed class UICameraSettings
    {
        public const float DefaultOrthographicSize = 5f;
        public const float DefaultNearClipPlane = 0.1f;
        public const float DefaultFarClipPlane = 1000f;
        public const float DefaultDepth = 100f;

        public static readonly UICameraSettings Default = new UICameraSettings(new UICameraSettingsInput
        {
            Name = "UICamera",
            ClearFlags = CameraClearFlags.Depth,
            BackgroundColor = Color.clear,
            OrthographicSize = DefaultOrthographicSize,
            NearClipPlane = DefaultNearClipPlane,
            FarClipPlane = DefaultFarClipPlane,
            Depth = DefaultDepth,
        });

        public UICameraSettings(UICameraSettingsInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (string.IsNullOrWhiteSpace(input.Name))
            {
                throw new ArgumentException("Camera name is required.", nameof(input.Name));
            }

            Name = input.Name;
            ClearFlags = input.ClearFlags;
            BackgroundColor = input.BackgroundColor;
            OrthographicSize = input.OrthographicSize;
            NearClipPlane = input.NearClipPlane;
            FarClipPlane = input.FarClipPlane;
            Depth = input.Depth;
        }

        public string Name { get; }
        public CameraClearFlags ClearFlags { get; }
        public Color BackgroundColor { get; }
        public float OrthographicSize { get; }
        public float NearClipPlane { get; }
        public float FarClipPlane { get; }
        public float Depth { get; }
    }
}

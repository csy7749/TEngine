using System;
using System.Collections.Generic;

namespace GameLogic.UI
{
    public enum UILayer
    {
        Background = 0,
        UI = 1,
        Popup = 2,
        System = 3,
    }

    public sealed class UILayerDefinition
    {
        public UILayerDefinition(UILayer layer, string name, int sortingOrder)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Layer name is required.", nameof(name));
            }

            Layer = layer;
            Name = name;
            SortingOrder = sortingOrder;
        }

        public UILayer Layer { get; }
        public string Name { get; }
        public int SortingOrder { get; }
    }

    public sealed class UILayerSettings
    {
        public const int BackgroundSortingOrder = 0;
        public const int UISortingOrder = 100;
        public const int PopupSortingOrder = 200;
        public const int SystemSortingOrder = 300;

        public const string BackgroundName = "Background";
        public const string UIName = "UI";
        public const string PopupName = "Popup";
        public const string SystemName = "System";

        public static readonly UILayerSettings Default = new UILayerSettings(new[]
        {
            new UILayerDefinition(UILayer.Background, BackgroundName, BackgroundSortingOrder),
            new UILayerDefinition(UILayer.UI, UIName, UISortingOrder),
            new UILayerDefinition(UILayer.Popup, PopupName, PopupSortingOrder),
            new UILayerDefinition(UILayer.System, SystemName, SystemSortingOrder),
        });

        public UILayerSettings(IReadOnlyList<UILayerDefinition> layers)
        {
            if (layers == null || layers.Count == 0)
            {
                throw new ArgumentException("Layer definitions are required.", nameof(layers));
            }

            Layers = new List<UILayerDefinition>(layers);
        }

        public IReadOnlyList<UILayerDefinition> Layers { get; }

        public UILayerDefinition GetDefinition(UILayer layer)
        {
            for (int i = 0; i < Layers.Count; i++)
            {
                var definition = Layers[i];
                if (definition.Layer == layer)
                {
                    return definition;
                }
            }

            throw new InvalidOperationException($"Layer definition not found for {layer}.");
        }
    }
}

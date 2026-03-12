using System;
using System.Collections.Generic;
using GameLogic.UI.UXTools.Color;
using GameLogic.UI.UXTools.Localization;
using GameLogic.UI.UXTools.Reddot;
using GameLogic.UI.UXTools.Widget;

namespace GameLogic.UI.UXTools
{
    public sealed class UXToolsManager
    {
        private static UXToolsManager _instance;

        private readonly List<UXComponent> _components = new List<UXComponent>();

        private UXToolsManager(UXToolsOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Localization = new LocalizationManager(options.LocalizationProvider);
            Colors = options.ColorManager;
            Reddot = options.ReddotManager;
            Widgets = options.WidgetRepository;
        }

        public static UXToolsManager Instance => _instance ?? throw new InvalidOperationException("UXToolsManager is not initialized.");
        public static bool IsInitialized => _instance != null;

        public static UXToolsManager Initialize(UXToolsOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (_instance != null)
            {
                throw new InvalidOperationException("UXToolsManager is already initialized.");
            }

            var manager = new UXToolsManager(options);
            _instance = manager;
            manager.InitializeComponents(options.InitialComponents);
            return manager;
        }

        public static void Shutdown()
        {
            if (_instance == null)
            {
                return;
            }

            _instance.ClearComponents();
            _instance = null;
        }

        public UXToolsOptions Options { get; }
        public LocalizationManager Localization { get; }
        public UIColorManager Colors { get; }
        public ReddotManager Reddot { get; }
        public WidgetRepository Widgets { get; }

        public void RegisterComponent(UXComponent component)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            if (_components.Contains(component))
            {
                return;
            }

            _components.Add(component);
            component.Initialize(this);
        }

        public void InitializeComponents(IReadOnlyList<UXComponent> components)
        {
            if (components == null)
            {
                return;
            }

            for (int i = 0; i < components.Count; i++)
            {
                RegisterComponent(components[i]);
            }
        }

        public void RefreshAll()
        {
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].Refresh();
            }
        }

        private void ClearComponents()
        {
            _components.Clear();
        }
    }
}

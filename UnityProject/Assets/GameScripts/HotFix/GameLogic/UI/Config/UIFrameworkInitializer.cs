using System;
using GameLogic.UI.UXTools;
using GameLogic.UI.UXTools.Color;
using GameLogic.UI.UXTools.Config;
using GameLogic.UI.UXTools.Localization;
using GameLogic.UI.UXTools.Reddot;
using GameLogic.UI.UXTools.Widget;
using TEngine;
using UnityEngine;

namespace GameLogic.UI.Config
{
    public sealed class UIFrameworkInitializationResult
    {
        public bool UIManagerInitialized { get; internal set; }
        public bool UXToolsInitialized { get; internal set; }
    }

    public static class UIFrameworkInitializer
    {
        private const string DefaultColorKey = "Default";

        public static UIFrameworkInitializationResult Initialize(UIFrameworkConfig uiConfig, UXToolsConfig uxConfig, IResourceModule resourceModule)
        {
            if (uiConfig == null)
            {
                throw new ArgumentNullException(nameof(uiConfig));
            }

            var result = new UIFrameworkInitializationResult();
            if (!uiConfig.EnableFramework)
            {
                return result;
            }

            ValidateHotUpdate(uiConfig);

            if (resourceModule == null)
            {
                Debug.LogError("UIFramework initialization failed: Resource module is null.");
                return result;
            }

            var resourceProvider = new TEngineUIResourceProvider(resourceModule);
            var managerOptions = uiConfig.BuildManagerOptions(resourceProvider, new ActivatorUIWindowFactory());
            UIManager.Initialize(managerOptions);
            result.UIManagerInitialized = true;

            if (uxConfig != null && uxConfig.EnableUXTools)
            {
                result.UXToolsInitialized = InitializeUXTools(uxConfig);
            }

            return result;
        }

        private static void ValidateHotUpdate(UIFrameworkConfig uiConfig)
        {
            if (string.IsNullOrWhiteSpace(uiConfig.HotUpdateAssemblyName))
            {
                throw new InvalidOperationException("HotUpdate assembly name is required.");
            }

            UIHotUpdateValidator.Validate(uiConfig.HotUpdateAssemblyName);
        }

        private static bool InitializeUXTools(UXToolsConfig config)
        {
            var localizationProvider = ResolveLocalizationProvider(config);
            var colorManager = ResolveColorManager(config);
            var reddotManager = new ReddotManager();
            var widgetRepository = new WidgetRepository();

            var options = new UXToolsOptions(new UXToolsOptionsInput
            {
                LocalizationProvider = localizationProvider,
                ColorManager = colorManager,
                ReddotManager = reddotManager,
                WidgetRepository = widgetRepository,
            });

            UXToolsManager.Initialize(options);
            return true;
        }

        private static ILocalizationProvider ResolveLocalizationProvider(UXToolsConfig config)
        {
            if (!config.EnableLocalization)
            {
                return new DisabledLocalizationProvider();
            }

            if (config.UseTEngineLocalization)
            {
                return new TEngineLocalizationProvider();
            }

            throw new InvalidOperationException("Custom localization provider is required when TEngine localization is disabled.");
        }

        private static UIColorManager ResolveColorManager(UXToolsConfig config)
        {
            if (!config.EnableColors)
            {
                var fallbackConfig = new UIColorConfig(new[]
                {
                    new UIColorEntry(DefaultColorKey, Color.white),
                });
                return new UIColorManager(fallbackConfig);
            }

            if (config.ColorConfig == null)
            {
                throw new InvalidOperationException("UIColorConfigAsset is required when colors are enabled.");
            }

            return new UIColorManager(config.ColorConfig.ToConfig());
        }
    }
}

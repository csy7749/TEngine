using GameLogic.UI.Config;
using GameLogic.UI.UXTools.Config;
using UnityEngine;

public partial class GameApp
{
    private const string UIFrameworkConfigPath = "Configs/UIFrameworkConfig";
    private const string UXToolsConfigPath = "Configs/UXToolsConfig";

    private static void InitializeUIFramework()
    {
        var uiConfig = Resources.Load<UIFrameworkConfig>(UIFrameworkConfigPath);
        if (uiConfig == null)
        {
            Debug.LogWarning($"UIFrameworkConfig not found at {UIFrameworkConfigPath}.");
            return;
        }

        var uxConfig = Resources.Load<UXToolsConfig>(UXToolsConfigPath);
        UIFrameworkInitializer.Initialize(uiConfig, uxConfig, GameModule.Resource);
    }
}

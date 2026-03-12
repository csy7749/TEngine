using UnityEngine;
using GameLogic.UI.UXTools.Color;

namespace GameLogic.UI.UXTools.Config
{
    [CreateAssetMenu(menuName = "GameLogic/UI/UXToolsConfig")]
    public sealed class UXToolsConfig : ScriptableObject
    {
        [SerializeField] private bool _enableUXTools = true;
        [SerializeField] private bool _enableLocalization = true;
        [SerializeField] private bool _useTEngineLocalization = true;
        [SerializeField] private bool _enableColors = true;
        [SerializeField] private bool _enableReddot = true;
        [SerializeField] private bool _enableWidgetRepository = true;
        [SerializeField] private UIColorConfigAsset _colorConfig;

        public bool EnableUXTools => _enableUXTools;
        public bool EnableLocalization => _enableLocalization;
        public bool UseTEngineLocalization => _useTEngineLocalization;
        public bool EnableColors => _enableColors;
        public bool EnableReddot => _enableReddot;
        public bool EnableWidgetRepository => _enableWidgetRepository;
        public UIColorConfigAsset ColorConfig => _colorConfig;
    }
}

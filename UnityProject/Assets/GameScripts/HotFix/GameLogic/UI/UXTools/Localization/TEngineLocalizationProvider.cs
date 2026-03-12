using System;
using TEngine.Localization;

namespace GameLogic.UI.UXTools.Localization
{
    public sealed class TEngineLocalizationProvider : ILocalizationProvider
    {
        public bool TryGetText(string key, out string text)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                text = string.Empty;
                return false;
            }

            LocalizationManager.InitializeIfNeeded();
            text = LocalizationManager.GetTranslation(key);
            return !string.IsNullOrEmpty(text);
        }
    }
}

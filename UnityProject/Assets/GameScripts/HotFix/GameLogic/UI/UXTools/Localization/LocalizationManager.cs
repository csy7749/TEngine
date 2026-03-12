using System;

namespace GameLogic.UI.UXTools.Localization
{
    public interface ILocalizationProvider
    {
        bool TryGetText(string key, out string text);
    }

    public sealed class LocalizationManager
    {
        private readonly ILocalizationProvider _provider;

        public LocalizationManager(ILocalizationProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public string GetText(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Localization key is required.", nameof(key));
            }

            if (!_provider.TryGetText(key, out var text))
            {
                throw new InvalidOperationException($"Localization key not found: {key}.");
            }

            return text;
        }
    }
}

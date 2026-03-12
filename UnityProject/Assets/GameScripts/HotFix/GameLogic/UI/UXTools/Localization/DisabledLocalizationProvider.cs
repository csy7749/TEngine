namespace GameLogic.UI.UXTools.Localization
{
    public sealed class DisabledLocalizationProvider : ILocalizationProvider
    {
        public bool TryGetText(string key, out string text)
        {
            text = string.Empty;
            return false;
        }
    }
}

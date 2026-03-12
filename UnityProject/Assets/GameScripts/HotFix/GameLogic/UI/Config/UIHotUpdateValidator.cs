using System;

namespace GameLogic.UI.Config
{
    public static class UIHotUpdateValidator
    {
        public static void Validate(string expectedAssemblyName)
        {
            if (string.IsNullOrWhiteSpace(expectedAssemblyName))
            {
                throw new ArgumentException("Expected assembly name is required.", nameof(expectedAssemblyName));
            }

            var uiAssembly = typeof(UIManager).Assembly.GetName().Name;
            var uxAssembly = typeof(UXTools.UXToolsManager).Assembly.GetName().Name;

            if (!string.Equals(uiAssembly, expectedAssemblyName, StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"UIManager assembly mismatch: {uiAssembly} (expected {expectedAssemblyName}).");
            }

            if (!string.Equals(uxAssembly, expectedAssemblyName, StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"UXToolsManager assembly mismatch: {uxAssembly} (expected {expectedAssemblyName}).");
            }
        }
    }
}

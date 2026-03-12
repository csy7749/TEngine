using System;

namespace GameLogic.UI
{
    public interface IUIWindowFactory
    {
        UIWindow Create(Type windowType);
    }

    public sealed class ActivatorUIWindowFactory : IUIWindowFactory
    {
        public UIWindow Create(Type windowType)
        {
            if (windowType == null)
            {
                throw new ArgumentNullException(nameof(windowType));
            }

            if (!typeof(UIWindow).IsAssignableFrom(windowType))
            {
                throw new InvalidOperationException($"Window type {windowType.FullName} must derive from {nameof(UIWindow)}.");
            }

            var instance = Activator.CreateInstance(windowType) as UIWindow;
            if (instance == null)
            {
                throw new InvalidOperationException($"Failed to create window instance for {windowType.FullName}.");
            }

            return instance;
        }
    }
}

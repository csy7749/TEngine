using System;
using UnityEngine;

namespace GameLogic.UI.UXTools
{
    public abstract class UXComponent : MonoBehaviour
    {
        private bool _initialized;

        public bool IsInitialized => _initialized;
        public UXToolsManager Manager { get; private set; }

        public void Initialize(UXToolsManager manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            if (_initialized)
            {
                return;
            }

            _initialized = true;
            Manager = manager;
            OnInitialize();
            Refresh();
        }

        public void Refresh()
        {
            if (!_initialized)
            {
                throw new InvalidOperationException($"{GetType().Name} is not initialized.");
            }

            OnRefresh();
        }

        protected virtual void OnInitialize() { }
        protected virtual void OnRefresh() { }
        protected virtual void OnRelease() { }

        private void OnDestroy()
        {
            if (!_initialized)
            {
                return;
            }

            OnRelease();
        }
    }
}

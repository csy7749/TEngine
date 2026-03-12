using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.UI
{
    public abstract class UIWidget
    {
        private readonly Dictionary<Type, Component> _componentCache = new Dictionary<Type, Component>();

        public UIWindow OwnerWindow { get; private set; }
        public GameObject GameObject { get; private set; }
        public Transform Transform => GameObject != null ? GameObject.transform : null;

        public void Initialize(UIWindow ownerWindow, GameObject root)
        {
            OwnerWindow = ownerWindow ?? throw new ArgumentNullException(nameof(ownerWindow));
            GameObject = root ?? throw new ArgumentNullException(nameof(root));
            OnInitialize();
        }

        public T GetComponentCached<T>() where T : Component
        {
            var type = typeof(T);
            if (_componentCache.TryGetValue(type, out var component))
            {
                return (T)component;
            }

            var result = GameObject.GetComponent<T>();
            if (result == null)
            {
                throw new InvalidOperationException($"Component {type.Name} not found on {GameObject.name}.");
            }

            _componentCache[type] = result;
            return result;
        }

        public virtual void Refresh() { }

        protected virtual void OnInitialize() { }
    }
}

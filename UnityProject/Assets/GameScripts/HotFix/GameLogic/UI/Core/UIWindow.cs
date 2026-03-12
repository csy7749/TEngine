using System;
using GameLogic.UI.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic.UI
{
    public abstract class UIWindow
    {
        private bool _created;
        private bool _visible;
        private RectTransform _rectTransform;
        private Canvas _canvas;
        private GraphicRaycaster _raycaster;

        protected UIWindow()
        {
            Bindings = new BindingManager();
        }

        public string WindowId { get; private set; }
        public UILayer Layer { get; private set; }
        public GameObject GameObject { get; private set; }
        public Transform Transform => GameObject != null ? GameObject.transform : null;
        public BindingManager Bindings { get; }

        protected RectTransform RectTransform => _rectTransform ??= GameObject.GetComponent<RectTransform>();
        protected Canvas Canvas => _canvas ??= GameObject.GetComponentInChildren<Canvas>(true);
        protected GraphicRaycaster GraphicRaycaster => _raycaster ??= GameObject.GetComponentInChildren<GraphicRaycaster>(true);

        internal void Initialize(UIWindowContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            WindowId = string.IsNullOrWhiteSpace(context.WindowId) ? GetType().Name : context.WindowId;
            Layer = context.Layer;
            GameObject = context.Instance;
        }

        internal void ShowInternal()
        {
            if (!_created)
            {
                _created = true;
                OnCreate();
            }

            if (_visible)
            {
                return;
            }

            _visible = true;
            OnShow();
        }

        internal void HideInternal()
        {
            if (!_visible)
            {
                return;
            }

            _visible = false;
            OnHide();
        }

        internal void DestroyInternal()
        {
            if (_visible)
            {
                _visible = false;
                OnHide();
            }

            Bindings.ClearBindings();
            OnDestroy();
        }

        protected virtual void OnCreate() { }
        protected virtual void OnShow() { }
        protected virtual void OnHide() { }
        protected virtual void OnDestroy() { }
    }
}

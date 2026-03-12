using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameLogic.UI.UXTools.Components
{
    [RequireComponent(typeof(Toggle))]
    public sealed class UXToggle : UXComponent
    {
        [SerializeField] private string _onColorKey;
        [SerializeField] private string _offColorKey;
        [SerializeField] private bool _useColorKeys = true;
        [SerializeField] private Graphic _targetGraphic;

        private Toggle _toggle;
        private UnityAction<bool> _valueChanged;
        private bool _subscribed;

        protected override void OnInitialize()
        {
            _toggle = GetComponent<Toggle>();
            if (_toggle == null)
            {
                throw new InvalidOperationException("Toggle component is required.");
            }

            if (_targetGraphic == null)
            {
                _targetGraphic = _toggle.targetGraphic;
            }

            if (_targetGraphic == null)
            {
                throw new InvalidOperationException("Target graphic is required for UXToggle.");
            }

            _valueChanged = OnValueChanged;
            Subscribe();
        }

        protected override void OnRefresh()
        {
            ApplyState(_toggle.isOn);
        }

        protected override void OnRelease()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            if (_subscribed)
            {
                return;
            }

            _toggle.onValueChanged.AddListener(_valueChanged);
            _subscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_subscribed)
            {
                return;
            }

            _toggle.onValueChanged.RemoveListener(_valueChanged);
            _subscribed = false;
        }

        private void OnValueChanged(bool isOn)
        {
            ApplyState(isOn);
        }

        private void ApplyState(bool isOn)
        {
            if (!_useColorKeys)
            {
                return;
            }

            var key = ResolveColorKey(isOn);
            _targetGraphic.color = Manager.Colors.GetColor(key);
        }

        private string ResolveColorKey(bool isOn)
        {
            var key = isOn ? _onColorKey : _offColorKey;
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidOperationException("Toggle color key is required.");
            }

            return key;
        }
    }
}

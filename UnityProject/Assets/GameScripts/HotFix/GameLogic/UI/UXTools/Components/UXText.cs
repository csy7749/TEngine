using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic.UI.UXTools.Components
{
    [RequireComponent(typeof(Text))]
    public sealed class UXText : UXComponent
    {
        [SerializeField] private string _localizationKey;
        [SerializeField] private bool _useLocalization = true;
        [SerializeField] private string _colorKey;
        [SerializeField] private bool _useColorKey = false;

        private Text _text;

        protected override void OnInitialize()
        {
            _text = GetComponent<Text>();
            if (_text == null)
            {
                throw new InvalidOperationException("Text component is required.");
            }
        }

        protected override void OnRefresh()
        {
            ApplyLocalization();
            ApplyColor();
        }

        private void ApplyLocalization()
        {
            if (!_useLocalization)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(_localizationKey))
            {
                throw new InvalidOperationException("Localization key is required for UXText.");
            }

            _text.text = Manager.Localization.GetText(_localizationKey);
        }

        private void ApplyColor()
        {
            if (!_useColorKey)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(_colorKey))
            {
                throw new InvalidOperationException("Color key is required for UXText.");
            }

            _text.color = Manager.Colors.GetColor(_colorKey);
        }
    }
}

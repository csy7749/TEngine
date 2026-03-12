using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic.UI.UXTools.Components
{
    [RequireComponent(typeof(Image))]
    public sealed class UXImage : UXComponent
    {
        [SerializeField] private string _colorKey;
        [SerializeField] private bool _useColorKey = true;
        [SerializeField] private Sprite _spriteOverride;
        [SerializeField] private bool _useSpriteOverride = false;

        private Image _image;

        protected override void OnInitialize()
        {
            _image = GetComponent<Image>();
            if (_image == null)
            {
                throw new InvalidOperationException("Image component is required.");
            }
        }

        protected override void OnRefresh()
        {
            ApplyColor();
            ApplySprite();
        }

        private void ApplyColor()
        {
            if (!_useColorKey)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(_colorKey))
            {
                throw new InvalidOperationException("Color key is required for UXImage.");
            }

            _image.color = Manager.Colors.GetColor(_colorKey);
        }

        private void ApplySprite()
        {
            if (!_useSpriteOverride)
            {
                return;
            }

            if (_spriteOverride == null)
            {
                throw new InvalidOperationException("Sprite override is required for UXImage.");
            }

            _image.sprite = _spriteOverride;
        }
    }
}

using System;
using GameLogic.UI.MVVM;
using UnityEngine.UI;

namespace GameLogic.UI.Examples
{
    public sealed class SampleWindow : UIWindow
    {
        private const string NameTextPath = "Root/NameText";
        private const string ReadyTogglePath = "Root/ReadyToggle";
        private const string ConfirmButtonPath = "Root/ConfirmButton";

        private Text _nameText;
        private Toggle _readyToggle;
        private Button _confirmButton;
        private SampleViewModel _viewModel;

        protected override void OnCreate()
        {
            InitializeComponents();
            InitializeViewModel();
            BindViewModel();
        }

        private void InitializeComponents()
        {
            _nameText = GetComponentAtPath<Text>(NameTextPath);
            _readyToggle = GetComponentAtPath<Toggle>(ReadyTogglePath);
            _confirmButton = GetComponentAtPath<Button>(ConfirmButtonPath);
        }

        private void InitializeViewModel()
        {
            _viewModel = new SampleViewModel();
        }

        private void BindViewModel()
        {
            Bindings.ClearBindings();
            Bindings.Bind(new PropertyBindingOptions<SampleViewModel, Text>(new PropertyBindingOptionsInput<SampleViewModel, Text>
            {
                Source = _viewModel,
                Target = _nameText,
                SourcePropertyName = nameof(SampleViewModel.PlayerName),
                SourceGetter = vm => vm.PlayerName,
                SourceSetter = (vm, value) => vm.PlayerName = (string)value,
                TargetGetter = target => target.text,
                TargetSetter = (target, value) => target.text = (string)value,
                SourceValueType = typeof(string),
                TargetValueType = typeof(string),
                Mode = BindingMode.OneWay,
            }));

            Bindings.Bind(new PropertyBindingOptions<SampleViewModel, Toggle>(new PropertyBindingOptionsInput<SampleViewModel, Toggle>
            {
                Source = _viewModel,
                Target = _readyToggle,
                SourcePropertyName = nameof(SampleViewModel.IsReady),
                SourceGetter = vm => vm.IsReady,
                SourceSetter = (vm, value) => vm.IsReady = (bool)value,
                TargetGetter = target => target.isOn,
                TargetSetter = (target, value) => target.isOn = (bool)value,
                SourceValueType = typeof(bool),
                TargetValueType = typeof(bool),
                Mode = BindingMode.OneWay,
            }));

            Bindings.BindCommand(new CommandBindingOptions(new CommandBindingOptionsInput
            {
                Source = new UnityButtonCommandSource(_confirmButton),
                Command = _viewModel.ToggleReadyCommand,
            }));
        }

        private T GetComponentAtPath<T>(string path) where T : UnityEngine.Component
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path is required.", nameof(path));
            }

            var target = Transform.Find(path);
            if (target == null)
            {
                throw new InvalidOperationException($"UI element not found at {path}.");
            }

            var component = target.GetComponent<T>();
            if (component == null)
            {
                throw new InvalidOperationException($"Component {typeof(T).Name} not found at {path}.");
            }

            return component;
        }
    }
}

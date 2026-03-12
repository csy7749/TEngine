using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameLogic.UI.MVVM
{
    public enum BindingMode
    {
        OneWay = 0,
        TwoWay = 1,
        OneTime = 2,
    }

    public interface IBinding : IDisposable { }

    public interface ITargetChangeNotifier
    {
        void Subscribe(Action handler);
        void Unsubscribe(Action handler);
    }

    public sealed class PropertyBindingOptionsInput<TSource, TTarget>
    {
        public TSource Source { get; set; }
        public TTarget Target { get; set; }
        public string SourcePropertyName { get; set; }
        public Func<TSource, object> SourceGetter { get; set; }
        public Action<TSource, object> SourceSetter { get; set; }
        public Func<TTarget, object> TargetGetter { get; set; }
        public Action<TTarget, object> TargetSetter { get; set; }
        public Type SourceValueType { get; set; }
        public Type TargetValueType { get; set; }
        public BindingMode Mode { get; set; } = BindingMode.OneWay;
        public IValueConverter Converter { get; set; }
        public ITargetChangeNotifier TargetChangeNotifier { get; set; }
    }

    public sealed class PropertyBindingOptions<TSource, TTarget>
    {
        public PropertyBindingOptions(PropertyBindingOptionsInput<TSource, TTarget> input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            Source = input.Source ?? throw new ArgumentNullException(nameof(input.Source));
            Target = input.Target ?? throw new ArgumentNullException(nameof(input.Target));
            SourcePropertyName = input.SourcePropertyName ?? string.Empty;
            SourceGetter = input.SourceGetter ?? throw new ArgumentNullException(nameof(input.SourceGetter));
            SourceSetter = input.SourceSetter;
            TargetGetter = input.TargetGetter;
            TargetSetter = input.TargetSetter ?? throw new ArgumentNullException(nameof(input.TargetSetter));
            SourceValueType = input.SourceValueType ?? throw new ArgumentNullException(nameof(input.SourceValueType));
            TargetValueType = input.TargetValueType ?? throw new ArgumentNullException(nameof(input.TargetValueType));
            Mode = input.Mode;
            Converter = input.Converter ?? DefaultValueConverter.Instance;
            TargetChangeNotifier = input.TargetChangeNotifier;
        }

        public TSource Source { get; }
        public TTarget Target { get; }
        public string SourcePropertyName { get; }
        public Func<TSource, object> SourceGetter { get; }
        public Action<TSource, object> SourceSetter { get; }
        public Func<TTarget, object> TargetGetter { get; }
        public Action<TTarget, object> TargetSetter { get; }
        public Type SourceValueType { get; }
        public Type TargetValueType { get; }
        public BindingMode Mode { get; }
        public IValueConverter Converter { get; }
        public ITargetChangeNotifier TargetChangeNotifier { get; }
    }

    public sealed class PropertyBinding<TSource, TTarget> : IBinding
    {
        private readonly PropertyBindingOptions<TSource, TTarget> _options;
        private readonly INotifyPropertyChanged _notifier;
        private readonly Action _targetChangedHandler;

        public PropertyBinding(PropertyBindingOptions<TSource, TTarget> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _notifier = options.Source as INotifyPropertyChanged
                ?? throw new InvalidOperationException("Source must implement INotifyPropertyChanged.");
            _targetChangedHandler = OnTargetChanged;

            if (_options.Mode != BindingMode.OneTime)
            {
                _notifier.PropertyChanged += OnSourcePropertyChanged;
            }

            if (_options.Mode == BindingMode.TwoWay)
            {
                SubscribeTarget();
            }

            UpdateTarget();
        }

        public void Dispose()
        {
            _notifier.PropertyChanged -= OnSourcePropertyChanged;
            UnsubscribeTarget();
        }

        private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (!IsMatchingProperty(args.PropertyName))
            {
                return;
            }

            UpdateTarget();
        }

        private bool IsMatchingProperty(string propertyName)
        {
            return string.IsNullOrEmpty(_options.SourcePropertyName)
                || string.Equals(_options.SourcePropertyName, propertyName, StringComparison.Ordinal);
        }

        private void UpdateTarget()
        {
            var sourceValue = _options.SourceGetter(_options.Source);
            var converted = ConvertValue(sourceValue, _options.TargetValueType, _options.Converter);
            _options.TargetSetter(_options.Target, converted);
        }

        private void OnTargetChanged()
        {
            if (_options.Mode != BindingMode.TwoWay)
            {
                return;
            }

            UpdateSource();
        }

        private void UpdateSource()
        {
            if (_options.SourceSetter == null || _options.TargetGetter == null)
            {
                throw new InvalidOperationException("Two-way binding requires SourceSetter and TargetGetter.");
            }

            var targetValue = _options.TargetGetter(_options.Target);
            var converted = ConvertValueBack(targetValue, _options.SourceValueType, _options.Converter);
            _options.SourceSetter(_options.Source, converted);
        }

        private void SubscribeTarget()
        {
            if (_options.TargetChangeNotifier == null)
            {
                throw new InvalidOperationException("Two-way binding requires TargetChangeNotifier.");
            }

            _options.TargetChangeNotifier.Subscribe(_targetChangedHandler);
        }

        private void UnsubscribeTarget()
        {
            _options.TargetChangeNotifier?.Unsubscribe(_targetChangedHandler);
        }

        private static object ConvertValue(object value, Type targetType, IValueConverter converter)
        {
            if (!converter.TryConvert(value, targetType, out var result))
            {
                throw new InvalidOperationException($"Value conversion failed to {targetType.Name}.");
            }

            return result;
        }

        private static object ConvertValueBack(object value, Type targetType, IValueConverter converter)
        {
            if (!converter.TryConvertBack(value, targetType, out var result))
            {
                throw new InvalidOperationException($"Value conversion failed to {targetType.Name}.");
            }

            return result;
        }
    }

    public interface ICommandSource
    {
        void Subscribe(Action handler);
        void Unsubscribe(Action handler);
        bool Interactable { get; set; }
    }

    public sealed class UnityButtonCommandSource : ICommandSource
    {
        private readonly Button _button;
        private readonly Dictionary<Action, UnityAction> _handlers = new Dictionary<Action, UnityAction>();

        public UnityButtonCommandSource(Button button)
        {
            _button = button ?? throw new ArgumentNullException(nameof(button));
        }

        public bool Interactable
        {
            get => _button.interactable;
            set => _button.interactable = value;
        }

        public void Subscribe(Action handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var unityAction = new UnityAction(handler);
            _handlers[handler] = unityAction;
            _button.onClick.AddListener(unityAction);
        }

        public void Unsubscribe(Action handler)
        {
            if (handler == null)
            {
                return;
            }

            if (_handlers.TryGetValue(handler, out var unityAction))
            {
                _button.onClick.RemoveListener(unityAction);
                _handlers.Remove(handler);
            }
        }
    }

    public sealed class CommandBindingOptionsInput
    {
        public ICommandSource Source { get; set; }
        public ICommand Command { get; set; }
        public object Parameter { get; set; }
        public Func<object> ParameterProvider { get; set; }
        public bool TrackCanExecute { get; set; } = true;
    }

    public sealed class CommandBindingOptions
    {
        public CommandBindingOptions(CommandBindingOptionsInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            Source = input.Source ?? throw new ArgumentNullException(nameof(input.Source));
            Command = input.Command ?? throw new ArgumentNullException(nameof(input.Command));
            Parameter = input.Parameter;
            ParameterProvider = input.ParameterProvider;
            TrackCanExecute = input.TrackCanExecute;
        }

        public ICommandSource Source { get; }
        public ICommand Command { get; }
        public object Parameter { get; }
        public Func<object> ParameterProvider { get; }
        public bool TrackCanExecute { get; }

        public object ResolveParameter()
        {
            return ParameterProvider != null ? ParameterProvider() : Parameter;
        }
    }

    public sealed class CommandBinding : IBinding
    {
        private readonly CommandBindingOptions _options;
        private readonly Action _executeHandler;
        private readonly Action _canExecuteHandler;

        public CommandBinding(CommandBindingOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _executeHandler = Execute;
            _canExecuteHandler = UpdateCanExecute;

            _options.Source.Subscribe(_executeHandler);
            if (_options.TrackCanExecute)
            {
                _options.Command.CanExecuteChanged += _canExecuteHandler;
                UpdateCanExecute();
            }
        }

        public void Dispose()
        {
            _options.Source.Unsubscribe(_executeHandler);
            _options.Command.CanExecuteChanged -= _canExecuteHandler;
        }

        private void Execute()
        {
            var parameter = _options.ResolveParameter();
            if (!_options.Command.CanExecute(parameter))
            {
                return;
            }

            _options.Command.Execute(parameter);
        }

        private void UpdateCanExecute()
        {
            var parameter = _options.ResolveParameter();
            _options.Source.Interactable = _options.Command.CanExecute(parameter);
        }
    }

    public sealed class BindingManager : IDisposable
    {
        private readonly List<IBinding> _bindings = new List<IBinding>();

        public PropertyBinding<TSource, TTarget> Bind<TSource, TTarget>(PropertyBindingOptions<TSource, TTarget> options)
        {
            var binding = new PropertyBinding<TSource, TTarget>(options);
            _bindings.Add(binding);
            return binding;
        }

        public CommandBinding BindCommand(CommandBindingOptions options)
        {
            var binding = new CommandBinding(options);
            _bindings.Add(binding);
            return binding;
        }

        public void ClearBindings()
        {
            for (int i = 0; i < _bindings.Count; i++)
            {
                _bindings[i].Dispose();
            }

            _bindings.Clear();
        }

        public void Dispose()
        {
            ClearBindings();
        }
    }
}

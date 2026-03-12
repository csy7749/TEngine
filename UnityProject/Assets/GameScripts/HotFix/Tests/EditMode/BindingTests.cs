using System;
using NUnit.Framework;
using GameLogic.UI.MVVM;

namespace GameLogic.Tests
{
    public sealed class BindingTests
    {
        private sealed class DummyViewModel : ViewModelBase
        {
            private string _name;

            public string Name
            {
                get => _name;
                set => SetProperty(ref _name, value);
            }
        }

        private sealed class DummyTarget
        {
            public string Text { get; set; }
        }

        private sealed class TestCommandSource : ICommandSource
        {
            private Action _handler;

            public bool Interactable { get; set; }

            public void Subscribe(Action handler)
            {
                _handler = handler;
            }

            public void Unsubscribe(Action handler)
            {
                if (_handler == handler)
                {
                    _handler = null;
                }
            }

            public void Trigger()
            {
                _handler?.Invoke();
            }
        }

        [Test]
        public void PropertyBinding_UpdatesTargetOnSourceChange()
        {
            var viewModel = new DummyViewModel();
            var target = new DummyTarget();

            using var binding = new PropertyBinding<DummyViewModel, DummyTarget>(
                new PropertyBindingOptions<DummyViewModel, DummyTarget>(
                    new PropertyBindingOptionsInput<DummyViewModel, DummyTarget>
                    {
                        Source = viewModel,
                        Target = target,
                        SourcePropertyName = nameof(DummyViewModel.Name),
                        SourceGetter = vm => vm.Name,
                        SourceSetter = (vm, value) => vm.Name = (string)value,
                        TargetGetter = t => t.Text,
                        TargetSetter = (t, value) => t.Text = (string)value,
                        SourceValueType = typeof(string),
                        TargetValueType = typeof(string),
                        Mode = BindingMode.OneWay,
                    }));

            viewModel.Name = "Alice";

            Assert.AreEqual("Alice", target.Text);
        }

        [Test]
        public void CommandBinding_ExecutesAndUpdatesInteractable()
        {
            var executed = false;
            var canExecute = true;
            var command = new RelayCommand(() => executed = true, () => canExecute);
            var source = new TestCommandSource();

            using var binding = new CommandBinding(new CommandBindingOptions(new CommandBindingOptionsInput
            {
                Source = source,
                Command = command,
                TrackCanExecute = true,
            }));

            Assert.IsTrue(source.Interactable);
            source.Trigger();
            Assert.IsTrue(executed);

            executed = false;
            canExecute = false;
            command.RaiseCanExecuteChanged();

            Assert.IsFalse(source.Interactable);
            source.Trigger();
            Assert.IsFalse(executed);
        }
    }
}

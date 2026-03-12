using System.Collections.Generic;
using NUnit.Framework;
using GameLogic.UI.MVVM;

namespace GameLogic.Tests
{
    public sealed class ViewModelBaseTests
    {
        private sealed class TestViewModel : ViewModelBase
        {
            private int _value;

            public int Value
            {
                get => _value;
                set => SetProperty(ref _value, value);
            }
        }

        [Test]
        public void SetProperty_RaisesEventWhenChanged()
        {
            var viewModel = new TestViewModel();
            var changes = new List<string>();
            viewModel.PropertyChanged += (_, args) => changes.Add(args.PropertyName);

            viewModel.Value = 10;

            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual(nameof(TestViewModel.Value), changes[0]);
        }

        [Test]
        public void SetProperty_DoesNotRaiseEventWhenSameValue()
        {
            var viewModel = new TestViewModel();
            var changes = new List<string>();
            viewModel.PropertyChanged += (_, args) => changes.Add(args.PropertyName);

            viewModel.Value = 5;
            viewModel.Value = 5;

            Assert.AreEqual(1, changes.Count);
        }
    }
}

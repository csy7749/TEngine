using GameLogic.UI.MVVM;

namespace GameLogic.UI.Examples
{
    public sealed class SampleViewModel : ViewModelBase
    {
        private string _playerName = "Player";
        private bool _isReady;

        public SampleViewModel()
        {
            ToggleReadyCommand = new RelayCommand(ToggleReady);
        }

        public string PlayerName
        {
            get => _playerName;
            set => SetProperty(ref _playerName, value);
        }

        public bool IsReady
        {
            get => _isReady;
            set => SetProperty(ref _isReady, value);
        }

        public RelayCommand ToggleReadyCommand { get; }

        private void ToggleReady()
        {
            IsReady = !IsReady;
        }
    }
}

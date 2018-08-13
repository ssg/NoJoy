using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace NoJoy
{
    enum GameControllerState
    {
        Disabled,
        Enabled,
        Disabling,
        Enabling,
    }

    class GameController: INotifyPropertyChanged
    {
        private const string standardArguments = "-InformationAction SilentlyContinue -PassThru -Confirm:$false";
        public string Name { get; set; }

        public string DeviceId { get; set; }

        private GameControllerState state;
        public GameControllerState State
        {
            get { return state; }
            set
            {
                state = value;
                onPropertyChanged(nameof(State));
                onPropertyChanged(nameof(IsButtonEnabled));
                onPropertyChanged(nameof(ButtonText));
                onPropertyChanged(nameof(IsEnabled));
            }
        }

        public bool IsEnabled => State == GameControllerState.Enabled;

        public string ButtonText => IsEnabled ? "Disable" : "Enable";

        public bool IsButtonEnabled => (State == GameControllerState.Enabled)
            || (State == GameControllerState.Disabled);

        public event PropertyChangedEventHandler PropertyChanged;

        private void onPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void Enable()
        {
            var oldState = State;
            State = GameControllerState.Enabling;
            changeStateInBackground("Enable-PnpDevice", GameControllerState.Enabled, oldState);
        }

        public void Disable()
        {
            var oldState = State;
            State = GameControllerState.Disabling;
            changeStateInBackground("Disable-PnpDevice", GameControllerState.Disabled, oldState);
        }

        private void changeStateInBackground(string command, GameControllerState newState, GameControllerState oldState)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                if (PowerShell.RunElevated($"{command} {standardArguments} -InstanceId '{DeviceId}'"))
                {
                    State = newState;
                }
                else
                {
                    State = oldState;
                }
            }, this);
        }
    }
}
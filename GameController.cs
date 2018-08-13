/*
Copyright 2018 Sedat Kapanoglu

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System.ComponentModel;
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

    class GameController : INotifyPropertyChanged
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
﻿/*
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
        public string Name { get; set; }

        public string DeviceId { get; set; }

        private GameControllerState state;
        public GameControllerState State
        {
            get => state;
            set
            {
                state = value;
                onPropertyChanged(nameof(State));
                onPropertyChanged(nameof(IsButtonEnabled));
                onPropertyChanged(nameof(IsEnabled));
            }
        }

        private string errorMessage;
        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                errorMessage = value;
                onPropertyChanged(nameof(ErrorMessage));
            }
        }

        public bool IsEnabled => State == GameControllerState.Enabled;

        public bool IsButtonEnabled => (State == GameControllerState.Enabled)
            || (State == GameControllerState.Disabled);

        public event PropertyChangedEventHandler PropertyChanged;

        private void onPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void Enable()
        {
            GameControllerState oldState = State;
            State = GameControllerState.Enabling;
            changeStateInBackground("/enable-device", GameControllerState.Enabled, oldState);
        }

        public void Disable()
        {
            GameControllerState oldState = State;
            State = GameControllerState.Disabling;
            changeStateInBackground("/disable-device", GameControllerState.Disabled, oldState);
        }

        private void changeStateInBackground(string verb, GameControllerState newState,
            GameControllerState oldState)
        {
            ErrorMessage = null;
            _ = ThreadPool.QueueUserWorkItem(delegate
            {
                string command = $"{verb} \"{DeviceId}\"";
                var result = PnpUtil.RunElevated(command);
                if (result.IsSuccess)
                {
                    State = newState;
                }
                else
                {
                    State = oldState;
                    ErrorMessage = result.ErrorMessage;
                }
            }, this);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace NoJoy
{
    class MainWindowViewModel
    {
        public List<GameController> Items { get; set; } = new List<GameController>();

        public Visibility NotFoundVisibility => Items.Any() ? Visibility.Hidden : Visibility.Visible;

        public string LastErrorMessage { get; set; }
    }
}
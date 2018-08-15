using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace NoJoy
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void EnableDisableButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var controller = button.DataContext as GameController;
            switch (controller.State)
            {
                case GameControllerState.Enabled:
                    controller.Disable();
                    break;

                case GameControllerState.Disabled:
                    controller.Enable();
                    break;
            }
        }

        private void OnErrorCloseButtonClicked(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var controller = button.DataContext as GameController;
            controller.ErrorMessage = null;
        }

        private void nameLink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/ssg/NoJoy");
        }
    }
}
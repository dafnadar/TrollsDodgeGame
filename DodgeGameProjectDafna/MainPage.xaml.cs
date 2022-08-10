using Logic;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DodgeGameProjectDafna
{
    public sealed partial class MainPage : Page
    {
        Manager manager;
        public MainPage()
        {
            this.InitializeComponent();
            manager = new Manager(cnv);
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Up) manager._goUp = true;
            if (e.Key == VirtualKey.Down) manager._goDown = true;
            if (e.Key == VirtualKey.Left) manager._goLeft = true;
            if (e.Key == VirtualKey.Right) manager._goRight = true;
            manager.GoodPMovement();
        }

        private void Page_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Up) manager._goUp = false;
            if (e.Key == VirtualKey.Down) manager._goDown = false;
            if (e.Key == VirtualKey.Left) manager._goLeft = false;
            if (e.Key == VirtualKey.Right) manager._goRight = false;
        }
    }
}

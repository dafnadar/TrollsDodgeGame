using System;
using Windows.UI.Xaml.Media.Imaging;

namespace Model
{
    public class BadPlayer : Players
    {
        public BadPlayer(): base()
        {
            MyImg.Source = new BitmapImage(new Uri(@"ms-appx:///Assets\badP.png"));
        }
    }
}

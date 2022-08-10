using System;
using Windows.UI.Xaml.Media.Imaging;

namespace Model
{
    public class GoodPlayer: Players
    {
        public GoodPlayer() : base()
        {
            MyImg.Source = new BitmapImage(new Uri(@"ms-appx:///Assets\goodP.png"));
        }
    }
}

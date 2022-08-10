using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Model
{
    public class Players
    {
        Image img;

        public Image MyImg
        {
            get { return img; }
            protected set { MyImg = value; }
        }

        public double ImgX
        {
            get { return Canvas.GetLeft(MyImg); }
            set { Canvas.SetLeft(MyImg, value); }
        }

        public double ImgY
        {
            get { return Canvas.GetTop(MyImg); }
            set { Canvas.SetTop(MyImg, value); }
        }

        public Players()
        {
            img = new Image();
            img.Width = 50;
            img.Height = 100;
            img.Stretch = Stretch.Fill;
        }
    }
}

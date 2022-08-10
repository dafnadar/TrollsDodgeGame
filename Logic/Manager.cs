using Model;
using System;
using System.Collections.Generic;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Logic
{
    public class Manager
    {
        Canvas _canvas;
        Random _rand = new Random();        
        GoodPlayer _goodTrol;
        List<BadPlayer> _badTrol;
        public bool _goUp = false;
        public bool _goDown = false;
        public bool _goLeft = false;
        public bool _goRight = false;
        bool _isTooClose = false;
        public DispatcherTimer _tmr;
        bool _isPlaying = false;
        Button _newGameBtn = new Button();
        Button _pauseBtn = new Button();
        Button _playBtn = new Button();
        Button _exitBtn = new Button();
        TextBlock _tbWins = new TextBlock();
        TextBlock _tbWinsCnt = new TextBlock();
        TextBlock _tbLives = new TextBlock();
        TextBlock _tbLivesCnt = new TextBlock();
        int _cntWins, _cntLives;
        MediaPlayer _music;
        Button _soundBtn;
        Button _muteBtn;
        double _baddiStep;

        public Manager(Canvas cnv)
        {
            _canvas = cnv;
            _cntLives = 3;
            _cntWins = 0;
            _baddiStep = 1;
            SetPlayers();
            SetButtons();
            _cntWins = 0;
            _music = new MediaPlayer();
            _tmr = new DispatcherTimer();
            _tmr.Interval = TimeSpan.FromMilliseconds(30);
            _tmr.Tick += TmrManagement;
            _tmr.Start();
            _isPlaying = true;
            
        }

        private void TmrManagement(object sender, object e)
        {
            BadPMovement();
            GoodNbadCollision();
            BaddiesCollision();
        }

        public void AddGoodPlayer()
        {
            _goodTrol = new GoodPlayer();
            Canvas.SetTop(_goodTrol.MyImg, Window.Current.Bounds.Height / 2);
            Canvas.SetLeft(_goodTrol.MyImg, Window.Current.Bounds.Width / 2);
            _canvas.Children.Add(_goodTrol.MyImg);
        }

        public void AddBadPlayer()
        {
            _badTrol = new List<BadPlayer>();
            double yScreen = Window.Current.Bounds.Height;
            double xScreen = Window.Current.Bounds.Width;

            for (int i = 0; i < 10; i++)
            {
                double x = _rand.Next(180, (int)xScreen - 200);
                double y = _rand.Next(100, (int)yScreen - 130);
                _isTooClose = CheckLocations(x, y);
                if (!_isTooClose)
                {
                    _badTrol.Add(new BadPlayer());
                    Canvas.SetLeft(_badTrol[i].MyImg, x);
                    Canvas.SetTop(_badTrol[i].MyImg, y);
                    _canvas.Children.Add(_badTrol[i].MyImg);
                }
                else
                {
                    i--;
                }
            }
        }

        public bool CheckLocations(double x, double y)
        {
            if (Math.Abs(_goodTrol.ImgX - x) < 150 && Math.Abs(_goodTrol.ImgY - y) < 200)
                return true;
            else
            {
                foreach (var bTrol in _badTrol)
                {
                    if (Math.Abs(bTrol.ImgX - x) < 150 && Math.Abs(bTrol.ImgY - y) < 200) return true;
                }
            }
            return false;
        }

        public void GoodPMovement()
        {
            if (_isPlaying)
            {
                double xLimit = Window.Current.Bounds.Width - 200;
                double yLimit = Window.Current.Bounds.Height - 130;
                double x = _goodTrol.ImgX;
                double y = _goodTrol.ImgY;
                if (_goUp == true && y > 120) Canvas.SetTop(_goodTrol.MyImg, y - 12);
                if (_goDown == true && y < yLimit) Canvas.SetTop(_goodTrol.MyImg, y + 12);
                if (_goLeft == true && x > 180) Canvas.SetLeft(_goodTrol.MyImg, x - 12);
                if (_goRight == true && x < xLimit) Canvas.SetLeft(_goodTrol.MyImg, x + 12);
            }
        }

        public void BadPMovement()
        {
            foreach (var bTrol in _badTrol)
            {
                if (Canvas.GetTop(_goodTrol.MyImg) > Canvas.GetTop(bTrol.MyImg))
                {
                    Canvas.SetTop(bTrol.MyImg, Canvas.GetTop(bTrol.MyImg) + _baddiStep);
                }
                else Canvas.SetTop(bTrol.MyImg, Canvas.GetTop(bTrol.MyImg) - _baddiStep);

                if (Canvas.GetLeft(_goodTrol.MyImg) > Canvas.GetLeft(bTrol.MyImg))
                {
                    Canvas.SetLeft(bTrol.MyImg, Canvas.GetLeft(bTrol.MyImg) + _baddiStep);
                }
                else Canvas.SetLeft(bTrol.MyImg, Canvas.GetLeft(bTrol.MyImg) - _baddiStep);
            }
        }

        public void BaddiesCollision()
        {
            for (int i = 0; i < _badTrol.Count; i++)
            {
                double xi = Canvas.GetLeft(_badTrol[i].MyImg);
                double yi = Canvas.GetTop(_badTrol[i].MyImg);
                for (int j = i + 1; j < _badTrol.Count; j++)
                {
                    double xj = Canvas.GetLeft(_badTrol[j].MyImg);
                    double yj = Canvas.GetTop(_badTrol[j].MyImg);
                    if (Math.Abs(xi - xj) < 45 && Math.Abs(yi - yj) < 90)
                    {
                        _canvas.Children.Remove(_badTrol[j].MyImg);
                        _badTrol.Remove(_badTrol[j]);
                        if (_badTrol.Count < 2)
                            WinGame();
                    }
                }
            }
        }

        public void GoodNbadCollision()
        {
            double xGood = _goodTrol.ImgX;
            double yGood = _goodTrol.ImgY;
            for (int i = 0; i < _badTrol.Count; i++)
            {
                double xBad = Canvas.GetLeft(_badTrol[i].MyImg);
                double yBad = Canvas.GetTop(_badTrol[i].MyImg);
                if (Math.Abs(xGood - xBad) < 42 && Math.Abs(yGood - yBad) < 90)
                {
                    _canvas.Children.Remove(_goodTrol.MyImg);
                    _tmr.Stop();
                    _isPlaying = false;
                    _cntLives--;
                    _tbLivesCnt.Text = _cntLives.ToString();
                    if (_cntLives == 0) LostGame();
                }
            }
        }

        private async void WinGame()
        {
            _cntWins++;
            _baddiStep = _baddiStep + 0.25;
            _tbWinsCnt.Text = _cntWins.ToString();
            _tmr.Stop();
            _isPlaying = false;
            await new MessageDialog("Good Job! Let's step it up...", "You Won !  ").ShowAsync();
        }

        private async void LostGame()
        {
            _tmr.Stop();
            _isPlaying = false;
            _baddiStep = 1;
            var closeCommand = new UICommand("Close");
            var dialog = new MessageDialog("Let's try again... :)", "GAME OVER !");
            dialog.Options = MessageDialogOptions.None;
            dialog.Commands.Add(closeCommand);
            var command = await dialog.ShowAsync();
            if (command == closeCommand)
            {
                _cntLives = 3;
                _cntWins = 0;
                _tbLivesCnt.Text = _cntLives.ToString();
                _tbWinsCnt.Text = _cntWins.ToString();
            }
        }

        public void Btn_NewGame()
        {
            _newGameBtn.Content = "New Game";
            _newGameBtn.Background = new SolidColorBrush(Colors.LightGray);
            _newGameBtn.Width = 100;
            _newGameBtn.Height = 30;
            Canvas.SetTop(_newGameBtn, 10);
            Canvas.SetLeft(_newGameBtn, 10);
            _newGameBtn.Click += NewGame_Click;
            _canvas.Children.Add(_newGameBtn);
        }

        public void Btn_Pause()
        {
            _pauseBtn.Content = "Pause";
            _pauseBtn.Background = new SolidColorBrush(Colors.LightGray);
            _pauseBtn.Width = 100;
            _pauseBtn.Height = 30;
            Canvas.SetTop(_pauseBtn, 10);
            Canvas.SetLeft(_pauseBtn, 130);
            _pauseBtn.Click += Pause_Click;
            _canvas.Children.Add(_pauseBtn);
        }

        private void SetPlayers()
        {
            AddGoodPlayer();
            AddBadPlayer();
        }

        public void Btn_Play()
        {
            _playBtn.Content = "Play";
            _playBtn.Background = new SolidColorBrush(Colors.LightGray);
            _playBtn.Width = 100;
            _playBtn.Height = 30;
            Canvas.SetTop(_playBtn, 10);
            Canvas.SetLeft(_playBtn, 250);
            _playBtn.Click += Play_Click;
            _canvas.Children.Add(_playBtn);
        }

        private void Btn_Exit()
        {
            _exitBtn.Content = "Exit";
            _exitBtn.Background = new SolidColorBrush(Colors.IndianRed);
            _exitBtn.Width = 100;
            _exitBtn.Height = 30;
            Canvas.SetTop(_exitBtn, 10);
            Canvas.SetLeft(_exitBtn, 1800);
            _exitBtn.Click += Exit_Click;
            _canvas.Children.Add(_exitBtn);
        }

        private void SetButtons()
        {
            Btn_NewGame();
            Btn_Pause();
            Btn_Play();
            Btn_Exit();
            Tb_wins();
            Tb_Lives();
            WinsCnt();
            LivesCnt();
            Btn_Sound();
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            _canvas.Children.Remove(_goodTrol.MyImg);
            foreach (var i in _badTrol)
            {
                _canvas.Children.Remove(i.MyImg);
            }            
            _isTooClose = false;
            SetPlayers();
            if (_cntLives == 0) _tbLivesCnt.Text = "3";
            _tmr.Start();
            _isPlaying = true;
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            _tmr.Stop();
            _isPlaying = false;
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            _tmr.Start();
            _isPlaying = true;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            ExitMsg();
        }

        private async void ExitMsg()
        {
            var title = "Exit Game";
            var content = "Are you sure you want to quit?";
            var yesCommand = new UICommand("Yes, I'm sure");
            var noCommand = new UICommand("No");

            var dialog = new MessageDialog(content, title);
            dialog.Options = MessageDialogOptions.None;
            dialog.Commands.Add(yesCommand);
            dialog.Commands.Add(noCommand);

            dialog.DefaultCommandIndex = 1;

            var command = await dialog.ShowAsync();

            if (command == yesCommand)
            {
                Application.Current.Exit();
            }
        }

        private void Tb_wins()
        {
            _tbWins.Text = "Wins:";
            _tbWins.Height = 30;
            _tbWins.Width = 60;
            _tbWins.FontSize = 24;
            _tbWins.FontStyle = FontStyle.Italic;
            Canvas.SetTop(_tbWins, 10);
            Canvas.SetLeft(_tbWins, 1300);
            _canvas.Children.Add(_tbWins);
        }

        private void WinsCnt()
        {
            _tbWinsCnt.Text = _cntWins.ToString();
            _tbWinsCnt.Height = 30;
            _tbWinsCnt.Width = 40;
            _tbWinsCnt.FontSize = 24;
            _tbWinsCnt.FontStyle = FontStyle.Italic;
            Canvas.SetTop(_tbWinsCnt, 10);
            Canvas.SetLeft(_tbWinsCnt, 1380);
            _canvas.Children.Add(_tbWinsCnt);
        }

        private void Tb_Lives()
        {
            _tbLives.Text = "Lives:";
            _tbLives.Height = 30;
            _tbLives.Width = 60;
            _tbLives.FontSize = 24;
            _tbLives.FontStyle = FontStyle.Italic;
            Canvas.SetTop(_tbLives, 10);
            Canvas.SetLeft(_tbLives, 1100);
            _canvas.Children.Add(_tbLives);
        }

        private void LivesCnt()
        {
            _tbLivesCnt.Text = _cntLives.ToString();
            _tbLivesCnt.Height = 30;
            _tbLivesCnt.Width = 40;
            _tbLivesCnt.FontSize = 24;
            _tbLivesCnt.FontStyle = FontStyle.Italic;
            Canvas.SetTop(_tbLivesCnt, 10);
            Canvas.SetLeft(_tbLivesCnt, 1180);
            _canvas.Children.Add(_tbLivesCnt);
        }

        private void Btn_Sound()
        {
            _soundBtn = new Button
            {
                Background = null,
                Width = 50,
                Height = 50,
                Content = new Image
                {
                    Source = new BitmapImage(new Uri(@"ms-appx:///Assets\soundIcon.png")),
                    VerticalAlignment = VerticalAlignment.Center
                }
            };
            _soundBtn.Click += BtnMusic_Click;
            Canvas.SetTop(_soundBtn, 0);
            Canvas.SetLeft(_soundBtn, 1500);
            _canvas.Children.Add(_soundBtn);
        }

        private void Btn_Mute()
        {
            _muteBtn = new Button
            {
                Background = null,
                Width = 50,
                Height = 50,
                Content = new Image
                {
                    Source = new BitmapImage(new Uri(@"ms-appx:///Assets\muteIcon.png")),
                    VerticalAlignment = VerticalAlignment.Center
                }
            };
            _muteBtn.Click += BtnMute_Click;
            Canvas.SetTop(_muteBtn, 0);
            Canvas.SetLeft(_muteBtn, 1500);
            _canvas.Children.Add(_muteBtn);
        }

        private void BtnMute_Click(object sender, RoutedEventArgs e)
        {
            _music.Source = null;
            _canvas.Children.Remove(_muteBtn);
            Btn_Sound();
        }

        private async void BtnMusic_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.StorageFolder musicFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets");
            Windows.Storage.StorageFile musicFile = await musicFolder.GetFileAsync("SoundtrackFoxBoggie.mp3");

            _music.AutoPlay = true;
            _music.Source = MediaSource.CreateFromStorageFile(musicFile);

            _music.Play();
            _canvas.Children.Remove(_soundBtn);
            Btn_Mute();
        }
    }
   
}

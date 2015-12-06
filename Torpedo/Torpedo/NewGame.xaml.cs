using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Torpedo.Modell;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Torpedo
{
    public partial class NewGame : PhoneApplicationPage
    {
        Rectangle[,] computerTectArray;
        Rectangle[,] youRectArray;
        Table table;

        int fieldWidth;

        public NewGame()
        {
            InitializeComponent();
            table = new Table();
        }

        Image bombimg;

        void rect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            computerCanvas.IsHitTestVisible = false;

            Rectangle rect = (Rectangle)sender;

            int row = (int)rect.Resources["row"];
            int col = (int)rect.Resources["col"];

            Storyboard sb = new Storyboard();

            DoubleAnimationUsingKeyFrames verticaldaukf = new DoubleAnimationUsingKeyFrames();
            LinearDoubleKeyFrame verticalldkf = new LinearDoubleKeyFrame();

            DoubleAnimationUsingKeyFrames horizontaldaukf = new DoubleAnimationUsingKeyFrames();
            LinearDoubleKeyFrame horizontalldkf = new LinearDoubleKeyFrame();

            verticalldkf.KeyTime = TimeSpan.FromSeconds(0);
            verticalldkf.Value = (row - 1) * fieldWidth;
            verticaldaukf.KeyFrames.Add(verticalldkf);

            sb.Duration = TimeSpan.FromMilliseconds(200);
            verticaldaukf.Duration = TimeSpan.FromMilliseconds(200);

            verticalldkf = new LinearDoubleKeyFrame();

            verticalldkf.KeyTime = TimeSpan.FromMilliseconds(200);
            verticalldkf.Value = row * fieldWidth;
            verticaldaukf.KeyFrames.Add(verticalldkf);

            Uri address = address = new Uri("/Images/bomb.png", UriKind.Relative);

            BitmapImage bmp = new BitmapImage();
            bmp.UriSource = address;
            Image bombimg = new Image();
            bombimg.Source = bmp;

            bombimg.Width = fieldWidth;
            bombimg.Height = fieldWidth;

            Thickness thickness = new Thickness();
            thickness.Left = col * fieldWidth;
            bombimg.Margin = thickness;

            bombimg.RenderTransform = new TranslateTransform();
            Storyboard.SetTarget(verticaldaukf, bombimg);
            Storyboard.SetTargetProperty(verticaldaukf, new PropertyPath(Canvas.TopProperty));

            sb.Children.Add(verticaldaukf);

            computerCanvas.Children.Add(bombimg);

            sb.Begin();
            sb.Completed += new EventHandler(sb_Completed);

            if (table.ComputerTable[row, col] == 0)
            {
                computerTectArray[row, col].Fill = new SolidColorBrush(Color.FromArgb(255, 15, 124, 125));
                table.ComputerTable[row, col] = 2;
            }
            else if (table.ComputerTable[row, col] == 1)
            {
                computerTectArray[row, col].Fill = new SolidColorBrush(Color.FromArgb(255, 252, 108, 133));
                table.ComputerTable[row, col] = 3;
                computerCanvas.IsHitTestVisible = true;

            }

            if (table.checkYouWin())
            {
                //you win
                NavigationService.Navigate(new Uri("/YouWinPage.xaml?steps=" + table.StepCount, UriKind.Relative));
                computerCanvas.IsHitTestVisible = false;
            }
        }

        DispatcherTimer afterYouStepDispatcherTimer;

        void sb_Completed(object sender, EventArgs e)
        {
            //computerCanvas.Children.Remove(bombimg);
            ++table.StepCount;

            if (!computerCanvas.IsHitTestVisible)
            {
                afterYouStepDispatcherTimer = new DispatcherTimer();
                afterYouStepDispatcherTimer.Interval = TimeSpan.FromSeconds(2);

                afterYouStepDispatcherTimer.Tick += new EventHandler(afterYouStepDispatcherTimer_Tick);

                afterYouStepDispatcherTimer.Start();
            }
        }

        DispatcherTimer afterComputerStepDispatcherTimer;

        bool computerHit = false;

        void afterYouStepDispatcherTimer_Tick(object sender, EventArgs e)
        {
            afterYouStepDispatcherTimer.Stop();

            afterComputerStepDispatcherTimer = new DispatcherTimer();
            afterComputerStepDispatcherTimer.Interval = TimeSpan.FromSeconds(2);
            afterComputerStepDispatcherTimer.Tick += new EventHandler(afterComputerStepDispatcherTimer_Tick);
            afterComputerStepDispatcherTimer.Start();

            torpedoPivot.SelectedIndex = 1;

            computerFireAnimation();
        }


        DispatcherTimer reBombTimer;

        private void computerFireAnimation()
        {
            computerHit = false;

            Point? computerShot = table.computerShot();

            if (computerShot != null)
            {
                Point p = (Point)computerShot;

                Storyboard sb = new Storyboard();

                DoubleAnimationUsingKeyFrames verticaldaukf = new DoubleAnimationUsingKeyFrames();
                LinearDoubleKeyFrame verticalldkf = new LinearDoubleKeyFrame();

                DoubleAnimationUsingKeyFrames horizontaldaukf = new DoubleAnimationUsingKeyFrames();
                LinearDoubleKeyFrame horizontalldkf = new LinearDoubleKeyFrame();

                verticalldkf.KeyTime = TimeSpan.FromSeconds(0);
                verticalldkf.Value = (p.Y - 1) * fieldWidth;
                verticaldaukf.KeyFrames.Add(verticalldkf);

                sb.Duration = TimeSpan.FromMilliseconds(800);
                verticaldaukf.Duration = TimeSpan.FromMilliseconds(800);

                verticalldkf = new LinearDoubleKeyFrame();

                verticalldkf.KeyTime = TimeSpan.FromMilliseconds(600);
                verticalldkf.Value = (p.Y - 1) * fieldWidth;
                verticaldaukf.KeyFrames.Add(verticalldkf);

                verticalldkf = new LinearDoubleKeyFrame();

                verticalldkf.KeyTime = TimeSpan.FromMilliseconds(800);
                verticalldkf.Value = p.Y * fieldWidth;
                verticaldaukf.KeyFrames.Add(verticalldkf);

                Uri address = address = new Uri("/Images/bomb.png", UriKind.Relative);

                BitmapImage bmp = new BitmapImage();
                bmp.UriSource = address;
                Image bombimg = new Image();
                bombimg.Source = bmp;

                bombimg.Width = fieldWidth;
                bombimg.Height = fieldWidth;

                Thickness thickness = new Thickness();
                thickness.Left = p.X * fieldWidth;
                bombimg.Margin = thickness;

                bombimg.RenderTransform = new TranslateTransform();
                Storyboard.SetTarget(verticaldaukf, bombimg);
                Storyboard.SetTargetProperty(verticaldaukf, new PropertyPath(Canvas.TopProperty));

                sb.Children.Add(verticaldaukf);

                youCanvas.Children.Add(bombimg);

                sb.Begin();


                if (table.YouTable[(int)p.X, (int)p.Y] == 0)
                {
                    youRectArray[(int)p.X, (int)p.Y].Fill = new SolidColorBrush(Color.FromArgb(255, 15, 124, 125));
                    table.YouTable[(int)p.X, (int)p.Y] = 2;
                }
                else if (table.YouTable[(int)p.X, (int)p.Y] == 1)
                {
                    youRectArray[(int)p.X, (int)p.Y].Fill = new SolidColorBrush(Color.FromArgb(255, 252, 108, 133));
                    table.YouTable[(int)p.X, (int)p.Y] = 3;

                    computerHit = true;

                    if (!table.checkComputerWin())
                    {
                        reBombTimer = new DispatcherTimer();
                        reBombTimer.Interval = TimeSpan.FromMilliseconds(500);
                        reBombTimer.Tick += new EventHandler(reBombTimer_Tick);
                        reBombTimer.Start();
                    }
                }

                if (table.checkComputerWin())
                {
                    if (MessageBoxResult.OK == MessageBox.Show("Game over, the phone won this game, try again :)"))
                    {
                        NavigationService.GoBack();
                    }
                }
            }
        }

        void reBombTimer_Tick(object sender, EventArgs e)
        {
            reBombTimer.Stop();
            computerFireAnimation();
        }



        void afterComputerStepDispatcherTimer_Tick(object sender, EventArgs e)
        {
            afterComputerStepDispatcherTimer.Stop();

            computerCanvas.IsHitTestVisible = true;
            torpedoPivot.SelectedIndex = 0;
        }

        private void youCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            fieldWidth = (int)youCanvas.Width / table.TableWidth;

            youRectArray = new Rectangle[table.TableWidth, table.TableHeight];

            for (int i = 0; i < table.TableWidth; i++)
            {
                for (int j = 0; j < table.TableHeight; j++)
                {
                    Rectangle rect = new Rectangle();

                    rect.Stroke = new SolidColorBrush(Colors.Blue);
                    rect.StrokeThickness = 2;
                    rect.Width = fieldWidth;
                    rect.Height = fieldWidth;
                    youRectArray[i, j] = rect;

                    rect.Fill = new SolidColorBrush(Color.FromArgb(255, 106, 162, 175));

                    Thickness thickness = new Thickness();
                    thickness.Left = (double)(i * fieldWidth);
                    thickness.Top = (double)(j * fieldWidth);
                    rect.Margin = thickness;

                    youCanvas.Children.Add(rect);
                }
            }

            for (int i = 0; i < table.ShipList.Count; i++)
            {
                Ship ship = table.ShipList.ElementAt(i);
                Uri address = address = new Uri("/Images/Ship/" + ship.Direction.ToString() + ship.Length.ToString() + ".png", UriKind.Relative);

                BitmapImage bmp = new BitmapImage();
                bmp.UriSource = address;
                Image img = new Image();
                img.Source = bmp;

                Thickness thickness = new Thickness();
                thickness.Left = (double)(ship.StartPoint.X * fieldWidth);
                thickness.Top = (double)(ship.StartPoint.Y * fieldWidth);
                img.Margin = thickness;

                if (ship.Direction == 0)
                {
                    img.Width = (double)fieldWidth * ship.Length;
                    img.Height = (double)fieldWidth;
                }
                else
                {
                    img.Width = (double)fieldWidth;
                    img.Height = (double)fieldWidth * ship.Length;
                }

                youCanvas.Children.Add(img);
            }
        }

        private void computerCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            fieldWidth = (int)computerCanvas.Width / table.TableWidth;

            computerTectArray = new Rectangle[table.TableWidth, table.TableHeight];

            for (int i = 0; i < table.TableWidth; i++)
            {
                for (int j = 0; j < table.TableHeight; j++)
                {
                    Rectangle rect = new Rectangle();

                    rect.Resources.Add("row", j);
                    rect.Resources.Add("col", i);
                    rect.MouseLeftButtonUp += new MouseButtonEventHandler(rect_MouseLeftButtonUp);
                    computerTectArray[j, i] = rect;

                    rect.Stroke = new SolidColorBrush(Colors.Blue);
                    rect.StrokeThickness = 2;
                    rect.Width = fieldWidth;
                    rect.Height = fieldWidth;

                    rect.Fill = new SolidColorBrush(Color.FromArgb(255, 106, 162, 175));

                    Thickness thickness = new Thickness();
                    thickness.Left = (double)(i * fieldWidth);
                    thickness.Top = (double)(j * fieldWidth);
                    rect.Margin = thickness;

                    computerCanvas.Children.Add(rect);
                }
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (table.checkYouWin() || table.checkComputerWin())
                NavigationService.GoBack();

            //base.OnNavigatedTo(e);
        }
    }
}
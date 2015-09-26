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
using MoovingBalls.Modell;
using System.Windows.Resources;
using System.Windows.Media.Imaging;
using System.Text;
using System.Windows.Markup;
using RouteEngine;
using System.IO.IsolatedStorage;
using System.IO;

namespace MoovingBalls
{
    public partial class NewGame : PhoneApplicationPage
    {
        Table table;

        Rectangle[,] rectArray;
        List<Image> ballImgList;

        public NewGame()
        {
            InitializeComponent();

            ballImgList = new List<Image>();
            table = new Table();
            rectArray = new Rectangle[table.TableWidth, table.TableHeight];

            for (int i = 0; i < table.TableWidth; i++)
            {
                for (int j = 0; j < table.TableHeight; j++)
                {
                    Rectangle rect = new Rectangle();                    

                    rect.Resources.Add("row", j);
                    rect.Resources.Add("col", i);
                    rect.MouseLeftButtonUp += new MouseButtonEventHandler(rect_MouseLeftButtonUp);
                    rectArray[j, i] = rect;

                    rect.Stroke = new SolidColorBrush(Colors.Green);
                    rect.StrokeThickness = 3;
                    rect.Width = 53;
                    rect.Height = 53;
            
                    rect.Fill = new SolidColorBrush(Colors.Gray);


                    Thickness thickness = new Thickness();
                    thickness.Left = (double)(i * 53);
                    thickness.Top = (double)(j * 53);
                    rect.Margin = thickness;

                    gameCanvas.Children.Add(rect);
                }
            }

            for (int i = 0; i < table.TableWidth; i++)
            {
                for (int j = 0; j < table.TableHeight; j++)
                {
                    if (table.GameTable[i, j] != 0)
                    {
                        drawBall(j, i, table.GameTable[i, j]);
                    }
                }
            }

            adWebBrowser.Source = new Uri("/ad.html", UriKind.Relative);
        }

        Image currentBallImg;
        int currentClickedRow;
        int currentClickedCol;
        short currentColor;

        void rect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;

            int row = (int)rect.Resources["row"];
            int col = (int)rect.Resources["col"];

            currentClickedRow = row;
            currentClickedCol = col;

            if (table.GameTable[row, col] != 0)
            {
                rectArray[row, col].Fill = new SolidColorBrush(Colors.Orange);
                rectArray[row, col].Stroke = new SolidColorBrush(Colors.Red);

                if (table.IsBallSelected) 
                {
                    rectArray[(int)table.SelectedPoint.X, (int)table.SelectedPoint.Y].Stroke = new SolidColorBrush(Colors.Green);
                    rectArray[(int)table.SelectedPoint.X, (int)table.SelectedPoint.Y].Fill = new SolidColorBrush(Colors.Gray);
                }

                table.IsBallSelected = true;
                table.SelectedPoint = new Point(row, col);
            }
            else 
            {
                if (table.IsBallSelected) 
                {
                    Route shortestRoute =  table.calculateRoute(table.SelectedPoint, new Point(row, col));

                    Storyboard sb = new Storyboard();

                    DoubleAnimationUsingKeyFrames verticaldaukf = new DoubleAnimationUsingKeyFrames();
                    LinearDoubleKeyFrame verticalldkf = new LinearDoubleKeyFrame();

                    DoubleAnimationUsingKeyFrames horizontaldaukf = new DoubleAnimationUsingKeyFrames();
                    LinearDoubleKeyFrame horizontalldkf = new LinearDoubleKeyFrame();
                    
                    verticalldkf.KeyTime = TimeSpan.FromSeconds(0);
                    verticalldkf.Value = table.SelectedPoint.X * 53;
                    verticaldaukf.KeyFrames.Add(verticalldkf);

                    horizontalldkf.KeyTime = TimeSpan.FromSeconds(0);
                    horizontalldkf.Value = table.SelectedPoint.Y * 53;
                    horizontaldaukf.KeyFrames.Add(horizontalldkf);

                    sb.Duration = TimeSpan.FromMilliseconds(shortestRoute.Connections.Count * 200);
                    verticaldaukf.Duration = TimeSpan.FromMilliseconds(shortestRoute.Connections.Count * 200);
                    horizontaldaukf.Duration = TimeSpan.FromMilliseconds(shortestRoute.Connections.Count * 200);

                    if (shortestRoute.Connections.Count == 0)
                    {
                        MessageBox.Show("Disabled step, othere balls are in the route.");
                    }
                    else
                    {
                        for (int i = 0; i < shortestRoute.Connections.Count; i++)
                        {
                            Connection conn = shortestRoute.Connections[i];
                            Location f = conn.A;
                            Location t = conn.B;

                            verticalldkf = new LinearDoubleKeyFrame();

                            verticalldkf.KeyTime = TimeSpan.FromMilliseconds((i + 1) * 200);
                            verticalldkf.Value = Int32.Parse(conn.B.Identifier.Split(',')[0]) * 53;
                            verticaldaukf.KeyFrames.Add(verticalldkf);

                            horizontalldkf = new LinearDoubleKeyFrame();

                            horizontalldkf.KeyTime = TimeSpan.FromMilliseconds((i + 1) * 200);
                            horizontalldkf.Value = Int32.Parse(conn.B.Identifier.Split(',')[1]) * 53;
                            horizontaldaukf.KeyFrames.Add(horizontalldkf);
                        }

                        currentBallImg = ballImgList.First(i => i.Margin.Top == (double)(table.SelectedPoint.X * 53) && i.Margin.Left == (double)(table.SelectedPoint.Y * 53));
                        currentBallImg.RenderTransform = new TranslateTransform();
                        Storyboard.SetTarget(verticaldaukf, currentBallImg);
                        Storyboard.SetTarget(horizontaldaukf, currentBallImg);
                        Storyboard.SetTargetProperty(verticaldaukf, new PropertyPath(Canvas.TopProperty));
                        Storyboard.SetTargetProperty(horizontaldaukf, new PropertyPath(Canvas.LeftProperty));

                        sb.Children.Add(verticaldaukf);
                        sb.Children.Add(horizontaldaukf);
                        Thickness thickness = new Thickness();
                        currentBallImg.Margin = thickness;

                        gameCanvas.IsHitTestVisible = false;
                        sb.Begin();
                        sb.Completed += new EventHandler(sb_Completed);

                        currentColor = table.GameTable[(int)table.SelectedPoint.X, (int)table.SelectedPoint.Y];
                        table.GameTable[row, col] = currentColor;
                        table.GameTable[(int)table.SelectedPoint.X, (int)table.SelectedPoint.Y] = 0;

                        table.IsBallSelected = false;
                        rectArray[(int)table.SelectedPoint.X, (int)table.SelectedPoint.Y].Stroke = new SolidColorBrush(Colors.Green);
                        rectArray[(int)table.SelectedPoint.X, (int)table.SelectedPoint.Y].Fill = new SolidColorBrush(Colors.Gray);
                        table.SelectedPoint = new Point(row, col);
                    }
                }
            }
        }

        void sb_Completed(object sender, EventArgs e)
        {
            ballImgList.Remove(currentBallImg);
            gameCanvas.Children.Remove(currentBallImg);

            drawBall(currentClickedCol, currentClickedRow, currentColor);

            List<Point> matchingPoints = table.checkMatching();

            if (matchingPoints.Count > 0)
            {
                PageTitle.Text = "Score: " + table.Score;

                foreach (Point item in matchingPoints)
                {
                    Image trashImg = ballImgList.First(i => i.Margin.Top == (double)(item.X * 53) && i.Margin.Left == (double)(item.Y * 53));
                    table.GameTable[(int)item.X, (int)item.Y] = 0;
                    gameCanvas.Children.Remove(trashImg);
                    ballImgList.Remove(trashImg);
                }
            }
            else
            {
                int[,] newballs = table.addNewBalls();

                if (newballs != null)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        drawBall(newballs[i, 1], newballs[i, 0], (short)newballs[i, 2]);
                    }
                }
                else
                {
                    NavigationService.Navigate(new Uri("/GameOver.xaml?score=" + table.Score, UriKind.Relative));
                }
            }

            gameCanvas.IsHitTestVisible = true;
        }

        public void drawBall(int row, int col, short color)
        {
            Uri address = null;

            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            using (var stream = new IsolatedStorageFileStream("trollface.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, store))
            {

                using (var reader = new StreamReader(stream))
                {
                    if (!reader.EndOfStream)
                    {
                        string trollfacever = reader.ReadToEnd();

                        if ("True" == trollfacever)
                        {
                            switch (color)
                            {
                                case 1:
                                    address = new Uri("/Images/Trolls/troll1_53.png", UriKind.Relative);
                                    break;
                                case 2:
                                    address = new Uri("/Images/Trolls/troll2_53.png", UriKind.Relative);
                                    break;
                                case 3:
                                    address = new Uri("/Images/Trolls/troll3_53.png", UriKind.Relative);
                                    break;
                                case 4:
                                    address = new Uri("/Images/Trolls/troll4_53.png", UriKind.Relative);
                                    break;
                                case 5:
                                    address = new Uri("/Images/Trolls/troll5_53.png", UriKind.Relative);
                                    break;
                                case 6:
                                    address = new Uri("/Images/Trolls/troll6_53.png", UriKind.Relative);
                                    break;
                                case 7:
                                    address = new Uri("/Images/Trolls/troll7_53.png", UriKind.Relative);
                                    break;
                            }
                        }
                        else
                        {
                            switch (color)
                            {
                                case 1:
                                    address = new Uri("/Images/Balls/blueball_53.png", UriKind.Relative);
                                    break;
                                case 2:
                                    address = new Uri("/Images/Balls/brownball_53.png", UriKind.Relative);
                                    break;
                                case 3:
                                    address = new Uri("/Images/Balls/greenball_53.png", UriKind.Relative);
                                    break;
                                case 4:
                                    address = new Uri("/Images/Balls/greyball_53.png", UriKind.Relative);
                                    break;
                                case 5:
                                    address = new Uri("/Images/Balls/purpleball_53.png", UriKind.Relative);
                                    break;
                                case 6:
                                    address = new Uri("/Images/Balls/redball_53.png", UriKind.Relative);
                                    break;
                                case 7:
                                    address = new Uri("/Images/Balls/yellowball_53.png", UriKind.Relative);
                                    break;
                            }
                        }
                    }
                    else 
                    {
                        switch (color)
                        {
                            case 1:
                                address = new Uri("/Images/Balls/blueball_53.png", UriKind.Relative);
                                break;
                            case 2:
                                address = new Uri("/Images/Balls/brownball_53.png", UriKind.Relative);
                                break;
                            case 3:
                                address = new Uri("/Images/Balls/greenball_53.png", UriKind.Relative);
                                break;
                            case 4:
                                address = new Uri("/Images/Balls/greyball_53.png", UriKind.Relative);
                                break;
                            case 5:
                                address = new Uri("/Images/Balls/purpleball_53.png", UriKind.Relative);
                                break;
                            case 6:
                                address = new Uri("/Images/Balls/redball_53.png", UriKind.Relative);
                                break;
                            case 7:
                                address = new Uri("/Images/Balls/yellowball_53.png", UriKind.Relative);
                                break;
                        }
                    }
                }
            }


            //StreamResourceInfo resourceinfo = Application.GetResourceStream(address);
            BitmapImage bmp = new BitmapImage();
            bmp.UriSource = address;
            Image img = new Image();
            img.Source = bmp;
            img.IsHitTestVisible = false;
            
            Thickness thickness = new Thickness();
            thickness.Left = (double)(row * 53);
            thickness.Top = (double)(col * 53);
            img.Margin = thickness;

            ballImgList.Add(img);

            gameCanvas.Children.Add(img);
        }
    }
}
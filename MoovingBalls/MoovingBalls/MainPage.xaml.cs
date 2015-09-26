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

namespace MoovingBalls
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();            
        }

        private void MainMenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainMenuListBox.SelectedIndex == 0) 
            {
                NavigationService.Navigate(new Uri("/NewGame.xaml",UriKind.Relative));
            }
            else if (MainMenuListBox.SelectedIndex == 1)
            {
                NavigationService.Navigate(new Uri("/HighScore.xaml", UriKind.Relative));
            }
            else if (MainMenuListBox.SelectedIndex == 3)
            {
                NavigationService.Navigate(new Uri("/FeedbackPage.xaml", UriKind.Relative));
            }
            else 
            {
                NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
            }
            

            MainMenuListBox.SelectedIndex = -1;
        }
    }
}
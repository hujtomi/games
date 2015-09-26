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
using SilverlightPhoneDatabase;

namespace MoovingBalls
{
    public partial class GameOver : PhoneApplicationPage
    {
        string score;
        public GameOver()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            score = NavigationContext.QueryString["score"];
            scoreTextBlock.Text += NavigationContext.QueryString["score"];
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            string databaseName = "score";

            Database db = null;
            if (Database.DoesDatabaseExists(databaseName))
            {
                db = Database.OpenDatabase(databaseName);
            }
            else
            {
                db = Database.CreateDatabase(databaseName);
                db.Save();
                db.CreateTable<Score>();
                db.Save();
            }

            if (db.Table<Score>() != null)
            {
                db.Table<Score>().Add(new Score() { Name = nameTextBox.Text, GameDate = DateTime.Now, ReachedScore = Int32.Parse(score) });
            }
            db.Save();

            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}
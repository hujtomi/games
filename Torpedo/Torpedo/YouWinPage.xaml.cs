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
using Torpedo.Modell;

namespace Torpedo
{
    public partial class YouWinPage : PhoneApplicationPage
    {
        string steps;

        public YouWinPage()
        {
            InitializeComponent();

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            steps = NavigationContext.QueryString["steps"];
            resultTextBlock1.Text += NavigationContext.QueryString["steps"];
        }

        private void saveScoreButton_Click(object sender, RoutedEventArgs e)
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
                db.Table<Score>().Add(new Score() { Name = nameTextBox.Text, GameDate = DateTime.Now, UsedBombs = Int32.Parse(steps) });
            }
            db.Save();

            NavigationService.GoBack();
        }
    }
}
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
    public partial class HighScore : PhoneApplicationPage
    {
        public HighScore()
        {
            InitializeComponent();

            string databaseName = "score";

            Database db = null;
            if (Database.DoesDatabaseExists(databaseName))
            {
                db = Database.OpenDatabase(databaseName);
                HighScoreListBox.ItemsSource = db.Table<Score>().OrderByDescending(s => s.ReachedScore).Take(20);
            }

            
        }
    }
}
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
using System.IO.IsolatedStorage;
using System.IO;

namespace MoovingBalls
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();

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
                            trollFaceCheckBox.IsChecked = true;
                        }
                        else 
                        {
                            trollFaceCheckBox.IsChecked = false;
                        }
                    }
                }
            }
        }

        private void trollFaceCheckBox_Click(object sender, RoutedEventArgs e)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (store.FileExists("trollface.txt"))
                    store.DeleteFile("trollface.txt");

                using (var stream = new IsolatedStorageFileStream("trollface.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, store))
                {                    
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(trollFaceCheckBox.IsChecked);
                    }
                }
            }
        }
    }
}
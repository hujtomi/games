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
    public partial class FeedbackPage : PhoneApplicationPage
    {
        public FeedbackPage()
        {
            InitializeComponent();
        }

        private void sendFeedbackButton_Click(object sender, RoutedEventArgs e)
        {
            if (feedbackTextBox.Text == "")
            {
                MessageBox.Show("The suggestion or preception textbox is empty");
            }
            else
            {
                FeedbackServiceReference.FeedbackWebServiceSoapClient client = new FeedbackServiceReference.FeedbackWebServiceSoapClient();
                client.MakeFeedbackAsync(nameTextBox.Text, emailTextBox.Text, feedbackTextBox.Text, "Mooving balls");

                client.MakeFeedbackCompleted += new EventHandler<FeedbackServiceReference.MakeFeedbackCompletedEventArgs>(client_MakeFeedbackCompleted);
            }
        }

        void client_MakeFeedbackCompleted(object sender, FeedbackServiceReference.MakeFeedbackCompletedEventArgs e)
        {
            if (e.Result == "success")
            {
                MessageBox.Show("Thank you for your feedback!");
                NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show("Feedback service is temporarily unavailable, the progem is: " + e.Result + " \nPlease try again later!");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Markup;

using DBLayer = EventManagerPro.DBLayer;
using EventManagerPro.ViewModels;
using EventManagerPro.Models;

namespace EventManagerPro.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Page
    {
        private LoginViewModel _vm;

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public LoginView()
        {
            this.InitializeComponent();

            // Clear SessionModel
            SessionModel.GetInstance().Clear();

            // Set the ViewModel as the DataContext.
            this._vm = new LoginViewModel();
            this.DataContext = this._vm;
        }
        #endregion

        #region Event Listeners
        private void UpcomingEventsFilter_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.upcomingEventsFilter.SelectedValue != null)
            {
                string filter = this.upcomingEventsFilter.SelectedValue.ToString();
                if (!filter.Equals("View By Month"))
                {
                    DateTime filterMonth = DateTime.Parse(filter);
                    this._vm.GetUpcomingEvents(filterMonth);
                }
                else
                {
                    this._vm.GetUpcomingEvents(DateTime.Now);
                }

                // Animate the transition between listings.
                if (this._vm.UpcomingEvents.Count > 0)
                {
                    this.noUpcomingEventsNotice.Visibility = Visibility.Collapsed;
                    this.upcomingEventsScrollViewer.Visibility = Visibility.Visible;

                    Storyboard sb = (Storyboard)this.FindResource("ContentFadeIn");
                    this.upcomingEventsScrollViewer.BeginStoryboard(sb);
                }
                else
                {
                    this.noUpcomingEventsNotice.Visibility = Visibility.Visible;
                    this.upcomingEventsScrollViewer.Visibility = Visibility.Collapsed;

                    Storyboard sb = (Storyboard)this.FindResource("ContentFadeIn");
                    this.noUpcomingEventsNotice.BeginStoryboard(sb);
                }
            }
        }

        private void PrevButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.upcomingEventsFilter.SelectedIndex != 0)
            {
                this.upcomingEventsFilter.SelectedIndex--;
            }
        }

        private void NextButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.upcomingEventsFilter.Items.Count != this.upcomingEventsFilter.SelectedIndex)
            {
                this.upcomingEventsFilter.SelectedIndex++;
            }
        }

        private void LoginButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this._vm.Login.Password = this.passwordText.Password;

            if (this._vm.ValidateUser())
            {
                SessionModel.GetInstance().StatusCode = SessionModel.STATUS_NOTICE;
                SessionModel.GetInstance().StatusMessage = "You have successfully logged in. Welcome!";

                EventsView eventsScreen = new EventsView();
                eventsScreen.SetupNavigationHandler(this.NavigationService);
                this.NavigationService.Navigate(eventsScreen);
            }
            else
            {
                MessageBox.Show(
                    "Your matriculation number and password were not recognized. Please check and try again.",
                    "Authentication Failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation
                );
            }
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)e.Source;
            EventModel selectedEvent = b.DataContext as EventModel;
            this.eventInfoDialog.Show(selectedEvent);
        }
        #endregion

    }
}

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
using System.Collections.ObjectModel;

using DBLayer = EventManagerPro.DBLayer;
using EventManagerPro.Models;
using EventManagerPro.ViewModels;

namespace EventManagerPro.Views
{
    /// <summary>
    /// Interaction logic for EventsScreen.xaml
    /// </summary>
    public partial class EventsView : Page
    {
        private EventsViewModel _vm;

        public EventsView()
        {
            InitializeComponent();
            this.eventInfoDialog.SetEventsScreen(this);

            // Set the ViewModel as the DataContext.
            this._vm = new EventsViewModel();
            this.DataContext = this._vm;
        }

        #region Class Methods
        public void SetupNavigationHandler(NavigationService ns)
        {
            ns.LoadCompleted += new LoadCompletedEventHandler(NavigationService_LoadCompleted);
        }

        public void EditEventByID(int id)
        {
            // Redirect user to EditEventScreen to edit their event.
            SessionModel.GetInstance().EditEventID = id;

            EventView editEventScreen = new EventView();
            editEventScreen.SetupNavigationHandler(this.NavigationService);
            this.NavigationService.Navigate(editEventScreen);
        }

        public void DeleteEventByID(int id)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this event? THIS CANNOT BE UNDONE!", "Delete Event", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                this._vm.DeleteEvent(id);

                // Display delete notice.
                this.statusMessage.Content = "Your event has been successfully deleted.";
                this.statusMessage.Style = (Style)this.FindResource("StatusMessageErrorStyle");
                Storyboard sb = (Storyboard)this.FindResource("StatusMessageFadeIn");
                this.statusMessage.BeginStoryboard(sb);
            }
        }

        public void Refresh()
        {
            this._vm.UpdateUpcomingEvents();
            this._vm.UpdateRegisteredEvents();
        }
        #endregion

        #region Event Listeners
        private void createEventBtn_Click(object sender, RoutedEventArgs e)
        {
            EventView editEventScreen = new EventView();
            editEventScreen.SetupNavigationHandler(this.NavigationService);
            this.NavigationService.Navigate(editEventScreen);
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to log out now?", "Logout", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                this.NavigationService.Navigate(new Uri("Views/LoginView.xaml", UriKind.Relative));
            } 
        }

        private void NavigationService_LoadCompleted(object sender, NavigationEventArgs e)
        {
            this.NavigationService.LoadCompleted -= NavigationService_LoadCompleted;

            if (!SessionModel.GetInstance().LoggedIn)
            {
                this.NavigationService.Navigate(new Uri("Views/LoginView.xaml", UriKind.Relative));
            }
            else
            {
                SessionModel.GetInstance().Refresh();
                this.loggedInUserLabel.Content = SessionModel.GetInstance().LoggedInUser.Name;
                this.eventInfoDialog.SetLoggedInUser(SessionModel.GetInstance().LoggedInUser);

                this._vm.UpdateUpcomingEvents();
                this._vm.UpdateCreatedEvents();
                this._vm.UpdateRegisteredEvents();
            }

            if (SessionModel.GetInstance().StatusCode != SessionModel.STATUS_NONE)
            {
                if (SessionModel.GetInstance().StatusCode == SessionModel.STATUS_ERROR)
                    this.statusMessage.Style = (Style)this.FindResource("StatusMessageErrorStyle");
                else
                    this.statusMessage.Style = (Style)this.FindResource("StatusMessageNoticeStyle");

                this.statusMessage.Content = SessionModel.GetInstance().StatusMessage;
                Storyboard sb = (Storyboard)this.FindResource("StatusMessageFadeIn");
                this.statusMessage.BeginStoryboard(sb);
            }

            SessionModel.GetInstance().ClearStatus();
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)e.Source;
            this.EditEventByID(Convert.ToInt32(b.Tag));
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)e.Source;
            this.DeleteEventByID(Convert.ToInt32(b.Tag));
        }

        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)e.Source;
            this._vm.RegisterGuest(Convert.ToInt32(b.Tag));
        }

        private void unregisterButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)e.Source;
            this._vm.UnregisterGuest(Convert.ToInt32(b.Tag));
        }

        private void infoButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)e.Source;
            EventModel eventItem = b.DataContext as EventModel;
            this.eventInfoDialog.Show(eventItem);
        }
        #endregion

    }
}

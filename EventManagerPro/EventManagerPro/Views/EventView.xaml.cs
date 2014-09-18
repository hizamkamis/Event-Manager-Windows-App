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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media.Animation;

using DBLayer = EventManagerPro.DBLayer;
using EventManagerPro.Models;
using EventManagerPro.ViewModels;

namespace EventManagerPro.Views
{
    /// <summary>
    /// Interaction logic for EditEventScreen.xaml
    /// </summary>
    public partial class EventView : Page
    {
        private EventViewModel _vm;

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public EventView()
        {
            InitializeComponent();
            this.DataContext = this;

            // Set the ViewModel as the DataContext.
            this._vm = new EventViewModel();
            this.DataContext = this._vm;
        }
        #endregion

        #region Event Listeners
        public void SetupNavigationHandler(NavigationService ns)
        {
            ns.LoadCompleted += new LoadCompletedEventHandler(NavigationService_LoadCompleted);
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
                this.loggedInUserLabel.Content = SessionModel.GetInstance().LoggedInUser.Name;
            }
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to log out now?", "Logout", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                this.NavigationService.Navigate(new Uri("Views/LoginView.xaml", UriKind.Relative));
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you wish to cancel? Any unsaved changes will be lost!", "Cancel", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Redirect back to Event Main screen with notify box showing that it is cancelled.
                EventsView eventsScreen = new EventsView();
                eventsScreen.SetupNavigationHandler(this.NavigationService);
                this.NavigationService.Navigate(eventsScreen);
            }
        }

        private void unregister_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)e.Source;
            this._vm.UnregisterGuest(b.Tag.ToString());
        }

        // Workaround to update the capacity progress bar when the value for capacity has changed.
        private void eventCapacityBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this._vm.UpdateCapacityUI();
        }

        private void budgetFormSave_Click(object sender, RoutedEventArgs e)
        {
            this._vm.SaveBudgetItem();
        }

        private void budgetFormCancel_Click(object sender, RoutedEventArgs e)
        {
            this._vm.ResetBudgetForm();
        }

        private void budgetItemEdit_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)e.Source;
            this._vm.SetActiveBudgetItem(b.DataContext as BudgetItemModel);
        }

        private void budgetItemDelete_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)e.Source;
            this._vm.DeleteBudgetItem(b.DataContext as BudgetItemModel);
        }

        private void addSubEventsButton_Click(object sender, RoutedEventArgs e)
        {
            this._vm.AddNewSubEvent();
        }

        private void clearAllSubEventsButton_Click(object sender, RoutedEventArgs e)
        {
            this._vm.ClearAllSubEvents();
        }

        private void subEventDelete_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)e.Source;
            SubEventModel s = b.DataContext as SubEventModel;
            this._vm.DeleteSubEvent(s);
        }

        private void ExecuteSave(object sender, ExecutedRoutedEventArgs e)
        {
            if (!this._vm.CanSave)
            {
                this.statusMessage.Content = "Please ensure that all event and programme information has been entered correctly.";
                Storyboard sb = (Storyboard)this.FindResource("StatusMessageFadeIn");
                this.statusMessage.BeginStoryboard(sb);
            }
            else
            {
                if (this._vm.Save())
                {
                    // Redirect back to Event Main screen with notify box showing that it has been saved.
                    SessionModel.GetInstance().StatusCode = SessionModel.STATUS_NOTICE;
                    if (this._vm.IsNewEvent)
                    {
                        SessionModel.GetInstance().StatusMessage = "Your event has been successfully created.";
                    }
                    else
                    {
                        SessionModel.GetInstance().StatusMessage = "Your event has been successfully edited.";
                    }

                    EventsView eventsScreen = new EventsView();
                    eventsScreen.SetupNavigationHandler(this.NavigationService);
                    this.NavigationService.Navigate(eventsScreen);
                } 
            }         
        }

        private void CanSave(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (this._vm != null && this._vm.CanSave);
        }

        private void eventBudgetBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this._vm.UpdateBudgetUI();
        }

        private void statusMessage_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this._vm != null && !String.IsNullOrEmpty(this._vm.WarningMessages))
            {
                this.statusMessage.Visibility = Visibility.Visible;
                Storyboard sb = (Storyboard)this.FindResource("StatusMessageFadeIn");
                this.statusMessage.BeginStoryboard(sb);
            }
            else
            {
                this.statusMessage.Visibility = Visibility.Collapsed;
                this.statusMessage.Opacity = 0;
            }
        }
        #endregion
    }
}

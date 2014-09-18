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
using System.ComponentModel;
using DBLayer = EventManagerPro.DBLayer;
using System.Windows.Media.Animation;
using EventManagerPro.Models;
using EventManagerPro.Views;

namespace EventManagerPro.Controls
{
    /// <summary>
    /// Interaction logic for EventInfoDialog.xaml
    /// </summary>
    public partial class EventInfoDialog : UserControl, INotifyPropertyChanged
    {
        private EventModel _curEventModel;
        private List<DBLayer.SubEvent> _subEventsList;
        private DBLayer.Student _loggedInUser;
        private EventsView _eventsView;

        public event PropertyChangedEventHandler PropertyChanged;

        public EventModel Event
        {
            get { return this._curEventModel; }
            set
            {
                this._curEventModel = value;

                this.NotifyPropertyChanged("EventModel");
                this.DataContext = this.Event;

                this.SubEventsList = DBLayer.DomainModels.SubEventModel.getAllByEventID(this._curEventModel.Id);
                this.subEventsListGrid.ItemsSource = this.SubEventsList;
            }
        }

        public List<DBLayer.SubEvent> SubEventsList
        {
            get { return this._subEventsList; }
            set { this._subEventsList = value; }
        }

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public EventInfoDialog()
        {
            InitializeComponent();
            this.Visibility = Visibility.Hidden;
            this.DataContext = Event;
        }

        public void SetEventsScreen(EventsView p)
        {
            this._eventsView = p;
        }

        public void SetLoggedInUser(DBLayer.Student s)
        {
            this._loggedInUser = s;
            this.actionsPanel.Visibility = Visibility.Visible;
        }

        public void Show(EventModel e)
        {
            this.Event = e;
            this.Visibility = Visibility.Visible;

            Storyboard sb = (Storyboard)this.FindResource("ContentFadeIn");
            this.BeginStoryboard(sb);
        }

        public void Hide()
        {
            Storyboard sb = (Storyboard)this.FindResource("ContentFadeOut");
            sb.Completed += new EventHandler(sb_Completed);
            this.BeginStoryboard(sb);
        }

        private void sb_Completed(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            if(this._eventsView != null) this._eventsView.Refresh();
        }

        private void eventEditButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            Button b = (Button)e.Source;
            EventsView p = this._eventsView as EventsView;
            this._eventsView.EditEventByID(Convert.ToInt32(b.Tag));
        }

        private void eventDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            Button b = (Button)e.Source;
            this._eventsView.DeleteEventByID(Convert.ToInt32(b.Tag));
        }

        private void eventRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)e.Source;

            if (DBLayer.DomainModels.EventModel.registerGuest(this._loggedInUser.MatricId, Convert.ToInt32(b.Tag)))
            {
                eventRegisterButton.Visibility = Visibility.Collapsed;
                eventUnregisterButton.Visibility = Visibility.Visible;
            }
        }

        private void eventUnregisterButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)e.Source;

            if (DBLayer.DomainModels.EventModel.unregisterGuest(this._loggedInUser.MatricId, Convert.ToInt32(b.Tag)))
            {
                eventRegisterButton.Visibility = Visibility.Visible;
                eventUnregisterButton.Visibility = Visibility.Collapsed;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using EventManagerPro.Models;
using DBLayer = EventManagerPro.DBLayer;

namespace EventManagerPro.ViewModels
{
    /// <summary>
    /// ViewModel class to be used for EventsView, which displays a list of events to the user who is currently logged in.
    /// </summary>
    public class EventsViewModel : BaseViewModel
    {
        private ObservableCollection<EventModel> _upcomingEventsList;
        private ObservableCollection<EventModel> _createdEventsList;
        private ObservableCollection<EventModel> _registeredEventsList;

        #region Class Properties
        /// <summary>
        /// ObservableCollection of the upcoming events, to be binded by a View.
        /// </summary>
        public ObservableCollection<EventModel> UpcomingEventsList
        {
            get { return this._upcomingEventsList; }
        }

        /// <summary>
        /// ObservableCollection of events created by user currently logged in, to be binded by a View.
        /// </summary>
        public ObservableCollection<EventModel> CreatedEventsList
        {
            get { return this._createdEventsList; }
        }

        /// <summary>
        /// ObservableCollection of the events registered by user currently logged in, to be binded by a View.
        /// </summary>
        public ObservableCollection<EventModel> RegisteredEventsList
        {
            get { return this._registeredEventsList; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for EventsViewModel class.
        /// </summary>
        public EventsViewModel()
        {
            this._upcomingEventsList = new ObservableCollection<EventModel>();
            this._createdEventsList = new ObservableCollection<EventModel>();
            this._registeredEventsList = new ObservableCollection<EventModel>();
        }
        #endregion

        #region Class Methods
        /// <summary>
        /// Get upcoming events from the database and update the List for upcoming events.
        /// </summary>
        public void UpdateUpcomingEvents()
        {
            this._upcomingEventsList.Clear();
            List<DBLayer.Event> upcomingEvents = DBLayer.DomainModels.EventModel.getAll();
            foreach (DBLayer.Event e in upcomingEvents)
                this._upcomingEventsList.Add(new EventModel(e));

            this.NotifyPropertyChanged("UpcomingEventsList");
        }
   
        /// <summary>
        /// Get events created by the logged in user from the database and update the List for created events.
        /// </summary>
        public void UpdateCreatedEvents()
        {
            // Update created events list.
            this._createdEventsList.Clear();

            // Refresh the session first!
            SessionModel.GetInstance().Refresh();

            ICollection<DBLayer.Event> ownedEvents = SessionModel.GetInstance().LoggedInUser.OwnedEvents;
            foreach (DBLayer.Event e in ownedEvents)
                this._createdEventsList.Add(new EventModel(e));

            this.NotifyPropertyChanged("CreatedEventsList");
        }

        /// <summary>
        /// Get events registered by the logged in user from the database and update the List for registered events.
        /// </summary>
        public void UpdateRegisteredEvents()
        {
            // Update registered events list.
            this._registeredEventsList.Clear();

            // Refresh the session first!
            SessionModel.GetInstance().Refresh();

            ICollection<DBLayer.Event> ownedEvents = SessionModel.GetInstance().LoggedInUser.RegisteredEvents;
            foreach (DBLayer.Event e in ownedEvents)
                this._registeredEventsList.Add(new EventModel(e));

            this.NotifyPropertyChanged("RegisteredEventsList");
        }

        /// <summary>
        /// Registers the current logged in user for an event.
        /// </summary>
        /// <param name="eventId">Event ID</param>
        /// <returns>True if successful, false if unsuccessful.</returns>
        public bool RegisterGuest(int eventId)
        {
            if (DBLayer.DomainModels.EventModel.registerGuest(SessionModel.GetInstance().LoggedInUser.MatricId, eventId))
            {
                this.UpdateRegisteredEvents();
                this.UpdateUpcomingEvents();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Unregisters the current logged in user for the event.
        /// </summary>
        /// <param name="eventId">Event ID</param>
        /// <returns>True if successful, false if unsuccessful.</returns>
        public bool UnregisterGuest(int eventId)
        {
            if (DBLayer.DomainModels.EventModel.unregisterGuest(SessionModel.GetInstance().LoggedInUser.MatricId, eventId))
            {
                this.UpdateRegisteredEvents();
                this.UpdateUpcomingEvents();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Deletes an event based on ID from the database.
        /// </summary>
        /// <param name="id">Event ID</param>
        public void DeleteEvent(int id)
        {
            DBLayer.DomainModels.EventModel.deleteById(id);
            this.UpdateCreatedEvents();
            this.UpdateUpcomingEvents();
        }
        #endregion
    }
}

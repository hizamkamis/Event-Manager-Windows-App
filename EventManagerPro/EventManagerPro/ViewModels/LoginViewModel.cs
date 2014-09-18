using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventManagerPro.Models;
using DBLayer = EventManagerPro.DBLayer;

namespace EventManagerPro.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private List<EventModel> _eventsList;
        private List<EventModel> _upcomingEventsList;

        /// <summary>
        /// For validating the login form.
        /// </summary>
        public LoginModel Login { get; set; }

        /// <summary>
        /// List of available months to search upcoming events from the ComboBox.
        /// </summary>
        public Dictionary<string, DateTime> MonthFilterOptions
        {
            get
            {
                var dateCounter = DateTime.Now;
                Dictionary<string, DateTime> opts = new Dictionary<string, DateTime>();

                // Add default option.
                opts.Add(dateCounter.ToString("y"), dateCounter);

                for (int i = 0; i < 12; i++)
                {
                    dateCounter = dateCounter.AddMonths(1);
                    opts.Add(dateCounter.ToString("y"), dateCounter);
                }

                return opts;
            }
        }

        /// <summary>
        /// List of upcoming events.
        /// </summary>
        public List<EventModel> UpcomingEvents
        {
            get { return this._upcomingEventsList; }
            set
            {
                this._upcomingEventsList = value;
                this.NotifyPropertyChanged("UpcomingEvents");
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public LoginViewModel()
        {
            // Get full event listing and move them into EventItems.
            this._eventsList = new List<EventModel>();
            this._upcomingEventsList = new List<EventModel>();

            List<DBLayer.Event> eventsData = DBLayer.DomainModels.EventModel.getAll();
            foreach (DBLayer.Event e in eventsData)
                this._eventsList.Add(new EventModel(e));

            this.GetUpcomingEvents(DateTime.Now);

            this.Login = new LoginModel();
        }

        /// <summary>
        /// Filters upcoming events by year and month.
        /// </summary>
        /// <param name="dateFilter"></param>
        public void GetUpcomingEvents(DateTime dateFilter)
        {
            List<EventModel> newList = new List<EventModel>();

            foreach (EventModel e in this._eventsList)
            {
                if (e.Start.Year == dateFilter.Year && e.Start.Month == dateFilter.Month)
                    newList.Add(e);
            }

            this.UpcomingEvents = newList;
        }

        /// <summary>
        /// Validates login information. To be used when user submits the login form in the View.
        /// </summary>
        /// <returns>True if user and password are valid, false if invalid.</returns>
        public bool ValidateUser()
        {
            if (DBLayer.DomainModels.StudentModel.authenticate(this.Login.Username, this.Login.Password))
            {
                DBLayer.Student s = DBLayer.DomainModels.StudentModel.getByMatricId(this.Login.Username);
                SessionModel.GetInstance().Login(s);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

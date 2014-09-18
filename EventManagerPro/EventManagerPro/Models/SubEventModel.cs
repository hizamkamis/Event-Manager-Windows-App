using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using DBLayer = EventManagerPro.DBLayer;

namespace EventManagerPro.Models
{
    /// <summary>
    /// Model class for SubEvents.
    /// </summary>
    public class SubEventModel : BaseModel, IDataErrorInfo
    {
        private DBLayer.SubEvent _dbObj;

        #region String Constants
        private const string NAME_ERROR = "Programme name must be at least 3 characters long.";
        private const string VENUE_ERROR_SLASH = "Venue is currently booked for another event. Note that you can have two programmes under the same event, at the same venue.";
        private const string VENUE_ERROR_CAP = "Venue must be able to hold your event's capacity.";
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SubEventModel"/> class.
        /// </summary>
        public SubEventModel()
        {
            this._dbObj = new DBLayer.SubEvent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubEventModel"/> class.
        /// </summary>
        /// <param name="s">SubEvent database entity object.</param>
        public SubEventModel(DBLayer.SubEvent s)
        {
            this._dbObj = s;
        }
        #endregion

        #region Class Properties
        public DBLayer.SubEvent DBObj
        {
            get { return this._dbObj; }
            set { this._dbObj = value; }
        }

        public int Id
        {
            get { return this.DBObj.Id; }
            set
            {
                this.DBObj.Id = value;
                this.NotifyPropertyChanged("Id");
            }
        }

        public String Name
        {
            get { return this.DBObj.Name; }
            set
            {
                if (this.NameIsValid(value) && this.DBObj.Name != value)
                {
                    this.DBObj.Name = value;
                    this.NotifyPropertyChanged("Name");
                }
            }
        }

        public DateTime Start
        {
            get { return this.DBObj.Start; }
            set
            {
                this.DBObj.Start = value;
                this.NotifyPropertyChanged("Start");
            }
        }

        public DateTime End
        {
            get { return this.DBObj.End; }
            set
            {
                this.DBObj.End = value;
                this.NotifyPropertyChanged("End");
            }
        }

        public int EventId
        {
            get { return this.DBObj.EventId; }
            set
            {
                this.DBObj.EventId = value;
                this.NotifyPropertyChanged("EventId");
            }
        }

        public int VenueId
        {
            get { return this.DBObj.VenueId; }
            set
            {
                if (this.VenueIsValid(value) && this.DBObj.VenueId != value)
                {
                    this.DBObj.VenueId = value;
                    this.NotifyPropertyChanged("VenueId");
                }
            }
        }

        public DBLayer.Event Event 
        {
            get
            {
                return this.DBObj.Event;
            }
            set
            {
                this.DBObj.Event = value;
                this.NotifyPropertyChanged("Event");
                this.EventId = DBObj.Event.Id;
            }
        }

        // Additional Class Properties
        public DateTime Date
        {
            get { return this.DBObj.Start; }
            set
            {
                int startHours = this.DBObj.Start.Hour;
                int endHours = this.DBObj.End.Hour;

                this.DBObj.Start = value.AddHours(startHours);
                this.DBObj.End = value.AddHours(endHours);

                this.NotifyPropertyChanged("AllowedStartHours");
                this.NotifyPropertyChanged("AllowedEndHours");
                this.NotifyPropertyChanged("StartHour");
                this.NotifyPropertyChanged("EndHour");

                // Force update VenueId
                this.VenueIsValid(this.VenueId);
                this.NotifyPropertyChanged("VenueId");
            }
        }

        public DateTime AllowedDate
        {
            get
            {
                if (this.DBObj.Id > 0)
                {
                    DBLayer.Event e = DBLayer.DomainModels.EventModel.getByID(this.DBObj.EventId);
                    return new DateTime(e.TimeCreated.Year, e.TimeCreated.Month, e.TimeCreated.Day).AddDays(3);
                }
                else
                    return DateTime.Now.AddDays(3);
            }
        }

        public List<DateTime> AllowedStartHours
        {
            get
            {
                List<DateTime> list = new List<DateTime>();

                for (int i = 8; i < 23; i++)
                    list.Add(new DateTime(this.StartHour.Year, this.StartHour.Month, this.StartHour.Day, i, 0, 0));

                return list;
            }
        }

        public List<DateTime> AllowedEndHours
        {
            get
            {
                List<DateTime> list = new List<DateTime>();

                for (int i = this.StartHour.Hour + 1; i < 24; i++)
                    list.Add(new DateTime(this.StartHour.Year, this.StartHour.Month, this.StartHour.Day, i, 0, 0));

                return list;
            }
        }

        public List<DBLayer.Venue> AllowedVenues
        {
            get
            {
                // @TODO: Only get venues with a certain capacity?
                List<DBLayer.Venue> list = DBLayer.DomainModels.VenueModel.getAll();
                return list;
            }
        }

        public DateTime StartHour
        {
            get { return this.DBObj.Start; }
            set
            {
                this._dbObj.Start = value;

                this.NotifyPropertyChanged("StartHour");
                this.NotifyPropertyChanged("AllowedEndHours");
                
                // Force update VenueId
                this.VenueIsValid(this.VenueId);
                this.NotifyPropertyChanged("VenueId");
            }
        }

        public DateTime EndHour
        {
            get { return this.DBObj.End; }
            set
            {
                this._dbObj.End = value;
                this.NotifyPropertyChanged("EndHour");

                // Force update VenueId
                this.VenueIsValid(this.VenueId);
                this.NotifyPropertyChanged("VenueId");
            }
        }
        #endregion

        #region Validation Methods
        /// <summary>
        /// Checks if the input for Name property is valid.
        /// </summary>
        /// <param name="value">Name to be updated.</param>
        /// <returns>True if valid, false if invalid.</returns>
        public bool NameIsValid(string value)
        {
            if (String.IsNullOrEmpty(value) || value.Length < 3)
            {
                base.AddError("Name", NAME_ERROR, false);
                return false;
            }
            else
            {
                base.RemoveError("Name", NAME_ERROR);
                return true;
            }
        }

        /// <summary>
        /// Checks if the input for VenueId property is valid.
        /// </summary>
        /// <param name="value">VenueId to be updated.</param>
        /// <returns>True if valid, false if invalid.</returns>
        public bool VenueIsValid(int value)
        {
            // Event not initalised yet, temp. set it to true.
            if (this.DBObj.Event == null)
                return true;

            Console.WriteLine("Clash: " + value + " " + this.Start + " " + this.End);
            List<DBLayer.SubEvent> list = DBLayer.DomainModels.SubEventModel.getAllByVenueIDAndTime(value, this.Start, this.End, this.DBObj.Event.Id);

            bool noClashResult = false;

            if (list != null && list.Count > 0)
            {
                base.AddError("VenueId", VENUE_ERROR_SLASH, false);
                noClashResult = false;
            }
            else
            {
                base.RemoveError("VenueId", VENUE_ERROR_SLASH);
                noClashResult = true;
            }

            DBLayer.Venue v = DBLayer.DomainModels.VenueModel.getByID(value);
            bool capResult = v.Capacity >= this.DBObj.Event.Capacity;

            if (!capResult)
            {
                base.AddError("VenueId", VENUE_ERROR_CAP, false);
            }
            else
            {
                base.RemoveError("VenueId", VENUE_ERROR_CAP);
            }

            Console.WriteLine("Clash: " + noClashResult + " Cap: " + capResult);
            return noClashResult && capResult;
        }
        #endregion

        # region IDataErrorInfo Members
        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string propertyName]
        {
            get
            {
                return (!_errors.ContainsKey(propertyName) ? null :
                    String.Join(Environment.NewLine, _errors[propertyName]));
            }
        }
        #endregion

    }
}

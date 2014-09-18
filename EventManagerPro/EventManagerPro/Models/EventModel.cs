using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using DBLayer = EventManagerPro.DBLayer;

namespace EventManagerPro.Models
{
    /// <summary>
    /// Mode class for Events.
    /// </summary>
    public class EventModel : BaseModel, IDataErrorInfo
    {
        private DBLayer.Event _dbObj;

        private BudgetModel _budget;

        #region String Constants
        private const string NAME_ERROR = "Event name must be at least 3 characters long.";
        private const string DESCRIPTION_ERROR = "Event description must not be empty.";
        private const string CAPACITY_ERROR = "Capacity must a positive number.";
        #endregion

        #region Constructors
        public EventModel()
        {
            this._dbObj = new DBLayer.Event();

            this.Id = -1;
            this.Name = "";
            this.Description = "";
            this.Capacity = 50;

            this.DBObj.Start = DateTime.Now;
            this.DBObj.End = DateTime.Now;
            this.DBObj.TimeCreated = DateTime.Now;

            if (SessionModel.GetInstance().LoggedIn)
            {
                this.StudentMatricID = SessionModel.GetInstance().LoggedInUser.MatricId;
            }

            this.DBObj.Budget = new DBLayer.Budget();
            this._budget = new BudgetModel(this.DBObj.Budget);
        }

        public EventModel(DBLayer.Event e)
        {
            this._dbObj = e;
        }
        #endregion

        #region Class Properties
        public DBLayer.Event DBObj
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

        public String Description
        {
            get { return this.DBObj.Description; }
            set
            {
                if (this.DescriptionIsValid(value) && this.DBObj.Description != value)
                {
                    this.DBObj.Description = value;
                    this.NotifyPropertyChanged("Description");
                }
            }
        }

        public int Capacity
        {
            get { return this.DBObj.Capacity; }
            set
            {
                if (this.CapacityIsValid(value) && this.DBObj.Capacity != value)
                {
                    this.DBObj.Capacity = value;
                    this.NotifyPropertyChanged("Capacity");
                }
            }
        }

        public string StudentMatricID
        {
            get { return this.DBObj.StudentMatricId; }
            set
            {
                this.DBObj.StudentMatricId = value;
                this.NotifyPropertyChanged("StudentMatricID");
            }
        }

        public BudgetModel Budget
        {
            get
            {
                if (this.Id > 0)
                    this._budget = new BudgetModel(this.DBObj.Budget);

                return this._budget;
            }
            set
            {
                this._budget = value;
                this.NotifyPropertyChanged("Budget");
            }
        }

        public ICollection<DBLayer.Student> Guests
        {
            get { return this._dbObj.Guests; }
            set
            {
                this._dbObj.Guests = value;
                this.NotifyPropertyChanged("Guests");
            }
        }

        public bool ViewAtLoginPage
        {
            get { return this.DBObj.ViewAtLoginPage == 1; }
            set
            {
                this.DBObj.ViewAtLoginPage = (short)(value ? 1 : 0);
            }
        }
        #endregion

        #region Validation Methods
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

        public bool DescriptionIsValid(string value)
        {
            if (String.IsNullOrEmpty(value) || value.Length < 3)
            {
                base.AddError("Description", DESCRIPTION_ERROR, false);
                return false;
            }
            else
            {
                base.RemoveError("Description", DESCRIPTION_ERROR);
                return true;
            }
        }

        public bool CapacityIsValid(int value)
        {
            if (value <= 0)
            {
                base.AddError("Capacity", CAPACITY_ERROR, false);
                return false;
            }
            else
            {
                base.RemoveError("Capacity", CAPACITY_ERROR);
                return true;
            }
        }
        #endregion

        #region Additional Class Properties
        public DateTime Start
        {
            get
            {
                DateTime start = DateTime.Now;
                // Get SubEvents for CurEvent.
                List<DBLayer.SubEvent> subEvents = DBLayer.DomainModels.SubEventModel.getAllByEventID(this.DBObj.Id);

                if (subEvents.Count > 0)
                {
                    start = subEvents[0].Start;

                    // Determine the earliest SubEvent.
                    foreach (DBLayer.SubEvent s in subEvents)
                    {
                        if (DateTime.Compare(start, s.Start) > 0)
                            start = s.Start;
                    }
                }

                return start;
            }
        }

        public DateTime End
        {
            get
            {
                DateTime end = DateTime.Now;
                // Get SubEvents for CurEvent.
                List<DBLayer.SubEvent> subEvents = DBLayer.DomainModels.SubEventModel.getAllByEventID(this.DBObj.Id);

                if (subEvents.Count > 0)
                {
                    end = subEvents[0].End;

                    // Determine the earliest SubEvent.
                    foreach (DBLayer.SubEvent s in subEvents)
                    {
                        if (DateTime.Compare(end, s.End) < 0)
                            end = s.End;
                    }
                }

                return end;
            }
        }

        public bool IsOwner
        {
            get
            {
                if (!SessionModel.GetInstance().LoggedIn) return false;
                else return SessionModel.GetInstance().LoggedInUser.MatricId == this.StudentMatricID;
            }
        }

        public bool IsRegistered
        {
            get
            {
                if (!SessionModel.GetInstance().LoggedIn) return false;

                bool IsRegistered = false;
                ICollection<DBLayer.Event> registeredEvents = SessionModel.GetInstance().LoggedInUser.RegisteredEvents;
                foreach (DBLayer.Event e in registeredEvents)
                    if (e.Id == this.Id) IsRegistered = true;

                return IsRegistered;
            }
        }

        public bool CanRegister
        {
            get { return !this.IsOwner && !this.IsRegistered && !this.IsRegistrationFull; }
        }

        public bool IsRegistrationFull
        {
            get { return this.Guests.Count >= this.Capacity; }
        }

        public bool IsGuestRegistrationFull
        {
            get { return !this.IsOwner && !this.IsRegistered && this.Guests.Count >= this.Capacity; }
        }

        public bool IsBudgetOverflow
        {
            get
            {
                List<DBLayer.BudgetItem> bItems = DBLayer.DomainModels.BudgetItemModel.getByBudgetId(this.Budget.Id);
                int budgetTotal = 0;

                foreach (DBLayer.BudgetItem b in bItems)
                    budgetTotal += b.Cost;

                return budgetTotal > this.Budget.AllocatedBudget;
            }
        }

        public bool HasSchedule
        {
            get
            {
                // Get SubEvents for CurEvent.
                // List<DBLayer.SubEvent> subEvents = DBLayer.DomainModels.SubEventModel.getAllByEventID(this.DBObj.Id);
                return this.DBObj.SubEvents.Count > 0;
            }
        }
        #endregion

        #region IDataErrorInfo Members
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

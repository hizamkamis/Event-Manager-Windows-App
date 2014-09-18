using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using DBLayer = EventManagerPro.DBLayer;
using EventManagerPro.Models;

namespace EventManagerPro.ViewModels
{
    /// <summary>
    /// EventViewModel class to be used in EventView as a DataContext.
    /// </summary>
    public class EventViewModel : BaseViewModel
    {
        private EventModel _event;
        private List<DBLayer.Venue> _venues;

        private ObservableCollection<BudgetItemModel> _budgetCollection;
        private ObservableCollection<BudgetItemModel> _budgetItemToDeleteList;
        private ObservableCollection<SubEventModel> _subEventCollection;
        private ObservableCollection<SubEventModel> _subEventsToDeleteList;

        private BudgetItemModel _curBudgetItem;

        private bool _isNewEvent = true;
        private bool _isBudgetItemEditingMode = false;

        private string _formTitle = TITLE_NEW;
        private string _budgetItemFormHeader = BUDGET_ITEM_NEW;
        private string _budgetItemSaveContent = BUDGET_ITEM_SAVE_NEW;

        private Dictionary<string, string> _warningMessages;

        #region String Constants
        private const string TITLE_EDIT = "Edit Event";
        private const string TITLE_NEW = "New Event";

        private const string BUDGET_ITEM_NEW = "Add New Budget Item";
        private const string BUDGET_ITEM_EDIT = "Edit Budget Item";

        private const string BUDGET_ITEM_SAVE_EDIT = "Save Item";
        private const string BUDGET_ITEM_SAVE_NEW = "Add Item";
        #endregion

        #region Class Properties
        /// <summary>
        /// List object consisting all Venues from the database.
        /// </summary>
        public List<DBLayer.Venue> Venues
        {
            get { return this._venues; }
            set
            {
                this._venues = value;
                this.NotifyPropertyChanged("Venues");
            }
        }

        /// <summary>
        /// Event Model layer to be binded onto the View via the current ViewModel.
        /// </summary>
        public EventModel Event
        {
            get { return this._event; }
            set
            {
                this._event = value;
                this.NotifyPropertyChanged("Event");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the current View is for creating a new Event.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if current View is for creating a new Event; otherwise, <c>false</c>.
        /// </value>
        public bool IsNewEvent
        {
            get { return this._isNewEvent; }
            set
            {
                this._isNewEvent = value;

                if (value)
                {
                    this.FormTitle = TITLE_NEW;
                }
                else
                {
                    this.FormTitle = TITLE_EDIT;
                }
            }
        }

        /// <summary>
        /// Inverse of IsNewEvent property. Indicates whether the current View is for editing an Event.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if current View is for editing an Event; otherwise, <c>false</c>.
        /// </value>
        public bool IsEditEvent
        {
            get { return !this._isNewEvent; }
        }

        /// <summary>
        /// Gets a value indicating whether the current Event being edited is not close to its starting date.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the current Event being edited is not close to its starting date; otherwise, <c>false</c>.
        /// </value>
        public bool IsNotCloseToEventDate
        {
            get
            {
                return this.IsNewEvent || (this.IsEditEvent && (DateTime.Compare(DateTime.Now.AddDays(2), this.Event.Start) < 0));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this View is editing an existing BudgetItem.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this View is editing an existing BudgetItem; otherwise, <c>false</c>.
        /// </value>
        public bool IsBudgetItemEditingMode
        {
            get { return this._isBudgetItemEditingMode; }
            set
            {
                if (value)
                {
                    this.BudgetItemFormHeader = BUDGET_ITEM_EDIT;
                    this.BudgetItemSaveContent = BUDGET_ITEM_SAVE_EDIT;
                }
                else
                {
                    this.BudgetItemFormHeader = BUDGET_ITEM_NEW;
                    this.BudgetItemSaveContent = BUDGET_ITEM_SAVE_NEW;
                }
                this._isBudgetItemEditingMode = value;
            }
        }

        /// <summary>
        /// Warning messages to be binded on the notice TextBox from the View.
        /// </summary>
        public string WarningMessages
        {
            get
            {
                if (this._warningMessages.Count <= 0)
                {
                    return String.Empty;
                }
                else
                {
                    return String.Join("\n", this._warningMessages.Values.Select(i => i.ToString()).ToArray());
                }
            }
        }

        /// <summary>
        /// Gets or sets the form title of the View.
        /// </summary>
        /// <value>
        /// The form title of the View (eg. Add New Event / Edit Event).
        /// </value>
        public string FormTitle
        {
            get { return this._formTitle; }
            set
            {
                this._formTitle = value;
                this.NotifyPropertyChanged("FormTitle");
            }
        }

        /// <summary>
        /// Gets or sets the BudgetItem form header.
        /// </summary>
        /// <value>
        /// The budget item form header (eg. Add New Item / Edit Item).
        /// </value>
        public string BudgetItemFormHeader
        {
            get { return this._budgetItemFormHeader; }
            set
            {
                this._budgetItemFormHeader = value;
                this.NotifyPropertyChanged("BudgetItemFormHeader");
            }
        }

        /// <summary>
        /// Gets or sets the content of the BudgetItem save button.
        /// </summary>
        /// <value>
        /// The content of the budget item save button (eg. Add Item / Save Item).
        /// </value>
        public string BudgetItemSaveContent
        {
            get { return this._budgetItemSaveContent; }
            set
            {
                this._budgetItemSaveContent = value;
                this.NotifyPropertyChanged("BudgetItemSaveContent");
            }
        }

        /// <summary>
        /// Gets or sets the current BudgetItem to be edited in the form.
        /// </summary>
        /// <value>
        /// The BudgetItem to be edited.
        /// </value>
        public BudgetItemModel CurBudgetItem
        {
            get { return this._curBudgetItem; }
            set
            {
                this._curBudgetItem = value;
                this.NotifyPropertyChanged("CurBudgetItem");
            }
        }

        public int Budget
        {
            get { return this.Event.Budget.AllocatedBudget; }
            set
            {
                this.Event.Budget.AllocatedBudget = value;
                this.UpdateBudgetUI();
            }
        }

        /// <summary>
        /// Gets the total cost of the Budget.
        /// </summary>
        public double BudgetTotal
        {
            get
            {
                double result = 0;

                if (this._budgetCollection.Count > 0)
                {
                    foreach (BudgetItemModel b in this._budgetCollection)
                        result += b.Cost;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the percentage of the total cost against the allocated budget.
        /// </summary>
        public double BudgetPercentage
        {
            get
            {
                if (this.Event.Budget.AllocatedBudget <= 0)
                    return 0;
                else
                    return ((double)this.BudgetTotal) / this.Event.Budget.AllocatedBudget * 100;
            }
        }

        /// <summary>
        /// Gets the current capacity of the guest list for the Event.
        /// </summary>
        public int CurrentCapacity
        {
            get { return this.Event.Guests.Count; }
        }

        /// <summary>
        /// Gets the percentage of the current capacity of the guest list against the specified maximum capacity.
        /// </summary>
        public double CapacityPercentage
        {
            get
            {
                if (this.Event.Capacity <= 0) 
                    return 0;
                else
                    return ((double)this.Event.Guests.Count) / this.Event.Capacity * 100;
            }
        }

        /// <summary>
        /// List of SubEventModels to be binded onto the View.
        /// </summary>
        public ObservableCollection<SubEventModel> SubEventCollection
        {
            get { return this._subEventCollection; }
        }

        /// <summary>
        /// List of BudgetItemModels to be binded onto the View.
        /// </summary>
        public ObservableCollection<BudgetItemModel> BudgetCollection
        {
            get { return this._budgetCollection; }
        }

        /// <summary>
        /// Gets a value indicating whether the form can be saved, passing all form validation.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the form has passed all form validation and can be saved; otherwise, <c>false</c>.
        /// </value>
        public bool CanSave
        {
            get
            {
                bool result = (this.Event != null && this.Event.HasNoErrors);

                foreach (BudgetItemModel b in this._budgetCollection)
                    if (!b.HasNoErrors) result = false;

                foreach (SubEventModel s in this._subEventCollection)
                    if (!s.HasNoErrors) result = false;

                return result;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public EventViewModel()
        {
            this.Venues = DBLayer.DomainModels.VenueModel.getAll();
            this.Event = new EventModel();

            this._subEventCollection = new ObservableCollection<SubEventModel>();
            this._budgetCollection = new ObservableCollection<BudgetItemModel>();

            this._subEventsToDeleteList = new ObservableCollection<SubEventModel>();
            this._budgetItemToDeleteList = new ObservableCollection<BudgetItemModel>();

            this._curBudgetItem = new BudgetItemModel();

            this._warningMessages = new Dictionary<string, string>();

            if (SessionModel.GetInstance().EditEventID > 0)
            {
                // Load Event from DB Model
                this.Event = new EventModel(DBLayer.DomainModels.EventModel.getByID(SessionModel.GetInstance().EditEventID));

                List<DBLayer.BudgetItem> bList = DBLayer.DomainModels.BudgetItemModel.getByBudgetId(this.Event.Budget.Id);
                foreach (DBLayer.BudgetItem b in bList)
                    this._budgetCollection.Add(new BudgetItemModel(b));

                List<DBLayer.SubEvent> sList = DBLayer.DomainModels.SubEventModel.getAllByEventID(this.Event.Id);
                foreach (DBLayer.SubEvent s in sList)
                    this._subEventCollection.Add(new SubEventModel(s));

                this.IsNewEvent = false;

                // Set default warning messages.
                if (this.Event.IsBudgetOverflow)
                    this.AddWarningMessage("BudgetOverflow", "Oops! Looks like you have busted your budget! Remove some items or increase your budget.");
                if (!this.IsNotCloseToEventDate)
                    this.AddWarningMessage("CloseToEventDate", "You cannot change certain information (capacity, programme) when your event's start date is less than 3 days from now.");
                if (this.Event.IsRegistrationFull)
                    this.AddWarningMessage("RegistrationFull", "Guest registration is full. Unless you increase the event's capacity, no new registrations are allowed.");
            }
            else
            {
                // Add new SubEvent since there must be at least one SubEvent.
                this.AddNewSubEvent();
            }
        }
        #endregion

        #region Class Methods
        /// <summary>
        /// Adds a warning message to be displayed onto the View's notice TextBox.
        /// </summary>
        /// <param name="key">Unique key.</param>
        /// <param name="message">Message to be displayed.</param>
        public void AddWarningMessage(string key, string message)
        {
            this._warningMessages.Add(key, message);
            this.NotifyPropertyChanged("WarningMessages");
        }

        /// <summary>
        /// Removes the warning message to be displayed onto the View's notice TextBox.
        /// </summary>
        /// <param name="key">Unique key.</param>
        public void RemoveWarningMessage(string key)
        {
            this._warningMessages.Remove(key);
            this.NotifyPropertyChanged("WarningMessages");
        }

        /// <summary>
        /// Saves the current Event information to the database.
        /// </summary>
        /// <returns><c>true</c> if successful; <c>false</c> if unsuccessful.</returns>
        public bool Save()
        {
            if (!this.IsNewEvent)
            {
                // Update entry to database.
                DBLayer.DomainModels.EventModel.updateObj(this.Event.DBObj);

                // Go through and update SubEvents to the database.
                foreach (SubEventModel s in this._subEventCollection)
                {
                    s.DBObj.Event = null;

                    if (s.DBObj.Id > 0)
                    {
                        DBLayer.DomainModels.SubEventModel.updateObj(s.DBObj);
                    }
                    else
                    { 
                        s.DBObj.EventId = this.Event.Id;
                        DBLayer.DomainModels.SubEventModel.createObj(s.DBObj);
                    }
                }

                foreach (SubEventModel s in this._subEventsToDeleteList)
                {
                    DBLayer.DomainModels.SubEventModel.deleteById(s.Id);
                }

                // Go through and update BudgetItems to the database.
                foreach (BudgetItemModel i in this._budgetCollection)
                {
                    if (i.Id > 0)
                    {
                        DBLayer.DomainModels.BudgetItemModel.updateObj(i.DBObj);
                    }
                    else
                    {
                        i.BudgetId = this.Event.Budget.Id;
                        DBLayer.DomainModels.BudgetItemModel.createObj(i.DBObj);
                    }
                }

                foreach (BudgetItemModel i in this._budgetItemToDeleteList)
                {
                    DBLayer.DomainModels.BudgetItemModel.deleteById(i.Id);
                }
            }
            else
            {
                this.Event = new EventModel(DBLayer.DomainModels.EventModel.createObj(this.Event.DBObj));
                // Go through and update SubEvents to the database.
                foreach (SubEventModel s in this._subEventCollection)
                {
                    s.DBObj.Event = null;
                    s.DBObj.EventId = this.Event.Id;
                    DBLayer.DomainModels.SubEventModel.createObj(s.DBObj);
                }

                foreach (BudgetItemModel i in this._budgetCollection)
                {
                    i.BudgetId = this.Event.Budget.Id;
                    DBLayer.DomainModels.BudgetItemModel.createObj(i.DBObj);
                }
            }

            return true;
        }

        /// <summary>
        /// Unregisters the guest from the guest list.
        /// </summary>
        /// <param name="matricID">Matric ID of the guest.</param>
        /// <returns><c>true</c> if successful; <c>false</c> if unsuccessful.</returns>
        public bool UnregisterGuest(string matricID)
        {
            bool result = DBLayer.DomainModels.EventModel.unregisterGuest(matricID, this.Event.Id);
            DBLayer.Event tempEvent = DBLayer.DomainModels.EventModel.getByID(this.Event.Id);
            this.Event.Guests = tempEvent.Guests;

            this.UpdateCapacityUI();

            return result;
        }

        /// <summary>
        /// Updates the UI for the 'Guest List' tab in the View.
        /// </summary>
        public void UpdateCapacityUI()
        {
            this.NotifyPropertyChanged("CurrentCapacity");
            this.NotifyPropertyChanged("CapacityPercentage");

            // Force check Venues in case of a capacity overflow.
            foreach (SubEventModel s in this._subEventCollection)
            {
                s.Event = this.Event.DBObj;
                s.VenueIsValid(s.VenueId);
                s.NotifyPropertyChanged("VenueId");
            }
        }

        /// <summary>
        /// Updates the UI for the 'Budget' tab in the View.
        /// </summary>
        public void UpdateBudgetUI()
        {
            this.NotifyPropertyChanged("BudgetTotal");
            this.NotifyPropertyChanged("BudgetPercentage");
            this.NotifyPropertyChanged("BudgetCollection");
        }

        /// <summary>
        /// Saves the current BudgetItem in the form.
        /// </summary>
        public void SaveBudgetItem()
        {
            if (this.CurBudgetItem.HasNoErrors)
            {
                if (!this.IsBudgetItemEditingMode)
                {
                    this.CurBudgetItem.BudgetId = this.Event.Budget.Id;

                    // Add new budget item into the DataGrid.
                    this._budgetCollection.Add(this._curBudgetItem);
                }

                this.UpdateBudgetUI();
                this.ResetBudgetForm();
            }
        }

        /// <summary>
        /// Resets the BudgetItem in the form.
        /// </summary>
        public void ResetBudgetForm()
        {
            this.CurBudgetItem = new BudgetItemModel();
            this.IsBudgetItemEditingMode = false;
        }

        /// <summary>
        /// Sets the active BudgetItem to be edited in the form.
        /// </summary>
        /// <param name="b">BudgetItemModel object to be edited.</param>
        public void SetActiveBudgetItem(BudgetItemModel b)
        {
            this.CurBudgetItem = b;
            this.IsBudgetItemEditingMode = true;
        }

        /// <summary>
        /// Deletes the BudgetItem.
        /// </summary>
        /// <param name="b">BudgetItemModel object to be deleted.</param>
        public void DeleteBudgetItem(BudgetItemModel b)
        {
            // BudgetItem exists in the database. Remember to delete it later!
            if (b.Id > 0) this._budgetItemToDeleteList.Add(b);

            this._budgetCollection.Remove(b);
            this.UpdateBudgetUI();
        }

        /// <summary>
        /// Adds an empty SubEvent into the list.
        /// </summary>
        public void AddNewSubEvent()
        {
            SubEventModel s = new SubEventModel(new DBLayer.SubEvent());
            s.Name = "";
            s.Start = new DateTime( DateTime.Now.Year, DateTime.Now.Month, (DateTime.Now.Day + 3), 8, 0, 0);
            s.End = new DateTime(DateTime.Now.Year, DateTime.Now.Month, (DateTime.Now.Day + 3), 9, 0, 0);
           
            this._subEventCollection.Add(s);
            this.NotifyPropertyChanged("SubEventCollection");

            s.VenueId = 1;
            this.UpdateCapacityUI();
        }

        /// <summary>
        /// Clears all SubEvents in the list.
        /// </summary>
        public void ClearAllSubEvents()
        {
            foreach (SubEventModel s in this._subEventCollection)
                if (s.Id > 0) this._subEventsToDeleteList.Add(s);

            this._subEventCollection.Clear();

            // Add a new SubEvent since there must be at least one!
            this.AddNewSubEvent();

            this.NotifyPropertyChanged("SubEventCollection");
        }

        /// <summary>
        /// Deletes the specified SubEvent from the list.
        /// </summary>
        /// <param name="s">SubEventModel object to be delted.</param>
        public void DeleteSubEvent(SubEventModel s)
        {
            // SubEvent exists in the database. Remember to delete it later!
            if (s.Id > 0)  this._subEventsToDeleteList.Add(s);
            this._subEventCollection.Remove(s);
            this.NotifyPropertyChanged("SubEventCollection");

            // If list is empty, add a new one since there must be at least one SubEvent.
            if (this._subEventCollection.Count <= 0)
                this.AddNewSubEvent();
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using DBLayer = EventManagerPro.DBLayer;


namespace EventManagerPro.Models
{
    public class BudgetItemModel : BaseModel, IDataErrorInfo
    {
        private DBLayer.BudgetItem _dbObj;

        private const string NAME_ERROR = "Budget item name must not be empty.";
        private const string COST_ERROR = "Budget item cost must be positive.";

        #region Constructors
        public BudgetItemModel()
        {
            this._dbObj = new DBLayer.BudgetItem();
            this.Id = -1;
            this.Name = "";
            this.Cost = 0;
        }

        public BudgetItemModel(DBLayer.BudgetItem b)
        {
            this._dbObj = b;
        }
        #endregion

        #region Class Properties
        public DBLayer.BudgetItem DBObj
        {
            get { return this._dbObj; }
        }

        public int Id 
        {
            get { return this._dbObj.Id; }
            set
            {
                this._dbObj.Id = value;
                this.NotifyPropertyChanged("Id");
            }
        }
        public string Name 
        {
            get { return this._dbObj.Name; }
            set
            {
                if (this.NameIsValid(value) && this._dbObj.Name != value)
                {
                    this._dbObj.Name = value;
                    this.NotifyPropertyChanged("Name");
                }
            }
        }
        public int Cost { 
            get { return this._dbObj.Cost; }
            set
            {
                if (this.CostIsValid(value) && this._dbObj.Cost != value)
                {
                    this._dbObj.Cost = value;
                    this.NotifyPropertyChanged("Cost");
                }
            }
        }
        public int BudgetId 
        { 
            get { return this._dbObj.BudgetId; }
            set 
            {
                this._dbObj.BudgetId = value;
                this.NotifyPropertyChanged("BudgetId");
            }
        }
        #endregion

        #region Validation Methods
        public bool NameIsValid(string value)
        {
            if (String.IsNullOrEmpty(value))
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

        public bool CostIsValid(int value)
        {
            if (value <= 0)
            {
                base.AddError("Cost", COST_ERROR, false);
                return false;
            }
            else
            {
                base.RemoveError("Cost", COST_ERROR);
                return true;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using DBLayer = EventManagerPro.DBLayer;


namespace EventManagerPro.Models
{
    public class BudgetModel : BaseModel, IDataErrorInfo
    {
        private DBLayer.Budget _dbObj;
        private const string ALLOCATED_ERROR = "Allocated budget must be positive.";

        #region Constructors
        public BudgetModel()
        {
            this._dbObj = new DBLayer.Budget();
            this._dbObj.Id = -1;
            this._dbObj.AllocatedBudget = 500;
        }

        public BudgetModel(DBLayer.Budget b)
        {
            this._dbObj = b;
        }
        #endregion

        #region Class Properties
        public DBLayer.Budget DBObj
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

        public int AllocatedBudget
        {
            get { return this._dbObj.AllocatedBudget; }
            set
            {
                if (this.AllocatedBudgetIsValid(value) && this._dbObj.AllocatedBudget != value)
                {
                    this._dbObj.AllocatedBudget = value;
                    this.NotifyPropertyChanged("AllocatedBudget");
                }
            }
        }
        #endregion

        #region Validation Methods
        public bool AllocatedBudgetIsValid(int value)
        {
            if (value < 0)
            {
                base.AddError("AllocatedBudget", ALLOCATED_ERROR, false);
                return false;
            }
            else
            {
                base.RemoveError("AllocatedBudget", ALLOCATED_ERROR);
                return true;
            }
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

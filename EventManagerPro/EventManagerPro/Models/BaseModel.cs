using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace EventManagerPro.Models
{
    /// <summary>
    /// This class is used to provide functionality to each Model class without the need to replicate code.
    /// </summary>
    public class BaseModel : INotifyPropertyChanged
    {
        protected Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Adds the specified error to the errors collection if it is not already present.
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="error">Error message</param>
        /// <param name="isWarning">Inserts error in the first position if false.</param>
        public void AddError(string propertyName, string error, bool isWarning)
        {
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();

            if (!_errors[propertyName].Contains(error))
            {
                if (isWarning) _errors[propertyName].Add(error);
                else _errors[propertyName].Insert(0, error);
            }
            this.NotifyPropertyChanged("HasNoErrors");
        }

        /// <summary>
        /// Removes the specified error from the errors collection if it is present.
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="error">Error message</param>
        public void RemoveError(string propertyName, string error)
        {
            if (_errors.ContainsKey(propertyName) &&
                _errors[propertyName].Contains(error))
            {
                _errors[propertyName].Remove(error);
                if (_errors[propertyName].Count == 0) _errors.Remove(propertyName);
            }
            this.NotifyPropertyChanged("HasNoErrors");
        }

        /// <summary>
        /// Checks if the Model has any errors.
        /// </summary>
        /// <returns>True if there are 0 errors, false if >0 errors.</returns>
        public bool HasNoErrors
        {
            get 
            {
                return this._errors.Count == 0; 
            }
        }
    }

}

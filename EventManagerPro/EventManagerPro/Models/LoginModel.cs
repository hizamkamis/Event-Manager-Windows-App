using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace EventManagerPro.Models
{
    public class LoginModel : BaseModel, IDataErrorInfo
    {
        private string _username;
        private string _password;

        private const string USERNAME_ERROR = "Username must not be empty and be at least 3 characters long.";
        
        // Constructor
        public LoginModel()
        {
            this.Username = "";
            this.Password = "";
        }

        #region Class Properties
        public string Username 
        {
            get { return this._username; }
            set
            {
                if (this.UsernameIsValid(value) && this._username != value)
                {
                    this._username = value;
                    this.NotifyPropertyChanged("Username");
                }   
            }
        }

        public string Password
        {
            get { return this._password; }
            set
            {
                this._password = value;
                this.NotifyPropertyChanged("Password");
            }
        }
        #endregion

        #region Validation Methods
        /// <summary>
        /// Checks if Username input is valid.
        /// </summary>
        /// <param name="value">User input for Username property.</param>
        /// <returns>True if valid, false if invalid.</returns>
        public bool UsernameIsValid(string value)
        {
            if (String.IsNullOrEmpty(value) || value.Length < 3)
            {
                base.AddError("Username", USERNAME_ERROR, false);
                return false;
            }
            else
            {
                base.RemoveError("Username", USERNAME_ERROR);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBLayer = EventManagerPro.DBLayer;

namespace EventManagerPro.Models
{
    /// <summary>
    /// Singleton class to handle login information and state for the application.
    /// </summary>
    public class SessionModel
    {
        public const int STATUS_NONE = 0;
        public const int STATUS_NOTICE = 1;
        public const int STATUS_ERROR = 2;

        private static SessionModel _instance;
        private static DBLayer.Student _loggedInUser;

        public string StatusMessage { get; set; }
        public int StatusCode { get; set; }
        public int EditEventID { get; set; }

        public SessionModel()
        {
            StatusCode = STATUS_NONE;
        }

        public static SessionModel GetInstance()
        {
            if (_instance == null)
                _instance = new SessionModel();

            return _instance;
        }

        public void Refresh()
        {
            _loggedInUser = DBLayer.DomainModels.StudentModel.getByMatricId(_loggedInUser.MatricId);
        }

        public void Clear()
        {
            _instance = new SessionModel();
        }

        public void ClearStatus()
        {
            StatusCode = 0;
            StatusMessage = "";
            EditEventID = -1;
        }

        // Login Methods
        public bool LoggedIn
        {
            get { return (_loggedInUser != null); }
        }

        public DBLayer.Student LoggedInUser
        {
            get 
            { 
                return _loggedInUser;
            }
        }

        public void Login(DBLayer.Student s)
        {
            _loggedInUser = s;
        }

    }
}

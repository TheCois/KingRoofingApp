using KRF.Core.Entities.AccessControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KRF.Web
{
    public static class SessionManager
    {
        public const string USERID = "UserID";
        public const string LOGGEDINUSER = "loggedinUser";
        public const string USERNAME = "UserName";
        public const string ROLEID = "RoleId";
        public const string EMPID = "EmpID";
        public const string USERFULLNAME = "UserFullName";
        public const string PAGES = "Pages";
        /// <summary>
        /// Session for UserID
        /// </summary>
        public static int UserID
        {
            set { HttpContext.Current.Session[USERID] = value; }
            get
            {
                int userID = 0;
                if (HttpContext.Current.Session[USERID] != null)
                {
                    int.TryParse(HttpContext.Current.Session[USERID].ToString(), out userID);
                }
                return userID;
            }
        }
        /// <summary>
        /// Session for RoleID
        /// </summary>
        public static int RoleId
        {
            set { HttpContext.Current.Session[ROLEID] = value; }
            get
            {
                int roleID = 0;
                if (HttpContext.Current.Session[ROLEID] != null)
                {
                    int.TryParse(HttpContext.Current.Session[ROLEID].ToString(), out roleID);
                }
                return roleID;
            }
        }
        /// <summary>
        /// Session for EmpID
        /// </summary>
        public static int EmpID
        {
            set { HttpContext.Current.Session[EMPID] = value; }
            get
            {
                int empID = 0;
                if (HttpContext.Current.Session[EMPID] != null)
                {
                    int.TryParse(HttpContext.Current.Session[EMPID].ToString(), out empID);
                }
                return empID;
            }
        }
        /// <summary>
        /// Session for UserName
        /// </summary>
        public static string UserName
        {
            set { HttpContext.Current.Session[USERNAME] = value; }
            get
            {
                return Convert.ToString(HttpContext.Current.Session[USERNAME]);
            }
        }
        /// <summary>
        /// Session for UserFullName
        /// </summary>
        public static string UserFullName
        {
            set { HttpContext.Current.Session[USERFULLNAME] = value; }
            get
            {
                return Convert.ToString(HttpContext.Current.Session[USERFULLNAME]);
            }
        }
        /// <summary>
        /// Session for LoggedInUser
        /// </summary>
        public static Core.Entities.Actors.User LoggedInUser
        {
            set { HttpContext.Current.Session[LOGGEDINUSER] = value; }
            get
            {
                return (Core.Entities.Actors.User)HttpContext.Current.Session[LOGGEDINUSER];
            }
        }
        /// <summary>
        /// Session for Page List
        /// </summary>
        public static List<Pages> Pages
        {
            set { HttpContext.Current.Session[PAGES] = value; }
            get
            {
                return (List<Pages>)HttpContext.Current.Session[PAGES];
            }
        }
    }
}
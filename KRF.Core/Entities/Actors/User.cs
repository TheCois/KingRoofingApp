using System;
using KRF.Core.Entities.AccessControl;
using KRF.Core.Entities.MISC;

namespace KRF.Core.Entities.Actors
{
    public class User
    {
        /// <summary>
        /// Holds the unique identifier information of the logged in user
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Holds the Username information of the logged in user
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Holds the Password information of the logged in user
        /// </summary>
        public string Password { get; set; }


        /// <summary>
        /// Holds the Email information of the logged in user
        /// </summary>
        public string Email { get; set; }

        public DateTime? DOB { get; set; }
        
        /// <summary>
        /// Holds the boolean value for account enabled information of the logged in user
        /// </summary>
        public bool IsDeleted { get; set; }
        public bool IsAdmin { get; set; }

        ///// <summary>
        ///// Holds the Role information of the logged in user
        ///// </summary>
        //public Role Role { get; set; }
    }
}


using System;
namespace KRF.Core.Entities.Sales
{
    public class Lead
    {
        /// <summary>
        /// Holds the Id information
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Holds the LeadName information
        /// </summary>
        public string LeadName { get; set; }

        /// <summary>
        /// Holds the Name information
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Holds the Last Name information
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Holds the Business Name information
        /// </summary>
        public string BusinessName { get; set; }

        /// <summary>
        /// Holds the Tel information
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// Holds the Email information
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Holds the Contact Method information
        /// </summary>
        public int ContactMethod { get; set; }

        /// <summary>
        /// Holds the Property Relationship information
        /// </summary>
        public int PropertyRelationship { get; set; }

        /// <summary>
        /// Holds the Heard About Us information
        /// </summary>
        public int HearAboutUs { get; set; }

        /// <summary>
        /// Holds the Heard About Us NOTE information
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Holds the info on who has take lead info
        /// </summary>
        public int InfoTakenBy { get; set; }

        /// <summary>
        /// Holds the info lead assigned to 
        /// </summary>
        public int AssignedTo { get; set; }

        /// <summary>
        /// Holds the info of appointment
        /// </summary>
        public DateTime AppointmentDateTime { get; set; }

        /// <summary>
        /// Holds the When to call information
        /// </summary>
        public string CallTime { get; set; }

        /// <summary>
        /// Holds the Project Type information
        /// </summary>
        public int ProjectType { get; set; }

        /// <summary>
        /// Holds the Roof type information
        /// </summary>
        public int RoofType { get; set; }

        /// <summary>
        /// Holds the Roof age information
        /// </summary>
        public int RoofAge { get; set; }

        /// <summary>
        /// Holds the Number Of Stories information
        /// </summary>
        public int NumberOfStories { get; set; }

        /// <summary>
        /// Holds the start of project time information
        /// </summary>
        public int ProjectStartTimeline { get; set; }
       
        /// <summary>
        /// Holds the status of lead information
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Holds the Billing address information
        /// </summary>
        public string BillAddress1 { get; set; }

        /// <summary>
        /// Holds the Billing address information
        /// </summary>
        public string BillAddress2 { get; set; }

        /// <summary>
        /// Holds the Billing address information
        /// </summary>
        public int BillState { get; set; }

        /// <summary>
        /// Holds the Billing address information
        /// </summary>
        public int BillCity { get; set; }

        /// <summary>
        /// Holds the bill country information
        /// </summary>
        public int BillCountry { get; set; }

        /// <summary>
        /// Holds the Billing address information
        /// </summary>
        public string BillZipCode { get; set; }

        /// <summary>
        /// Holds the Job address information
        /// </summary>
        public string JobAddress1 { get; set; }

        /// <summary>
        /// Holds the Job address information
        /// </summary>
        public string JobAddress2 { get; set; }

        /// <summary>
        /// Holds the Job address information
        /// </summary>
        public int JobState { get; set; }

        /// <summary>
        /// Holds the Job address information
        /// </summary>
        public int JobCity { get; set; }

        /// <summary>
        /// Holds the Job country information
        /// </summary>
        public int JobCountry { get; set; }

        /// <summary>
        /// Holds the Job address information
        /// </summary>
        public string JobZipCode { get; set; }

        /// <summary>
        /// Holds the Job address information
        /// </summary>
        public string AdditionalInfo { get; set; }

        /// <summary>
        /// Holds the Prospect ID information
        /// </summary>
        public int ProspectID { get; set; }
        /// <summary>
        /// Holds the Cell information
        /// </summary>
        public string Cell { get; set; }
        /// <summary>
        /// Holds the Office Tel information
        /// </summary>
        public string Office { get; set; }
        /// <summary>
        /// Holds the Fax information
        /// </summary>
        public string Fax { get; set; }
        /// <summary>
        /// Holds the ExistingRoof information
        /// </summary>
        public int? ExistingRoof { get; set; }
        /// <summary>
        /// Holds the Lead's stage information
        /// </summary>
        public int LeadStage { get; set; }
    }
}

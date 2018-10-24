using DapperExtensions;
using KRF.Core.DTO.Sales;
using KRF.Core.Entities.Employee;
using KRF.Core.Entities.Sales;
using KRF.Core.Entities.ValueList;
using KRF.Core.FunctionalContracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class LeadManagement : ILeadManagement
    {
        private string _connectionString;

        /// <summary>
        /// Constructor
        /// </summary>
        public LeadManagement()
        {
            _connectionString = Convert.ToString(ConfigurationManager.AppSettings["ApplicationDSN"]);
        }

        /// <summary>
        /// Create an Lead
        /// </summary>
        /// <param name="Lead">Lead details</param>
        /// <returns>Newly created Lead identifier</returns>
        public int Create(Lead lead, IList<CustomerAddress> customerAddress)
        {
            using (var transactionScope = new TransactionScope())
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    var id = sqlConnection.Insert<Lead>(lead);
                    lead.ID = id;
                    foreach (var address in customerAddress)
                    {
                        sqlConnection.Insert<CustomerAddress>(customerAddress);
                    }

                    transactionScope.Complete();
                    return id;
                }
            }
        }

        /// <summary>
        /// Edit an Lead based on updated Lead details.
        /// </summary>
        /// <param name="Lead">Updated Lead details.</param>
        /// <returns>Updated Lead details.</returns>
        public Lead Edit(Lead lead, IList<CustomerAddress> customerAddress)
        {
            using (var transactionScope = new TransactionScope())
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    var isEdited = sqlConnection.Update<Lead>(lead);

                    foreach (var address in customerAddress)
                    {
                        sqlConnection.Insert<CustomerAddress>(customerAddress);
                    }

                    transactionScope.Complete();
                    return lead;
                }
            }
        }

        /// <summary>
        /// Create Job Address
        /// </summary>
        /// <param name="customerAddress"></param>
        /// <returns></returns>
        public int CreateJobAddress(IList<CustomerAddress> customerAddress)
        {
            int id = 0;
            if (customerAddress.Any())
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    id = sqlConnection.Insert<CustomerAddress>(customerAddress[0]);
                }
            }
            return id;
        }

        /// <summary>
        /// Edit Job Address
        /// </summary>
        /// <param name="customerAddress"></param>
        /// <returns></returns>
        public bool EditJobAddress(IList<CustomerAddress> customerAddress)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                var isEdited = sqlConnection.Update<CustomerAddress>(customerAddress[0]);
                return isEdited;
            }
        }

        /// <summary>
        /// Delete Job Address
        /// </summary>
        /// <param name="JobAddID"></param>
        /// <returns></returns>
        public bool DeleteJobAddress(int JobAddID)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                bool isDeleted = false;
                try
                {
                    var predicateGroupEst = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    predicateGroupEst.Predicates.Add(Predicates.Field<Estimate>(s => s.JobAddressID, Operator.Eq, JobAddID));

                    IList<Estimate> esimtates = sqlConnection.GetList<Estimate>(predicateGroupEst).ToList();

                    var predicateGroupJob = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    predicateGroupJob.Predicates.Add(Predicates.Field<Core.Entities.Customer.Job>(s => s.JobAddressID, Operator.Eq, JobAddID));

                    IList<Core.Entities.Customer.Job> jobs = sqlConnection.GetList<Core.Entities.Customer.Job>(predicateGroupJob).ToList();
                    if (!esimtates.Any() && !jobs.Any())
                    {
                        var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                        predicateGroup.Predicates.Add(Predicates.Field<CustomerAddress>(s => s.ID, Operator.Eq, JobAddID));

                        sqlConnection.Open();
                        isDeleted = sqlConnection.Delete<CustomerAddress>(predicateGroup);
                    }
                }
                catch (Exception ex)
                {
                    isDeleted = false;
                }
                return isDeleted;
            }
        }

        /// <summary>
        /// Delete an  Lead.
        /// </summary>
        /// <param name="LeadId"> Lead unique identifier</param>
        /// <returns>True - if successful deletion; False - If failure.</returns>
        public bool Delete(int id)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    predicateGroup.Predicates.Add(Predicates.Field<Lead>(s => s.ID, Operator.Eq, id));

                    sqlConnection.Open();
                    var isDeleted = sqlConnection.Delete<Lead>(predicateGroup);
                    return isDeleted;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Get all Lead created in the system.
        /// </summary>
        /// <param name="isActive">If true - returns only active  Leads else return all</param>
        /// <returns>List of  Leads.</returns>
        public LeadDTO GetLeads(Func<Lead, bool> predicate, bool isActive = true)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                IList<Lead> leads = sqlConnection.GetList<Lead>().Where(predicate).ToList();
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<City>(s => s.Active, Operator.Eq, true));
                IList<City> cities = sqlConnection.GetList<City>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<State>(s => s.Active, Operator.Eq, true));
                IList<State> states = sqlConnection.GetList<State>(predicateGroup).ToList();
                IList<Country> countries = sqlConnection.GetList<Country>().ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<Status>(s => s.Active, Operator.Eq, true));
                IList<Status> status = sqlConnection.GetList<Status>(predicateGroup).ToList();
                status = status.Where(k => k.ID != 4).ToList();
                IList<ContactMethod> contactMethod = sqlConnection.GetList<ContactMethod>().ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<HearAboutUs>(s => s.Active, Operator.Eq, true));
                IList<HearAboutUs> hearAboutUs = sqlConnection.GetList<HearAboutUs>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<PropertyRelationship>(s => s.Active, Operator.Eq, true));
                IList<PropertyRelationship> propertyRelationship = sqlConnection.GetList<PropertyRelationship>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<ProjectStartTimeline>(s => s.Active, Operator.Eq, true));
                IList<ProjectStartTimeline> projectStartTimeline = sqlConnection.GetList<ProjectStartTimeline>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<ProjectType>(s => s.Active, Operator.Eq, true));
                IList<ProjectType> projectType = sqlConnection.GetList<ProjectType>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<RoofAge>(s => s.Active, Operator.Eq, true));
                IList<RoofAge> roofAge = sqlConnection.GetList<RoofAge>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<NumberOfStories>(s => s.Active, Operator.Eq, true));
                IList<NumberOfStories> numberOfStories = sqlConnection.GetList<NumberOfStories>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<RoofType>(s => s.Active, Operator.Eq, true));
                IList<RoofType> roofType = sqlConnection.GetList<RoofType>(predicateGroup).ToList();
                IList<Employee> employees = sqlConnection.GetList<Employee>().ToList();
                
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<ExistingRoof>(s => s.Active, Operator.Eq, true));
                IList<ExistingRoof> existingRoof = sqlConnection.GetList<ExistingRoof>(predicateGroup).ToList();
                
                return new LeadDTO
                {
                    Leads = leads,
                    Cities = cities.OrderBy(p => p.Description).ToList(),
                    States = states.OrderBy(p => p.Description).ToList(),
                    Countries = countries.OrderBy(p => p.Description).ToList(),
                    Statuses = status.OrderBy(p => p.Description).ToList(),
                    ContactMethod = contactMethod.Where(p => p.Active == true).OrderBy(p => p.Description).ToList(),
                    HearAboutUsList = hearAboutUs.Where(p => p.Active == true).OrderBy(p => p.Description).ToList(),
                    PropertyRelationship = propertyRelationship.Where(p => p.Active == true).OrderBy(p => p.Description).ToList(),
                    ProjectStartTimelines = projectStartTimeline.OrderBy(p => p.Description).ToList(),
                    ProjectTypes = projectType.OrderBy(p => p.Description).ToList(),
                    RoofAgeList = roofAge.OrderBy(p => p.ID).ToList(),
                    BuildingStoriesList = numberOfStories.OrderBy(p => p.Description).ToList(),
                    RoofTypes = roofType.OrderBy(p => p.Description).ToList(),
                    Employees = employees.OrderBy(p => p.FirstName).ToList(),
                    ExistingRoofs = existingRoof.OrderBy(p => p.Description).ToList()
                };
            }
        }

        /// <summary>
        /// Get  Lead details based on  id.
        /// </summary>
        /// <param name="LeadId">Lead's unique identifier</param>
        /// <returns>Lead details.</returns>
        public LeadDTO GetLead(int id)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<Lead>(s => s.ID, Operator.Eq, id));
                sqlConnection.Open();
                Lead lead = sqlConnection.Get<Lead>(id);
                IList<Lead> leads = new List<Lead>();
                leads.Add(lead);

                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<City>(s => s.Active, Operator.Eq, true));
                IList<City> cities = sqlConnection.GetList<City>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<State>(s => s.Active, Operator.Eq, true));
                IList<State> states = sqlConnection.GetList<State>(predicateGroup).ToList();

                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<CustomerAddress>(s => s.LeadID, Operator.Eq, id));
                IList<CustomerAddress> customerAddress = sqlConnection.GetList<CustomerAddress>(predicateGroup).ToList();

                return new LeadDTO
                {
                    Leads = leads,
                    Cities = cities.OrderBy(p => p.Description).ToList(),
                    States = states.OrderBy(p => p.Description).ToList(),
                    CustomerAddress = customerAddress
                };
            }
        }

        /// <summary>
        /// Search and filter Lead based on search text.
        /// </summary>
        /// <param name="searchText">Search text which need to be mapped with any of  Lead related fields.</param>
        /// <returns> Lead list.</returns>
        public IList<Lead> SearchLead(string searchText)
        {
            return null;
        }

        /// <summary>
        /// Set  Lead to active / Deactive
        /// </summary>
        /// <param name="LeadId"> Lead unique identifier</param>
        /// <param name="isActive">True - active; False - Deactive.</param>
        /// <returns>True - if successful deletion; False - If failure.</returns>
        public bool SetActive(int LeadId, bool isActive)
        {
            return false;
        }

        private CustomerAddress CustomerAddress(int leadID, Lead lead)
        {
            CustomerAddress caddress = new CustomerAddress
            {
                LeadID = leadID,
                Address1 = lead.JobAddress1??"",
                Address2 = lead.JobAddress2??"",
                City = lead.JobCity,
                State = lead.JobState,
                ZipCode = lead.JobZipCode??"",
            };

            return caddress;
        }
    }
}

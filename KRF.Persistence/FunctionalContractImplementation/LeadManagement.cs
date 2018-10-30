using DapperExtensions;
using KRF.Core.DTO.Sales;
using KRF.Core.Entities.Employee;
using KRF.Core.Entities.Sales;
using KRF.Core.Entities.ValueList;
using KRF.Core.FunctionalContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class LeadManagement : ILeadManagement
    {
        /// <summary>
        /// Create an Lead
        /// </summary>
        /// <param name="lead">Lead details</param>
        /// <param name="customerAddress"></param>
        /// <returns>Newly created Lead identifier</returns>
        public int Create(Lead lead, IList<CustomerAddress> customerAddress)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    var id = conn.Insert(lead);
                    lead.ID = id;
                    foreach (var address in customerAddress)
                    {
                        conn.Insert<CustomerAddress>(customerAddress); // TODO this looks weird; check it out
                    }

                    transactionScope.Complete();
                    return id;
                }
            }
        }

        /// <summary>
        /// Edit an Lead based on updated Lead details.
        /// </summary>
        /// <param name="lead">Updated Lead details.</param>
        /// <returns>Updated Lead details.</returns>
        public Lead Edit(Lead lead, IList<CustomerAddress> customerAddress)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    conn.Update(lead);

                    foreach (var address in customerAddress)
                    {
                        conn.Insert<CustomerAddress>(customerAddress); // TODO this looks weird
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
            var id = 0;
            if (customerAddress.Any())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    id = conn.Insert(customerAddress[0]);
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
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                var isEdited = conn.Update(customerAddress[0]);
                return isEdited;
            }
        }

        /// <summary>
        /// Delete Job Address
        /// </summary>
        /// <param name="jobAddId"></param>
        /// <returns></returns>
        public bool DeleteJobAddress(int jobAddId)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                var isDeleted = false;
                try
                {
                    var predicateGroupEst = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    predicateGroupEst.Predicates.Add(Predicates.Field<Estimate>(s => s.JobAddressID, Operator.Eq, jobAddId));

                    IList<Estimate> estimates = conn.GetList<Estimate>(predicateGroupEst).ToList();

                    var predicateGroupJob = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    predicateGroupJob.Predicates.Add(Predicates.Field<Core.Entities.Customer.Job>(s => s.JobAddressID, Operator.Eq, jobAddId));

                    IList<Core.Entities.Customer.Job> jobs = conn.GetList<Core.Entities.Customer.Job>(predicateGroupJob).ToList();
                    if (!estimates.Any() && !jobs.Any())
                    {
                        var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                        predicateGroup.Predicates.Add(Predicates.Field<CustomerAddress>(s => s.ID, Operator.Eq, jobAddId));

                        conn.Open();
                        isDeleted = conn.Delete<CustomerAddress>(predicateGroup);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    isDeleted = false;
                }
                return isDeleted;
            }
        }

        /// <summary>
        /// Delete an  Lead.
        /// </summary>
        /// <param name="id"> Lead unique identifier</param>
        /// <returns>True - if successful deletion; False - If failure.</returns>
        public bool Delete(int id)
        {
            try
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    predicateGroup.Predicates.Add(Predicates.Field<Lead>(s => s.ID, Operator.Eq, id));

                    conn.Open();
                    var isDeleted = conn.Delete<Lead>(predicateGroup);
                    return isDeleted;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Get all Lead created in the system.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="isActive">If true - returns only active  Leads else return all</param>
        /// <returns>List of  Leads.</returns>
        public LeadDTO GetLeads(Func<Lead, bool> predicate, bool isActive = true)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                IList<Lead> leads = conn.GetList<Lead>().Where(predicate).ToList();
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<City>(s => s.Active, Operator.Eq, true));
                IList<City> cities = conn.GetList<City>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<State>(s => s.Active, Operator.Eq, true));
                IList<State> states = conn.GetList<State>(predicateGroup).ToList();
                IList<Country> countries = conn.GetList<Country>().ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<Status>(s => s.Active, Operator.Eq, true));
                IList<Status> status = conn.GetList<Status>(predicateGroup).ToList();
                status = status.Where(k => k.ID != 4).ToList();
                IList<ContactMethod> contactMethod = conn.GetList<ContactMethod>().ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<HearAboutUs>(s => s.Active, Operator.Eq, true));
                IList<HearAboutUs> hearAboutUs = conn.GetList<HearAboutUs>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<PropertyRelationship>(s => s.Active, Operator.Eq, true));
                IList<PropertyRelationship> propertyRelationship = conn.GetList<PropertyRelationship>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<ProjectStartTimeline>(s => s.Active, Operator.Eq, true));
                IList<ProjectStartTimeline> projectStartTimeline = conn.GetList<ProjectStartTimeline>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<ProjectType>(s => s.Active, Operator.Eq, true));
                IList<ProjectType> projectType = conn.GetList<ProjectType>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<RoofAge>(s => s.Active, Operator.Eq, true));
                IList<RoofAge> roofAge = conn.GetList<RoofAge>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<NumberOfStories>(s => s.Active, Operator.Eq, true));
                IList<NumberOfStories> numberOfStories = conn.GetList<NumberOfStories>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<RoofType>(s => s.Active, Operator.Eq, true));
                IList<RoofType> roofType = conn.GetList<RoofType>(predicateGroup).ToList();
                IList<Employee> employees = conn.GetList<Employee>().ToList();
                
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<ExistingRoof>(s => s.Active, Operator.Eq, true));
                IList<ExistingRoof> existingRoof = conn.GetList<ExistingRoof>(predicateGroup).ToList();
                
                return new LeadDTO
                {
                    Leads = leads,
                    Cities = cities.OrderBy(p => p.Description).ToList(),
                    States = states.OrderBy(p => p.Description).ToList(),
                    Countries = countries.OrderBy(p => p.Description).ToList(),
                    Statuses = status.OrderBy(p => p.Description).ToList(),
                    ContactMethod = contactMethod.Where(p => p.Active).OrderBy(p => p.Description).ToList(),
                    HearAboutUsList = hearAboutUs.Where(p => p.Active).OrderBy(p => p.Description).ToList(),
                    PropertyRelationship = propertyRelationship.Where(p => p.Active).OrderBy(p => p.Description).ToList(),
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
        /// <param name="id">Lead unique identifier</param>
        /// <returns>Lead details.</returns>
        public LeadDTO GetLead(int id)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<Lead>(s => s.ID, Operator.Eq, id));
                conn.Open();
                var lead = conn.Get<Lead>(id);
                IList<Lead> leads = new List<Lead>();
                leads.Add(lead);

                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<City>(s => s.Active, Operator.Eq, true));
                IList<City> cities = conn.GetList<City>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<State>(s => s.Active, Operator.Eq, true));
                IList<State> states = conn.GetList<State>(predicateGroup).ToList();

                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<CustomerAddress>(s => s.LeadID, Operator.Eq, id));
                IList<CustomerAddress> customerAddress = conn.GetList<CustomerAddress>(predicateGroup).ToList();

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
        /// Set  Lead to active / Inactive
        /// </summary>
        /// <param name="leadId"> Lead unique identifier</param>
        /// <param name="isActive">True - active; False - Inactive.</param>
        /// <returns>True - if successful deletion; False - If failure.</returns>
        public bool SetActive(int leadId, bool isActive)
        {
            return false;
        }

        private CustomerAddress CustomerAddress(int leadId, Lead lead)
        {
            var caddress = new CustomerAddress
            {
                LeadID = leadId,
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

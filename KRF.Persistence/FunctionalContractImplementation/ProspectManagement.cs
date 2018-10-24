using System;
using System.Collections.Generic;
using System.Configuration;
using KRF.Core.Entities.Sales;
using KRF.Core.FunctionalContracts;
using KRF.Core.DTO.Product;
using KRF.Core.DTO.Sales;
using System.Data.SqlClient;
using DapperExtensions;
using System.Linq;
using KRF.Core.Entities.ValueList;
using System.Transactions;
using System.Data;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class ProspectManagement : IProspectManagement
    {
        private string _connectionString;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProspectManagement()
        {
            _connectionString = Convert.ToString(ConfigurationManager.AppSettings["ApplicationDSN"]);
        }

        /// <summary>
        /// Create an Prospect
        /// </summary>
        /// <param name="Prospect">Prospect details</param>
        /// <returns>Newly created Prospect identifier</returns>
        public int Create(Prospect prospect)
        {
            using (var transactionScope = new TransactionScope())
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    var id = sqlConnection.Insert<Prospect>(prospect);

                    if (prospect.Status == 4)
                    {
                        var lead = InsertProspectIntoLead(prospect);
                        sqlConnection.Insert<Lead>(lead);
                    }

                    transactionScope.Complete();
                    return id;
                }
            }
        }

        /// <summary>
        /// Edit an Prospect based on updated Prospect details.
        /// </summary>
        /// <param name="Prospect">Updated Prospect details.</param>
        /// <returns>Updated Prospect details.</returns>
        public Prospect Edit(Prospect prospect)
        {
            using (var transactionScope = new TransactionScope())
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    var isEdited = sqlConnection.Update<Prospect>(prospect);

                    if (prospect.Status == 4)
                    {
                        var lead = InsertProspectIntoLead(prospect);
                        sqlConnection.Insert<Lead>(lead);
                    }

                    transactionScope.Complete();
                    return prospect;
                }
            }
        }

        /// <summary>
        /// Save prospects 
        /// </summary>
        /// <param name="prospects"></param>
        public void SaveProspects(IList<Prospect> prospects)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                IList<Prospect> existingProspects = sqlConnection.GetList<Prospect>().ToList();
                foreach (var prospect in prospects)
                {
                    sqlConnection.Insert<Prospect>(prospect);
                }
            }
        }


        static DataTable ConvertListToDataTable(List<string[]> list)
        {
            // New table.
            DataTable table = new DataTable();

            // Get max columns.
            int columns = 0;
            foreach (var array in list)
            {
                if (array.Length > columns)
                {
                    columns = array.Length;
                }
            }

            // Add columns.
            for (int i = 0; i < columns; i++)
            {
                table.Columns.Add();
            }

            // Add rows.
            foreach (var array in list)
            {
                table.Rows.Add(array);
            }

            return table;
        }


        /// <summary>
        /// Delete an  Prospect.
        /// </summary>
        /// <param name="ProspectId"> Prospect unique identifier</param>
        /// <returns>True - if successful deletion; False - If failure.</returns>
        public bool Delete(int id)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    predicateGroup.Predicates.Add(Predicates.Field<Prospect>(s => s.ID, Operator.Eq, id));

                    sqlConnection.Open();
                    var isDeleted = sqlConnection.Delete<Prospect>(predicateGroup);
                    return isDeleted;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Get all Prospect created in the system.
        /// </summary>
        /// <param name="isActive">If true - returns only active  Prospect else return all</param>
        /// <returns>List of  Prospect.</returns>
        public ProspectDTO GetProspects(bool isActive = true)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                IList<Prospect> prospects = sqlConnection.GetList<Prospect>().ToList();
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
                status = status.Where(k => k.ID != 5).ToList();

                return new ProspectDTO { Propects = prospects, States = states.OrderBy(p => p.Description).ToList(), Statuses = status.OrderBy(p => p.Description).ToList(), Cities = cities.OrderBy(p => p.Description).ToList(), Countries = countries.OrderBy(p => p.Description).ToList() };
            }
        }

        /// <summary>
        /// Get  Prospect details based on  id.
        /// </summary>
        /// <param name="ProspectId">Prospect's unique identifier</param>
        /// <returns>Prospect details.</returns>
        public ProspectDTO GetProspect(int id)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<Prospect>(s => s.ID, Operator.Eq, id));
                sqlConnection.Open();
                Prospect prospect = sqlConnection.Get<Prospect>(id);
                IList<Prospect> p = new List<Prospect>();
                p.Add(prospect);
                return new ProspectDTO { Propects = p }; ;
            }
        }

        /// <summary>
        /// Search and filter Prospect based on search text.
        /// </summary>
        /// <param name="searchText">Search text which need to be mapped with any of  Prospect related fields.</param>
        /// <returns> Prospect list.</returns>
        public IList<Prospect> SearchProspect(string searchText)
        {
            return null;
        }

        /// <summary>
        /// Set  Prospect to active / Deactive
        /// </summary>
        /// <param name="ProspectId"> Prospect unique identifier</param>
        /// <param name="isActive">True - active; False - Deactive.</param>
        /// <returns>True - if successful deletion; False - If failure.</returns>
        public bool SetActive(int ProspectId, bool isActive)
        {
            return false;
        }

        private Lead InsertProspectIntoLead(Prospect prospect)
        {

            Lead lead = new Lead
            {
                FirstName = prospect.FirstName,
                LastName = prospect.LastName,
                Email = prospect.Email,
                Telephone = prospect.Telephone,
                BillAddress1 = prospect.Address1,
                BillAddress2 = prospect.Address2,
                BillZipCode = prospect.ZipCode,
                BillCity = prospect.City,
                BillState = prospect.State,
                BillCountry = prospect.Country,
                Status = 1,
                ProspectID = prospect.ID,
                AppointmentDateTime = DateTime.Now,
                AssignedTo = 2
            };

            return lead;
        }
    }
}

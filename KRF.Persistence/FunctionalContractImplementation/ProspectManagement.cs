using System;
using System.Collections.Generic;
using KRF.Core.Entities.Sales;
using KRF.Core.FunctionalContracts;
using KRF.Core.DTO.Sales;
using DapperExtensions;
using System.Linq;
using KRF.Core.Entities.ValueList;
using System.Transactions;
using System.Data;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class ProspectManagement : IProspectManagement
    {
        /// <summary>
        /// Create an Prospect
        /// </summary>
        /// <param name="prospect">Prospect details</param>
        /// <returns>Newly created Prospect identifier</returns>
        public int Create(Prospect prospect)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    var id = conn.Insert(prospect);

                    if (prospect.Status == 4)
                    {
                        var lead = InsertProspectIntoLead(prospect);
                        conn.Insert(lead);
                    }

                    transactionScope.Complete();
                    return id;
                }
            }
        }

        /// <summary>
        /// Edit an Prospect based on updated Prospect details.
        /// </summary>
        /// <param name="prospect">Updated Prospect details.</param>
        /// <returns>Updated Prospect details.</returns>
        public Prospect Edit(Prospect prospect)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    conn.Update(prospect);

                    if (prospect.Status == 4)
                    {
                        var lead = InsertProspectIntoLead(prospect);
                        conn.Insert(lead);
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
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                //TODO weird; why are we reading this?
                IList<Prospect> existingProspects = conn.GetList<Prospect>().ToList();
                foreach (var prospect in prospects)
                {
                    conn.Insert(prospect);
                }
            }
        }


        static DataTable ConvertListToDataTable(List<string[]> list)
        {
            // New table.
            var table = new DataTable();

            // Get max columns.
            var columns = 0;
            foreach (var array in list)
            {
                if (array.Length > columns)
                {
                    columns = array.Length;
                }
            }

            // Add columns.
            for (var i = 0; i < columns; i++)
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
        /// <param name="id"> Prospect unique identifier</param>
        /// <returns>True - if successful deletion; False - If failure.</returns>
        public bool Delete(int id)
        {
            try
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    var predicateGroup = new PredicateGroup
                        {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                    predicateGroup.Predicates.Add(Predicates.Field<Prospect>(s => s.ID, Operator.Eq, id));

                    conn.Open();
                    var isDeleted = conn.Delete<Prospect>(predicateGroup);
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
        /// Get all Prospect created in the system.
        /// </summary>
        /// <param name="isActive">If true - returns only active  Prospect else return all</param>
        /// <returns>List of  Prospect.</returns>
        public ProspectDTO GetProspects(bool isActive = true)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                IList<Prospect> prospects = conn.GetList<Prospect>().ToList();
                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<City>(s => s.Active, Operator.Eq, true));
                IList<City> cities = conn.GetList<City>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<State>(s => s.Active, Operator.Eq, true));
                IList<State> states = conn.GetList<State>(predicateGroup).ToList();
                IList<Country> countries = conn.GetList<Country>().ToList();
                predicateGroup = new PredicateGroup {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<Status>(s => s.Active, Operator.Eq, true));
                IList<Status> status = conn.GetList<Status>(predicateGroup).ToList();
                status = status.Where(k => k.ID != 5).ToList();

                return new ProspectDTO
                {
                    Propects = prospects, States = states.OrderBy(p => p.Description).ToList(),
                    Statuses = status.OrderBy(p => p.Description).ToList(),
                    Cities = cities.OrderBy(p => p.ID).ToList(),
                    Countries = countries.OrderBy(p => p.Description).ToList()
                };
            }
        }

        /// <summary>
        /// Get  Prospect details based on  id.
        /// </summary>
        /// <param name="id">Prospect's unique identifier</param>
        /// <returns>Prospect details.</returns>
        public ProspectDTO GetProspect(int id)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<Prospect>(s => s.ID, Operator.Eq, id));
                conn.Open();
                var prospect = conn.Get<Prospect>(id);
                IList<Prospect> p = new List<Prospect>();
                p.Add(prospect);
                return new ProspectDTO {Propects = p};
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

            var lead = new Lead
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
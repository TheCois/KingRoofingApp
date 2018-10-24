using DapperExtensions;
using KRF.Core.DTO.Master;
using KRF.Core.Entities.Master;
using KRF.Core.Entities.Employee;
using KRF.Core.FunctionalContracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class CrewManagement : ICrewManagement
    {
        private string _connectionString;

        /// <summary>
        /// Constructor
        /// </summary>
        public CrewManagement()
        {
            _connectionString = Convert.ToString(ConfigurationManager.AppSettings["ApplicationDSN"]);
        }
        /// <summary>
        /// Create Crew
        /// </summary>
        /// <param name="crew"></param>
        /// <returns></returns>
        public int CreateCrew(CrewDTO crew)
        {
            using (var transactionScope = new TransactionScope())
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    crew.Crews[0].DateCreated = DateTime.Now;
                    crew.Crews[0].Active = true;
                    var id = sqlConnection.Insert<Crew>(crew.Crews[0]);

                    if (crew.CrewDetails != null)
                    {
                        foreach (var c in crew.CrewDetails)
                        {
                            c.CrewID = id;
                            sqlConnection.Insert<CrewDetail>(c);
                        }
                    }

                    transactionScope.Complete();
                    return id;
                }
            }
        }
        /// <summary>
        /// Edit Crew detail
        /// </summary>
        /// <param name="crew"></param>
        /// <returns></returns>
        public bool EditCrew(CrewDTO crew)
        {
            using (var transactionScope = new TransactionScope())
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    crew.Crews[0].DateUpdated = DateTime.Now;
                    var isEdited = sqlConnection.Update<Crew>(crew.Crews[0]);
                    int id = crew.Crews[0].CrewID;
                    if (crew.CrewDetails != null)
                    {
                        var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                        predicateGroup.Predicates.Add(Predicates.Field<CrewDetail>(s => s.CrewID, Operator.Eq, id));
                        IList<CrewDetail> crewDetailToBeDelete = sqlConnection.GetList<CrewDetail>(predicateGroup).ToList();
                        foreach (var c in crewDetailToBeDelete)
                        {
                            sqlConnection.Delete<CrewDetail>(c);
                        }
                        foreach (var c in crew.CrewDetails)
                        {
                            c.CrewID = id;
                            sqlConnection.Insert<CrewDetail>(c);
                        }
                    }
                    transactionScope.Complete();
                    return isEdited;
                }
            }
        }
        /// <summary>
        /// Active/Inactive Crew Status
        /// </summary>
        /// <param name="crewId"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public bool SetActiveInactiveCrew(int crewId, bool tobeEnabled)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                Crew crew = sqlConnection.Get<Crew>(crewId);
                crew.Active = tobeEnabled;
                crew.DateUpdated = DateTime.Now;
                var isUpdated = sqlConnection.Update<Crew>(crew);
                return isUpdated;
            }
        }
        /// <summary>
        /// Active/Inactive crew detail status
        /// </summary>
        /// <param name="crewDetailId"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public bool SetActiveInactiveCrewDetail(int crewDetailId, bool tobeEnabled)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                CrewDetail crewDetail = sqlConnection.Get<CrewDetail>(crewDetailId);
                crewDetail.Active = tobeEnabled;
                var isUpdated = sqlConnection.Update<CrewDetail>(crewDetail);
                return isUpdated;
            }
        }
        /// <summary>
        /// Get Crew by CrewID
        /// </summary>
        /// <param name="crewID"></param>
        /// <returns></returns>
        public CrewDTO GetCrew(int crewID)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<CrewDetail>(s => s.CrewID, Operator.Eq, crewID));
                sqlConnection.Open();
                Crew crew = sqlConnection.Get<Crew>(crewID);
                IList<Crew> p = new List<Crew>();
                p.Add(crew);
                IList<CrewDetail> crewDetails = sqlConnection.GetList<CrewDetail>(predicateGroup).ToList();
                IList<Employee> employees = sqlConnection.GetList<Employee>().ToList();
                return new CrewDTO
                {
                    Crews = p,
                    CrewDetails = crewDetails,
                    Employees = employees
                };
            }
        }
        /// <summary>
        /// Get all Crews
        /// </summary>
        /// <returns></returns>
        public CrewDTO ListAllCrews()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<Crew>(s => s.Active, Operator.Eq, true));

                var predicateGroupDetail = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroupDetail.Predicates.Add(Predicates.Field<CrewDetail>(s => s.Active, Operator.Eq, true));
                
                sqlConnection.Open();
                IList<Crew> crews = sqlConnection.GetList<Crew>(predicateGroup).ToList();
                IList<CrewDetail> crewDetails = sqlConnection.GetList<CrewDetail>(predicateGroupDetail).ToList();
                IList<Employee> employees = sqlConnection.GetList<Employee>().ToList();
                return new CrewDTO
                {
                    Crews = crews,
                    CrewDetails = crewDetails,
                    Employees = employees
                }; ;
            }
        }
    }
}

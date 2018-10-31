using DapperExtensions;
using KRF.Core.DTO.Master;
using KRF.Core.Entities.Master;
using KRF.Core.Entities.Employee;
using KRF.Core.FunctionalContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class CrewManagement : ICrewManagement
    {
        /// <summary>
        /// Create Crew
        /// </summary>
        /// <param name="crew"></param>
        /// <returns></returns>
        public int CreateCrew(CrewDTO crew)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    crew.Crews[0].DateCreated = DateTime.Now;
                    crew.Crews[0].Active = true;
                    var id = conn.Insert(crew.Crews[0]);

                    if (crew.CrewDetails != null)
                    {
                        foreach (var c in crew.CrewDetails)
                        {
                            c.CrewID = id;
                            conn.Insert(c);
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
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    crew.Crews[0].DateUpdated = DateTime.Now;
                    var isEdited = conn.Update(crew.Crews[0]);
                    var id = crew.Crews[0].CrewID;
                    if (crew.CrewDetails != null)
                    {
                        var predicateGroup = new PredicateGroup
                            {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                        predicateGroup.Predicates.Add(Predicates.Field<CrewDetail>(s => s.CrewID, Operator.Eq, id));
                        IList<CrewDetail> crewDetailToBeDelete = conn.GetList<CrewDetail>(predicateGroup).ToList();
                        foreach (var c in crewDetailToBeDelete)
                        {
                            conn.Delete(c);
                        }

                        foreach (var c in crew.CrewDetails)
                        {
                            c.CrewID = id;
                            conn.Insert(c);
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
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                var crew = conn.Get<Crew>(crewId);
                crew.Active = tobeEnabled;
                crew.DateUpdated = DateTime.Now;
                var isUpdated = conn.Update(crew);
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
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                var crewDetail = conn.Get<CrewDetail>(crewDetailId);
                crewDetail.Active = tobeEnabled;
                var isUpdated = conn.Update(crewDetail);
                return isUpdated;
            }
        }

        /// <summary>
        /// Get Crew by CrewID
        /// </summary>
        /// <param name="crewId"></param>
        /// <returns></returns>
        public CrewDTO GetCrew(int crewId)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<CrewDetail>(s => s.CrewID, Operator.Eq, crewId));
                conn.Open();
                var crew = conn.Get<Crew>(crewId);
                IList<Crew> p = new List<Crew>();
                p.Add(crew);
                IList<CrewDetail> crewDetails = conn.GetList<CrewDetail>(predicateGroup).ToList();
                IList<Employee> employees = conn.GetList<Employee>().ToList();
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
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<Crew>(s => s.Active, Operator.Eq, true));

                var predicateGroupDetail = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroupDetail.Predicates.Add(Predicates.Field<CrewDetail>(s => s.Active, Operator.Eq, true));

                conn.Open();
                IList<Crew> crews = conn.GetList<Crew>(predicateGroup).ToList();
                IList<CrewDetail> crewDetails = conn.GetList<CrewDetail>(predicateGroupDetail).ToList();
                IList<Employee> employees = conn.GetList<Employee>().ToList();
                return new CrewDTO
                {
                    Crews = crews,
                    CrewDetails = crewDetails,
                    Employees = employees
                };
            }
        }
    }
}
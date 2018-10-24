using DapperExtensions;
using KRF.Core.DTO.Master;
using KRF.Core.Entities.Employee;
using KRF.Core.Entities.Master;
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
    public class FleetManagement : IFleetManagement
    {
        private string _connectionString;

        /// <summary>
        /// Constructor
        /// </summary>
        public FleetManagement()
        {
            _connectionString = Convert.ToString(ConfigurationManager.AppSettings["ApplicationDSN"]);
        }
        /// <summary>
        /// Create new fleet record in DB
        /// </summary>
        /// <param name="fleet"></param>
        /// <param name="fleetServices"></param>
        /// <param name="fleetAssignments"></param>
        /// <returns></returns>
        public int CreateFleet(Fleet fleet, List<FleetService> fleetServices, List<FleetAssignment> fleetAssignments)
        {
            using (var transactionScope = new TransactionScope())
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    var id = sqlConnection.Insert<Fleet>(fleet);
                    if (fleetServices != null)
                    {
                        foreach (FleetService fleetService in fleetServices)
                        {
                            fleetService.FleetID = id;
                            sqlConnection.Insert<FleetService>(fleetService);
                        }
                    }
                    if (fleetAssignments != null)
                    {
                        foreach (FleetAssignment fleetAssignment in fleetAssignments)
                        {
                            fleetAssignment.FleetID = id;
                            sqlConnection.Insert<FleetAssignment>(fleetAssignment);
                        }
                    }

                    transactionScope.Complete();
                    return id;
                }
            }
        }
        /// <summary>
        /// Edit Fleet record
        /// </summary>
        /// <param name="fleet"></param>
        /// <param name="fleetServices"></param>
        /// <param name="fleetAssignments"></param>
        /// <returns></returns>
        public bool EditFleet(Fleet fleet, List<FleetService> fleetServices, List<FleetAssignment> fleetAssignments)
        {
            using (var transactionScope = new TransactionScope())
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    int id = fleet.FleetID;
                    var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    predicateGroup.Predicates.Add(Predicates.Field<FleetService>(s => s.FleetID, Operator.Eq, id));
                    Fleet fleetCurrent = sqlConnection.Get<Fleet>(id);
                    fleet.DateCreated = fleetCurrent.DateCreated;
                    fleet.Active = fleetCurrent.Active;
                    IList<FleetService> fleetSrvs = sqlConnection.GetList<FleetService>(predicateGroup).ToList();
                    foreach (FleetService fleetSrv in fleetSrvs)
                    {
                        sqlConnection.Delete(fleetSrv);
                    }
                    if (fleetServices != null)
                    {
                        foreach (FleetService fleetService in fleetServices)
                        {
                            fleetService.FleetID = id;
                            sqlConnection.Insert<FleetService>(fleetService);
                        }
                    }
                    predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    predicateGroup.Predicates.Add(Predicates.Field<FleetAssignment>(s => s.FleetID, Operator.Eq, id));
                    IList<FleetAssignment> fleetAssigmts = sqlConnection.GetList<FleetAssignment>(predicateGroup).ToList();
                    foreach (FleetAssignment fleetAssigmt in fleetAssigmts)
                    {
                        sqlConnection.Delete(fleetAssigmt);
                    }
                    if (fleetAssignments != null)
                    {
                        foreach (FleetAssignment fleetAssignment in fleetAssignments)
                        {
                            fleetAssignment.FleetID = id;
                            sqlConnection.Insert<FleetAssignment>(fleetAssignment);
                        }
                    }

                    var isEdited = sqlConnection.Update<Fleet>(fleet);
                    transactionScope.Complete();
                    return isEdited;
                }
            }
        }
        /// <summary>
        /// Active/Inactive Fleet status
        /// </summary>
        /// <param name="fleetID"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public bool ToggleFleetStatus(int fleetID, bool tobeEnabled)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                Fleet fleet = sqlConnection.Get<Fleet>(fleetID);
                fleet.Active = tobeEnabled;
                fleet.DateUpdated = DateTime.Now;
                var isUpdated = sqlConnection.Update<Fleet>(fleet);
                return isUpdated;
            }
        }
        /// <summary>
        /// Get Fleet Detail by FleetID
        /// </summary>
        /// <param name="fleetID"></param>
        /// <returns></returns>
        public FleetDTO GetFleetDetail(int fleetID)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                Fleet fleet = sqlConnection.Get<Fleet>(fleetID);
                IList<Fleet> p = new List<Fleet>();
                p.Add(fleet);
                
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<FleetService>(s => s.FleetID, Operator.Eq, fleetID));
                IList<FleetService> services = sqlConnection.GetList<FleetService>(predicateGroup).ToList();

                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<FleetAssignment>(s => s.FleetID, Operator.Eq, fleetID));
                IList<FleetAssignment> assignments = sqlConnection.GetList<FleetAssignment>(predicateGroup).ToList();
                
                IList<FleetStatus> fleetstatus = sqlConnection.GetList<FleetStatus>().ToList();
                IList<Employee> employees = sqlConnection.GetList<Employee>().ToList();
                
                return new FleetDTO
                {
                    Fleets = p,
                    FleetServices = services,
                    FleetAssignments = assignments,
                    FleetStatus = fleetstatus.OrderBy(q => q.StatusName).ToList(),
                    Employes = employees.OrderBy(q => q.FirstName).ToList()
                }; ;
            }
        }
        /// <summary>
        /// Get Fleet Details
        /// </summary>
        /// <returns></returns>
        public FleetDTO GetFleetDetails()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                IList<Fleet> fleets = sqlConnection.GetList<Fleet>().ToList();
                IList<FleetStatus> fleetstatus = sqlConnection.GetList<FleetStatus>().ToList();
                IList<FleetService> services = sqlConnection.GetList<FleetService>().ToList();
                IList<FleetAssignment> assignments = sqlConnection.GetList<FleetAssignment>().ToList();
                IList<Employee> employees = sqlConnection.GetList<Employee>().ToList();
                
                return new FleetDTO
                {
                    Fleets = fleets,
                    FleetServices = services,
                    FleetAssignments = assignments,
                    FleetStatus = fleetstatus.OrderBy(q => q.StatusName).ToList(),
                    Employes = employees.OrderBy(q => q.FirstName).ToList()
                }; ;
            }
        }
    }
}

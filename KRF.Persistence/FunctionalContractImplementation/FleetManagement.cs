using DapperExtensions;
using KRF.Core.DTO.Master;
using KRF.Core.Entities.Employee;
using KRF.Core.Entities.Master;
using KRF.Core.Entities.ValueList;
using KRF.Core.FunctionalContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class FleetManagement : IFleetManagement
    {
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
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    var id = conn.Insert(fleet);
                    if (fleetServices != null)
                    {
                        foreach (var fleetService in fleetServices)
                        {
                            fleetService.FleetID = id;
                            conn.Insert(fleetService);
                        }
                    }

                    if (fleetAssignments != null)
                    {
                        foreach (var fleetAssignment in fleetAssignments)
                        {
                            fleetAssignment.FleetID = id;
                            conn.Insert(fleetAssignment);
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
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    var id = fleet.FleetID;
                    var predicateGroup = new PredicateGroup
                        {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                    predicateGroup.Predicates.Add(Predicates.Field<FleetService>(s => s.FleetID, Operator.Eq, id));
                    var fleetCurrent = conn.Get<Fleet>(id);
                    fleet.DateCreated = fleetCurrent.DateCreated;
                    fleet.Active = fleetCurrent.Active;
                    IList<FleetService> fleetSrvs = conn.GetList<FleetService>(predicateGroup).ToList();
                    foreach (var fleetSrv in fleetSrvs)
                    {
                        conn.Delete(fleetSrv);
                    }

                    if (fleetServices != null)
                    {
                        foreach (var fleetService in fleetServices)
                        {
                            fleetService.FleetID = id;
                            conn.Insert(fleetService);
                        }
                    }

                    predicateGroup = new PredicateGroup
                        {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                    predicateGroup.Predicates.Add(Predicates.Field<FleetAssignment>(s => s.FleetID, Operator.Eq, id));
                    IList<FleetAssignment> assignments = conn.GetList<FleetAssignment>(predicateGroup).ToList();
                    foreach (var fleetAssignment in assignments)
                    {
                        conn.Delete(fleetAssignment);
                    }

                    if (fleetAssignments != null)
                    {
                        foreach (var fleetAssignment in fleetAssignments)
                        {
                            fleetAssignment.FleetID = id;
                            conn.Insert(fleetAssignment);
                        }
                    }

                    var isEdited = conn.Update(fleet);
                    transactionScope.Complete();
                    return isEdited;
                }
            }
        }

        /// <summary>
        /// Active/Inactive Fleet status
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public bool ToggleFleetStatus(int fleetId, bool tobeEnabled)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                var fleet = conn.Get<Fleet>(fleetId);
                fleet.Active = tobeEnabled;
                fleet.DateUpdated = DateTime.Now;
                var isUpdated = conn.Update(fleet);
                return isUpdated;
            }
        }

        /// <summary>
        /// Get Fleet Detail by FleetID
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns></returns>
        public FleetDTO GetFleetDetail(int fleetId)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                var fleet = conn.Get<Fleet>(fleetId);
                IList<Fleet> p = new List<Fleet>();
                p.Add(fleet);

                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<FleetService>(s => s.FleetID, Operator.Eq, fleetId));
                IList<FleetService> services = conn.GetList<FleetService>(predicateGroup).ToList();

                predicateGroup = new PredicateGroup {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<FleetAssignment>(s => s.FleetID, Operator.Eq, fleetId));
                IList<FleetAssignment> assignments = conn.GetList<FleetAssignment>(predicateGroup).ToList();

                IList<FleetStatus> fleetstatus = conn.GetList<FleetStatus>().ToList();
                IList<Employee> employees = conn.GetList<Employee>().ToList();

                return new FleetDTO
                {
                    Fleets = p,
                    FleetServices = services,
                    FleetAssignments = assignments,
                    FleetStatus = fleetstatus.OrderBy(q => q.StatusName).ToList(),
                    Employes = employees.OrderBy(q => q.FirstName).ToList()
                };
            }
        }

        /// <summary>
        /// Get Fleet Details
        /// </summary>
        /// <returns></returns>
        public FleetDTO GetFleetDetails()
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                IList<Fleet> fleets = conn.GetList<Fleet>().ToList();
                IList<FleetStatus> fleetStatuses = conn.GetList<FleetStatus>().ToList();
                IList<FleetService> services = conn.GetList<FleetService>().ToList();
                IList<FleetAssignment> assignments = conn.GetList<FleetAssignment>().ToList();
                IList<Employee> employees = conn.GetList<Employee>().ToList();

                return new FleetDTO
                {
                    Fleets = fleets,
                    FleetServices = services,
                    FleetAssignments = assignments,
                    FleetStatus = fleetStatuses.OrderBy(q => q.StatusName).ToList(),
                    Employes = employees.OrderBy(q => q.FirstName).ToList()
                };
            }
        }
    }
}

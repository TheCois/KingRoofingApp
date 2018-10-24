using DapperExtensions;
using KRF.Core.DTO.Master;
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
    public class EquipmentManagement : IEquipmentManagement
    {
        private string _connectionString;

        /// <summary>
        /// Constructor
        /// </summary>
        public EquipmentManagement()
        {
            _connectionString = Convert.ToString(ConfigurationManager.AppSettings["ApplicationDSN"]);
        }
        /// <summary>
        /// Create Equipment
        /// </summary>
        /// <param name="equipment"></param>
        /// <returns></returns>
        public int CreateEquipment(Equipment equipment)
        {
            using (var transactionScope = new TransactionScope())
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    equipment.Active = true;
                    equipment.DateCreated = DateTime.Now;
                    var id = sqlConnection.Insert<Equipment>(equipment);
                    transactionScope.Complete();
                    return id;
                }
            }
        }
        /// <summary>
        /// Edit Equipment
        /// </summary>
        /// <param name="equipment"></param>
        /// <returns></returns>
        public bool EditEquipment(Equipment equipment)
        {
            using (var transactionScope = new TransactionScope())
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    equipment.DateUpdated = DateTime.Now;
                    var isEdited = sqlConnection.Update<Equipment>(equipment);
                    transactionScope.Complete();
                    return isEdited;
                }
            }
        }
        /// <summary>
        /// Active/Inactive Equipment status
        /// </summary>
        /// <param name="equipmentID"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public bool ToggleEquipmentStatus(int equipmentID, bool tobeEnabled)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                sqlConnection.Open();
                Equipment Equipment = sqlConnection.Get<Equipment>(equipmentID);
                Equipment.Active = tobeEnabled;
                Equipment.DateUpdated = DateTime.Now;
                var isUpdated = sqlConnection.Update<Equipment>(Equipment);
                return isUpdated;
            }
        }
        /// <summary>
        /// Get Equipment by EquipmentID
        /// </summary>
        /// <param name="equipmentID"></param>
        /// <returns></returns>
        public EquipmentDTO GetEquipment(int equipmentID)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                //predicateGroup.Predicates.Add(Predicates.Field<Equipment>(s => s.EquipmentID, Operator.Eq, equipmentID));
                sqlConnection.Open();
                Equipment equpment = sqlConnection.Get<Equipment>(equipmentID);
                IList<EquipmentStatus> equipmentStatus = sqlConnection.GetList<EquipmentStatus>().ToList();
                IList<Equipment> p = new List<Equipment>();
                p.Add(equpment);
                return new EquipmentDTO
                {
                    Equipments = p,
                    EquipmentStatus = equipmentStatus
                }; ;
            }
        }
        /// <summary>
        /// Get all Equipments
        /// </summary>
        /// <returns></returns>
        public EquipmentDTO GetEquipments()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<Equipment>(s => s.Active, Operator.Eq, true));
                sqlConnection.Open();
                IList<Equipment> equipments = sqlConnection.GetList<Equipment>(predicateGroup).ToList();
                IList<EquipmentStatus> equipmentStatus = sqlConnection.GetList<EquipmentStatus>().ToList();
                return new EquipmentDTO
                {
                    Equipments = equipments,
                    EquipmentStatus = equipmentStatus
                }; ;
            }
        }
    }
}

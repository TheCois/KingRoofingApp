using DapperExtensions;
using KRF.Core.DTO.Master;
using KRF.Core.Entities.Master;
using KRF.Core.Entities.ValueList;
using KRF.Core.FunctionalContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class EquipmentManagement : IEquipmentManagement
    {
        /// <summary>
        /// Create Equipment
        /// </summary>
        /// <param name="equipment"></param>
        /// <returns></returns>
        public int CreateEquipment(Equipment equipment)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    equipment.Active = true;
                    equipment.DateCreated = DateTime.Now;
                    var id = conn.Insert(equipment);
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
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    equipment.DateUpdated = DateTime.Now;
                    var isEdited = conn.Update(equipment);
                    transactionScope.Complete();
                    return isEdited;
                }
            }
        }

        /// <summary>
        /// Active/Inactive Equipment status
        /// </summary>
        /// <param name="equipmentId"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public bool ToggleEquipmentStatus(int equipmentId, bool tobeEnabled)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                conn.Open();
                var equipment = conn.Get<Equipment>(equipmentId);
                equipment.Active = tobeEnabled;
                equipment.DateUpdated = DateTime.Now;
                var isUpdated = conn.Update(equipment);
                return isUpdated;
            }
        }

        /// <summary>
        /// Get Equipment by EquipmentID
        /// </summary>
        /// <param name="equipmentId"></param>
        /// <returns></returns>
        public EquipmentDTO GetEquipment(int equipmentId)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                //predicateGroup.Predicates.Add(Predicates.Field<Equipment>(s => s.EquipmentID, Operator.Eq, equipmentID));
                conn.Open();
                var equipment = conn.Get<Equipment>(equipmentId);
                IList<EquipmentStatus> equipmentStatus = conn.GetList<EquipmentStatus>().ToList();
                IList<Equipment> p = new List<Equipment>();
                p.Add(equipment);
                return new EquipmentDTO
                {
                    Equipments = p,
                    EquipmentStatus = equipmentStatus
                };
            }
        }

        /// <summary>
        /// Get all Equipments
        /// </summary>
        /// <returns></returns>
        public EquipmentDTO GetEquipments()
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<Equipment>(s => s.Active, Operator.Eq, true));
                conn.Open();
                IList<Equipment> equipments = conn.GetList<Equipment>(predicateGroup).ToList();
                IList<EquipmentStatus> equipmentStatus = conn.GetList<EquipmentStatus>().ToList();
                return new EquipmentDTO
                {
                    Equipments = equipments,
                    EquipmentStatus = equipmentStatus
                };
            }
        }
    }
}
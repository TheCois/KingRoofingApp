using DapperExtensions;
using KRF.Core.DTO.Master;
using KRF.Core.Entities.Master;
using KRF.Core.FunctionalContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class VendorManagement : IVendorManagement
    {
        /// <summary>
        /// Create Vendor
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        public int CreateVendor(Vendor vendor)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    vendor.DateCreated = DateTime.Now;
                    vendor.Active = true;
                    var id = conn.Insert(vendor);

                    transactionScope.Complete();
                    return id;
                }
            }
        }

        /// <summary>
        /// Edit Vendor detail
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        public bool EditVendor(Vendor vendor)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    vendor.DateUpdated = DateTime.Now;
                    var isEdited = conn.Update(vendor);
                    transactionScope.Complete();
                    return isEdited;
                }
            }
        }

        /// <summary>
        /// Toggle vendor active field
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public bool SetActiveInactiveVendor(int vendorId, bool active)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                Vendor vendor = conn.Get<Vendor>(vendorId);
                vendor.Active = active;
                vendor.DateUpdated = DateTime.Now;
                var isUpdated = conn.Update(vendor);
                return isUpdated;
            }
        }

        /// <summary>
        /// Get Vendor by vendorID
        /// </summary>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        public VendorDTO GetVendor(int vendorId)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                Vendor vendor = conn.Get<Vendor>(vendorId);
                IList<Vendor> p = new List<Vendor>();
                p.Add(vendor);
                return new VendorDTO
                {
                    Vendors = p
                };
            }
        }

        /// <summary>
        /// Get all Vendors
        /// </summary>
        /// <returns></returns>
        public VendorDTO ListAllVendors()
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                //var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                //predicateGroup.Predicates.Add(Predicates.Field<Vendor>(s => s.Active, Operator.Eq, true));

                conn.Open();
                IList<Vendor> vendors = conn.GetList<Vendor>().ToList();
                return new VendorDTO
                {
                    Vendors = vendors
                };
            }
        }
    }
}
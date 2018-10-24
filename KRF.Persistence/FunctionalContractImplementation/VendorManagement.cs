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
    public class VendorManagement : IVendorManagement
    {
        private string _connectionString;

        /// <summary>
        /// Constructor
        /// </summary>
        public VendorManagement()
        {
            _connectionString = Convert.ToString(ConfigurationManager.AppSettings["ApplicationDSN"]);
        }
        /// <summary>
        /// Create Vendor
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        public int CreateVendor(Vendor vendor)
        {
            using (var transactionScope = new TransactionScope())
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    vendor.DateCreated = DateTime.Now;
                    vendor.Active = true;
                    var id = sqlConnection.Insert<Vendor>(vendor);

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
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    vendor.DateUpdated = DateTime.Now;
                    var isEdited = sqlConnection.Update<Vendor>(vendor);
                    transactionScope.Complete();
                    return isEdited;
                }
            }
        }
        /// <summary>
        /// Toggle vendor active field
        /// </summary>
        /// <param name="vendorID"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public bool SetActiveInactiveVendor(int vendorID, bool active)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                Vendor vendor = sqlConnection.Get<Vendor>(vendorID);
                vendor.Active = active;
                vendor.DateUpdated = DateTime.Now;
                var isUpdated = sqlConnection.Update<Vendor>(vendor);
                return isUpdated;
            }
        }
        /// <summary>
        /// Get Vendor by vendorID
        /// </summary>
        /// <param name="vendorID"></param>
        /// <returns></returns>
        public VendorDTO GetVendor(int vendorID)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                Vendor vendor = sqlConnection.Get<Vendor>(vendorID);
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
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                //var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                //predicateGroup.Predicates.Add(Predicates.Field<Vendor>(s => s.Active, Operator.Eq, true));
                
                sqlConnection.Open();
                IList<Vendor> vendords = sqlConnection.GetList<Vendor>().ToList();
                return new VendorDTO
                {
                    Vendors = vendords
                };
            }
        }
    }
}

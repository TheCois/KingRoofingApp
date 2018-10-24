using DapperExtensions;
using KRF.Core.DTO.Master;
using KRF.Core.Entities.AccessControl;
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
    public class RolePermissionManagement : IRolePermissionManagement
    {
        private string _connectionString;

        /// <summary>
        /// Constructor
        /// </summary>
        public RolePermissionManagement()
        {
            _connectionString = Convert.ToString(ConfigurationManager.AppSettings["ApplicationDSN"]);
        }
        /// <summary>
        /// Get role permissions by roleID
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public RolePermissionDTO GetRolePermissionDetail(int roleID)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<Role>(s => s.Active, Operator.Eq, true));
                IList<Role> roles = sqlConnection.GetList<Role>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<Pages>(s => s.Active, Operator.Eq, true));
                IList<Pages> pages = sqlConnection.GetList<Pages>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<Permissions>(s => s.Active, Operator.Eq, true));
                IList<Permissions> permissions = sqlConnection.GetList<Permissions>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<RolePermission>(s => s.RoleID, Operator.Eq, roleID));
                IList<RolePermission> rolePermissions = sqlConnection.GetList<RolePermission>(predicateGroup).ToList();

                return new RolePermissionDTO
                {
                    Roles = roles.OrderBy(p=>p.RoleName).ToList(),
                    Pages = pages,
                    Permissions = permissions,
                    RolePermissions = rolePermissions
                };
            }
        }

        /// <summary>
        /// Save Role Permission
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="rolePermissions"></param>
        /// <returns></returns>
        public bool SaveRolePermissions(int roleID, List<RolePermission> rolePermissions)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    predicateGroup.Predicates.Add(Predicates.Field<RolePermission>(s => s.RoleID, Operator.Eq, roleID));
                    IList<RolePermission> currentRolePermissions = sqlConnection.GetList<RolePermission>(predicateGroup).ToList();
                    foreach (RolePermission rolePermission in currentRolePermissions)
                    {
                        sqlConnection.Delete(rolePermission);
                    }
                    if(rolePermissions != null && rolePermissions.Count > 0)
                    {
                        foreach (RolePermission rolePermission in rolePermissions)
                        {
                            sqlConnection.Insert<RolePermission>(rolePermission);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// Get Role Permission by roleID and pageID
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="pageID"></param>
        /// <returns></returns>
        public List<RolePermission> GetRolePermission(int roleID, int pageID)
        {
            List<RolePermission> rolePermissions = new List<RolePermission>();
            try
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    predicateGroup.Predicates.Add(Predicates.Field<RolePermission>(s => s.RoleID, Operator.Eq, roleID));
                    predicateGroup.Predicates.Add(Predicates.Field<RolePermission>(s => s.PageID, Operator.Eq, pageID));
                    rolePermissions = sqlConnection.GetList<RolePermission>(predicateGroup).ToList();
                }
                return rolePermissions;
            }
            catch (Exception ex)
            {
                return rolePermissions;
            }
        }
        /// <summary>
        /// Get Page List
        /// </summary>
        /// <returns></returns>
        public List<Pages> GetPages()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<Pages>(s => s.Active, Operator.Eq, true));
                return sqlConnection.GetList<Pages>(predicateGroup).ToList();
            }
        }
    }
}

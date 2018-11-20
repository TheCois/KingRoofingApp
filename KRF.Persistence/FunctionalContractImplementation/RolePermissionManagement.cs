using DapperExtensions;
using KRF.Core.DTO.Master;
using KRF.Core.Entities.AccessControl;
using KRF.Core.FunctionalContracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class RolePermissionManagement : IRolePermissionManagement
    {
        /// <summary>
        /// Get role permissions by roleID
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public RolePermissionDTO GetRolePermissionDetail(int roleId)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<Role>(s => s.Active, Operator.Eq, true));
                IList<Role> roles = conn.GetList<Role>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<Pages>(s => s.Active, Operator.Eq, true));
                IList<Pages> pages = conn.GetList<Pages>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<Permissions>(s => s.Active, Operator.Eq, true));
                IList<Permissions> permissions = conn.GetList<Permissions>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<RolePermission>(s => s.RoleID, Operator.Eq, roleId));
                IList<RolePermission> rolePermissions = conn.GetList<RolePermission>(predicateGroup).ToList();

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
        /// <param name="roleId"></param>
        /// <param name="rolePermissions"></param>
        /// <returns></returns>
        public bool SaveRolePermissions(int roleId, List<RolePermission> rolePermissions)
        {
            try
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    predicateGroup.Predicates.Add(Predicates.Field<RolePermission>(s => s.RoleID, Operator.Eq, roleId));
                    IList<RolePermission> currentRolePermissions = conn.GetList<RolePermission>(predicateGroup).ToList();
                    foreach (var rolePermission in currentRolePermissions)
                    {
                        conn.Delete(rolePermission);
                    }
                    if(rolePermissions != null && rolePermissions.Count > 0)
                    {
                        foreach (var rolePermission in rolePermissions)
                        {
                            conn.Insert(rolePermission);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
        /// <summary>
        /// Get Role Permission by roleID and pageID
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public List<RolePermission> GetRolePermission(int roleId, int pageId)
        {
            var rolePermissions = new List<RolePermission>();
            try
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    predicateGroup.Predicates.Add(Predicates.Field<RolePermission>(s => s.RoleID, Operator.Eq, roleId));
                    predicateGroup.Predicates.Add(Predicates.Field<RolePermission>(s => s.PageID, Operator.Eq, pageId));
                    rolePermissions = conn.GetList<RolePermission>(predicateGroup).ToList();
                }
                return rolePermissions;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return rolePermissions;
            }
        }
        /// <summary>
        /// Get Page List
        /// </summary>
        /// <returns></returns>
        public List<Pages> GetPages()
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<Pages>(s => s.Active, Operator.Eq, true));
                return conn.GetList<Pages>(predicateGroup).ToList();
            }
        }
    }
}

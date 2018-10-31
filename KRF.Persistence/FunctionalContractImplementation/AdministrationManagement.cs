using KRF.Core.FunctionalContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using DapperExtensions;
using KRF.Core.Entities.ValueList;
using KRF.Core.Enums;
using Dapper;
using KRF.Core.Entities.MISC;


namespace KRF.Persistence.FunctionalContractImplementation
{
    public class AdministrationDML
    {
        public int type { get; set; }
        public string TableName { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
        public string Field4 { get; set; }
    }

    public class AdministrationManagement : IAdministrationManagement
    {
        private List<AdministrationDML> administrationDmLs_;

        /// <summary>
        /// Constructor
        /// </summary>
        public AdministrationManagement()
        {
            ConstructDMLClass();
        }

        private void ConstructDMLClass()
        {
            administrationDmLs_ = new List<AdministrationDML>();
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                try
                {
                    const string query = "Select ID as type, TableName, Field1, Field2, Field3, Field4 FROM AdministrationType";
                    administrationDmLs_ = conn.Query<AdministrationDML>(query).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// Create Master Record by Type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="description"></param>
        /// <param name="extraField1"></param>
        /// <returns></returns>
        public int Create(int type, string description, string extraField1 = "")
        {
            return InsertMasterRecord(type, description, extraField1);
        }

        /// <summary>
        /// Edit Master Record by Type and ID
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="description"></param>
        /// <param name="active"></param>
        /// <param name="extraField1"></param>
        /// <returns></returns>
        public bool Edit(int type, int id, string description, bool active, string extraField1 = "")
        {
            return UpdateMasterRecord(type, id, active, description, extraField1);
        }

        /// <summary>
        /// Set Active/Inactive master record by type and ID
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public bool SetActiveInactive(int type, int id, bool active)
        {
            return UpdateMasterRecord(type, id, active);
        }

        /// <summary>
        /// Get Master Records by Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<MasterRecords> GetMasterRecordsByType(int type)
        {
            return GetMasterRecordsByTypeId(type);
        }

        /// <summary>
        /// Get Master Records by Type and ID
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public MasterRecords GetMasterRecordsByTypeAndID(int type, int id)
        {
            var masterRecords = GetMasterRecordsByTypeId(type, id);
            return masterRecords.FirstOrDefault();
        }

        private List<MasterRecords> GetMasterRecordsByTypeId(int type, int id = 0)
        {
            var masterRecords = new List<MasterRecords>();
            var administrationDml = administrationDmLs_.FirstOrDefault(p => p.type == type);
            if (administrationDml == null)
            {
                Console.WriteLine("No administration record matches type " + type);
                return masterRecords;
            }
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                try
                {
                    string query;
                    if (type == (int) AdministrationTypes.State)
                        query = id <= 0
                            ? "Select ID, Description, Abbreviation as ExtraField1, Active from State"
                            : "Select ID, Description, Abbreviation as ExtraField1, Active from State WHERE ID = " +
                              id;
                    else
                        query = id <= 0
                            ? "Select " + administrationDml.Field1 + " as ID, " + administrationDml.Field2 +
                              " as Description, " + administrationDml.Field3 + " as Active from " +
                              administrationDml.TableName 
                            : "Select " + administrationDml.Field1 + " as ID, " + administrationDml.Field2 +
                              " as Description, " + administrationDml.Field3 + " from " + administrationDml.TableName +
                              " WHERE " + administrationDml.Field1 + " = " + id;

                    masterRecords = conn.Query<MasterRecords>(query).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                return masterRecords;
            }
        }

        private bool UpdateMasterRecord(int type, int id, bool active, string description = "", string extraField1 = "")
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                try
                {
                    var administrationDml = administrationDmLs_.FirstOrDefault(p => p.type == type);
                    if (administrationDml == null)
                    {
                        Console.WriteLine("No administration record matches type " + type);
                        return false;
                    }
                    var query = "UPDATE " + administrationDml.TableName + " SET " + administrationDml.Field3 +
                                   " = @active " +
                                   (!string.IsNullOrEmpty(description)
                                       ? "," + administrationDml.Field2 + " = @description"
                                       : "") + " " +
                                   (!string.IsNullOrEmpty(extraField1)
                                       ? "," + administrationDml.Field4 + " = @extraField1"
                                       : "") + "  WHERE " + administrationDml.Field1 + " = @id";

                    if (string.IsNullOrEmpty(description))
                    {
                        conn.Execute(query, new {active, id});
                    }
                    else
                    {
                        if (type == (int) AdministrationTypes.State)
                        {
                            conn.Execute(query, new {active, id, description, extraField1});
                        }
                        else
                        {
                            conn.Execute(query, new {active, id, description});
                        }
                    }

                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }
        }

        /// <summary>
        /// Get administration types
        /// </summary>
        /// <returns></returns>
        public List<AdministrationType> GetAdministrationType()
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                return conn.GetList<AdministrationType>().OrderBy(p => p.Description).ToList();
            }
        }

        private int InsertMasterRecord(int type, string description, string extraField1 = "")
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                try
                {
                    var administrationDml = administrationDmLs_.FirstOrDefault(p => p.type == type);
                    if (administrationDml == null)
                    {
                        Console.WriteLine("No administration");
                        return 0;
                    }
                    string query;
                    if (type == (int) AdministrationTypes.State)
                        query =
                            "INSERT State (Description, Abbreviation, Active) VALUES (@description, @extraField1, @active)";
                    else
                        query = "INSERT " + administrationDml.TableName + " (" + administrationDml.Field2 + ", " +
                                administrationDml.Field3 + ") VALUES (@description, @active)";

                    var result = 0;
                    result = string.IsNullOrEmpty(extraField1)
                        ? conn.Execute(query, new {description, active = true})
                        : conn.Execute(query, new {description, extraField1, active = true});

                    return result;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return 0;
                }
            }
        }

    }
}

using KRF.Common;
using KRF.Core.FunctionalContracts;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions;
using KRF.Core.Entities.ValueList;
using System.Data;
using System.Transactions;
using System.Configuration;
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
        private string _connectionString;
        private List<AdministrationDML> administrationDMLs;
        /// <summary>
        /// Constructor
        /// </summary>
        public AdministrationManagement()
        {
            _connectionString = Convert.ToString(ConfigurationManager.AppSettings["ApplicationDSN"]);
            ConstructDMLClass();
        }

        private void ConstructDMLClass()
        {
            administrationDMLs = new List<AdministrationDML>();
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                try
                {
                    string query = "Select ID as type, TableName, Field1, Field2, Field3, Field4 FROM [AdministrationType]";
                    administrationDMLs = sqlConnection.Query<AdministrationDML>(query).ToList();
                }
                catch (Exception e)
                {

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
            List<MasterRecords> masterRecords = GetMasterRecordsByTypeId(type, id);
            return masterRecords.FirstOrDefault();
        }

        private List<MasterRecords> GetMasterRecordsByTypeId(int type, int id = 0)
        {
            List<MasterRecords> masterRecords = new List<MasterRecords>();
            string query = string.Empty;
            AdministrationDML administrationDML = administrationDMLs.Where(p => p.type == type).FirstOrDefault();
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                try
                {
                    if (type == (int)AdministrationTypes.State)
                        query = id <= 0 ? "Select ID, Description, Abbreviation as ExtraField1, Active from [State]" : "Select ID, Description, Abbreviation as ExtraField1, Active from [State] WHERE ID = " + id.ToString();
                    else
                        query = id <= 0 ? "Select " + administrationDML.Field1 + " as ID, " + administrationDML.Field2 + " as Description, " + administrationDML.Field3 + " as Active from [" + administrationDML.TableName + "]" : "Select " + administrationDML.Field1 + " as ID, " + administrationDML.Field2 + " as Description, " + administrationDML.Field3 + " from [" + administrationDML.TableName + "] WHERE " + administrationDML.Field1 + " = " + id.ToString();

                    masterRecords = sqlConnection.Query<MasterRecords>(query).ToList();
                }
                catch (Exception e)
                {

                }
                return masterRecords;
            }
        }

        private bool UpdateMasterRecord(int type, int id, bool active, string description = "", string extraField1 = "")
        {
            string query = string.Empty;
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                try
                {
                    AdministrationDML administrationDML = administrationDMLs.FirstOrDefault(p => p.type == type);
                    query = "UPDATE [" + administrationDML.TableName + "] SET " + administrationDML.Field3 + " = @active " + (!string.IsNullOrEmpty(description) ? "," + administrationDML.Field2 + " = @description" : "") + " " + (!string.IsNullOrEmpty(extraField1) ? "," + administrationDML.Field4 + " = @extraField1" : "") + "  WHERE " + administrationDML.Field1 + " = @id";

                    if (string.IsNullOrEmpty(description))
                    {
                        var result = sqlConnection.Execute(query, new { active, id });
                    }
                    else
                    {
                        if (type == (int)AdministrationTypes.State)
                        {
                            var result = sqlConnection.Execute(query, new { active, id, description, extraField1 });
                        }
                        else
                        {
                            var result = sqlConnection.Execute(query, new { active, id, description });
                        }
                    }

                    return true;
                }
                catch (Exception e)
                {
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
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                return sqlConnection.GetList<AdministrationType>().OrderBy(p => p.Description).ToList();
            }
        }

        private int InsertMasterRecord(int type, string description, string extraField1 = "")
        {
            string query = string.Empty;
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                try
                {
                    AdministrationDML administrationDML = administrationDMLs.FirstOrDefault(p => p.type == type);
                    if (type == (int)AdministrationTypes.State)
                        query = "INSERT [State] (Description, Abbreviation, Active) VALUES (@description, @extraField1, @active)";
                    else
                        query = "INSERT [" + administrationDML.TableName + "] (" + administrationDML.Field2 + ", " + administrationDML.Field3 + ") VALUES (@description, @active)";

                    int result = 0;
                    if (string.IsNullOrEmpty(extraField1))
                    {
                        result = sqlConnection.Execute(query, new { description, active = true });
                    }
                    else
                    {
                        result = sqlConnection.Execute(query, new { description, extraField1, active = true });
                    }

                    return result;
                }
                catch (Exception e)
                {
                    return 0;
                }
            }
        }

    }
}

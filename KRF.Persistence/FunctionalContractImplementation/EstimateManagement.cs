using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KRF.Core.FunctionalContracts;
using KRF.Core.Repository;
using KRF.Core.Entities.Product;
using StructureMap;
using KRF.Core.Entities.ValueList;
using KRF.Core.DTO.Product;
using KRF.Core.Entities.Sales;
using System.Configuration;
using KRF.Core.DTO.Sales;
using System.Data.SqlClient;
using Dapper;
using DapperExtensions;
using System.Transactions;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using KRF.Core.Entities.Employee;
using System.Globalization;
using KRF.Core.Enums;

namespace KRF.Persistence.RepositoryImplementation
{
    public class EstimateManagement : IEstimateManagement
    {
        private readonly string _connectionString;
        /// <summary>
        /// Constructor
        /// </summary>
        public EstimateManagement()
        {
            _connectionString = Convert.ToString(ConfigurationManager.AppSettings["ApplicationDSN"]);
        }

        public int Create(Estimate estimate, IList<EstimateItem> estimateItem)
        {
            using (var transactionScope = new TransactionScope())
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    estimate.CreatedDate = DateTime.Now;
                    var ID = sqlConnection.Insert<Estimate>(estimate);

                    foreach (var c in estimateItem)
                    {
                        c.EstimateID = ID;
                        sqlConnection.Insert<EstimateItem>(c);
                    }
                    if (estimate.Status == (int)KRF.Core.Enums.EstimateStatusType.Complete)
                    {
                        DeleteQuantityInInventory(sqlConnection, estimateItem.ToList(), ID);
                    }
                    transactionScope.Complete();
                    return ID;
                }
            }
        }

        public Estimate Edit(Estimate estimate, IList<EstimateItem> estimateItem)
        {
            using (var transactionScope = new TransactionScope())
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    var curEstimate = sqlConnection.Get<Estimate>(estimate.ID);
                    if (estimate.CreatedDate == DateTime.MinValue)
                    {
                        estimate.CreatedDate = DateTime.Now;
                    }
                    var isEdited = sqlConnection.Update<Estimate>(estimate);

                    var pgEstimate = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    pgEstimate.Predicates.Add(Predicates.Field<EstimateItem>(s => s.EstimateID, Operator.Eq, estimate.ID));
                    
                    var curEstimateList = sqlConnection.GetList<EstimateItem>(pgEstimate).ToList();
                    if (curEstimate.Status != (int)KRF.Core.Enums.EstimateStatusType.Complete && estimate.Status == (int)KRF.Core.Enums.EstimateStatusType.Complete)
                    {
                        DeleteQuantityInInventory(sqlConnection, estimateItem.ToList(), estimate.ID);
                    }
                    else if (curEstimate.Status == (int)KRF.Core.Enums.EstimateStatusType.Complete && estimate.Status == (int)KRF.Core.Enums.EstimateStatusType.Complete)
                    {
                        foreach (var c in estimateItem)
                        {
                            if (!curEstimateList.Any(p => p.ItemAssemblyID == c.ItemAssemblyID))
                            {
                                List<EstimateItem> qtyToBeDelete = new List<EstimateItem>();
                                qtyToBeDelete.Add(c);
                                DeleteQuantityInInventory(sqlConnection, qtyToBeDelete, estimate.ID);
                            }
                            else
                            {
                                EstimateItem curEst = curEstimateList.FirstOrDefault(p => p.ItemAssemblyID == c.ItemAssemblyID);
                                if (curEst != null)
                                {
                                    if (curEst.Quantity < c.Quantity)
                                    {
                                        decimal qty = c.Quantity - curEst.Quantity;
                                        DeleteQuantityInInventory(sqlConnection, c.ItemAssemblyID, qty, estimate.ID);
                                    }
                                    else
                                    {
                                        decimal qty = curEst.Quantity - c.Quantity;
                                        AddQuantityInInventory(sqlConnection, c.ItemAssemblyID, qty, estimate.ID);
                                    }
                                }
                            }
                        }
                        foreach (var c1 in curEstimateList)
                        {
                            if (!estimateItem.Any(p => p.ItemAssemblyID == c1.ItemAssemblyID))
                            {
                                List<EstimateItem> qtyToBeDelete = new List<EstimateItem>();
                                qtyToBeDelete.Add(c1);
                                AddQuantityInInventory(sqlConnection, qtyToBeDelete, estimate.ID);
                            }
                        }
                    }
                    
                    foreach (var c1 in curEstimateList)
                    {
                        sqlConnection.Delete<EstimateItem>(c1);
                    }

                    foreach (var c in estimateItem)
                    {
                        c.EstimateID = estimate.ID;
                        sqlConnection.Insert<EstimateItem>(c);
                    }

                    transactionScope.Complete();
                    return estimate;
                }
            }
        }

        public bool Delete(int estimateId)
        {
            var isDeleted = false;
            using (var transactionScope = new TransactionScope())
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    predicateGroup.Predicates.Add(Predicates.Field<Estimate>(s => s.ID, Operator.Eq, estimateId));
                    sqlConnection.Open();
                    var estimate = sqlConnection.Get<Estimate>(estimateId);
                    int estimateStatus = estimate.Status;
                    isDeleted = sqlConnection.Delete<Estimate>(predicateGroup);

                    var predicateGroup1 = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    predicateGroup1.Predicates.Add(Predicates.Field<EstimateItem>(s => s.EstimateID, Operator.Eq, estimateId));
                    if (estimateStatus == (int)KRF.Core.Enums.EstimateStatusType.Complete)
                    {
                        var estimateItems = sqlConnection.GetList<EstimateItem>(predicateGroup1).ToList();
                        AddQuantityInInventory(sqlConnection, estimateItems, estimateId);
                    }
                    isDeleted = sqlConnection.Delete<EstimateItem>(predicateGroup1);
                }
                transactionScope.Complete();
                return isDeleted;
            }
        }

        private void DeleteQuantityInInventory(SqlConnection sqlConnection, int assemblyID, decimal qty, int estimateId)
        {
            var predicateGroupInv = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
            predicateGroupInv.Predicates.Add(Predicates.Field<AssemblyItem>(s => s.AssemblyId, Operator.Eq, assemblyID));
            var assemblyItems = sqlConnection.GetList<AssemblyItem>(predicateGroupInv).ToList();
            foreach (var assemblyItem in assemblyItems)
            {
                predicateGroupInv = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroupInv.Predicates.Add(Predicates.Field<Inventory>(s => s.ItemID, Operator.Eq, assemblyItem.ItemId));
                var inventory = sqlConnection.GetList<Inventory>(predicateGroupInv).FirstOrDefault();
                if (inventory != null)
                {
                    inventory.Qty = inventory.Qty - qty;
                    inventory.Type = "Assigned";
                    inventory.Comment = "Estimate ID : " + Convert.ToString(estimateId);
                    sqlConnection.Update<Inventory>(inventory);
                }
            }
        }
        private void AddQuantityInInventory(SqlConnection sqlConnection, int assemblyID, decimal qty, int estimateId)
        {
            var predicateGroupInv = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
            predicateGroupInv.Predicates.Add(Predicates.Field<AssemblyItem>(s => s.AssemblyId, Operator.Eq, assemblyID));
            var assemblyItems = sqlConnection.GetList<AssemblyItem>(predicateGroupInv).ToList();
            foreach (var assemblyItem in assemblyItems)
            {
                predicateGroupInv = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroupInv.Predicates.Add(Predicates.Field<Inventory>(s => s.ItemID, Operator.Eq, assemblyItem.ItemId));
                var inventory = sqlConnection.GetList<Inventory>(predicateGroupInv).FirstOrDefault();
                if (inventory != null)
                {
                    inventory.Qty = inventory.Qty + qty;
                    inventory.Type = "Assigned";
                    inventory.Comment = "Estimate ID : " + Convert.ToString(estimateId);
                    sqlConnection.Update<Inventory>(inventory);
                }
            }
        }

        private void AddQuantityInInventory(SqlConnection sqlConnection, List<EstimateItem> estimateItems, int estimateId)
        {
            foreach (EstimateItem estimateItem in estimateItems)
            {
                var predicateGroupInv = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroupInv.Predicates.Add(Predicates.Field<AssemblyItem>(s => s.AssemblyId, Operator.Eq, estimateItem.ItemAssemblyID));
                var assemblyItems = sqlConnection.GetList<AssemblyItem>(predicateGroupInv).ToList();
                foreach (var assemblyItem in assemblyItems)
                {
                    predicateGroupInv = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    predicateGroupInv.Predicates.Add(Predicates.Field<Inventory>(s => s.ItemID, Operator.Eq, assemblyItem.ItemId));
                    var inventory = sqlConnection.GetList<Inventory>(predicateGroupInv).FirstOrDefault();
                    if (inventory != null)
                    {
                        inventory.Qty = inventory.Qty + estimateItem.Quantity;
                        inventory.Type = "Assigned";
                        inventory.Comment = "Estimate ID : " + Convert.ToString(estimateId);
                        sqlConnection.Update<Inventory>(inventory);
                    }
                }
            }
        }
        private void DeleteQuantityInInventory(SqlConnection sqlConnection, List<EstimateItem> estimateItems, int estimateId)
        {
            foreach (var c in estimateItems)
            {
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<AssemblyItem>(s => s.AssemblyId, Operator.Eq, c.ItemAssemblyID));
                var assemblyItems = sqlConnection.GetList<AssemblyItem>(predicateGroup).ToList();
                decimal qty = 0;
                foreach (var assemblyItem in assemblyItems)
                {
                    predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    predicateGroup.Predicates.Add(Predicates.Field<Inventory>(s => s.ItemID, Operator.Eq, assemblyItem.ItemId));
                    var inventory = sqlConnection.GetList<Inventory>(predicateGroup).FirstOrDefault();
                    if (inventory != null)
                    {
                        qty = inventory.Qty - c.Quantity;
                        inventory.Qty = qty;
                        inventory.Type = "Assigned";
                        inventory.Comment = "Estimate ID : " + Convert.ToString(estimateId);
                        sqlConnection.Update<Inventory>(inventory);
                    }
                }
            }
        }

        public EstimateDTO ListAll()
        {
            EstimateDTO estimateDTO = new EstimateDTO();
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                estimateDTO.Estimates = sqlConnection.GetList<Estimate>().ToList();
                estimateDTO.EstimateItems = sqlConnection.GetList<EstimateItem>().ToList();
                estimateDTO.Leads = sqlConnection.GetList<Lead>().Where(l => l.LeadStage == (int)LeadStageType.Lead).ToList();
                estimateDTO.Customers = sqlConnection.GetList<Lead>().Where(l => l.LeadStage == (int)LeadStageType.Customer).ToList();
                estimateDTO.CustomerAddress = sqlConnection.GetList<CustomerAddress>().ToList();
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<Status>(s => s.Active, Operator.Eq, true));
                estimateDTO.Status = sqlConnection.GetList<Status>(predicateGroup).OrderBy(p => p.Description).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<RoofType>(s => s.Active, Operator.Eq, true));
                IList<RoofType> roofType = sqlConnection.GetList<RoofType>(predicateGroup).ToList();
                estimateDTO.RoofTypes = roofType.OrderBy(p => p.Description).ToList();
            }
            return estimateDTO;
        }

        public IList<EstimateDTO> ListAllEstimatesForACustomer(int customerId)
        {
            return null;
        }

        public EstimateDTO Select(int estimateId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                EstimateDTO estimateDTO = new EstimateDTO();

                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<EstimateItem>(s => s.EstimateID, Operator.Eq, estimateId));

                sqlConnection.Open();

                var estimate = sqlConnection.Get<Estimate>(estimateId);

                estimateDTO.Estimates = new List<Estimate>();
                estimateDTO.Estimates.Add(estimate);

                estimateDTO.EstimateItems = sqlConnection.GetList<EstimateItem>(predicateGroup).ToList();

                estimateDTO.Leads = sqlConnection.GetList<Lead>().Where(l => l.LeadStage == (int)LeadStageType.Lead).ToList();
                estimateDTO.Customers = sqlConnection.GetList<Lead>().Where(l => l.LeadStage == (int)LeadStageType.Customer).ToList();
                estimateDTO.CustomerAddress = sqlConnection.GetList<CustomerAddress>().ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<Status>(s => s.Active, Operator.Eq, true));
                estimateDTO.Status = sqlConnection.GetList<Status>(predicateGroup).OrderBy(p => p.Description).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<UnitOfMeasure>(s => s.Active, Operator.Eq, true));
                estimateDTO.UnitOfMeasure = sqlConnection.GetList<UnitOfMeasure>(predicateGroup).ToList();

                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<RoofType>(s => s.Active, Operator.Eq, true));
                IList<RoofType> roofType = sqlConnection.GetList<RoofType>(predicateGroup).ToList();
                estimateDTO.RoofTypes = roofType.OrderBy(p => p.Description).ToList();

                estimateDTO.Items = sqlConnection.GetList<Item>().ToList();
                estimateDTO.Assemblies = sqlConnection.GetList<Assembly>().ToList();
                estimateDTO.AssemblyItems = sqlConnection.GetList<AssemblyItem>().ToList();

                return estimateDTO;
            }
        }

        public int SaveDocument(EstimateDocument estimateDocument)
        {
            using (var transactionScope = new TransactionScope())
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();

                    if (estimateDocument.ID > 0)
                    {
                        var curEstimate = sqlConnection.Get<EstimateDocument>(estimateDocument.ID);
                        curEstimate.Description = estimateDocument.Description;

                        var isEdited = sqlConnection.Update<EstimateDocument>(curEstimate);
                    }
                    else
                    {
                        estimateDocument.ID = sqlConnection.Insert<EstimateDocument>(estimateDocument);
                    }

                    transactionScope.Complete();
                    return estimateDocument.ID;
                }
            }
        }

        public bool DeleteEstimateDocument(int estimateDocumentID)
        {
            var isDeleted = false;
            using (var transactionScope = new TransactionScope())
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    predicateGroup.Predicates.Add(Predicates.Field<EstimateDocument>(s => s.ID, Operator.Eq, estimateDocumentID));

                    sqlConnection.Open();
                    isDeleted = sqlConnection.Delete<EstimateDocument>(predicateGroup);
                }

                transactionScope.Complete();
                return isDeleted;
            }
        }

        public EstimateDocument GetEstimateDocument(int estimateDocumentID)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var estimateDocument = sqlConnection.Get<EstimateDocument>(estimateDocumentID);
                return estimateDocument;
            }
        }

        public EstimateDocument GetEstimateDocumentByType(int type)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<EstimateDocument>(s => s.Type, Operator.Eq, type));

                EstimateDocument estimateDocument = sqlConnection.GetList<EstimateDocument>(predicateGroup).First();
                return estimateDocument;
            }
        }

        public IList<EstimateDocument> GetEstimateDocuments(int estimateID)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                const string query = "select ID, EstimateID, Name, Type, Description, UploadDateTime, Null as Tesxt from EstimateDocument " +
                             "WHERE EstimateID = @EstimateID";

                var estimateDocuments = sqlConnection.Query<EstimateDocument>(query,
                       new { EstimateID = estimateID }).ToList();
                return estimateDocuments;
            }
        }

        public string NumbersToWords(double inputNumber)
        {
            int inputNo = Convert.ToInt32(inputNumber);

            if (inputNo == 0)
                return "Zero";

            int[] numbers = new int[4];
            int first = 0;
            int u, h, t;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (inputNo < 0)
            {
                sb.Append("Minus ");
                inputNo = -inputNo;
            }

            string[] words0 = {"" ,"One ", "Two ", "Three ", "Four ",
            "Five " ,"Six ", "Seven ", "Eight ", "Nine "};
            string[] words1 = {"Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ",
            "Fifteen ","Sixteen ","Seventeen ","Eighteen ", "Nineteen "};
            string[] words2 = {"Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ",
            "Seventy ","Eighty ", "Ninety "};
            string[] words3 = { "Thousand ", "Lakh ", "Crore " };

            numbers[0] = inputNo % 1000; // units
            numbers[1] = inputNo / 1000;
            numbers[2] = inputNo / 100000;
            numbers[1] = numbers[1] - 100 * numbers[2]; // thousands
            numbers[3] = inputNo / 10000000; // crores
            numbers[2] = numbers[2] - 100 * numbers[3]; // lakhs

            for (int i = 3; i > 0; i--)
            {
                if (numbers[i] != 0)
                {
                    first = i;
                    break;
                }
            }
            for (int i = first; i >= 0; i--)
            {
                if (numbers[i] == 0) continue;
                u = numbers[i] % 10; // ones
                t = numbers[i] / 10;
                h = numbers[i] / 100; // hundreds
                t = t - 10 * h; // tens
                if (h > 0) sb.Append(words0[h] + "Hundred ");
                if (u > 0 || t > 0)
                {
                    if (h > 0 || i == 0) sb.Append("and ");
                    if (t == 0)
                        sb.Append(words0[u]);
                    else if (t == 1)
                        sb.Append(words1[u]);
                    else
                        sb.Append(words2[t - 2] + words0[u]);
                }
                if (i != 0) sb.Append(words3[i - 1]);
            }
            return sb.ToString().TrimEnd();
        }

        public string ConvertNumbertoWords(int number)
        {
            if (number == 0)
                return "ZERO";
            if (number < 0)
                return "minus " + ConvertNumbertoWords(Math.Abs(number));
            string words = "";
            if ((number / 1000000) > 0)
            {
                words += ConvertNumbertoWords(number / 1000000) + " MILLION ";
                number %= 1000000;
            }
            if ((number / 1000) > 0)
            {
                words += ConvertNumbertoWords(number / 1000) + " THOUSAND ";
                number %= 1000;
            }
            if ((number / 100) > 0)
            {
                words += ConvertNumbertoWords(number / 100) + " HUNDRED ";
                number %= 100;
            }
            if (number > 0)
            {
                if (words != "")
                    words += "AND ";
                var unitsMap = new[] { "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN" };
                var tensMap = new[] { "ZERO", "TEN", "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += " " + unitsMap[number % 10];
                }
            }
            return words;
        }

        public byte[] CreateProposalDocument(int estimateID)
        {
            Estimate estimate = null;
            CustomerAddress customerAddress = null;
            Lead lead = null;
            Employee employee = null;

            IList<City> cities = null;
            IList<State> states = null;
            List<Assembly> assemblies = null;

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                estimate = sqlConnection.Get<Estimate>(estimateID);
                var leadID = estimate.LeadID;

                lead = sqlConnection.Get<Lead>(leadID);
                customerAddress = sqlConnection.Get<CustomerAddress>(estimate.JobAddressID);
                employee = sqlConnection.Get<Employee>(lead.AssignedTo);

                states = sqlConnection.GetList<State>().ToList();
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<City>(s => s.Active, Operator.Eq, true));
                cities = sqlConnection.GetList<City>(predicateGroup).ToList();

                var lookup = new Dictionary<int, Assembly>();
                sqlConnection.Query<Assembly, EstimateItem, Assembly>(@"
                SELECT i.*, a.*
                FROM Assembly i
                INNER JOIN EstimateItem a ON i.Id= a.ItemAssemblyId  and EstimateID = @EstimateID               
                ", (s, a) =>
                 {
                     Assembly asm;
                     if (!lookup.TryGetValue(s.Id, out asm))
                     {
                         lookup.Add(s.Id, asm = s);
                     }
                     return asm;
                 }, new { EstimateID = estimateID }
                 ).AsQueryable();

                assemblies = lookup.Values.ToList();
            }

            var citiesKV = cities.ToDictionary(k => k.ID);
            var statesKV = states.ToDictionary(k => k.ID);

            if (lead == null || customerAddress == null)
                return null;

            var document = GetEstimateDocumentByType(1);
            byte[] byteArray = document.Text;

            using (MemoryStream mem = new MemoryStream())
            {
                mem.Write(byteArray, 0, (int)byteArray.Length);
                using (WordprocessingDocument wordDoc =
                    WordprocessingDocument.Open(mem, true))
                {
                    XNamespace w =
                        "http://schemas.openxmlformats.org/wordprocessingml/2006/main";


                    IDictionary<String, BookmarkStart> bookmarkMap = new Dictionary<String, BookmarkStart>();

                    foreach (BookmarkStart bookmarkStart in wordDoc.MainDocumentPart.RootElement.Descendants<BookmarkStart>())
                    {
                        bookmarkMap[bookmarkStart.Name] = bookmarkStart;
                    }

                    foreach (BookmarkStart bookmarkStart in bookmarkMap.Values)
                    {
                        if (!bookmarkStart.Name.ToString().Trim().StartsWith("_"))
                        {
                            Run bookmarkText = bookmarkStart.NextSibling<Run>();
                            if (bookmarkText != null)
                            {
                                if (bookmarkStart.Name.ToString() == "Name")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = lead.LastName + ',' + lead.FirstName;
                                }

                                if (bookmarkStart.Name.ToString() == "Phone")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = "";
                                }

                                if (bookmarkStart.Name.ToString() == "PH")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = Common.EncryptDecrypt.formatPhoneNumber(lead.Telephone, "(###) ###-####");
                                }

                                if (bookmarkStart.Name.ToString() == "Date")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = String.Format("{0:MM/dd/yyyy}", estimate.CreatedDate);
                                }

                                if (bookmarkStart.Name.ToString() == "Address")
                                {
                                    var city = citiesKV[lead.BillCity].Description;
                                    var state = statesKV[lead.BillState].Description;

                                    bookmarkText.GetFirstChild<Text>().Text = lead.BillAddress1.Trim() + ' ' +
                                        (string.IsNullOrEmpty(lead.BillAddress2) ? "" : lead.BillAddress2.Trim() + ", ") + city + ", " + state; ;
                                }

                                if (bookmarkStart.Name.ToString() == "Email")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = lead.Email;
                                }

                                if (bookmarkStart.Name.ToString() == "Jobaddress")
                                {
                                    var city = citiesKV[customerAddress.City].Description;
                                    var state = statesKV[customerAddress.State].Description;
                                    bookmarkText.GetFirstChild<Text>().Text = customerAddress.Address1.Trim() + ',' + (string.IsNullOrEmpty(customerAddress.Address2) ? "" : customerAddress.Address2.Trim() + ", ") + city + ", " + state;
                                }

                                if (bookmarkStart.Name.ToString() == "Salesrep")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = employee.LastName.Trim() + ", " + employee.FirstName.Trim();
                                }

                                if (bookmarkStart.Name.ToString() == "PP")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = "Install New                                            Roofing Including:";
                                }

                                if (bookmarkStart.Name.ToString() == "PText")
                                {
                                    StringBuilder list = new StringBuilder();
                                    int count = 1;
                                    foreach (var i in assemblies)
                                    {
                                        if (!string.IsNullOrWhiteSpace(i.ProposalText))
                                        {
                                            list.AppendLine((count++).ToString() + ". " + i.ProposalText);
                                        }
                                    }

                                    bookmarkText.GetFirstChild<Text>().Text = list.ToString();
                                }

                                if (bookmarkStart.Name.ToString() == "Contract")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = "(" + ConvertNumbertoWords(Convert.ToInt32(estimate.ContractPrice)) + ")" + " $ " + estimate.ContractPrice.ToString("N", new CultureInfo("en-US"));
                                }

                                if (bookmarkStart.Name.ToString() == "CurDate")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = String.Format("{0:MM/dd/yyyy}", DateTime.Now);
                                }
                            }
                        }
                    }
                }

                EstimateDocument e = new EstimateDocument
                {
                    EstimateID = estimateID,
                    Name = "Proposal.docx",
                    Description = "Proposal",
                    UploadDateTime = DateTime.Now,
                    Text = mem.ToArray()
                };

                return e.Text;
            }
        }
    }
}

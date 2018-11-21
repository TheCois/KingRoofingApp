using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using Dapper;
using DapperExtensions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using KRF.Core.DTO.Sales;
using KRF.Core.Entities.Employee;
using KRF.Core.Entities.Product;
using KRF.Core.Entities.Sales;
using KRF.Core.Entities.ValueList;
using KRF.Core.Enums;
using KRF.Core.FunctionalContracts;
using NLog;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class EstimateManagement : IEstimateManagement
    {
        private Logger logger_;

        public EstimateManagement()
        {
            logger_ = NLog.LogManager.GetCurrentClassLogger();
        }
        
        public int Create(Estimate estimate, IList<EstimateItem> estimateItem)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    estimate.CreatedDate = DateTime.Now;
                    var id = conn.Insert(estimate);

                    foreach (var c in estimateItem)
                    {
                        c.EstimateID = id;
                        conn.Insert(c);
                    }
                    if (estimate.Status == (int)EstimateStatusType.Complete)
                    {
                        DeleteQuantityInInventory(conn, estimateItem.ToList(), id);
                    }
                    transactionScope.Complete();
                    return id;
                }
            }
        }

        public Estimate Edit(Estimate estimate, IList<EstimateItem> estimateItem)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    var curEstimate = conn.Get<Estimate>(estimate.ID);
                    if (estimate.CreatedDate == DateTime.MinValue)
                    {
                        estimate.CreatedDate = DateTime.Now;
                    }
                    conn.Update(estimate);

                    var pgEstimate = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    pgEstimate.Predicates.Add(Predicates.Field<EstimateItem>(s => s.EstimateID, Operator.Eq, estimate.ID));
                    
                    var curEstimateList = conn.GetList<EstimateItem>(pgEstimate).ToList();
                    if (curEstimate.Status != (int)EstimateStatusType.Complete && estimate.Status == (int)EstimateStatusType.Complete)
                    {
                        DeleteQuantityInInventory(conn, estimateItem.ToList(), estimate.ID);
                    }
                    else if (curEstimate.Status == (int)EstimateStatusType.Complete && estimate.Status == (int)EstimateStatusType.Complete)
                    {
                        foreach (var c in estimateItem)
                        {
                            if (curEstimateList.All(p => p.ItemAssemblyID != c.ItemAssemblyID))
                            {
                                var qtyToBeDelete = new List<EstimateItem> {c};
                                DeleteQuantityInInventory(conn, qtyToBeDelete, estimate.ID);
                            }
                            else
                            {
                                var curEst = curEstimateList.FirstOrDefault(p => p.ItemAssemblyID == c.ItemAssemblyID);
                                if (curEst == null) continue;
                                if (curEst.Quantity < c.Quantity)
                                {
                                    var qty = c.Quantity - curEst.Quantity;
                                    DeleteQuantityInInventory(conn, c.ItemAssemblyID, qty, estimate.ID);
                                }
                                else
                                {
                                    var qty = curEst.Quantity - c.Quantity;
                                    AddQuantityInInventory(conn, c.ItemAssemblyID, qty, estimate.ID);
                                }
                            }
                        }
                        foreach (var c1 in curEstimateList)
                        {
                            if (estimateItem.All(p => p.ItemAssemblyID != c1.ItemAssemblyID))
                            {
                                var qtyToBeDelete = new List<EstimateItem> {c1};
                                AddQuantityInInventory(conn, qtyToBeDelete, estimate.ID);
                            }
                        }
                    }
                    
                    foreach (var c1 in curEstimateList)
                    {
                        conn.Delete(c1);
                    }

                    foreach (var c in estimateItem)
                    {
                        c.EstimateID = estimate.ID;
                        conn.Insert(c);
                    }

                    transactionScope.Complete();
                    return estimate;
                }
            }
        }

        public bool Delete(int estimateId)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                bool isDeleted;
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    predicateGroup.Predicates.Add(Predicates.Field<Estimate>(s => s.ID, Operator.Eq, estimateId));
                    conn.Open();
                    var estimate = conn.Get<Estimate>(estimateId);
                    var estimateStatus = estimate.Status;
                    conn.Delete<Estimate>(predicateGroup);

                    var predicateGroup1 = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    predicateGroup1.Predicates.Add(Predicates.Field<EstimateItem>(s => s.EstimateID, Operator.Eq, estimateId));
                    if (estimateStatus == (int)EstimateStatusType.Complete)
                    {
                        var estimateItems = conn.GetList<EstimateItem>(predicateGroup1).ToList();
                        AddQuantityInInventory(conn, estimateItems, estimateId);
                    }
                    isDeleted = conn.Delete<EstimateItem>(predicateGroup1);
                }
                transactionScope.Complete();
                return isDeleted;
            }
        }

        private void DeleteQuantityInInventory(IDbConnection conn, int assemblyId, decimal qty, int estimateId)
        {
            var predicateGroupInv = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
            predicateGroupInv.Predicates.Add(Predicates.Field<AssemblyItem>(s => s.AssemblyId, Operator.Eq, assemblyId));
            var assemblyItems = conn.GetList<AssemblyItem>(predicateGroupInv).ToList();
            foreach (var assemblyItem in assemblyItems)
            {
                predicateGroupInv = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroupInv.Predicates.Add(Predicates.Field<Inventory>(s => s.ItemID, Operator.Eq, assemblyItem.ItemId));
                var inventory = conn.GetList<Inventory>(predicateGroupInv).FirstOrDefault();
                if (inventory != null)
                {
                    inventory.Qty = inventory.Qty - qty;
                    inventory.Type = "Assigned";
                    inventory.Comment = "Estimate ID : " + Convert.ToString(estimateId);
                    conn.Update(inventory);
                }
            }
        }
        private void AddQuantityInInventory(IDbConnection conn, int assemblyId, decimal qty, int estimateId)
        {
            var predicateGroupInv = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
            predicateGroupInv.Predicates.Add(Predicates.Field<AssemblyItem>(s => s.AssemblyId, Operator.Eq, assemblyId));
            var assemblyItems = conn.GetList<AssemblyItem>(predicateGroupInv).ToList();
            foreach (var assemblyItem in assemblyItems)
            {
                predicateGroupInv = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroupInv.Predicates.Add(Predicates.Field<Inventory>(s => s.ItemID, Operator.Eq, assemblyItem.ItemId));
                var inventory = conn.GetList<Inventory>(predicateGroupInv).FirstOrDefault();
                if (inventory != null)
                {
                    inventory.Qty = inventory.Qty + qty;
                    inventory.Type = "Assigned";
                    inventory.Comment = "Estimate ID : " + Convert.ToString(estimateId);
                    conn.Update(inventory);
                }
            }
        }

        private void AddQuantityInInventory(IDbConnection conn, IEnumerable<EstimateItem> estimateItems, int estimateId)
        {
            foreach (var estimateItem in estimateItems)
            {
                var predicateGroupInv = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroupInv.Predicates.Add(Predicates.Field<AssemblyItem>(s => s.AssemblyId, Operator.Eq, estimateItem.ItemAssemblyID));
                var assemblyItems = conn.GetList<AssemblyItem>(predicateGroupInv).ToList();
                foreach (var assemblyItem in assemblyItems)
                {
                    predicateGroupInv = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    predicateGroupInv.Predicates.Add(Predicates.Field<Inventory>(s => s.ItemID, Operator.Eq, assemblyItem.ItemId));
                    var inventory = conn.GetList<Inventory>(predicateGroupInv).FirstOrDefault();
                    if (inventory != null)
                    {
                        inventory.Qty = inventory.Qty + estimateItem.Quantity;
                        inventory.Type = "Assigned";
                        inventory.Comment = "Estimate ID : " + Convert.ToString(estimateId);
                        conn.Update(inventory);
                    }
                }
            }
        }
        private void DeleteQuantityInInventory(IDbConnection conn, IEnumerable<EstimateItem> estimateItems, int estimateId)
        {
            foreach (var c in estimateItems)
            {
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<AssemblyItem>(s => s.AssemblyId, Operator.Eq, c.ItemAssemblyID));
                var assemblyItems = conn.GetList<AssemblyItem>(predicateGroup).ToList();
                foreach (var assemblyItem in assemblyItems)
                {
                    predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    predicateGroup.Predicates.Add(Predicates.Field<Inventory>(s => s.ItemID, Operator.Eq, assemblyItem.ItemId));
                    var inventory = conn.GetList<Inventory>(predicateGroup).FirstOrDefault();
                    if (inventory != null)
                    {
                        var qty = inventory.Qty - c.Quantity;
                        inventory.Qty = qty;
                        inventory.Type = "Assigned";
                        inventory.Comment = "Estimate ID : " + Convert.ToString(estimateId);
                        conn.Update(inventory);
                    }
                }
            }
        }

        public EstimateDTO ListAll()
        {
            var estimateDto = new EstimateDTO();
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                estimateDto.Estimates = conn.GetList<Estimate>().ToList();
                estimateDto.EstimateItems = conn.GetList<EstimateItem>().ToList();
                estimateDto.Leads = conn.GetList<Lead>().Where(l => l.LeadStage == (int)LeadStageType.Lead).ToList();
                estimateDto.Customers = conn.GetList<Lead>().Where(l => l.LeadStage == (int)LeadStageType.Customer).ToList();
                estimateDto.CustomerAddress = conn.GetList<CustomerAddress>().ToList();
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<Status>(s => s.Active, Operator.Eq, true));
                estimateDto.Status = conn.GetList<Status>(predicateGroup).OrderBy(p => p.Description).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<RoofType>(s => s.Active, Operator.Eq, true));
                IList<RoofType> roofType = conn.GetList<RoofType>(predicateGroup).ToList();
                estimateDto.RoofTypes = roofType.OrderBy(p => p.Description).ToList();
            }
            return estimateDto;
        }

        public IList<EstimateDTO> ListAllEstimatesForACustomer(int customerId)
        {
            return null;
        }

        public EstimateDTO Select(int estimateId)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                var estimateDto = new EstimateDTO();

                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<EstimateItem>(s => s.EstimateID, Operator.Eq, estimateId));

                var estimate = conn.Get<Estimate>(estimateId);

                estimateDto.Estimates = new List<Estimate> {estimate};

                estimateDto.EstimateItems = conn.GetList<EstimateItem>(predicateGroup).ToList();

                estimateDto.Leads = conn.GetList<Lead>().Where(l => l.LeadStage == (int)LeadStageType.Lead).ToList();
                estimateDto.Customers = conn.GetList<Lead>().Where(l => l.LeadStage == (int)LeadStageType.Customer).ToList();
                estimateDto.CustomerAddress = conn.GetList<CustomerAddress>().ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<Status>(s => s.Active, Operator.Eq, true));
                estimateDto.Status = conn.GetList<Status>(predicateGroup).OrderBy(p => p.Description).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<UnitOfMeasure>(s => s.Active, Operator.Eq, true));
                estimateDto.UnitOfMeasure = conn.GetList<UnitOfMeasure>(predicateGroup).ToList();

                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<RoofType>(s => s.Active, Operator.Eq, true));
                IList<RoofType> roofType = conn.GetList<RoofType>(predicateGroup).ToList();
                estimateDto.RoofTypes = roofType.OrderBy(p => p.Description).ToList();

                estimateDto.Items = conn.GetList<Item>().ToList();
                estimateDto.Assemblies = conn.GetList<Assembly>().ToList();
                estimateDto.AssemblyItems = conn.GetList<AssemblyItem>().ToList();

                return estimateDto;
            }
        }

        public int SaveDocument(EstimateDocument estimateDocument)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    if (estimateDocument.ID > 0)
                    {
                        var curEstimate = conn.Get<EstimateDocument>(estimateDocument.ID);
                        curEstimate.Description = estimateDocument.Description;

                        conn.Update(curEstimate);
                    }
                    else
                    {
                        estimateDocument.ID = conn.Insert(estimateDocument);
                    }

                    transactionScope.Complete();
                    return estimateDocument.ID;
                }
            }
        }

        public bool DeleteEstimateDocument(int estimateDocumentId)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                bool isDeleted;
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    predicateGroup.Predicates.Add(Predicates.Field<EstimateDocument>(s => s.ID, Operator.Eq, estimateDocumentId));

                    conn.Open();
                    isDeleted = conn.Delete<EstimateDocument>(predicateGroup);
                }

                transactionScope.Complete();
                return isDeleted;
            }
        }

        public EstimateDocument GetEstimateDocument(int estimateDocumentId)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                var estimateDocument = conn.Get<EstimateDocument>(estimateDocumentId);
                return estimateDocument;
            }
        }

        public EstimateDocument GetEstimateDocumentByType(string type)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<EstimateDocument>(s => s.Type, Operator.Eq, type));

                var estimateDocument = conn.GetList<EstimateDocument>(predicateGroup).FirstOrDefault();
                logger_.Info(
                    "Getting Estimate Document of type {0} yielded document with ID {1} and Name '{2}' with Text Length {3}",
                    type, estimateDocument?.ID, estimateDocument?.Name, estimateDocument?.Text?.Length);
                return estimateDocument;
            }
        }

        public IList<EstimateDocument> GetEstimateDocuments(int estimateId)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                const string query = "select ID, EstimateID, Name, Type, Description, UploadDateTime, Null as Text from EstimateDocument " +
                             "WHERE EstimateID = @EstimateID";

                var estimateDocuments = conn.Query<EstimateDocument>(query,
                       new { EstimateID = estimateId }).ToList();
                return estimateDocuments;
            }
        }

        public string NumbersToWords(double inputNumber)
        {
            var inputNo = Convert.ToInt32(inputNumber);

            if (inputNo == 0)
                return "Zero";

            var numbers = new int[4];
            var first = 0;
            int u, h, t;
            var sb = new StringBuilder();

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

            for (var i = 3; i > 0; i--)
            {
                if (numbers[i] != 0)
                {
                    first = i;
                    break;
                }
            }
            for (var i = first; i >= 0; i--)
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

        public string ConvertNumberToWords(int number)
        {
            if (number == 0)
                return "ZERO";
            if (number < 0)
                return "minus " + ConvertNumberToWords(Math.Abs(number));
            var words = "";
            if ((number / 1000000) > 0)
            {
                words += ConvertNumberToWords(number / 1000000) + " MILLION ";
                number %= 1000000;
            }
            if ((number / 1000) > 0)
            {
                words += ConvertNumberToWords(number / 1000) + " THOUSAND ";
                number %= 1000;
            }
            if ((number / 100) > 0)
            {
                words += ConvertNumberToWords(number / 100) + " HUNDRED ";
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

        public byte[] CreateProposalDocument(int estimateId)
        {
            logger_.Info("CreateProposalDocument for estimate Id {0}", estimateId);
            Estimate estimate;
            CustomerAddress customerAddress;
            Lead lead;
            Employee employee;

            IList<City> cities;
            IList<State> states;
            List<Assembly> assemblies;

            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                estimate = conn.Get<Estimate>(estimateId);
                var leadId = estimate.LeadID;

                lead = conn.Get<Lead>(leadId);
                logger_.Info("Retrieving customer address for JobAddress ID {0}", estimate.JobAddressID);
                customerAddress = conn.Get<CustomerAddress>(estimate.JobAddressID);
                employee = conn.Get<Employee>(lead.AssignedTo);

                states = conn.GetList<State>().ToList();
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<City>(s => s.Active, Operator.Eq, true));
                cities = conn.GetList<City>(predicateGroup).ToList();

                var lookup = new Dictionary<int, Assembly>();
                conn.Query<Assembly, EstimateItem, Assembly>(@"
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
                 }, new { EstimateID = estimateId }
                 );

                assemblies = lookup.Values.ToList();
            }

            var citiesKv = cities.ToDictionary(k => k.ID);
            var statesKv = states.ToDictionary(k => k.ID);

            if (customerAddress == null)
            {
                logger_.Warn("Customer Address is null. Will use a Placeholder");
            }

            var document = GetEstimateDocumentByType("pdf");
            if (document == null)
            {
                logger_.Warn("Returned document is null. Returning an empty byte[]");
                return new byte[0];
            }

            var byteArray = document.Text;            

            using (var mem = new MemoryStream())
            {
                mem.Write(byteArray, 0, byteArray.Length);
                using (var wordDoc =
                    WordprocessingDocument.Open(mem, true))
                {
                    IDictionary<string, BookmarkStart> bookmarkMap = new Dictionary<string, BookmarkStart>();

                    foreach (var bookmarkStart in wordDoc.MainDocumentPart.RootElement.Descendants<BookmarkStart>())
                    {
                        bookmarkMap[bookmarkStart.Name] = bookmarkStart;
                    }

                    foreach (var x in bookmarkMap)
                    {
                        var bmName = x.Key.Trim();
                        var bmObject = x.Value;
                        if (!bmName.StartsWith("_"))
                        {
                            var bookmarkText = bmObject.NextSibling<Run>();
                            if (bookmarkText != null)
                            {
                                switch (bmName)
                                {
                                    case "Name":
                                    {
                                        bookmarkText.GetFirstChild<Text>().Text = lead.LastName + ',' + lead.FirstName;
                                        break;
                                    }

                                    case "Phone":
                                    {
                                        bookmarkText.GetFirstChild<Text>().Text = "";
                                        break;
                                    }

                                    case "PH":
                                    {
                                        bookmarkText.GetFirstChild<Text>().Text =
                                            Common.EncryptDecrypt.formatPhoneNumber(lead.Telephone, "(###) ###-####");
                                        break;
                                    }

                                    case "Date":
                                    {
                                        bookmarkText.GetFirstChild<Text>().Text =
                                            $"{estimate.CreatedDate:MM/dd/yyyy}";
                                        break;
                                    }

                                    case "Address":
                                    {
                                        var city = citiesKv[lead.BillCity].Description;
                                        var state = statesKv[lead.BillState].Description;

                                        bookmarkText.GetFirstChild<Text>().Text = lead.BillAddress1.Trim() + ' ' +
                                                                                  (string.IsNullOrEmpty(
                                                                                          lead.BillAddress2)
                                                                                          ? ""
                                                                                          : lead.BillAddress2.Trim() +
                                                                                            ", "
                                                                                  ) +
                                                                                  city + ", " + state;
                                        break;
                                    }

                                    case "Email":
                                    {
                                        bookmarkText.GetFirstChild<Text>().Text = lead.Email;
                                        break;
                                    }

                                    case "Jobaddress":
                                    {
                                        var addressLine = "[Job Address not set]";
                                        if (customerAddress != null)
                                        {
                                            var city = citiesKv[customerAddress.City].Description;
                                            var state = statesKv[customerAddress.State].Description;
                                            addressLine =
                                                customerAddress.Address1.Trim() + ',' +
                                                (string.IsNullOrEmpty(customerAddress.Address2)
                                                    ? ""
                                                    : customerAddress.Address2.Trim() + ", ") + city + ", " + state;
                                        }

                                        bookmarkText.GetFirstChild<Text>().Text = addressLine;

                                        break;
                                    }

                                    case "Salesrep":
                                    {
                                        bookmarkText.GetFirstChild<Text>().Text =
                                            employee.LastName.Trim() + ", " + employee.FirstName.Trim();
                                        break;
                                    }

                                    case "PP":
                                    {
                                        bookmarkText.GetFirstChild<Text>().Text =
                                            "Install New                                            Roofing Including:";
                                        break;
                                    }

                                    case "PText":
                                    {
                                        var list = new StringBuilder();
                                        var count = 1;
                                        foreach (var i in assemblies)
                                        {
                                            if (!string.IsNullOrWhiteSpace(i.ProposalText))
                                            {
                                                list.AppendLine((count++).ToString() + ". " + i.ProposalText);
                                            }
                                        }

                                        bookmarkText.GetFirstChild<Text>().Text = list.ToString();
                                        break;
                                    }

                                    case "Contract":
                                    {
                                        bookmarkText.GetFirstChild<Text>().Text =
                                            "(" + ConvertNumberToWords(Convert.ToInt32(estimate.ContractPrice)) + ")" +
                                            " $ " + estimate.ContractPrice.ToString("N", new CultureInfo("en-US"));
                                        break;
                                    }

                                    case "CurDate":
                                    {
                                        bookmarkText.GetFirstChild<Text>().Text =
                                            $"{DateTime.Now:MM/dd/yyyy}";
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                var e = new EstimateDocument
                {
                    EstimateID = estimateId,
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

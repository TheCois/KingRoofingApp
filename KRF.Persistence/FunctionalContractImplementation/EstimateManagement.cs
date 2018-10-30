﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Xml.Linq;
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

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class EstimateManagement : IEstimateManagement
    {
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
                            if (!curEstimateList.Any(p => p.ItemAssemblyID == c.ItemAssemblyID))
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
                            if (!estimateItem.Any(p => p.ItemAssemblyID == c1.ItemAssemblyID))
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
                    isDeleted = conn.Delete<Estimate>(predicateGroup);

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

        private void DeleteQuantityInInventory(IDbConnection conn, int assemblyID, decimal qty, int estimateId)
        {
            var predicateGroupInv = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
            predicateGroupInv.Predicates.Add(Predicates.Field<AssemblyItem>(s => s.AssemblyId, Operator.Eq, assemblyID));
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
        private void AddQuantityInInventory(IDbConnection conn, int assemblyID, decimal qty, int estimateId)
        {
            var predicateGroupInv = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
            predicateGroupInv.Predicates.Add(Predicates.Field<AssemblyItem>(s => s.AssemblyId, Operator.Eq, assemblyID));
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

                conn.Open();

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

        public bool DeleteEstimateDocument(int estimateDocumentID)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                bool isDeleted;
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    predicateGroup.Predicates.Add(Predicates.Field<EstimateDocument>(s => s.ID, Operator.Eq, estimateDocumentID));

                    conn.Open();
                    isDeleted = conn.Delete<EstimateDocument>(predicateGroup);
                }

                transactionScope.Complete();
                return isDeleted;
            }
        }

        public EstimateDocument GetEstimateDocument(int estimateDocumentID)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                var estimateDocument = conn.Get<EstimateDocument>(estimateDocumentID);
                return estimateDocument;
            }
        }

        public EstimateDocument GetEstimateDocumentByType(int type)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<EstimateDocument>(s => s.Type, Operator.Eq, type));

                var estimateDocument = conn.GetList<EstimateDocument>(predicateGroup).First();
                return estimateDocument;
            }
        }

        public IList<EstimateDocument> GetEstimateDocuments(int estimateID)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                const string query = "select ID, EstimateID, Name, Type, Description, UploadDateTime, Null as Tesxt from EstimateDocument " +
                             "WHERE EstimateID = @EstimateID";

                var estimateDocuments = conn.Query<EstimateDocument>(query,
                       new { EstimateID = estimateID }).ToList();
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

        public string ConvertNumbertoWords(int number)
        {
            if (number == 0)
                return "ZERO";
            if (number < 0)
                return "minus " + ConvertNumbertoWords(Math.Abs(number));
            var words = "";
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
            Estimate estimate;
            CustomerAddress customerAddress;
            Lead lead;
            Employee employee;

            IList<City> cities = null;
            IList<State> states = null;
            List<Assembly> assemblies = null;

            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                estimate = conn.Get<Estimate>(estimateID);
                var leadID = estimate.LeadID;

                lead = conn.Get<Lead>(leadID);
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
                 }, new { EstimateID = estimateID }
                 ).AsQueryable();

                assemblies = lookup.Values.ToList();
            }

            var citiesKV = cities.ToDictionary(k => k.ID);
            var statesKV = states.ToDictionary(k => k.ID);

            if (lead == null || customerAddress == null)
                return null;

            var document = GetEstimateDocumentByType(1);
            var byteArray = document.Text;

            using (var mem = new MemoryStream())
            {
                mem.Write(byteArray, 0, (int)byteArray.Length);
                using (var wordDoc =
                    WordprocessingDocument.Open(mem, true))
                {
                    XNamespace w =
                        "http://schemas.openxmlformats.org/wordprocessingml/2006/main";


                    IDictionary<String, BookmarkStart> bookmarkMap = new Dictionary<String, BookmarkStart>();

                    foreach (var bookmarkStart in wordDoc.MainDocumentPart.RootElement.Descendants<BookmarkStart>())
                    {
                        bookmarkMap[bookmarkStart.Name] = bookmarkStart;
                    }

                    foreach (var bookmarkStart in bookmarkMap.Values)
                    {
                        if (!bookmarkStart.Name.ToString().Trim().StartsWith("_"))
                        {
                            var bookmarkText = bookmarkStart.NextSibling<Run>();
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

                var e = new EstimateDocument
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
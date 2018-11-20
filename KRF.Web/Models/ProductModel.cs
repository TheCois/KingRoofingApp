using KRF.Core.DTO.Product;
using KRF.Core.Entities.Sales;
using System.Collections.Generic;
using System.Linq;
using ProductNS = KRF.Core.Entities.Product;
using ValueListNS = KRF.Core.Entities.ValueList;

namespace KRF.Web.Models
{
    public class Item
    {
        public string Id { get; set; }
        public int ItemTypeId { get; set; }
        public string ItemCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryId { get; set; }
        public string ManufacturerId { get; set; }
        public string UnitOfMeasureId { get; set; }
        public string Category { get; set; }
        public string Manufacturer { get; set; }
        public string UnitOfMeasure { get; set; }
        public string Division { get; set; }
        public string Price { get; set; }
        public bool IsInventoryItem { get; set; }
    }

    public class Assembly
    {
        public string ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string TotalCost { get; set; }
        public string LaborHour { get; set; }
        public string TotalRetailCost { get; set; }
        public IList<AssemblyItem> AssemblyItems { get; set; }
        public IList<AvailableAssemblyItem> AvailableAssemblyItems { get; set; }
        public bool IsItemAssembly { get; set; }
        public string ProposalText { get; set; }
        public int UnitOfMeasureId { get; set; }
        public string UnitOfMeasure { get; set; }
    }
    public class Inventory
    {
        public int ID { get; set; }
        public decimal Qty { get; set; }
    }

    public class AssemblyItem
    {
        public string ID { get; set; }
        public string ItemID { get; set; }
        public string ItemCode { get; set; }
        public string Name { get; set; }
        public string Quantity { get; set; }
        public decimal PercentageOfItem { get; set; }
        public decimal Price { get; set; }
        public decimal Value { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal Cost { get; set; }
        public decimal CostPercent { get; set; }
        public decimal RetailCost { get; set; }
    }

    public class AvailableAssemblyItem
    {
        public string ID { get; set; }
        public string Description { get; set; }
        public string ItemID { get; set; }
        public string ItemCode { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Manufacturer { get; set; }
        public decimal Price { get; set; }
    }

    public class Manufacturer
    {
        public string ID { get; set; }
        public string Description { get; set; }
    }

    public class Category
    {
        public string ID { get; set; }
        public string Description { get; set; }
    }

    public class ItemType
    {
        public string ID { get; set; }
        public string Description { get; set; }
    }

    public class UnitsOfMeasure
    {
        public string ID { get; set; }
        public string Description { get; set; }
    }

    public class Division
    {
        public string ID { get; set; }
        public string Description { get; set; }
    }


    public static class ModelData
    {
        public static IList<Item> Items(ProductDTO product)
        {
            IList<Item> items = new List<Item>();
            //var itemTypesKeyValue = product.ItemTypes.ToDictionary(k => k.Id);

            items = (from m in product.Items
                     //let itemType = (m.ItemTypeId != null && itemTypesKeyValue.ContainsKey(m.ItemTypeId))?
                     //itemTypesKeyValue[m.ItemTypeId].Description : string.Empty
                     select new Item
                     {
                         Id = m.Id.ToString() + "," + m.Code + "," + m.Price.ToString(),
                         ItemCode = m.Code,
                         //Description = m.Description, //Commented on 14-Apr-2015 - Convering search to dropdown
                         Description = m.Code + "[" + m.Name + " (" + (product.ItemTypes.FirstOrDefault(p => p.Id == m.ItemTypeId) != null ? product.ItemTypes.FirstOrDefault(p => p.Id == m.ItemTypeId).Description.Trim() : "") + ")" + "]",
                         Name = m.Name,
                         Price = m.Price.ToString(),
                         Category = m.CategoryId > 0 ? product.Categories.Where(k => k.Id == m.CategoryId).First().Name : "nil",
                         Manufacturer = m.ManufacturerId > 0 ? product.Manufacturers.Where(k => k.Id == m.ManufacturerId).First().Name : "nil",
                         UnitOfMeasure = m.UnitOfMeasureId > 0 ? product.UnitsOfMeasure.Where(k => k.Id == m.UnitOfMeasureId).First().Name : "nil",
                         IsInventoryItem = m.IsInventoryItem
                     }
                     ).ToList();

            return items;
        }

        public static string GetCategoryName(IList<ValueListNS.Category> categories, int id)
        {

            var name = (from i in categories where i.Id == id select i.Name).First();
            return name;

        }

        public static IList<Assembly> Assemblies(ProductDTO product)
        {
            var assemblies = (from a in product.Assemblies
                              select new Assembly
                              {
                                  ID = a.Id.ToString(),
                                  Code = a.Code,
                                  Name = a.AssemblyName,
                                  Description = a.Description,
                                  UnitOfMeasure = a.UnitOfMeasureId > 0 ? product.UnitsOfMeasure.Where(k => k.Id == a.UnitOfMeasureId).First().Name : "nil",
                              }).ToList();
            return assemblies;
        }

        public static IList<ItemType> ItemTypes(IList<ValueListNS.ItemType> itemTypes)
        {
            IList<ItemType> itemTypeList = new List<ItemType>();

            itemTypeList = (from m in itemTypes orderby m.Description select new ItemType { ID = m.Id.ToString(), Description = m.Description }).ToList();
            return itemTypeList;
        }

        public static IList<Category> Catogories(IList<ValueListNS.Category> categories)
        {
            IList<Category> categoryList = new List<Category>();

            categoryList = (from m in categories orderby m.Name select new Category { ID = m.Id.ToString(), Description = m.Name }).ToList();
            return categoryList;
        }

        public static IList<Manufacturer> Manufacturers(IList<ValueListNS.Manufacturer> manufacturers)
        {
            IList<Manufacturer> manufacturerList = new List<Manufacturer>();

            manufacturerList = (from m in manufacturers orderby m.Name select new Manufacturer { ID = m.Id.ToString(), Description = m.Name }).ToList();
            return manufacturerList;
        }

        public static IList<UnitsOfMeasure> UnitsOfMeasure(IList<ValueListNS.UnitOfMeasure> unitOfMeasures)
        {
            IList<UnitsOfMeasure> unitsOfMeasureList = new List<UnitsOfMeasure>();

            unitsOfMeasureList = (from m in unitOfMeasures orderby m.Name select new UnitsOfMeasure { ID = m.Id.ToString(), Description = m.Name }).OrderBy(m => m.ID).ToList();
            return unitsOfMeasureList;
        }

        public static IList<Division> Divisions()
        {
            var m = new Division { ID = "1", Description = "Division 1" };
            var m1 = new Division { ID = "2", Description = "Division 2" };
            var m2 = new Division { ID = "3", Description = "Division 3" };

            IList<Division> divisions = new List<Division>();
            divisions.Add(m);
            divisions.Add(m1);
            divisions.Add(m2);

            return divisions;
        }

        public static ProductNS.Item PopulateEntityItem(Item item)
        {
            var price = decimal.Parse(string.IsNullOrEmpty(item.Price) ? "0" : item.Price);
            var categoryId = int.Parse(item.CategoryId);
            var manufacturerId = int.Parse(item.ManufacturerId);
            var unitOfMeasureId = int.Parse(item.UnitOfMeasureId);
            var entityItem = new ProductNS.Item
            {
                Id = int.Parse(item.Id),
                ItemTypeId = item.ItemTypeId,
                Name = item.Name,
                Code = item.ItemCode,
                Description = item.Description,
                Price = decimal.Parse(item.Price),
                CategoryId = categoryId,
                ManufacturerId = manufacturerId,
                UnitOfMeasureId = unitOfMeasureId,
                IsInventoryItem = item.IsInventoryItem
            };

            return entityItem;
        }

        public static Item PopulateModelItem(ProductNS.Item item)
        {
            var item1 = new Item
            {
                Name = item.Name,
                ItemTypeId = item.ItemTypeId,
                ItemCode = item.Code,
                Description = item.Description,
                Price = item.Price.ToString(),
                CategoryId = item.CategoryId > 0 ? item.CategoryId.ToString() : "1",
                ManufacturerId = item.ManufacturerId > 0 ? item.ManufacturerId.ToString() : "1",
                UnitOfMeasureId = item.UnitOfMeasureId > 0 ? item.UnitOfMeasureId.ToString() : "1",
                IsInventoryItem = item.IsInventoryItem,
                Division = "1"
            };

            return item1;
        }

        public static Assembly populateAssemblyModel(AssemblyItemDTO ai, IList<ProductNS.Item> items)
        {
            var assemblyDTO = ai.assembly;
            var assemblyItemDTO = ai.assemblyItem;
            var assemlyItems = (from i in assemblyItemDTO
                                let entityItem = GetItemFromList(i.ItemId, items)
                                select new AssemblyItem
                                {
                                    ID = i.id.ToString(),
                                    ItemID = i.ItemId.ToString(),
                                    ItemCode = entityItem != null ? entityItem.ItemCode : string.Empty,
                                    Name = entityItem != null ? entityItem.Name : string.Empty,
                                    Quantity = i.Quantity.ToString(),
                                    PercentageOfItem = i.PercentageOfItem,
                                    Price = i.Price,
                                    Value = i.Value,
                                    TaxPercent = i.TaxPercent,
                                    Cost = i.Cost,
                                    CostPercent = i.CostPercent,
                                    RetailCost = i.RetailCost
                                }).ToList();

            var a = new Assembly
            {
                ID = assemblyDTO.Id.ToString(),
                Code = assemblyDTO.Code,
                Name = assemblyDTO.AssemblyName,
                Description = assemblyDTO.Description,
                UnitOfMeasureId = assemblyDTO.UnitOfMeasureId,
                LaborHour = assemblyDTO.LaborHour.ToString(),
                TotalRetailCost = assemblyDTO.TotalRetailCost.ToString(),
                TotalCost = assemblyDTO.TotalCost.ToString(),
                AssemblyItems = assemlyItems,
                IsItemAssembly = assemblyDTO.IsItemAssembly,
                AvailableAssemblyItems = AvailableItems(items, assemblyItemDTO.Select(k => k.id).ToArray()),
                ProposalText = (!string.IsNullOrEmpty(assemblyDTO.ProposalText) ? assemblyDTO.ProposalText.Trim() : "")                
            };

            return a;
        }

        public static AssemblyItemDTO populateAssemblyEntity(Assembly assembly)
        {
            var assemblyId = int.Parse(assembly.ID);
            var assemblyItemDTO = new AssemblyItemDTO();

            assemblyItemDTO.assembly = new ProductNS.Assembly
            {
                Id = assemblyId,
                Code = assembly.Code,
                Description = assembly.Description,
                UnitOfMeasureId = assembly.UnitOfMeasureId,
                AssemblyName = assembly.Name,
                //LaborHour = int.Parse(assembly.LaborHour),
                TotalRetailCost = decimal.Parse(assembly.TotalRetailCost),
                TotalCost = decimal.Parse(assembly.TotalCost),
                MarkUpPercentageByDivision = 0,
                WorkOrderText = string.Empty,
                IsItemAssembly = assembly.IsItemAssembly,
                ProposalText = (!string.IsNullOrEmpty(assembly.ProposalText) ? assembly.ProposalText.Trim() : "")
            };

            assemblyItemDTO.assemblyItem = (from ai in assembly.AssemblyItems
                                            select new ProductNS.AssemblyItem
                                            {
                                                AssemblyId = assemblyId,
                                                ItemId = int.Parse(ai.ItemID),
                                                id = int.Parse(ai.ID),
                                                PercentageOfItem = ai.PercentageOfItem,
                                                Price = ai.Price,
                                                Value = ai.Value,
                                                TaxPercent = ai.TaxPercent,
                                                Cost = ai.Cost,
                                                CostPercent = ai.CostPercent,
                                                RetailCost = ai.RetailCost
                                            }).ToList();

            return assemblyItemDTO;
        }

        public static Item GetItemFromList(int ItemId, IList<ProductNS.Item> items)
        {
            var list = (from item in items where item.Id == ItemId select item).ToList();
            if (list.Count > 0)
                return PopulateModelItem(list[0]);
            else
                return null;
        }

        public static IList<AvailableAssemblyItem> AvailableItems(IList<ProductNS.Item> items, int[] selected)
        {
            var avaliableList = (from item in items.OrderBy(p=>p.Code)
                                 where selected == null || (selected.Count() == 0) || selected.Any(k => k != item.Id)
                                 select new AvailableAssemblyItem
                                 {
                                     ID = item.Id.ToString()+","+item.Code+","+item.Price.ToString(),
                                     Description = item.Code+"["+ item.Name +"]",
                                     ItemID = item.Id.ToString(),
                                     ItemCode = item.Code,
                                     Name = item.Name,
                                     Price = item.Price,
                                     Category = "test",
                                     Manufacturer = "mtest"
                                 }).ToList();
            return avaliableList;
        }

        public static List<ProductNS.Inventory> ToInventoryList(InventoryData inventoryData)
        {
            var inventoryList = new List<ProductNS.Inventory>();
            if (inventoryData.Inventories != null && inventoryData.Inventories.Any())
            {
                inventoryList = inventoryData.Inventories.Select(p => new ProductNS.Inventory() { ID = p.ID, Qty = p.Qty }).ToList();
            }
            return inventoryList;
        }
    }

    public class ProspectModel
    {
        public IList<ProspectData> ProspectData { get; set; }
    }

    public class ProspectData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
    }

    public class CustomerData
    {
        public Lead Lead { get; set; }
        public IList<CustomerAddress> CustomerAddress { get; set; }
    }

    public class EstimateData
    {
        public Estimate Estimate { get; set; }
        public IList<EstimateItem> EstimateItem { get; set; }
    }
    public class InventoryData
    {
        public IList<Inventory> Inventories { get; set; }
    }
}
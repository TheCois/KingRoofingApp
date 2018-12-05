using KRF.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using KRF.Core.Repository;
using ProductNS = KRF.Core.Entities.Product;
using KRF.Core.DTO.Product;

namespace KRF.Web.Controllers
{
    [CustomActionFilter.CustomActionFilter]
    public class ProductController : BaseController
    {
        //
        // GET: /Product/

        public ActionResult Index()
        {
            var itemRepository = ObjectFactory.GetInstance<IItemManagementRepository>();
            var product = itemRepository.GetProduct();
            TempData["Product"] = product;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Products()
        {
            var product = (ProductDTO)TempData["Product"];

            var itemTypes = product.ItemTypes;
            var categories = product.Categories;
            var manufacturers = product.Manufacturers;
            var unitOfMeasures = product.UnitsOfMeasure;

            return Json(new
            {
                items = ModelData.Items(product).Select(k => new { ID = k.Id, k.Description }),
                itemTypes = ModelData.ItemTypes(itemTypes),
                assemblies = ModelData.Assemblies(product).Select(k => new {k.ID, k.Description }),
                manufacturers = ModelData.Manufacturers(manufacturers),
                categories = ModelData.Catogories(categories),
                unitsOfMeasure = ModelData.UnitsOfMeasure(unitOfMeasures),
                divisions = ModelData.Divisions()
            });
        }

        public ActionResult GetItems(jQueryDataTableParamModel param)
        {
            var itemRepository = ObjectFactory.GetInstance<IItemManagementRepository>();
            var product = itemRepository.GetProduct();
            var items = ModelData.Items(product);
            var categories = product.Categories;
            var manufacturers = product.Manufacturers;
            var unitOfMeasures = product.UnitsOfMeasure;
            //var assemblies = product.Assemblies;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = items.Count,
                iTotalDisplayRecords = items.Count,
                aaData = (from p in items
                          select new[] {
                              "<span class='edit-item' data-val=" + p.Id + "><ul><li class='edit'><a href='#non'></a></li></ul></span>",
                              "<a class='clone-item' data-val=" + p.Id + " href='#non'>" + p.ItemCode + "</a>",
                              p.Name,
                              p.Category,
                              p.IsInventoryItem ? "YES" : "",
                              p.Manufacturer,
                              p.UnitOfMeasure,
                              p.Price,
                              "<span class='delete-item' data-val=" + p.Id + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>"
                          }).ToArray(),
                keyValue = new
                {
                    itemTypes = ModelData.ItemTypes(product.ItemTypes),
                    manufacturers = ModelData.Manufacturers(manufacturers),
                    categories = ModelData.Catogories(categories),
                    unitsOfMeasure = ModelData.UnitsOfMeasure(unitOfMeasures),
                    divisions = ModelData.Divisions()
                }
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAssemblies(jQueryDataTableParamModel param)
        {
            var itemRepository = ObjectFactory.GetInstance<IItemManagementRepository>();
            var product = itemRepository.GetProduct();
            //var items = ModelData.Items(product);
            var categories = product.Categories;
            var manufacturers = product.Manufacturers;
            var unitOfMeasures = product.UnitsOfMeasure;
            var assemblies = ModelData.Assemblies(product);

            return Json(new
            {
                param.sEcho,
                iTotalRecords = assemblies.Count,
                iTotalDisplayRecords = assemblies.Count,
                aaData = (from p in assemblies
                          select new[] {
                              "<span class='edit-assembly' data-val=" + p.ID + "><ul><li class='edit'><a href='#non'>View</a></li></ul></span>",
                              p.Code,
                              p.Name,
                              p.Description,
                              p.UnitOfMeasure,
                              "<span class='delete-assembly' data-val=" + p.ID + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>"
                          }).ToArray(),
                keyValue = new
                {
                    itemTypes = ModelData.ItemTypes(product.ItemTypes),
                    manufacturers = ModelData.Manufacturers(manufacturers),
                    categories = ModelData.Catogories(categories),
                    unitsOfMeasure = ModelData.UnitsOfMeasure(unitOfMeasures),
                    divisions = ModelData.Divisions()
                }
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetItems()
        {
            var item = new Item { ItemCode = "FHR-1001", Name = "Coil Nails", Category = "Category 1", Manufacturer = "Manufacturer1", Price = "$100" };
            IList<Item> items = new List<Item>();
            items.Add(item);
            return Json(new {items });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetItem(string id)
        {
            var repository = ObjectFactory.GetInstance<IItemManagementRepository>();
            var i = repository.GetItem(int.Parse(id));
            var item = ModelData.PopulateModelItem(i);
            //Item item = new Item { ItemCode = "FHR-1001", Name = "Coil Nails", Category = "2", Manufacturer = "1", Price = "$100",  
            //    UnitOfMeasure= "3", Division="2"};            
            return Json(new {item });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveItem(Item item)
        {
            string message;
            var entityItem = ModelData.PopulateEntityItem(item);
            var repository = ObjectFactory.GetInstance<IItemManagementRepository>();
            var id = item.Id == null ? 0 : int.Parse(item.Id);
            if (id > 0)
            {
                repository.Edit(entityItem);
                message = "Record successfully updated!";
            }
            else
            {
                id = repository.Create(entityItem);
                message = "Record successfully inserted!";
            }
            return Json(new {id, message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteItem(string itemId)
        {
            var itemRepo = ObjectFactory.GetInstance<IItemManagementRepository>();
            itemRepo.Delete(int.Parse(itemId));
            return Json(new { id = itemId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetAssemblies()
        {
            return Json(new { assemblies = new object() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetAssembly(string id)
        {
            var assemblyRepo = ObjectFactory.GetInstance<IAssemblyManagementRepository>();
            var assembly = assemblyRepo.GetAssembly(int.Parse(id));

            var itemRepo = ObjectFactory.GetInstance<IItemManagementRepository>();
            var items = itemRepo.GetAllItems();
            var product = itemRepo.GetProduct();
            var itemTypeKeyValue = product.ItemTypes.ToDictionary(k => k.Id);
            foreach (var i in items)
            {
                if (i.ItemTypeId >0 && itemTypeKeyValue.ContainsKey(i.ItemTypeId))
                {
                    i.Name = i.Name + " (" + itemTypeKeyValue [i.ItemTypeId].Description.Trim() + ")";
                }
            }

            var assemblyModel = ModelData.populateAssemblyModel(assembly, items);
            return Json(new { assembly = assemblyModel });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveAssembly(Assembly assembly)
        {
            string message;
            var assemblyRepo = ObjectFactory.GetInstance<IAssemblyManagementRepository>();
            var id = int.Parse(assembly.ID);
            var assemblyItemDto = ModelData.populateAssemblyEntity(assembly);

            if (id == 0)
            {
                id = assemblyRepo.Create(assemblyItemDto);
                message = "Record successfully inserted!";
            }
            else
            {
                assemblyRepo.Edit(assemblyItemDto);
                message = "Record successfully updated!";
            }

            return Json(new {id, message  });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteAssembly(string assemblyId)
        {
            var assemblyRepo = ObjectFactory.GetInstance<IAssemblyManagementRepository>();
            assemblyRepo.Delete(int.Parse(assemblyId));

            return Json(new { id = assemblyId });
        }
        public ActionResult Inventory()
        {
            var itemRepository = ObjectFactory.GetInstance<IItemManagementRepository>();
            var product = itemRepository.GetProduct();
            TempData["Product"] = product;
            return View(); // TODO go write this View
        }
    }

}

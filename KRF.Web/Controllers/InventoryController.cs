using KRF.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using KRF.Core.Repository;
using ProductNS = KRF.Core.Entities.Product;
using ValueListNS = KRF.Core.Entities.ValueList;

namespace KRF.Web.Controllers
{
    [CustomActionFilter.CustomActionFilter]
    public class InventoryController : BaseController
    {
        //
        // GET: /Inventory/

        public ActionResult Index()
        {
            var itemRepository = ObjectFactory.GetInstance<IItemManagementRepository>();
            var product = itemRepository.GetProduct();
            TempData["Product"] = product;
            return View();
        }

        public ActionResult GetInventory(jQueryDataTableParamModel param)
        {
            var itemRepository = ObjectFactory.GetInstance<IItemManagementRepository>();
            var product = itemRepository.GetInventory();
            var categories = product.Categories;
            var manufacturers = product.Manufacturers;
            var unitOfMeasures = product.UnitsOfMeasure;
            var items = product.Items;
            var inventories = product.Inventories;

            var list = from p in items
                       where p.IsInventoryItem == true
                       join q in inventories 
                       on p.Id equals q.ItemID
                       select new { p.Id, p.Code, InventoryID = q == null ? 0 : q.ID, p.Name, p.Description, Qty = q == null ? 0 : q.Qty };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = 97,
                iTotalDisplayRecords = 3,
                aaData = (from p in list
                          select new string[] {
                              "<span class='edit-item' data-val=" + p.Id.ToString() + "><ul><li class='edit'><a href='#non' tabIndex='-1'>View</a></li></ul></span>",
                              p.Code,
                              p.Name,
                              p.Description,
                              "<input type='text' style='width:100px' maxlength='8' class='qty'  onkeypress='enterOnlyNumericForQty(this, event)' value = " + ((p.Qty == 0) ? string.Empty : p.Qty.ToString()) + ">",
                              "<span class='update-inventory' data-val=" + p.InventoryID.ToString() + "><ul><li class='update'><a href='#non' tabIndex='-1'>Update</a></li></ul></span>",
                              "<span class='view-history' data-val=" + p.InventoryID.ToString() + "><ul><li class='history'><a href='#non' tabIndex='-1'>History</a></li></ul></span>",
                              "<span class='delete-inventory' data-val=" + p.InventoryID.ToString() + "><ul><li class='delete'><a href='#non' tabIndex='-1'>Delete</a></li></ul></span>"
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
        public ActionResult GetInventoryHistory(int ID)
        {
            var itemRepository = ObjectFactory.GetInstance<IItemManagementRepository>();
            var product = itemRepository.GetInventoryAudit(ID);
            var items = product.Items;
            var inventories = product.InventoryAudits;

            var list = from p in items
                       join q in inventories
                       on p.Id equals q.ItemID
                       orderby q.DateCreated descending
                       select new { p.Id, InventoryID = q.ID, p.Name, p.Description, q.Qty, q.DateCreated, q.Type, q.Comment };
                       
            return Json(new
            {
                aaData = (from p in list
                          select new string[] { "<span>"+ p.Qty +"</span>", p.Type, p.Comment,
                              "<span>"+ p.DateCreated.ToString("MM/dd/yyyy HH:mm") +"</span>"}).ToArray()
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UpdateInventory(InventoryData inventoryData)
        {
            var success = false;
            var message = string.Empty;

            var inventoryList = ModelData.ToInventoryList(inventoryData);
            var itemRepository = ObjectFactory.GetInstance<IItemManagementRepository>();
            success = itemRepository.UpdateInventory(inventoryList);
            if(success)
            {
                message = "Inventory updated successfully.";
            }
            else
            {
                message = "Inventory could not be updated.";
            }
            return Json(new { success = success, message = message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteInventory(InventoryData inventoryData)
        {
            var success = false;
            var message = string.Empty;

            var inventoryList = ModelData.ToInventoryList(inventoryData);
            var itemRepository = ObjectFactory.GetInstance<IItemManagementRepository>();
            success = itemRepository.DeleteInventory(inventoryList);
            if (success)
            {
                message = "Inventory item deleted successfully.";
            }
            else
            {
                message = "Inventory item could not be updated.";
            }
            return Json(new { success = success, message = message }, JsonRequestBehavior.AllowGet);
        }

    }
}

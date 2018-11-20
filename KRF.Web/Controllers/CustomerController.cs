using KRF.Core.DTO.Sales;
using KRF.Core.Entities.Sales;
using KRF.Core.Enums;
using KRF.Core.Repository;
using KRF.Web.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace KRF.Web.Controllers
{
    [CustomActionFilter.CustomActionFilter]
    public class CustomerController : BaseController
    {
        //
        // GET: /Customer/

        public ActionResult Index()
        {
            var leadRepo = ObjectFactory.GetInstance<ILeadManagementRepository>();
            var customers = leadRepo.GetLeads(GetCustomersPredicate());
            TempData["Customers"] = customers;
            return View();
        }

        public ActionResult GetCustomers(jQueryDataTableParamModel param)
        {
            var leadRepo = ObjectFactory.GetInstance<ILeadManagementRepository>();
            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();

            var estimateDTO = (EstimateDTO)TempData["Estimates"];
            if (estimateDTO == null)
                estimateDTO = estimateRepo.ListAll();

            var customerDTO = (LeadDTO)TempData["Customers"];
            if (customerDTO == null)
                customerDTO = leadRepo.GetLeads(GetCustomersPredicate());

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = 97,
                iTotalDisplayRecords = 3,
                aaData = (from p in customerDTO.Leads
                          select new string[] {
                              "<span class='edit-customer' data-val=" + p.ID.ToString() + "><ul><li class='edit'><a href='#non'>View</a></li></ul></span>",
                              p.ID.ToString(),
                              p.FirstName,
                              p.LastName,
                              "<a href=\"mailto:" + p.Email + "\" >" + p.Email + "</a>",
                              p.Telephone,
                              p.Cell,
                              p.Office,
                              (estimateDTO.Estimates.Where(e => e.LeadID == p.ID).FirstOrDefault() != null ? estimateDTO.Estimates.Where(e => e.LeadID == p.ID).Count().ToString() : "0"),
                              "<span class='delete-customer' data-val=" + p.ID.ToString() + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>"
                          }).ToArray(),
                keyValue = new
                {
                    city = customerDTO.Cities,
                    state = customerDTO.States,
                    countries = customerDTO.Countries,
                    status = customerDTO.Statuses
                }
            }, JsonRequestBehavior.AllowGet);
        }

        private Func<Lead, bool> GetCustomersPredicate()
        {
            return x => x.LeadStage == (int)LeadStageType.Customer;
        }

        public ActionResult GetCustomer(int id)
        {
            var leadRepo = ObjectFactory.GetInstance<ILeadManagementRepository>();
            var customertDTO = leadRepo.GetLead(id);
            return Json(new
            {
                customer = customertDTO.Leads.First(),
                customerAddressTable = (from p in customertDTO.CustomerAddress
                                        select new string[] {p.ID.ToString(), p.Address1, p.Address2, 
                  p.City == 0?"": customertDTO.Cities.Where(k=>k.ID == p.City).First().Description, 
                  p.State == 0?"": customertDTO.States.Where(k=>k.ID == p.State).First().Description, 
                  p.ZipCode,
                    "<span class='edit-cust-addr' data-val=" + p.ID.ToString() + "><ul><li class='edit'><a href='#non'>Edit</a></li></ul></span>", 
                    "<span class='delete-cust-addr' data-val=" + p.ID.ToString() + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>"}),
                customerAddress = customertDTO.CustomerAddress
            });
        }

        //[ValidateAntiForgeryToken]
        public JsonResult Save(CustomerData customerData)
        {
            var leadRepo = ObjectFactory.GetInstance<ILeadManagementRepository>();
            var customer = customerData.Lead;
            var customerAddress = customerData.CustomerAddress;

            if (customer.ID == 0)
            {
                customer.LeadStage = (int)LeadStageType.Customer;
                leadRepo.Create(customer, customerAddress);
            }
            else
            {
                leadRepo.Edit(customer, customerAddress);
            }

            return Json(new { hasSaved = true });
        }

        //[ValidateAntiForgeryToken]
        public JsonResult SaveJobAddress(CustomerData customerData)
        {
            var leadRepo = ObjectFactory.GetInstance<ILeadManagementRepository>();
            var customer = customerData.Lead;
            var customerAddress = customerData.CustomerAddress;
            if (customerAddress[0].ID == 0)
            {
                leadRepo.CreateJobAddress(customerAddress);
            }
            else
            {
                leadRepo.EditJobAddress(customerAddress);
            }
            var customertDTO = leadRepo.GetLead(customerAddress[0].LeadID);
            return Json(new
            {
                customerAddressTable = (from p in customertDTO.CustomerAddress
                                        select new string[] {p.ID.ToString(), p.Address1, p.Address2, 
                  p.City == 0?"": customertDTO.Cities.Where(k=>k.ID == p.City).First().Description, 
                  p.State == 0?"": customertDTO.States.Where(k=>k.ID == p.State).First().Description, 
                  p.ZipCode,
                    "<span class='edit-cust-addr' data-val=" + p.ID.ToString() + "><ul><li class='edit'><a href='#non'>Edit</a></li></ul></span>", 
                    "<span class='delete-cust-addr' data-val=" + p.ID.ToString() + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>"}),
                customerAddress = customertDTO.CustomerAddress
            });
        }

        public JsonResult DeleteJobAddress(int id)
        {
            var leadRepo = ObjectFactory.GetInstance<ILeadManagementRepository>();
            var isDeleted = leadRepo.DeleteJobAddress(id);

            return Json(new { hasDeleted = isDeleted});
        }

        public JsonResult DeleteCustomer(int id)
        {
            var leadRepo = ObjectFactory.GetInstance<ILeadManagementRepository>();
            leadRepo.Delete(id);

            return Json(new { hasDeleted = true });
        }
    }
}

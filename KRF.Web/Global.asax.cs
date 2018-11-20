using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using KRF.DependencyResolution;
using KRF.Web.Models;
using System.Web.Script.Serialization;

namespace KRF.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            IoC.Initialize();
            ModelBinders.Binders.Add(typeof(Item), new JSONDataModelBinder<Item>("item"));
            ModelBinders.Binders.Add(typeof(Assembly), new JSONDataModelBinder<Assembly>("assembly"));
            ModelBinders.Binders.Add(typeof(ProspectModel), new JSONDataModelBinder<ProspectModel>("prospectModel"));
            ModelBinders.Binders.Add(typeof(CustomerData), new JSONDataModelBinder<CustomerData>("customerData"));
            ModelBinders.Binders.Add(typeof(EstimateData), new JSONDataModelBinder<EstimateData>("estimateData"));

        }


        protected void Session_End(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Session_End");
        }

        //This method checks if we have an AJAX request or not
        private bool IsAjaxRequest()
        {
            //The easy way
            var isAjaxRequest = (Request["X-Requested-With"] == "XMLHttpRequest")
            || ((Request.Headers != null)
            && (Request.Headers["X-Requested-With"] == "XMLHttpRequest"));

            //If we are not sure that we have an AJAX request or that we have to return JSON 
            //we fall back to Reflection
            if (!isAjaxRequest)
            {
                try
                {
                    //The controller and action
                    var controllerName = Request.RequestContext.
                                            RouteData.Values["controller"].ToString();
                    var actionName = Request.RequestContext.
                                        RouteData.Values["action"].ToString();

                    //We create a controller instance
                    var controllerFactory = new DefaultControllerFactory();
                    var controller = controllerFactory.CreateController(
                    Request.RequestContext, controllerName) as Controller;

                    //We get the controller actions
                    var controllerDescriptor =
                    new ReflectedControllerDescriptor(controller.GetType());
                    var controllerActions =
                    controllerDescriptor.GetCanonicalActions();

                    //We search for our action
                    foreach (ReflectedActionDescriptor actionDescriptor in controllerActions)
                    {
                        if (actionDescriptor.ActionName.ToUpper().Equals(actionName.ToUpper()))
                        {
                            //If the action returns JsonResult then we have an AJAX request
                            if (actionDescriptor.MethodInfo.ReturnType
                            .Equals(typeof(JsonResult)))
                                return true;
                        }
                    }
                }
                catch
                {

                }
            }

            return isAjaxRequest;
        }

        //protected void Application_Error(object sender, EventArgs e)
        //{
        //    Exception exception = Server.GetLastError();

        //    // Clear the error on server.
        //    Server.ClearError();

        //    // Log the exception.
        //    //ILogger logger = Container.Resolve<ILogger>();
        //    //logger.Error(exception);

        //    Response.Clear();

        //    HttpException httpException = exception as HttpException;
        //    RouteData routeData = new RouteData();
        //    routeData.Values.Add("controller", "Error");

        //    //if (IsAjaxRequest())
        //    //{
        //    //    Response.Headers.Add("AJAX_EXECUTION_ERROR", "1");
        //    //}
        //    //else
        //    //{
        //    //    routeData.Values.Add("action", "General");
        //    //}

        //    if (httpException == null)
        //    {
        //        routeData.Values.Add("action", "Index");
        //    }
        //    else //It's an Http Exception, Let's handle it.
        //    {
        //        switch (httpException.GetHttpCode())
        //        {
        //            case 404:
        //                // Page not found.
        //                routeData.Values.Add("action", "HttpError404");
        //                break;
        //            case 500:
        //                // Server error.
        //                routeData.Values.Add("action", "HttpError500");
        //                break;
        //            // Here you can handle Views to other error codes.
        //            // I choose a General error template  
        //            default:
        //                routeData.Values.Add("action", "General");
        //                break;
        //        }
        //    }

        //    // Pass exception details to the target error View.
        //    routeData.Values.Add("Shared", exception);



        //    // Call target Controller and pass the routeData.
        //    IController errorController = new ErrorController();
        //    errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));

        //}
    }

    public class JSONDataModelBinder<T> : IModelBinder
    {
        private string valueObjName { get; set; }

        public JSONDataModelBinder(string objName)
        {
            valueObjName = objName;
        }

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            // First check if request validation is required
            var shouldPerformRequestValidation = controllerContext.Controller.ValidateRequest &&
                bindingContext.ModelMetadata.RequestValidationEnabled;

            var valueProviderResult = bindingContext.GetValueFromValueProvider(shouldPerformRequestValidation);

            var incomingData = string.Empty;
            // Get the raw attempted value from the value provider
            if (valueProviderResult != null)
            {
                incomingData = valueProviderResult.AttemptedValue;
            }

            if (!string.IsNullOrEmpty(incomingData))
            {
                var json_serializer = new JavaScriptSerializer();
                var model = json_serializer.Deserialize<T>(incomingData);

                return model;
            }

            return null;
        }

    }

    public static class ModeBinderExtentionExtensions
    {
        public static ValueProviderResult GetValueFromValueProvider(this ModelBindingContext bindingContext, bool performRequestValidation)
        {
            var unvalidatedValueProvider = bindingContext.ValueProvider as IUnvalidatedValueProvider;
            return (unvalidatedValueProvider != null)
                       ? unvalidatedValueProvider.GetValue(bindingContext.ModelName, !performRequestValidation)
                       : bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        }
    }

}
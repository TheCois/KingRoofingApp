using System.Web.Optimization;

namespace KRF.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                      "~/Scripts/jquery-{version}.js",
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));



            //bundles.Add(new ScriptBundle("~/bundles/jqueryminui").Include(
             //           "~/Scripts/jquery-1.11.0.js"));

            bundles.Add(new ScriptBundle("~/bundles/abcDropDown").Include(
                        "~/Scripts/abcDropDown.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-ui-1.10.4.custom").Include(
                        "~/Scripts/jquery-ui-1.10.4.custom.js"));

            bundles.Add(new ScriptBundle("~/bundles/enscroll-min").Include(
                        "~/Scripts/enscroll-0.6.0.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/Global").Include(
                        "~/Scripts/Global.js"));

            bundles.Add(new ScriptBundle("~/bundles/Job").Include(
                       "~/Scripts/Job/job-information.js",
                       "~/Scripts/Job/job-assignment.js",
                       "~/Scripts/Job/job-milestone.js",
                       "~/Scripts/bootstrap-2.3.2.min.js",
                       "~/Scripts/bootstrap-multiselect.js",
                       "~/Scripts/Job/job-po.js",
                       "~/Scripts/Job/job-co.js",
                       "~/Scripts/Job/job-wo.js",
                       "~/Scripts/Job/job-inv.js",
                       "~/Scripts/Job/job-document.js",
                       "~/Scripts/Job/job-permit.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.unobtrusive*",
            //            "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));


            //bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));


            bundles.Add(new StyleBundle("~/Content/themes/base/default-css").Include(
                        "~/Content/themes/base/default_styles.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/abcGrid").Include(
                        "~/Content/themes/base/abcGrid.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/abcDropdown").Include(
                        "~/Content/themes/base/abcDropdown.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/jquery-ui-1.10.4.custom").Include(
                        "~/Content/themes/base/jquery-ui-1.10.4.custom.css"));


            bundles.Add(new StyleBundle("~/Content/themes/base/jquery-ui").Include(
                        "~/Content/themes/base/jquery-ui.css"));

            // The below refrence are required to change to have unform
            //"~/Content/assets/js/abcDropDown.js",
            //"~/Content/assets/js/abcSpinner.js",
            //"~/Content/js/jquery-ui-1.10.4.custom.js",
            //"~/Content/assets/js/enscroll-0.6.0.min.js",
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                       //"~/Scripts/jquery.validate.min.js",
                       "~/Scripts/jquery.unobtrusive-ajax.min.js",
                       //"~/Scripts/bootstrap1.min.js",
                       "~/Content/assets/js/abcDropDown.js",
                       "~/Content/assets/js/abcSpinner.js",
                       "~/Scripts/jquery-ui-1.10.4.custom.js"
                        ));

            //"~/Content/assets/css/abcGrid.css",
            //"~/Content/assets/css/abcSpinner.css",
            //"~/Content/assets/css/abcDropDown.css",
            //"~/Content/css/ui-lightness/jquery-ui-1.10.4.custom.css",
            //"~/Content/themes/base/default_styles.css",
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      //"~/Content/themes/base/bootstrap.css",
                      "~/Content/assets/css/default_styles.css",
                      "~/Content/themes/base/abcGrid.css",
                      "~/Content/themes/base/abcSpinner.css",
                      "~/Content/themes/base/abcDropdown.css",
                      "~/Content/themes/base/jquery-ui-1.10.4.custom.css"
              ));




        }
    }
}
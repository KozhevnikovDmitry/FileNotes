using System.Web.Optimization;

namespace FileNotes.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/site").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-responsive.css",
                      "~/Content/layout.css",
                      "~/Content/Site.css",
                      "~/Content/jquery-ui.css",
                      "~/Content/ui.jqgrid.css"));
        }
    }
}
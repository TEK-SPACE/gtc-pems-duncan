using System.Web.Optimization;

namespace Duncan.PEMS.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = false;

            bundles.Add( new StyleBundle( "~/css/main" ).Include(
                "~/css/main.css",
                "~/css/errordialog.css" ) );

            bundles.Add( new StyleBundle( "~/css/Kendo" ).Include(
                "~/css/Kendo/kendo.common.css",
                "~/css/Kendo/kendo.custom.css",
                "~/css/Kendo/kendo.overrides.css" ) );

            bundles.Add( new ScriptBundle( "~/bundles/jquery" ).Include(
                "~/Scripts/Kendo/jquery.min.js",
                "~/Scripts/jquery.idletimer.js", 
                "~/Scripts/jquery.idletimeout.js"
                ) );

            bundles.Add( new ScriptBundle( "~/bundles/common" ).Include(
                "~/Scripts/common.js" ) );

            bundles.Add( new ScriptBundle( "~/bundles/kendo" ).Include(
                "~/Scripts/Kendo/kendo.all.min.js",
                "~/scripts/Kendo/kendo.aspnetmvc.min.js",
                "~/Scripts/gridExtension.js" ) );

            bundles.Add( new ScriptBundle( "~/bundles/jquery-plugins" ).Include(
                "~/Scripts/jquery.cookie.js",
                "~/Scripts/jquery.collapse.js",
                "~/scripts/jquery.collapse_storage.js",
                "~/Scripts/jquery.collapse_cookie_storage.js",
                "~/Scripts/jquery.menu-collapse-0.8.js",
                "~/Scripts/jquery.qtip.js" ) );

            bundles.Add( new ScriptBundle( "~/bundles/IEcompat" ).Include(
                "~/Scripts/html5.js",
                "~/scripts/respond.min.js",
                "~/Scripts/json2.js",
                "~/Scripts/selectivizr-min.js" ) );

            // Clear all items from the default ignore list to allow minified CSS and JavaScript files to be included in debug mode
            bundles.IgnoreList.Clear();

            // Add back the default ignore list rules sans the ones which affect minified files and debug mode
            bundles.IgnoreList.Ignore( "*.intellisense.js" );
            bundles.IgnoreList.Ignore( "*-vsdoc.js" );
            bundles.IgnoreList.Ignore( "*.debug.js", OptimizationMode.WhenEnabled );
            //ignoreList.Ignore("*.min.js", OptimizationMode.WhenDisabled);
            //bundles.IgnoreList.Ignore("*.min.css", OptimizationMode.WhenDisabled);

        }
    }
}
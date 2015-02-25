﻿using System.Web;
using System.Web.Optimization;

namespace OralHistory
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/OralHistoryApp")
                .IncludeDirectory("~/Scripts/Controllers", "*.js")
                .IncludeDirectory("~/Scripts/Factories", "*.js")
                .Include("~/Scripts/OralHistoryApp.js"));

            bundles.Add(new ScriptBundle("~/bundles/Angular")
                .Include("~/Scripts/angular.js")
                .Include("~/Scripts/angular-route.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/site.css"));
        }
    }
}

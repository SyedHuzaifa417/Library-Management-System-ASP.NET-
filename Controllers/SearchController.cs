using Library.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Library.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        private Learning_1Entities db = new Learning_1Entities();


        [HttpGet]
        public ActionResult Index(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return View(new List<SearchLibraryData_Result>());
            }

            var results = db.SearchLibraryData(searchTerm);
            Debug.WriteLine("Search results count: " + results.Count());
            foreach (var result in results)
            {
                Debug.WriteLine($"Source: {result.Source}, Result: {result.Result}");
            }
            return View(results);
        }
    }
}
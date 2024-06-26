﻿using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging.Signing;
using System.Diagnostics;
using ThreeFriends.Models;

namespace ThreeFriends.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        Appdbcontxt entity = new Appdbcontxt();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
			entity.Database.EnsureCreated();
		}

        public IActionResult Test() // Test Icons
        {
            // Define the icons folder path
            string iconsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "icons");

            List<string> iconNames = new List<string>();

            // Get all files in the icons folder
            string[] iconFiles = Directory.GetFiles(iconsFolderPath);

            // Extract file names
            foreach (var iconFile in iconFiles)
            {
                iconNames.Add(Path.GetFileName(iconFile));
            }
            return View(iconNames);
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                return RedirectToAction("index", "Login");
            }
            return RedirectToAction("index", "Login");
        }


        public ActionResult History()
        {
            SharedValues.setHover("History");
            var historyItems = entity.History.Where(h => h.UserId == SharedValues.CurUser.Id 
            && h.Timestamp.Date>=GeneralSettings.DataRangeFrom && h.Timestamp.Date <=GeneralSettings.DataRangeTo).ToList();
            historyItems.Reverse(); // Reverse the list to show the latest history items first
            return View(historyItems);
        }

		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BirdsiteLive.Models;
using BirdsiteLive.Common.Settings;

namespace BirdsiteLive.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly InstanceSettings _instanceSettings;

        public HomeController(ILogger<HomeController> logger, InstanceSettings instanceSettings)
        {
            _logger = logger;
            _instanceSettings = instanceSettings;
        }

        public IActionResult Index()
        {
            return View(_instanceSettings);
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

        [HttpPost]
        public IActionResult Index(string handle)
        {
            return RedirectToAction("Index", "Users", new {id = handle});
        }
    }
}

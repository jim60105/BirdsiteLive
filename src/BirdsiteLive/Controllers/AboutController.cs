using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BirdsiteLive.Domain.Repository;
using BirdsiteLive.Services;

namespace BirdsiteLive.Controllers
{
    public class AboutController : Controller
    {
        private readonly IAboutPageService _aboutPageService;

        #region Ctor
        public AboutController(IAboutPageService cachedStatisticsService)
        {
            _aboutPageService = cachedStatisticsService;
        }
        #endregion

        public async Task<IActionResult> Index()
        {
            var stats = await _aboutPageService.GetAboutPageDataAsync();
            return View(stats);
        }
    }


}

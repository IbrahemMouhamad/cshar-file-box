using System.Linq;
using Microsoft.AspNetCore.Mvc;
using file_box.Service;
using file_box.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System;
using Microsoft.AspNetCore.Identity;

namespace file_box.Controllers
{
    public class LogController : Controller
    {
        private readonly ActionlogService _logger;

        public LogController(ActionlogService logger)
        {
            // for get log action from database
            _logger = logger;
        }

        public IActionResult Index()
        {
            // get all logs
            var allLogs = _logger.GetAll();
            // to list
            var model = allLogs.ToList();
            // pass data to view (Index.cshtml)
            return View(model);
        }

    }
}

using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace YourNamespace.Controllers
{
    public class LoginsController : Controller
    {
        private readonly ILogger<LoginsController> _logger;

        public LoginsController(ILogger<LoginsController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LoginsView()
        {
            return View("LoginsView");
        }

        [HttpPost]
        public IActionResult RunAzureLogin()
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = "-File \"Scripts\\azlogin.ps1\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processInfo))
            {
                process.WaitForExit();
            }

            return Ok();
        }

        [HttpPost]
        public IActionResult RunTFELogin()
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = "-File \"Scripts\\tfelogin.ps1\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processInfo))
            {
                process.WaitForExit();
            }

            return Ok();
        }









    }
}

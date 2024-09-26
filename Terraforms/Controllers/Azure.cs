using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraforms.Models;
using Microsoft.Extensions.Logging;

namespace Terraforms.Controllers
{
    public class Azure : Controller
    {
        private readonly ILogger<Azure> _logger;

        public Azure(ILogger<Azure> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult VMTemplate()
        {
            return View();
        }

        public IActionResult Roadmap()
        {
            var roadmapItems = new List<AzRoadmapItem>();

            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "azroadmap.csv");
                _logger.LogInformation("Reading CSV file from path: {FilePath}", filePath);

                if (!System.IO.File.Exists(filePath))
                {
                    _logger.LogError("CSV file not found at path: {FilePath}", filePath);
                    return StatusCode(500, "CSV file not found");
                }

                var lines = System.IO.File.ReadAllLines(filePath);
                _logger.LogInformation("CSV file read successfully. Number of lines: {LineCount}", lines.Length);

                foreach (var line in lines.Skip(1)) // Skip header row
                {
                    var values = line.Split(',');

                    if (values.Length != 4)
                    {
                        _logger.LogError("CSV file format is incorrect. Line: {Line}", line);
                        return StatusCode(500, "CSV file format is incorrect");
                    }

                    var item = new AzRoadmapItem
                    {
                        Step = values[0],
                        Command = values[1],
                        LinkToForm = values[2],
                        CanBeCoded = values[3]
                    };

                    roadmapItems.Add(item);
                }

                _logger.LogInformation("CSV data processed successfully. Number of items: {ItemCount}", roadmapItems.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while reading the CSV file.");
                return StatusCode(500, "Internal server error");
            }

            return View(roadmapItems);
        }

        public IActionResult CreateRG()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Terraforms.Controllers
{
    public class Repo : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateRepo()
        {
            return View();
        }

        public IActionResult SyncRepo()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DownloadRepo(string pat, string repoUrl, string environment)
        {
            string downloadDir = environment switch
            {
                "prod" => "E:/Subscriptions/Shared_Prod",
                "nonprod" => "E:/Subscriptions/Shared_NonProd",
                "backups" => "E:/Subscriptions/Backups_Shared_Prod",
                _ => throw new ArgumentException("Invalid environment")
            };

            string repoName = System.IO.Path.GetFileNameWithoutExtension(repoUrl);
            string fullDownloadPath = System.IO.Path.Combine(downloadDir, repoName);

            if (!System.IO.Directory.Exists(fullDownloadPath))
            {
                System.IO.Directory.CreateDirectory(fullDownloadPath);
            }

            string tempScriptFile = System.IO.Path.GetTempFileName() + ".ps1";
            string scriptContent = $"git clone --quiet --config credential.helper='Store' https://{pat}@github.com/{repoUrl} {fullDownloadPath}";
            System.IO.File.WriteAllText(tempScriptFile, scriptContent);

            var processInfo = new ProcessStartInfo("powershell.exe", $"-ExecutionPolicy Bypass -File \"{tempScriptFile}\"")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(processInfo);
            process.WaitForExit();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            System.IO.File.Delete(tempScriptFile);

            if (process.ExitCode == 0)
            {
                ViewBag.Status = "Success";
            }
            else
            {
                ViewBag.Status = $"Download failed: {error}";
            }

            return View("SyncRepo");
        }

        [HttpPost]
        public IActionResult CreateRepo([FromBody] RepoCreateModel model)
        {
            string isPrivate = model.IsPrivate ? "--private" : "--public";
            string command = $"gh repo create cbrewintel/{model.RepoName} {isPrivate}";

            Environment.SetEnvironmentVariable("GH_TOKEN", model.Pat);

            var processInfo = new ProcessStartInfo("cmd.exe", $"/c {command}")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(processInfo);
            process.WaitForExit();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            if (process.ExitCode == 0)
            {
                string repoUrl = $"https://github.com/cbrewintel/{model.RepoName}";
                return Json(new { success = true, message = "Repository created successfully", repoUrl });
            }
            else
            {
                return Json(new { success = false, message = $"Repository creation failed: {error}" });
            }
        }
    }

    public class RepoCreateModel
    {
        public string Pat { get; set; }
        public string RepoName { get; set; }
        public string RepoDescription { get; set; }
        public bool IsPrivate { get; set; }
    }
}

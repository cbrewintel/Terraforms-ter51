using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Terraforms.Controllers
{
    public class TFE : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult TFETeams()
        {
            return View("Teams");
        }

        public IActionResult TFEProject()
        {
            return View("Project");
        }

        [HttpPost]
        public IActionResult RunPipeline(string team_name, string sreadmin_users, string developer_users, string readonly_users)
        {
            string script = $@"
                az pipelines run --organization 'https://dev.azure.com/cbre/' --project 'Cloud' --id 9520 `
                --variables `
                    team_name='{team_name}' `
                    sreadmin_users='{sreadmin_users}' `
                    developer_users='{developer_users}' `
                    readonly_users='{readonly_users}'
            ";

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = $"-Command \"{script}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(psi))
            {
                process.WaitForExit();
            }

            return RedirectToAction("TFETeams");
        }

        [HttpPost]
        public IActionResult RunProjectPipeline(string project_name, string team_names)
        {
            string script = $@"
                az pipelines run --organization 'https://dev.azure.com/cbre/' --project 'Cloud' --id 16752 `
                --variables `
                    project_name='{project_name}' `
                    team_names='{team_names}' `
                    request_number='null' `
                    sys_id='null'
            ";

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = $"-Command \"{script}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(psi))
            {
                process.WaitForExit();
            }

            return RedirectToAction("TFEProject");
        }
    }
}

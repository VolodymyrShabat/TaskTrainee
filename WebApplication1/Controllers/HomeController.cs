using ExcelDataReader;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(List<User> users)
        {
            users = users == null ? new List<User>() : users;
            return View(users);
        }

        [HttpPost]
        public IActionResult Index(IFormFile file , [FromServices] IHostingEnvironment hostingEnvironment )
        {
            string filename = $"{hostingEnvironment.WebRootPath}\\files\\{file.FileName}";
            using(FileStream fs = System.IO.File.Create(filename))
            {
                file.CopyTo(fs);
                fs.Flush();
            }
            var users = this.GetListOfUsers(file.FileName);
            return View(users);
        }

        public List<User> GetListOfUsers (string fileName)
        {
            List<User> list = new List<User>();
            var filename = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\files"}" + "\\" + fileName;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.Open(filename, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateCsvReader(stream))
                {
                    while (reader.Read())
                    {
                        list.Add(
                            new User
                            {
                                Name = reader.GetValue(0).ToString(),
                                Phone = reader.GetValue(1).ToString(),
                                Salary = decimal.Parse(reader.GetValue(2).ToString()),
                                DateOfBirth = DateTime.Parse(reader.GetValue(3).ToString()),
                                Married = bool.Parse(reader.GetValue(4).ToString())

                            });
                    }
                }
            }
            return list;
            
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

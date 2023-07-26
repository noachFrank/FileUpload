using FileUpload.data;
using FileUploadDemo.web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FileUploadDemo.web.Controllers
{
    public class PictureController : Controller
    {
        private IWebHostEnvironment _webHostEnvironment;
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=FileUpload; Integrated Security=true;";

        public PictureController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile file, string password)
        {
            var fileName = Guid.NewGuid().ToString();
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "Images", fileName);
            using var fs = new FileStream(path, FileMode.CreateNew);
            file.CopyTo(fs);

            var manager = new DbManager(_connectionString);
            int id = manager.UploadImage(fileName, password);

            var vm = new UploadViewModel { Link = $"https://localhost:7216/picture/viewimage?id={id}", Password = password };

            return View(vm);
        }

       
        public IActionResult ViewImage(int id)
        {
            var manager = new DbManager(_connectionString);
            var vm = new ViewImageViewModel
            {
                Pic = manager.GetPicture(id)
            };
            var ids = HttpContext.Session.Get<List<int>>("ids");
            
            if (ids == null)
            {
                vm.HasSeenThisOne = false;
            }
            else
            {
                vm.HasSeenThisOne = ids.Contains(id);
                manager.IncrementViews(id);
            }
            vm.IncorrectPassword = (string)TempData["IncorrectPassword"];

            return View(vm);
        }

        [HttpPost]
        public IActionResult ViewImage(int id, string password)
        {
            var manager = new DbManager(_connectionString);
            var pic = manager.GetPicture(id);
            var ids = HttpContext.Session.Get<List<int>>("ids");
            if (ids == null)
            {
                ids = new List<int>();
            }
            if (password == pic.Password)
            {
                ids.Add(pic.Id);
                HttpContext.Session.Set<List<int>>("ids", ids);
                //vm.HasSeenThisOne = ids.Contains(id);
                //manager.IncrementViews(id);
            }
            else
            {
                TempData["IncorrectPassword"] = "INCORRECT PASSWORD! PLEASE TRY AGAIN!!";
            }

            return Redirect($"/picture/ViewImage?id={id}");
        }

    }
}
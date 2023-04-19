using April_17_homework.Data;
using April_17_homework.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace April_17_homework.Web.Controllers
{
    public class HomeController : Controller
    {
        private IWebHostEnvironment _webHostEnvironment;
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=Images; Integrated Security=true;";

        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile imageFile, string password)
        {
            var fileName = $"{Guid.NewGuid()}-{imageFile.FileName}";
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
            using var fs = new FileStream(filePath, FileMode.CreateNew);
            imageFile.CopyTo(fs);

            ImageManager manager = new(_connectionString);
            int id = manager.AddImage(fileName, password);
            UploadViewModel model = new()
            {
                Id = id,
                Password = password
            };
            return View(model);
        }

        public IActionResult ViewImage(int id)
        {
            ImageManager manager = new(_connectionString);
            ViewImageViewModel model = new()
            {
                Id = id,
                ShouldShow = false,
                Image = manager.GetImage(id),
                Ids = HttpContext.Session.Get<List<int>>("ids") ?? new List<int>()
            };

            if (model.Ids.Contains(id))
            {
                manager.UpdateView(id, model.Image.Views + 1);
                model.Image.Views = model.Image.Views + 1;
            }

            return View(model);
        }
        [HttpPost]
        public IActionResult ViewImage(int id, string password)
        {

            ImageManager manager = new(_connectionString);
            ViewImageViewModel model = new()
            {
                Image = manager.GetImage(id),
                Id = id,
                Ids = HttpContext.Session.Get<List<int>>("ids") ?? new List<int>()
            };

            model.ShouldShow = model.Image.Password == password;
            if (model.ShouldShow)
            {
                manager.UpdateView(id, model.Image.Views + 1);
                model.Image.Views = model.Image.Views + 1;

                if (!model.Ids.Contains(id))
                {
                    model.Ids.Add(model.Id);
                }

                HttpContext.Session.Set("ids", model.Ids);
            }
            else
            {
                model.NotCorrect = true;
            }

            return View(model);
        }

    }
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonSerializer.Deserialize<T>(value);
        }
    }

}
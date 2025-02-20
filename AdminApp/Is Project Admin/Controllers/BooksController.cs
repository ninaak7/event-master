using Is_Project_Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Is_Project_Admin.Controllers
{
    public class BooksController : Controller
    {
        public IActionResult Index()
        {
            HttpClient client = new HttpClient();
            string URL = "https://bookstore-webapp-bzc2bhdtbddnffgg.switzerlandnorth-01.azurewebsites.net/api/Admin/GetAllBooks";

            HttpResponseMessage response = client.GetAsync(URL).Result;
            var data = response.Content.ReadAsAsync<List<Book>>().Result;
            return View(data);
        }

        public IActionResult Details(string id)
        {
            HttpClient client = new HttpClient();

            string URL = $"https://bookstore-webapp-bzc2bhdtbddnffgg.switzerlandnorth-01.azurewebsites.net/api/Admin/GetDetails?id={id}";

            HttpResponseMessage response = client.GetAsync(URL).Result;

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsAsync<Book>().Result;
                return View(result);
            }

            return View("Error"); // Handle errors appropriately
        }
    }
}

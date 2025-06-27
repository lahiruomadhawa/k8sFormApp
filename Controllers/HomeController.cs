using System.Diagnostics;
using System.Text.Json;
using k8sFormApp.Models;
using k8sFormApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace k8sFormApp.Controllers
{
    public class HomeController : Controller
    {

        private readonly IRedisService _redisService;

        public HomeController(IRedisService redisService)
        {
            _redisService = redisService;
        }

        public IActionResult Index()
        {            
            return View(new Person());
        }

        [HttpPost]
        public async Task<IActionResult> Index(Person person)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var personJson = JsonSerializer.Serialize(new
                    {
                        FirstName = person.FirstName,
                        LastName = person.LastName,
                        Address = person.Address,
                        CreatedAt = DateTime.UtcNow
                    });
                    await _redisService.AddPersonAsync(personJson);

                    ViewBag.Message = "Person saved successfully!";
                    ModelState.Clear(); // Clear the model state to reset the form
                    return View(new Person()); // Return a new instance of Person to reset the form
                }
                catch (Exception ex)
                {
                    ViewBag.Message = $"Error saving person: {ex.Message}";
                }
            }
            return View(person);
        }


    }
}

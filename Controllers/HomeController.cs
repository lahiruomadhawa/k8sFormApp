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
        private readonly ILogger<HomeController> _logger;

        public HomeController(IRedisService redisService, ILogger<HomeController> logger)
        {
            _redisService = redisService;
            _logger = logger;
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
                    _logger.LogInformation("------------------------------------------------------------------------------------");
                    _logger.LogInformation("Person saved successfully: {Person}", personJson);
                    _logger.LogInformation("------------------------------------------------------------------------------------");

                    Console.WriteLine("------------------------------------------------------------------------------------");
                    Console.WriteLine("Person saved successfully: " + person.FirstName.ToString());
                    Console.WriteLine("------------------------------------------------------------------------------------");
                    ModelState.Clear();
                    return View(new Person());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString(), "Error saving person to Redis");
                    ViewBag.Message = $"Error saving person: {ex.Message}";
                }
            }
            return View(person);
        }


    }
}

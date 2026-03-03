using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using healthify.Models;
using Healthify.ViewModels;
using Healthify.Data;


namespace healthify.Controllers;

public class HomeController : Controller
{
    private readonly DbConnectionFactory _dbFactory;
    private readonly HomeRepository _homeRepository;

    public HomeController(DbConnectionFactory dbFactory , HomeRepository homeRepository)
    {
            _dbFactory = dbFactory;
            _homeRepository = homeRepository;
    }

    public IActionResult Index()
    {
        var model = new HomeViewModel
        {
            Products = _homeRepository.GetAllProducts(),
            Categories = _homeRepository.GetAllCategories() 
        };     
        return View (model);
    }

   
    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    public IActionResult Services()
    {
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

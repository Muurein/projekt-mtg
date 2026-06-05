using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using projekt_mtg.Data;
using projekt_mtg.Models;
using Projekt_mtg.DTOs;
using Projekt_mtg.Services;


namespace projekt_mtg.Controllers;

public class HomeController : Controller
{
    //connect to database and Scryfall API
    private readonly ApplicationDbContext _context;
    private readonly ScryfallService _scryfallService;

    public HomeController(ApplicationDbContext context, ScryfallService scryfallService)
    {
        _context = context;
        _scryfallService = scryfallService;
    }

    //show what the user is searching for, otherwise shows 12 random cards from Scryfall's database
    public async Task<IActionResult> Index(string searchString)
    {
        
        if(!string.IsNullOrWhiteSpace(searchString))
        {
            var cards = await _scryfallService.SearchScryfall(searchString);

            return View(cards);
        }

        var randomCardsHome = new List<CardDto>();

        for(int i = 0; i < 12; i++)
        {
            var card = await _scryfallService.RandomCardHome();

            
            if (card != null)
            {
                randomCardsHome.Add(card);
            }
        }

        
        return View(randomCardsHome);
    }

    //privacy page
    public IActionResult Privacy()
    {
        return View();
    }

    //error page
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projekt_mtg.Models;
using Projekt_mtg.Models.ViewModels;
using projekt_mtg.Data;
using System.Security.Claims;
using Projekt_mtg.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace projekt_mtg.Controllers
{
    [Authorize]
    public class CardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ScryfallService _scryfallService;
        private string? searchString;

        public CardController(ApplicationDbContext context, ScryfallService scryfallService)
        {
            _context = context;
            _scryfallService = scryfallService;
        }

        // GET: MtgCard
        public async Task<IActionResult> Index(string searchString)
        {
            if(_context.Cards == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Cards' is null");
            }
            
            var cards = from c in _context.Cards 
                        select c;

            if(!String.IsNullOrEmpty(searchString))
            {
                cards = cards.Where(c =>
                c.Name.ToUpper().Contains(searchString.ToUpper()) ||
                (c.ManaCost ?? "").ToUpper().Contains(searchString.ToUpper()) ||
                (c.TypeLine ?? "").ToUpper().Contains(searchString.ToUpper()) ||
                (c.Rarity ?? "").ToUpper().Contains(searchString.ToUpper())
                );
            }

            return View(await cards.ToListAsync()); 
        }

        // GET: MtgCard/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var card = await _context.Cards
                .FirstOrDefaultAsync(m => m.Id == id);
            if (card == null)
            {
                return NotFound();
            }

            return View(card);
        }

        // GET: MtgCard/Create
        public IActionResult Create()
        {
            //return view model
            return View(new CardAndCollectionVM());
        }

        // POST: MtgCard/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CardAndCollectionVM vm)
        // ([Bind("Id,SetId,Title,Color,ManaValue,Type,Description,Rarity")] Card card)
        {
            // if (ModelState.IsValid)
            // {
            //     _context.Add(card);
            //     await _context.SaveChangesAsync();
            //     return RedirectToAction(nameof(Index));
            // }
            // return View(card);

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            //hämtar data från Scryfall
            var scryfallCard = await _scryfallService.GetCardByName(vm.CardName);

            if(scryfallCard == null)
            {
                ModelState.AddModelError("", "Kortet hittated inte");
                return View(vm);
            }

            //skapa kort från API-data
            //finns redan kortet?
            var card = await _context.Cards
                .FirstOrDefaultAsync(c => c.Name == scryfallCard.Name);

            //om nej, skapa nytt kort
            if(card == null)
            {
                card = new Card
                {
                    Name = scryfallCard.Name,
                    ManaCost = scryfallCard.ManaCost,
                    TypeLine = scryfallCard.TypeLine,
                    OracleText = scryfallCard.OracleText,
                    Rarity = scryfallCard.Rarity,
                    ImageUri = scryfallCard.ImageUri
                };  

                _context.Cards.Add(card);
                await _context.SaveChangesAsync();
            }


            //lägger till i collection och kollar om ett kort redan finns (om ja, uppdatera collection istället för att skapa en ny rad)
            var currentCollection = await _context.Collections
                .FirstOrDefaultAsync(c =>
                    c.CardId == card.Id &&
                    c.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier));


            if(currentCollection == null)
            {
                var collection = new Collection
                {
                    CardId = card.Id,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!, //! = vet att den itne är null
                    OwnedQuantity = vm.OwnedQuantity,
                    WishlistQuantity = vm.WishlistQuantity,
                    InDeckQuantity = vm.InDeckQuantity
                };

                _context.Collections.Add(collection);
            }  
            else
            {
                currentCollection.OwnedQuantity += vm.OwnedQuantity;
                currentCollection.WishlistQuantity += vm.WishlistQuantity;
                currentCollection.InDeckQuantity += vm.InDeckQuantity;

            }
            
            
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Create));  
        }


        // GET: MtgCard/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var card = await _context.Cards.FindAsync(id);
            if (card == null)
            {
                return NotFound();
            }
            return View(card);
        }

        // POST: MtgCard/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ManaCost,TypeLine,OracleText,Rarity, ImageUri")] Card card)
        {
            if (id != card.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(card);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CardExists(card.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(card);
        }

        // GET: MtgCard/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var card = await _context.Cards
                .FirstOrDefaultAsync(m => m.Id == id);
            if (card == null)
            {
                return NotFound();
            }

            return View(card);
        }

        // POST: MtgCard/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var card = await _context.Cards.FindAsync(id);
            if (card != null)
            {
                _context.Cards.Remove(card);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CardExists(int id)
        {
            return _context.Cards.Any(e => e.Id == id);
        }


        //söker efter kort
        [HttpGet]
        public async Task<IActionResult> SearchCardNames(string query)
        {
            var results = await _scryfallService.SearchCards(query);

            return Json(results);
        }
    }
}

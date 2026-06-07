using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt_mtg.Models;
using Projekt_mtg.Models.ViewModels;
using projekt_mtg.Data;
using System.Security.Claims;
using Projekt_mtg.Services;
using Microsoft.AspNetCore.Authorization;

namespace projekt_mtg.Controllers
{
    [Authorize]
    public class CardController : Controller
    {   
        //connect to database and Scryfall API
        private readonly ApplicationDbContext _context;
        private readonly ScryfallService _scryfallService;

        public CardController(ApplicationDbContext context, ScryfallService scryfallService)
        {
            _context = context;
            _scryfallService = scryfallService;
        }

        // GET: Card
        //displays cards and lets user search for cards according to four attributes
        public async Task<IActionResult> Index(string searchString)
        {
            if(_context.Cards == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Cards' is null");
            }
            
            var cards = from c in _context.Cards 
                        select c;

            //checks if user entered search string
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

        // GET: Card/Details/5
        public async Task<IActionResult> Details(int? id)
        {   
            //in case no ID was provided
            if (id == null)
            {
                return NotFound();
            }

            //find card by ID
            var card = await _context.Cards.FirstOrDefaultAsync(m => m.Id == id);

            if (card == null)
            {
                return NotFound();
            }

            return View(card);
        }

        // GET: Card/Create
        //sends empty ViewModel to form for user to add card
        public IActionResult Create()
        {
            return View(new CardAndCollectionVM());
        }

        // POST: Card/Create
        //gets data from Scryfall, creates card from API-data, adds it to user's collection
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CardAndCollectionVM vm)
        {
            //validates form input
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var scryfallCard = await _scryfallService.GetCardByName(vm.CardName);

            if(scryfallCard == null)
            {
                ModelState.AddModelError(nameof(vm.CardName), $"The card could not be found");
                return View(vm);
            }

            //does card already exist? No -> create new card
            var card = await _context.Cards.FirstOrDefaultAsync(c => c.Name == scryfallCard.Name);

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


            //adds card to collection and checks t osee if the card is already there. If yes, update collection instead of creating another row with the same card
            //in other words, updates quantities instead of creating duplicate row
            var currentCollection = await _context.Collections.FirstOrDefaultAsync(c =>c.CardId == card.Id &&c.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier));


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

            //reload Create-page
            return RedirectToAction(nameof(Create));  
        }


        // GET: Card/Edit/5
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

        // POST: Card/Edit/5
        //creates card
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ManaCost,TypeLine,OracleText,Rarity, ImageUri")] Card card)
        {
            //makes sure that the right card is edited
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
                    //if card was deleted by another user
                    if (!CardExists(card.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                //redirct to Index-page
                return RedirectToAction(nameof(Index));
            }
            return View(card);
        }

        // GET: Card/Delete/5
        //delete-page
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var card = await _context.Cards.FirstOrDefaultAsync(m => m.Id == id);

            if (card == null)
            {
                return NotFound();
            }

            return View(card);
        }

        // POST: Card/Delete/5
        //acutally deletes the card from the database
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

        //double checks that the card actually exists in database
        private bool CardExists(int id)
        {
            return _context.Cards.Any(e => e.Id == id);
        }


        //search for card, matches card name with data from Scryfall
        // [HttpGet]
        // public async Task<IActionResult> SearchCardNames(string query)
        // {
        //     var results = await _scryfallService.SearchCards(query);

        //     return Json(results);
        // }
    }
}

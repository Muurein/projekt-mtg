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

namespace projekt_mtg.Controllers
{
    public class MtgCardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MtgCardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MtgCard
        public async Task<IActionResult> Index()
        {
            return View(await _context.Cards.ToListAsync());
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

                //sparar Card först för att skapa PK Id
                _context.Cards.Add(vm.Card);
                await _context.SaveChangesAsync();

                //skapar en rad i Collection
                var collection = new Collection
                {
                    CardId = vm.Card.Id,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Status = vm.Status,
                    Quantity = vm.Quantity
                };

                _context.Collections.Add(collection);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index"); //(nameof(Index)); istället?
           
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,SetId,Title,Color,ManaValue,Type,Description,Rarity")] Card card)
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
    }
}

using Microsoft.AspNetCore.Mvc;
using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace HotelBookingSystem.Controllers
{
    
    public class HotelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HotelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Hotels
        public async Task<IActionResult> Index()
        {
            return View(await _context.Hotels.ToListAsync());
        }

        // GET: Hotels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Hotels/Create
         [Authorize (Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Location")] Hotel hotel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hotel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hotel);
        }

        // GET: Hotels/Edit/
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null) return NotFound();

            return View(hotel);
        }

        // POST: Hotels/Edit/
        
        [Authorize (Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Location")] Hotel hotel)
        {
            if (id != hotel.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(hotel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hotel);
        }

        // GET: Hotels/Delete/
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null) return NotFound();

            return View(hotel);
        }

        // POST: Hotels/Delete/
        [Authorize (Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel != null)
            {
                _context.Hotels.Remove(hotel);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

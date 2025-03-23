using Microsoft.AspNetCore.Mvc;
using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace HotelBookingSystem.Controllers
{
    
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Rooms
        public async Task<IActionResult> Index(int? hotelId)
        {
            var rooms = _context.Rooms.Include(r => r.Hotel).AsQueryable();
            
            if (hotelId.HasValue)
            {
                rooms = rooms.Where(r => r.HotelId == hotelId);
            }

            return View(await rooms.ToListAsync());
        }

        // GET: Rooms/Create
       public IActionResult Create()
{
    var hotels = _context.Hotels.ToList();
    
    if (hotels == null || !hotels.Any())
    {
        Console.WriteLine("❌ No hotels found in the database!");
    }
    else
    {
        Console.WriteLine($"✅ Found {hotels.Count} hotels in the database.");
    }

    ViewBag.Hotels = hotels;
    return View();
}


        // POST: Rooms/Create
 [Authorize (Roles = "Admin")]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create([Bind("Id,HotelId,RoomType,PricePerNight,IsAvailable")] Room room)
{
    Console.WriteLine($"🔍 Received Room Data: HotelId={room.HotelId}, RoomType={room.RoomType}, Price={room.PricePerNight}, Available={room.IsAvailable}");

    if (!ModelState.IsValid)
    {
        Console.WriteLine("❌ ModelState is invalid. Errors:");
        foreach (var entry in ModelState)
        {
            foreach (var error in entry.Value.Errors)
            {
                Console.WriteLine($"⚠️ Field: {entry.Key}, Error: {error.ErrorMessage}");
            }
        }

        ViewBag.Hotels = _context.Hotels.ToList();
        return View(room);
    }

    // ✅ Explicitly remove Hotel reference (not needed for saving)
    room.Hotel = null;

    _context.Add(room);
    await _context.SaveChangesAsync();
    Console.WriteLine("✅ Room added successfully!");
    return RedirectToAction(nameof(Index));
}





        // GET: Rooms/Edit/5
         [Authorize (Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            ViewBag.Hotels = _context.Hotels.ToList();
            return View(room);
        }

        // POST: Rooms/Edit/5
         [Authorize (Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HotelId,RoomType,PricePerNight,IsAvailable")] Room room)
        {
            if (id != room.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        // GET: Rooms/Delete/5
         [Authorize (Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            return View(room);
        }

        // POST: Rooms/Delete/5
         [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

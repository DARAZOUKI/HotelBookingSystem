using Microsoft.AspNetCore.Mvc;
using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace HotelBookingSystem.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BookingsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings.Include(b => b.Room).ThenInclude(r => r.Hotel).ToListAsync();
            return View(bookings);
        }

        // GET: Bookings/Create
        public IActionResult Create(int roomId)
        {
            var room = _context.Rooms.Include(r => r.Hotel).FirstOrDefault(r => r.Id == roomId);
            if (room == null) return NotFound();

            ViewBag.Room = room;
            return View();
        }

        // POST: Bookings/Create
     [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create([Bind("RoomId,CheckIn,CheckOut")] Booking booking)
{
    var user = await _userManager.GetUserAsync(User);
    if (user == null) return RedirectToAction("Login", "Account");

    var room = await _context.Rooms.FindAsync(booking.RoomId);
    if (room == null || !room.IsAvailable) return NotFound();

    // ✅ Ensure UTC timestamps
    booking.CheckIn = DateTime.SpecifyKind(booking.CheckIn, DateTimeKind.Utc);
    booking.CheckOut = DateTime.SpecifyKind(booking.CheckOut, DateTimeKind.Utc);

    booking.UserId = user.Id;
    booking.TotalPrice = (decimal)(booking.CheckOut - booking.CheckIn).TotalDays * room.PricePerNight;

    _context.Add(booking);
    room.IsAvailable = false;
    await _context.SaveChangesAsync();

    return RedirectToAction(nameof(Index));
}



        // POST: Bookings/Cancel/5
       [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Cancel(int id)
{
    var booking = await _context.Bookings.Include(b => b.Room).FirstOrDefaultAsync(b => b.Id == id);
    if (booking != null)
    {
        // Check if this was the last booking for the room
        var otherBookings = await _context.Bookings
            .Where(b => b.RoomId == booking.RoomId && b.Id != id)
            .AnyAsync();

        if (!otherBookings)
        {
            booking.Room.IsAvailable = true;
        }

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
    }
    return RedirectToAction(nameof(Index));
}

    }
}

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
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var bookings = await _context.Bookings
            .Include(b => b.Room)
            .ThenInclude(r => r.Hotel)
            .Where(b => b.UserId == user.Id) // Only show logged-in user's bookings
            .ToListAsync();

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

    // 🔍 Check for overlapping bookings
    var existingBooking = await _context.Bookings
        .Where(b => b.RoomId == booking.RoomId)  // Same room
        .Where(b => b.CheckOut > booking.CheckIn && b.CheckIn < booking.CheckOut)  // Overlapping dates
        .FirstOrDefaultAsync();

    if (existingBooking != null)
    {
        ModelState.AddModelError("", "Room is already booked for the selected dates.");
        var roomDetails = await _context.Rooms.Include(r => r.Hotel).FirstOrDefaultAsync(r => r.Id == booking.RoomId);
        ViewBag.Room = roomDetails;
        return View(booking); // Return to form with error message
    }

    // ✅ Assign user & calculate total price
    booking.UserId = user.Id;
    booking.TotalPrice = (decimal)(booking.CheckOut - booking.CheckIn).TotalDays * room.PricePerNight;

    _context.Add(booking);
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
public async Task<IActionResult> Details(int id)
{
    var booking = await _context.Bookings
        .Include(b => b.Room)
        .ThenInclude(r => r.Hotel)
        .FirstOrDefaultAsync(b => b.Id == id);

    if (booking == null) return NotFound();
    return View(booking);
}

    }
}

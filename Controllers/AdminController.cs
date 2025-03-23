using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBookingSystem.Data;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> ManageBookings()
    {
        var bookings = await _context.Bookings
            .Include(b => b.Room)
            .ThenInclude(r => r.Hotel)
            .ToListAsync();

        return View(bookings);
    }
}
